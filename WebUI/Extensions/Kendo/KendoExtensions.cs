using Base.QueryableExtensions;
using Base.Service;
using Base.UI;
using Framework;
using Kendo.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;

namespace WebUI.Extensions.Kendo
{
    public static class KendoExtensions
    {
        public static async Task<DataSourceResult> ToDataSourceResultAsync(this IQueryable queryable, DataSourceRequest request, IBaseObjectCRUDService service, ViewModelConfig config)
        {
            var result = new DataSourceResult();

            bool execute = true;

            if (config.ListView.DataSource != null && config.ListView.DataSource.ExecuteQueryBeforeApplyClientFilter)
            {
                queryable = (await queryable.ToEnumerableAsync()).AsQueryable();
                execute = false;
            }

            var data = queryable;

            var filters = new List<IFilterDescriptor>();

            if (request.Filters != null)
            {
                KendoExtensions.PatchFilter(ref data, request.Filters);

                filters.AddRange(request.Filters);
            }

            if (filters.Any())
            {
                data = data.Where(filters);
            }

            var sort = new List<SortDescriptor>();

            if (request.Sorts != null)
            {
                sort.AddRange(request.Sorts);
            }

            var temporarySortDescriptors = new List<SortDescriptor>();

            IList<GroupDescriptor> groups = new List<GroupDescriptor>();

            if (request.Groups != null)
            {
                groups.AddRange(request.Groups);
            }

            var aggregates = new List<AggregateDescriptor>();

            if (request.Aggregates != null)
            {
                aggregates.AddRange(request.Aggregates);
            }

            if (aggregates.Any())
            {
                IQueryable source = data;

                if (filters.Any())
                {
                    source = source.Where(filters);
                }
                
                result.AggregateResults = source.Aggregate(aggregates.SelectMany(a => a.Aggregates));

                if (groups.Any() && aggregates.Any())
                {
                    groups.Each(g => g.AggregateFunctions.AddRange(aggregates.SelectMany(a => a.Aggregates)));
                }
            }

            result.Total = await data.CountAsync();

            if (groups.Any())
            {
                groups.Reverse().Each(groupDescriptor =>
                {
                    var sortDescriptor = new SortDescriptor
                    {
                        Member = groupDescriptor.Member,
                        SortDirection = groupDescriptor.SortDirection
                    };

                    sort.Insert(0, sortDescriptor);
                    temporarySortDescriptors.Add(sortDescriptor);
                });
            }

            if (sort.Any())
            {
                data = data.Sort(sort);
            }

            var notPagedData = data;

            data = data.Page(request.Page - 1, request.PageSize);

            if (groups.Any())
            {
                //TODO: 
                data = (await data.ToEnumerableAsync()).AsQueryable();
                execute = false;

                data = data.GroupBy(groups);
            }

            if (execute)
                result.Data = await data.ToEnumerableAsync();
            else
                result.Data = data;


            temporarySortDescriptors.Each(sortDescriptor => sort.Remove(sortDescriptor));

            return result;
        }

        private static void PatchFilter(ref IQueryable q, IList<IFilterDescriptor> filters)
        {
            foreach (IFilterDescriptor f in filters)
            {
                if (f is CompositeFilterDescriptor)
                {
                    KendoExtensions.PatchFilter(ref q, (f as CompositeFilterDescriptor).FilterDescriptors);
                }
                else if (f is FilterDescriptor)
                {
                    FilterDescriptor filter = f as FilterDescriptor;

                    bool isbasecoll = filter.Member.EndsWith(JsonNetResult.BASE_COLLECTION_SUFFIX);
                    bool iseasycoll = filter.Member.EndsWith(JsonNetResult.EASY_COLLECTION_SUFFIX);

                    if (isbasecoll || iseasycoll)
                    {
                        string idname = "ID";

                        if (isbasecoll)
                        {
                            filter.Member = filter.Member.Replace(JsonNetResult.BASE_COLLECTION_SUFFIX, "");
                        }
                        else if (iseasycoll)
                        {
                            filter.Member = filter.Member.Replace(JsonNetResult.EASY_COLLECTION_SUFFIX, "");
                            idname = "ObjectID";
                        }

                        if (filter.Value.ToString() != "null")
                        {
                            int id = 0;

                            if (Int32.TryParse(filter.Value.ToString(), out id))
                            {
                                if (filter.Operator == FilterOperator.IsEqualTo)
                                    q = q.Where(String.Format("it.{0}.Any({1} == {2})", filter.Member, idname, id));
                                else
                                    q = q.Where(String.Format("!it.{0}.Any({1} == {2})", filter.Member, idname, id));
                            }
                        }
                        else
                        {
                            if (filter.Operator == FilterOperator.IsEqualTo)
                                q = q.Where("!it." + filter.Member + ".Any()");
                            else
                                q = q.Where("it." + filter.Member + ".Any()");
                        }

                        //NOTE: подменим фильтр
                        filter.Member = "ID";
                        filter.Operator = FilterOperator.IsNotEqualTo;
                        filter.Value = 0;
                    }
                    else
                    {
                        if (filter.Value.ToString() == "null")
                        {
                            if (filter.Operator == FilterOperator.IsEqualTo)
                                q = q.Where("it." + filter.Member + " == null");
                            else
                                q = q.Where("it." + filter.Member + " != null");

                            //NOTE: подменим фильтр
                            filter.Member = "ID";
                            filter.Operator = FilterOperator.IsNotEqualTo;
                            filter.Value = 0;
                        }
                    }
                }
            }
        }
    }
}