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
	public class Rick137:ITransactionManager
	{ 
		#region Properties

		public Queue<IStep> QueueToBeDone { get;  }
		public Queue<IStep> QueueDone { get; }
		public bool IsRunning { get;  }
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


		public void Enqueue(IStep step)
		{
			step.BackupDirectory = BackupFolder;
			if (!IsRunning)
				QueueToBeDone.Enqueue(step);
			Loggmanager.Log("=========Step Enqueue has been added");
		}

		/// <summary>
		/// Starts and if fails rollback
		/// </summary>
		public async Task<IStepLog> StartTransaction()
		{
			var result = await StartAsync();
			if (!result.IsSuccess)
			{
				result = await RollBackAsync();				
			}
			Loggmanager.Log(result.Message);
			return result;
		}

		/// <summary>
		/// Stops with rollback
		/// </summary>
		public async Task<IStepLog> StopTransaction()
		{
			cts.Cancel();
			var result = await RollBackAsync();
			Loggmanager.Log(result.Message);
			return result;
		}

		/// <summary>
		/// Rollback
		/// </summary>
		public async Task<IStepLog> RollBackTransaction()
		{
			var result = await RollBackAsync();
			Loggmanager.Log(result.Message);
			return result;
		}


		#region Private emethods

		private async Task<IStepLog> StartAsync()
		{
			IStepLog logM = new LogMessage() { IsSuccess = true, Message = "Transaction finished with success" };
			var task = new TaskFactory().StartNew<IStepLog>(() =>
			{
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
				return logM;
			});
			return task.Result;
		}

		private async Task<IStepLog> RollBackAsync()
		{
			var task = new TaskFactory().StartNew<IStepLog>(() =>
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
			});
			return task.Result;
		}

		#endregion
	}
}
