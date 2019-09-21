using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Management.Smo;
using RockStar.Core.UpdateConstructor.Interfaces;

namespace RockStar.Core.UpdateConstructor.Steps
{
	public class BackupDatabaseStep : IStep
	{

		private void RestoreDatabase(string connectionString)
		{
			Server s = new Server();
		}

		public IStepLog Backup()
		{
			throw new NotImplementedException();
		}

		public IStepLog Do()
		{
			throw new NotImplementedException();
		}

		public IStepLog RollBack()
		{
			throw new NotImplementedException();
		}
	}
}
