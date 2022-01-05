namespace ProjetGED
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using ProjetGED.Models;
    using System.Data.SqlClient;

    public partial class GEDContext : DbContext
    {
        public GEDContext()
            : base("name=EFGEDModel")
        {
            
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<GEDContext>());
            
        }
        public DbSet<User> Users { get; set; }
        
        /*protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }*/
    }
}
