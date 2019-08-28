using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RockStar.Core.UpdateConstructor.Interfaces
{
	public interface IBaseTransactionManager
	{
		Queue<IStep> QueueToBeDone { get; }
		Queue<IStep> QueueDone { get; }
		bool IsRunning { get; }
		CancellationTokenSource cts { get; }
	}
}
