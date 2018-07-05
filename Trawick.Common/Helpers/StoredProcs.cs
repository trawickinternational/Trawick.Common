using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Dynamic;
using System.Data.Common;
using System.Data;
using System.Data.Entity;
using Trawick.Common.Data;
namespace Trawick.Common.Helpers
{
	public static class StoredProcs
	{


		public static IEnumerable<Dictionary<string, object>> GetPolicy(int schoolId)
		{
			List<Dictionary<string, object>> data;
			SqlParameter[] parametes = new[]
			{
					new SqlParameter("schoolID", schoolId)
			};
			using (AdminContext ctx = new AdminContext())
			{
				data = SqlHelper.ExecProcedure(ctx, "spGetSchool", parametes);
			}
			return data.AsEnumerable();
		}


		// rsSchoolPolicy
		public static Dictionary<string, object> GetSchoolPolicy(int schoolId, int policyId)
		{
			List<Dictionary<string, object>> data;
			SqlParameter[] parametes = new[]
			{
					new SqlParameter("SchoolId", schoolId), 
					new SqlParameter("PolicyId", policyId)
			};
			using (AdminContext ctx = new AdminContext())
			{
				data = SqlHelper.ExecProcedure(ctx, "spGetSchoolPolicy", parametes);
			}
			return data.AsEnumerable().FirstOrDefault();
		}


		// rsPolicies
		public static IEnumerable<Dictionary<string, object>> GetSchoolPolicies(int schoolId)
		{
			List<Dictionary<string, object>> data;
			SqlParameter[] parametes = new[]
			{
					new SqlParameter("SchoolId", schoolId)
			};
			using (AdminContext ctx = new AdminContext())
			{
				data = SqlHelper.ExecProcedure(ctx, "spGetSchoolPolicies", parametes);
			}
			var rsPolicies = data.AsEnumerable();

			var FilteredPolicies = rsPolicies
				.Where(x => x["term_date"] != null && Convert.ToDateTime(x["term_date"]) >= DateTime.Now);

			var OrderedPolicies = FilteredPolicies
				.OrderByDescending(x => x["eff_date"])
				.ThenBy(x => x["policy_number"])
				.ThenBy(x => x["description"]);

			return OrderedPolicies;
			//return data.AsEnumerable();
		}


		// rsPlans
		public static IEnumerable<Dictionary<string, object>> GetPolicyPlans(int policyId)
		{
			List<Dictionary<string, object>> data;
			SqlParameter[] parametes = new[]
			{
					new SqlParameter("PolicyId", policyId)
			};
			using (AdminContext ctx = new AdminContext())
			{
				data = SqlHelper.ExecProcedure(ctx, "spGetPolicyPlans", parametes);
			}
			return data.AsEnumerable();
		}


		// rsAgeBands
		public static IEnumerable<Dictionary<string, object>> GetPlanAgeBands(int planId)
		{
			List<Dictionary<string, object>> data;
			SqlParameter[] parametes = new[]
			{
					new SqlParameter("PlanId", planId)
			};
			using (AdminContext ctx = new AdminContext())
			{
				data = SqlHelper.ExecProcedure(ctx, "spGetPlanAgeBands", parametes);
			}
			return data.AsEnumerable();
		}


		// rsRates
		public static IEnumerable<Dictionary<string, object>> GetPlanAgeBandRates(int ageBandId)
		{
			List<Dictionary<string, object>> data;
			SqlParameter[] parametes = new[]
			{
					new SqlParameter("AgeBandId", ageBandId)
			};
			using (AdminContext ctx = new AdminContext())
			{
				data = SqlHelper.ExecProcedure(ctx, "spGetPlanAgeBandRates", parametes);
			}
			var rsRates = data.AsEnumerable();

			// Filter and order by effective date
			var FilteredRates = rsRates.Where(x => x["display_rate_on_web"] != null && Convert.ToBoolean(x["display_rate_on_web"]));
			var OrderedRates = FilteredRates.OrderBy(x => x["eff_date"]);

			return OrderedRates;
			//return data.AsEnumerable();
		}


		// rsAdditionalFees
		public static IEnumerable<Dictionary<string, object>> GetAdditionalFees(int planId)
		{
			List<Dictionary<string, object>> data;
			SqlParameter[] parametes = new[]
			{
					new SqlParameter("planID", planId)
			};
			using (AdminContext ctx = new AdminContext())
			{
				data = SqlHelper.ExecProcedure(ctx, "si_spGetAdditionalFees", parametes);
			}
			return data.AsEnumerable();
		}


	}
}
