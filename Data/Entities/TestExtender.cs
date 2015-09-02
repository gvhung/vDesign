using Base;
using Base.Attributes;
using Base.BusinessProcesses.Entities;
using Base.BusinessProcesses.Services.Abstract;
using Base.BusinessProcesses.Services.Concrete;
using Base.DAL;
using Base.Security;
using Base.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Data.Entities
{
    public interface ITestExtenderService : IStageExtenderService<TestExtender>
    {

    }

    public class TestExtenderService : StageExtenderService<TestExtender, TestObject>, ITestExtenderService
    {
        public TestExtenderService(IBaseObjectServiceFacade facade)
            : base(facade)
        {
        }

        protected override IObjectSaver<TestExtender> GetForSave(IUnitOfWork unitOfWork,
            IObjectSaver<TestExtender> objectSaver)
        {
            return base.GetForSave(unitOfWork, objectSaver)
                .SaveOneObject(x => x.User)
                .SaveOneToMany(x => x.Inners, saver => saver.SaveOneObject(i => i.TestObject));
        }

        public override TestExtender CloneExtender(TestExtender stageExtender)
        {
            return new TestExtender
            {
                Date = stageExtender.Date,
                UserID = stageExtender.UserID
            };
        }

        public override void OnStageEnter(ISecurityUser securityUser, ExtendedStage stage, TestExtender stageExtender,
            TestObject obj)
        {
            //obj.Title = "Enter";

            //Debug.WriteLine(string.Format("stage enter->> {0}", obj.ID.ToString()));
        }

        public override void OnStageLeave(ISecurityUser securityUser, ExtendedStage stage, TestExtender stageExtender,
            TestObject obj)
        {
            obj.Title = "Leave";

            Debug.WriteLine(string.Format("stage leave->> {0}", obj.ID.ToString()));
        }
    }

    [Table("TestExtender")]
    public class TestExtender : StageExtender
    {
        [DetailView("Дата епт")]
        public DateTime Date { get; set; }

        public int? UserID { get; set; }

        [DetailView("Пользователь")]
        public virtual User User { get; set; }

        [DetailView("TestObject")]
        public virtual ICollection<TestExtenderInner> Inners { get; set; }
    }

    public class TestExtenderInner : BaseObject
    {
        public int TestExtenderID { get; set; }
        public virtual TestExtender TestExtender { get; set; }

        [DetailView("TestObject", Required = true)]
        public virtual TestObject TestObject { get; set; }
        public int? TestObjectID { get; set; }
    }
}