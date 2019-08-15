using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RockStar.Core.UpdateConstructor.Interfaces
{
	public interface ITransactionManager
	{
		Queue<IStep> QueueToBeDone { get; }
		Queue<IStep> QueueDone { get; }
		bool IsRunning { get; }
		CancellationTokenSource cts { get; }

		Task<IStepLog> StartTransaction();
		Task<IStepLog> StopTransaction();
		Task<IStepLog> RollBackTransaction();
	}
}
