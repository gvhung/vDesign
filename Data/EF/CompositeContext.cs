using System;
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
            //_entityConfiguration = new EntityConfiguration();
            //Config.Init(_entityConfiguration);
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

            //foreach (var config in _entityConfiguration.GetContextConfig(this))
            //{
            //    var method = modelBuilder.GetType().GetMethod("Entity");
            //    method = method.MakeGenericMethod(new Type[] { config.EntityType });
            //    method.Invoke(modelBuilder, null);
            //}

            //modelBuilder.Conventions.Add(new NonPublicColumnAttributeConvention());
        }


    }
}
