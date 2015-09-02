using Base.Conference.Entities;
using Base.DAL;
using Base.DAL.EF;
using System.Data.Entity;

namespace Data.EF
{
    public class CompositeContext : EFContext
    {
        private readonly IEntityConfiguration _entityConfiguration;

        static CompositeContext()
        {
            Database.SetInitializer<CompositeContext>(new MigrateDatabaseToLatestVersion<CompositeContext, EFContextConfiguration>());
        }

        public CompositeContext()
        {
        }

        public CompositeContext(IEntityConfiguration entityConfiguration)
            : base(entityConfiguration)
        {
            _entityConfiguration = entityConfiguration;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ConferenceMessage>()
                .Ignore(x => x.RowVersion);

            modelBuilder.Entity<PublicMessage>()
                .HasOptional(x => x.ToConference)
                .WithMany(x => x.Messages)
                .HasForeignKey(x => x.ToConferenceId);

            base.OnModelCreating(modelBuilder);
        }


    }
}
