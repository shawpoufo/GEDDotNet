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
        
        /*protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }*/
    }
}
