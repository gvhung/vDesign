using Base;
using Base.Audit.Entities;
using Base.BusinessProcesses.Entities;
using Base.BusinessProcesses.Entities.Steps;
using Base.Conference.Entities;
using Base.Content.Entities;
using Base.DAL;
using Base.DAL.EF;
using Base.Document.Entities;
using Base.Event.Entities;
using Base.FileStorage;
using Base.Forum.Entities;
using Base.Help.Entities;
using Base.LinkedObjects.Entities;
using Base.Notification.Entities;
using Base.OpenID.Entities;
using Base.Security;
using Base.Security.ObjectAccess;
using Base.Settings;
using Base.Task.Entities;
using Base.UI;
using Data.EF;
using Data.Entities;
using Data.Entities.Workgroup;
using Branch = Base.BusinessProcesses.Entities.Branch;
using Event = Base.Event.Entities.Event;
using Task = Base.Task.Entities.Task;

namespace Data
{
    public class Config
    {
        public static void Init(IEntityConfiguration config)
        {
            config.Config()
                .Context<CompositeContext>()
                .Repository<EFRepository<BaseObject>>()
                .Entity<User>()
                .Entity<UserCategory>()
                .Entity<Role>()
                .Entity<UserFriend>()
                .Entity<ChildRole>()
                .Entity<Permission>()
                .Entity<Post>()
                .Entity<Department>()
                .Entity<UserPromotion>()

                .Entity<PresetRegistor>()
                .Entity<SettingCategory>()
                .Entity<SettingItem>()

                .Entity<FileData>()
                .Entity<FileStorageItem>()
                .Entity<FileStorageCategory>()

                .Entity<LinkBaseObjects>()
                .Entity<LinkedDocument>()

                .Entity<Task>()
                .Entity<TaskFile>()
                .Entity<TaskCategory>()

                .Entity<StageAction>()
                .Entity<ActionRole>()
                .Entity<ChangeHistory>()
                .Entity<AgreementItem>()
                .Entity<TemplateMacroItem>()
                .Entity<TemplateAction>()
                .Entity<Output>()
                .Entity<StageActionInitItem>()
                .Entity<StageUser>()
                .Entity<BPTask>()
                .Entity<CreateObjectStep>()
                .Entity<CreateObjectStepMemberInitItem>()
                .Entity<Branch>()
                .Entity<BranchConditionItem>()
                .Entity<BranchingStep>()
                .Entity<RestoreStep>()
                .Entity<Step>()
                .Entity<Stage>()
                .Entity<ExtendedStage>()
                .Entity<StageTemplate>()
                .Entity<EndStep>()
                .Entity<WorkflowOwnerStep>()
                .Entity<Workflow>()
                .Entity<WorkflowCategory>()
                .Entity<TaskStepUser>()
                .Entity<TaskStep>()
                .Entity<GotoStep>()
                .Entity<ParallelizationStep>()
                .Entity<ParallelEndStep>()
                .Entity<WorkflowContext>()
                .Entity<StageInitItems>()
                .Entity<StageActionValidationItem>()
                .Entity<StagePerform>()
                .Entity<StageUserCategory>()

                .Entity<TestObject>()
                .Entity<TestExtender>()
                .Entity<TestExtenderInner>()
                .Entity<TestObjectEntry>()
                .Entity<TestObjectNestedEntry>()

                .Entity<Notification>()
                .Entity<DocumentTemplate>()
                .Entity<DocumentTemplateCategory>()
                .Entity<AuditItem>()
                .Entity<ObjectAccessItem>()
                .Entity<UserAccess>()
                .Entity<UserCategoryAccess>()
                .Entity<Event>()
                .Entity<EventType>()
                .Entity<ContentCategory>()
                .Entity<ContentItem>()
                .Entity<Tag>()
                .Entity<TagCategory>()
                .Entity<ContentSubscriber>()
                .Entity<Okved>()
                .Entity<Weekend>()

                .Entity<ExtAccount>()

                .Entity<HelpItem>()
                .Entity<HelpItemTag>()

                .Entity<DetailViewSetting>()
                .Entity<FieldSetting>()
                .Entity<FieldRoleVisible>()
                .Entity<FieldRoleEnable>()

                .Entity<ForumPost>()
                .Entity<ForumSection>()
                .Entity<ForumTopic>()

                .Entity<Expert>()
                .Entity<ExpertStatus>()
                .Entity<WorkGroup>()
                .Entity<WorkGroupExpert>()
                .Entity<ExpertStatusInWorkGroup>()

                .Entity<Conference>()
                .Entity<ConferenceMember>()
                .Entity<ConferenceMessage>()
                .Entity<PublicMessage>()
                .Entity<PrivateMessage>();

        }
    }
}