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
        public GEDContext()
            : base("name=EFGEDModel")
        {
            
            Database.SetInitializer(new DropCreateDatabaseAlways<GEDContext>());
            
        }
        public DbSet<User> OurUsers { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Folder> Folders { get; set; }
        public DbSet<FolderPrivilege> FolderPrivileges { get; set; }
        public DbSet<DocumentPrivilege> DocumentPrivileges { get; set; }
        
        /*protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }*/
    }
}
