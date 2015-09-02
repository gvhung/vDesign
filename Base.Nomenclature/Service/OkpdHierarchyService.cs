using Base.DAL;
using Base.Nomenclature.Entities;
using Base.Service;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Base.Nomenclature.Service
{
    public class OkpdHierarchyService : BaseCategoryService<OkpdHierarchy>, IOkpdHierarchyService
    {
        public OkpdHierarchyService(IBaseObjectServiceFacade facade) : base(facade) { }

        public void PopulateOkpdDb(IUnitOfWork unitOfWork, string path)
        {
            IEnumerable<OkpdHierarchy> okpds = OkpdFromXml(path);
            this.CreateCollection(unitOfWork, okpds);

            IEnumerable<OkpdHierarchy> dbOkpds = this.GetAll(unitOfWork).OrderBy(x => x.ID);
            int i = 0;
            var updatedCollection = new List<OkpdHierarchy>();
            foreach (OkpdHierarchy item in dbOkpds)
            {
                item.SetParent(item.Parent_);
                updatedCollection.Add(item);
                i++;
                if (i % 100 == 0)
                {
                    this.UpdateCollection(unitOfWork, updatedCollection);
                    updatedCollection.Clear();
                }
            }
            this.UpdateCollection(unitOfWork, updatedCollection);
        }


        public ICollection<OkpdHierarchy> OkpdFromXml(string path)
        {
            XDocument xDoc = XDocument.Load(path);

            return xDoc.Root != null
                ? xDoc.Root.Elements("OKPD").Select(ParseOkpdNode).ToList()
                : new List<OkpdHierarchy>();
        }

        private static OkpdHierarchy ParseOkpdNode(XElement xOkpd)
        {
            OkpdHierarchy okpd = new OkpdHierarchy()
            {
                Code = xOkpd.Attribute("Code").Value,
                Name = xOkpd.Attribute("Text").Value
            };

            if (!xOkpd.HasElements) return okpd;

            foreach (OkpdHierarchy subOkpd in xOkpd.Elements().Select(ParseOkpdNode))
            {
                if (okpd.Children_ == null)
                {
                    okpd.Children_ = new List<OkpdHierarchy>();
                }

                okpd.Children_.Add(subOkpd);

                subOkpd.Parent_ = okpd;
            }

            return okpd;
        }
    }
}
