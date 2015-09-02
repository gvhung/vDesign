using Base.BusinessProcesses.Entities;
using Base.BusinessProcesses.Exceptions;
using Base.Security;
using Framework.Maybe;
using IronPython.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Base.BusinessProcesses.Services.Concrete
{
    public class WFObjectInitializer : IWFObjectInitializer
    {
        #region HelperClasses from anonymus

        private class InitItem
        {
            public MacroType MacroType { get; set; }
            public string Value { get; set; }
        }


        private class ObjectRefItem
        {
            public string Type { get; set; }
            public int ID { get; set; }
        }

        private class RefObject
        {
            private readonly PropertyInfo _property;
            private readonly object _o;

            public PropertyInfo Property
            {
                get { return _property; }
            }

            public object Object
            {
                get { return _o; }
            }

            public RefObject(PropertyInfo property, object o)
            {
                _property = property;
                _o = o;
            }
        }

        private class MacroPropertyPair
        {
            private readonly IEnumerable<InitItem> _macroses;
            private readonly PropertyInfo _property;

            public IEnumerable<InitItem> Macroses
            {
                get { return _macroses; }
            }

            public PropertyInfo Property
            {
                get { return _property; }
            }

            public MacroPropertyPair(IEnumerable<InitItem> macroses, PropertyInfo property)
            {
                _macroses = macroses;
                _property = property;
            }
        }

        #endregion

        public void InitializeObject(ISecurityUser securityUser, BaseObject src, BaseObject dest, IEnumerable<Entities.InitItem> inits)
        {
            var resultScript = String.Empty;

            try
            {
                var engine = Python.CreateEngine();

                engine.Runtime.LoadAssembly(Assembly.GetExecutingAssembly());

                var scope = engine.CreateScope(new Dictionary<string, object> { { "Dest", dest }, { "Src", src } });

                IEnumerable<RefObject> refObjects;

                var macroses = PrepareMacros(dest, inits, out refObjects);

                foreach (var refObj in refObjects)
                    scope.SetVariable(String.Format("_refobject_{0}", refObj.Property.Name), refObj.Object);

                foreach (var pair in macroses.AsEnumerable().Reverse())
                {
                    var script = new StringBuilder(String.Format("Dest.{0} =", pair.Property.Name));

                    foreach (var initItem in pair.Macroses)
                    {
                        switch (initItem.MacroType)
                        {
                            case MacroType.String:
                                script.AppendFormat(" '{0}'", initItem.Value);
                                break;
                            case MacroType.Number:
                                {
                                    if (pair.Property.PropertyType.IsEnum)
                                    {
                                        var tempName = String.Format("_enum{0}", pair.Property.Name);

                                        scope.SetVariable(tempName, Enum.Parse(pair.Property.PropertyType, initItem.Value));

                                        script.AppendFormat(tempName);

                                        break;
                                    }

                                    goto case MacroType.Operator;
                                }
                            case MacroType.Boolean:
                            {
                                script.AppendFormat(" '{0}'", initItem.Value);

                                break;
                            }
                            case MacroType.Operator:
                                script.AppendFormat(" {0}", initItem.Value);
                                break;
                            case MacroType.InitObject:
                                {
                                    script.AppendFormat(" Src.{0}", initItem.Value);
                                    break;
                                }
                            case MacroType.BaseObject:
                                if (dest is ICategorizedItem)
                                {
                                    var foreignKey = pair.Property.GetCustomAttribute<ForeignKeyAttribute>();

                                    if (foreignKey != null && foreignKey.Name == "CategoryID")
                                    {
                                        script.Append(" None");

                                        dest.GetType().GetProperty("CategoryID").SetValue(dest, JsonConvert.DeserializeObject<ObjectRefItem>(initItem.Value).ID);

                                        break;
                                    }
                                }

                                script.AppendFormat(" _refobject_{0}", pair.Property.Name);
                                break;
                            case MacroType.Function:
                                if (initItem.Value == "dtn()")
                                {
                                    var locVar = this.GenerateVaribleName();

                                    scope.SetVariable(locVar, DateTime.Now);

                                    script.AppendFormat(" {0}", locVar);
                                }
                                break;
                            case MacroType.DateTime:
                            {
                                DateTime dt;
                                if (DateTime.TryParse(initItem.Value, out dt))
                                {
                                    var locVar = this.GenerateVaribleName();

                                    scope.SetVariable(locVar, dt);

                                    script.AppendFormat(" {0}", locVar);

                                    break;
                                }

                                throw ExceptionHelper.ActionInvokeException(
                                    String.Format("Дата имеет неверный формат {0} -> {1}", pair.Property.Name, initItem.Value));
                            }
                            case MacroType.TimeSpan:
                            {
                                double minutes;
                                if (double.TryParse(initItem.Value, out minutes))
                                {
                                    var locVar = this.GenerateVaribleName();

                                    scope.SetVariable(locVar, TimeSpan.FromMinutes(minutes));

                                    script.AppendFormat(" {0}", locVar);

                                    break;
                                }

                                throw ExceptionHelper.ActionInvokeException(
                                    String.Format("Временной интервал имел неверный формат {0} -> {1}", pair.Property.Name, initItem.Value));
                            }

                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }

                    script.Append(";");

                    resultScript = script.ToString();

                    engine.Execute(resultScript, scope);
                }
            }
            catch (Exception e)
            {
                throw new ScriptExecutionException("Ошибка выполнения макроса" + Environment.NewLine + resultScript, e)
                    .IfNotNull(x => x.Data["Script"] = resultScript);
            }
        }

        private static IEnumerable<MacroPropertyPair> PrepareMacros(BaseObject dest, IEnumerable<Entities.InitItem> inits, out IEnumerable<RefObject> refObjects)
        {
            try
            {
                var macroses = inits.Join(dest.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance),
                    item => item.Member,
                    info => info.Name,
                    (item, info) => new MacroPropertyPair(JsonConvert.DeserializeObject<IEnumerable<InitItem>>(item.Value), info))
                    .ToList();

                refObjects = macroses.SelectMany(x => x.Macroses,
                    (x, init) => new { init.MacroType, InitItem = init, x.Property })
                    .Where(x => x.MacroType == MacroType.BaseObject)
                    .Select(x => new { x.Property, ObjectRefItem = JsonConvert.DeserializeObject<ObjectRefItem>(x.InitItem.Value) })
                    .Select(x => new RefObject(x.Property, Activator.CreateInstance(x.Property.PropertyType)
                        .IfNotNull(o => o.GetType().GetProperty("ID").SetValue(o, x.ObjectRefItem.ID))));

                return macroses;
            }
            catch (Exception e)
            {
                throw new BadMacroException("BadMacroException", e);
            }
        }

        private string GenerateVaribleName()
        {
            return String.Format("lcv_{0}",
                Guid.NewGuid().ToString().Split('-')[0]);
        }
    }
}