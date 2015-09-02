using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Base;
using Base.Attributes;
using Base.BusinessProcesses.Entities;
using Base.Validation;
using WebUI.Models.Validation;

namespace WebUI.Controllers
{
    public class ValidationController : BaseController
    {
        private readonly IValidationConfigManager _validationConfigManager;

        public ValidationController(IBaseControllerServiceFacade facade, IValidationConfigManager configManager)
            : base(facade)
        {
            _validationConfigManager = configManager;
        }

        public ActionResult GetEditorsVm(string objectType, ICollection<StageActionValidationItem> validationItems)
        {
            var type = Type.GetType(objectType) ?? GetType(objectType);

            if (type != null)
            {
                var props = this.GetEditorsVms(type, x =>
                {
                    if (x != typeof(String) && x.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                    {
                        return false;
                    }

                    return true;
                }).ToList();

                var rules = _validationConfigManager.GetRules(type).Select(x=> new ValidationRuleVm() { Title = x.Title, Type = x.GetType().GetTypeName() }).ToList();

                return PartialView("EditValidationProperties", new ValidationVm()
                {
                    Editors = props,
                    ObjectType = type,
                    Mnemonic = this.DefaultViewModelConfig(type).Mnemonic, 
                    ObjectValidationRules = rules,
                });
            }

            return null;
        }

        public ActionResult GetValidationRules(string property, string objectType)
        {
            var type = Type.GetType(objectType) ?? GetType(objectType);
            if (type != null)
            {
                PropertyValidationVm vm = new PropertyValidationVm
                {
                    ValidationRules = _validationConfigManager.GetRules(type),                    
                };
                return View("PropertyValidation", vm);
            }
            return null;
        }

        private IEnumerable<PropertyEditorVm> GetEditorsVms(Type type, Func<Type, bool> predicate = null)
        {            
            var allProps = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.SetField).Where(x => x.CanWrite).ToList();

            if (predicate != null)
                allProps = allProps.Where(x => predicate(x.PropertyType)).ToList();
            return allProps.Select(x => new { Prop = x, Attr = x.GetCustomAttribute<DetailViewAttribute>() })
                .Where(x => x.Attr != null).Select(x => new PropertyEditorVm(x.Attr.Name, x.Prop));
        }
        private Type GetType(string objectType)
        {
            if (!String.IsNullOrEmpty(objectType))
                return ViewModelConfigs.Select(x => x.TypeEntity)
                   .FirstOrDefault(x => x.FullName == objectType);

            return null;
        }

    }
}