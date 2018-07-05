using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace Trawick.Common.Helpers
{
	public static class SqlHelper
	{

		public static List<Dictionary<string, object>> ExecProcedure(DbContext context, string name, SqlParameter[] param)
		{
			return GetTableRows(QueryToTable(context, name, param, true));
		}


		public static SqlParameter ToParam(this object v, string name)
		{
			return new SqlParameter(name, v);
		}


		public static SqlParameter[] ToParams(Dictionary<string, object> param)
		{
			var parameters = new List<SqlParameter>();
			if (param != null)
			{
				foreach (var item in param)
				{
					parameters.Add(new SqlParameter(item.Key, item.Value));
				}
			}
			return parameters.ToArray();
		}


		private static DataTable QueryToTable(DbContext db, string query, SqlParameter[] param, bool isProc = false)
		{
			using (DbDataAdapter adapter = new SqlDataAdapter())
			{
				adapter.SelectCommand = db.Database.Connection.CreateCommand();
				adapter.SelectCommand.CommandText = query;
				if (isProc)
					adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
				if (param != null)
					adapter.SelectCommand.Parameters.AddRange(param);
				DataTable table = new DataTable();
				adapter.Fill(table);
				return table;
			}
		}


		private static List<Dictionary<string, object>> GetTableRows(DataTable data)
		{
			var table = new List<Dictionary<string, object>>();
			foreach (DataRow dr in data.Rows)
			{
				var row = new Dictionary<string, object>();
				foreach (DataColumn col in data.Columns)
				{
					row.Add(col.ColumnName, dr[col]);
				}
				table.Add(row);
			}
			return table;
		}

	}
}