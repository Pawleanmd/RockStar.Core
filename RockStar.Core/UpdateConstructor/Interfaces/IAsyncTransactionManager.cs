using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RockStar.Core.UpdateConstructor.Interfaces
{
	public interface IAsyncTransactionManager:IBaseTransactionManager
	{
		Task<IStepLog> StartTransactionAsync();
		Task<IStepLog> StopTransactionAsync();
		Task<IStepLog> RollBackTransactionAsync();
	}
}
