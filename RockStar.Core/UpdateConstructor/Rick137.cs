using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RockStar.Core.Logger;
using RockStar.Core.UpdateConstructor.Interfaces;

namespace RockStar.Core.UpdateConstructor
{
	public class Rick137:ITransactionManager,IAsyncTransactionManager
	{ 
		#region Properties

		public Queue<IStep> QueueToBeDone { get;  }
		public Queue<IStep> QueueDone { get; }
		public bool IsRunning { get; private set; }
		public CancellationTokenSource cts { get; }

		private string BackupFolder { get; set; }

		#endregion


		#region Constructor

		public Rick137()
		{
			cts = new CancellationTokenSource();
			QueueToBeDone = new Queue<IStep>();
			QueueDone = new Queue<IStep>();
			BackupFolder = "C:\\TempFolderForBackup";
		}

		#endregion

		public void EnqueueRange(IEnumerable<IStep> steps)
		{
			foreach (var s in steps)
			{
				Enqueue(s);
			}
		}

		public void Enqueue(IStep step)
		{
			step.BackupDirectory = BackupFolder;
			if (!IsRunning)
				QueueToBeDone.Enqueue(step);
			Loggmanager.Log("=========Step Enqueue has been added");
		}

	
		public IStepLog StartTransaction()
		{
			var result = Start();
			if (!result.IsSuccess)
			{
				result = RollBack();
			}
			return result;
		}

		public IStepLog StopTransaction()
		{
			cts.Cancel();
			var result = RollBack();
			return result;
		}

		public IStepLog RollBackTransaction()
		{
			var result = RollBack();
			return result;
		}


		//Implementation of async transaction manager
		#region Async wrapper

		public async Task<IStepLog> StartTransactionAsync()
		{
			var result = await StartAsync();
			if (!result.IsSuccess)
			{
				result = await RollBackAsync();
			}
			Loggmanager.Log(result.Message);
			return result;
		}

		public async Task<IStepLog> StopTransactionAsync()
		{
			cts.Cancel();
			var result = await RollBackAsync();
			Loggmanager.Log(result.Message);
			return result;
		}

		public async Task<IStepLog> RollBackTransactionAsync()
		{
			var result = await RollBackAsync();
			Loggmanager.Log(result.Message);
			return result;
		}

		#endregion


		#region Private methods

		//Core methods
		#region Main

		private IStepLog Start()
		{			
			IStepLog logM = new LogMessage() { IsSuccess = true, Message = $"Transaction finished with success" };
			IsRunning = true;
			while (QueueToBeDone.Count != 0)
			{
				if (cts.IsCancellationRequested)
				{
					QueueToBeDone.Clear();
					logM = new LogMessage() { IsSuccess = false, Message = "Transaction cancelled by user" };
					break;
				}
				//
				var item = QueueToBeDone.Dequeue();
				var backuped = item.Backup();
				if (!backuped.IsSuccess)
				{
					logM = backuped;
					break;
				}
				var done = item.Do();
				if (!done.IsSuccess)
				{
					logM = done;
					break;
				}
			}
			IsRunning = false;
			//
			return logM;
		}
		private IStepLog RollBack()
		{
			IsRunning = true;
			LogMessage logM = new LogMessage() { IsSuccess = true, Message = $"rollback step has finished with success" };
			while (QueueDone.Count != 0)
			{
				var item = QueueDone.Dequeue();
				var rollbackResult = item.RollBack();
				if (!rollbackResult.IsSuccess)
				{
					logM = new LogMessage() { IsSuccess = false, Message = $"rollback step has failed. Message: {rollbackResult.Message} " };
					break;
				}
			}
			IsRunning = false;
			return logM;
		}

		#endregion

		//Async implementation of private core methods
		#region Async Wrapper

		private async Task<IStepLog> StartAsync()
		{
			var task = new TaskFactory().StartNew<IStepLog>(() => { return Start(); });
			return task.Result;
		}

		private async Task<IStepLog> RollBackAsync()
		{
			var task = new TaskFactory().StartNew<IStepLog>(() => { return RollBack(); });
			return task.Result;
		}

		#endregion

		#endregion
	}
}
