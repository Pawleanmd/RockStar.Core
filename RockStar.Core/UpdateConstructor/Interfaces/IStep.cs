using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RockStar.Core.UpdateConstructor.Interfaces
{
	public interface IStep
	{
		string BackupDirectory { get; set; }
		IStepLog Backup();
		IStepLog Do();
		IStepLog RollBack();
	}
}
