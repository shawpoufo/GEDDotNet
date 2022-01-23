namespace ProjetGED
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using ProjetGED.Models;
    using System.Data.SqlClient;
    using Microsoft.AspNet.Identity.EntityFramework;

    public partial class GEDContext : IdentityDbContext<ApplicationUser>
    {
        public GEDContext(): base("name=EFGEDModel")
        {
            
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<GEDContext>());
            
        }

        public DbSet<User> OurUsers { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Folder> Folders { get; set; }
        public DbSet<AccessFolder> AccessFolders { get; set; }
        public DbSet<AccessDocument> AccessDocuments { get; set; }
        public object User { get; internal set; }

        


        /*protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Folder>()
                       .HasMany(f => f.Documents)
                       .WithOptional()
                       .WillCascadeOnDelete(false);
        }*/
    }
}
