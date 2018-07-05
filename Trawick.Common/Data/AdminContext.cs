using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Trawick.Common.Data
{
	public class AdminContext : DbContext
	{

		public AdminContext() : base("name=siAdmin")
		{
			Database.SetInitializer<AdminContext>(null);
		}
		
		//public DbSet<Application> Applications { get; set; }


		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			// this option keeps table names in singular form
			//modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
			//modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

			base.OnModelCreating(modelBuilder);
		}

	}
}