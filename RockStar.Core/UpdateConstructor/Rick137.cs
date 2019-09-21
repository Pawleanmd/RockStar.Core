using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RockStar.Core.Logger;
using RockStar.Core.UpdateConstructor.Interfaces;
using RockStar.Core.UpdateConstructor.Steps;

namespace RockStar.Core.UpdateConstructor
{
	public class Rick137:ITransactionManager,IAsyncTransactionManager
	{ 
		#region Properties

		public Queue<IStep> QueueToBeDone { get;  }
		public Queue<IStep> QueueDone { get; }
		public bool IsRunning { get; private set; }
		public CancellationTokenSource cts { get; }
		public string BackupFolder { get; private set; }
		private string CurrentTransactionBackupFolder { get; set; }

		#endregion


		#region Constructor

		public Rick137(string backupRootFolder)
		{
			cts = new CancellationTokenSource();
			QueueToBeDone = new Queue<IStep>();
			QueueDone = new Queue<IStep>();
			BackupFolder = backupRootFolder;
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
			if (!IsRunning)
				QueueToBeDone.Enqueue(step);
			Loggmanager.Log("=========Step Enqueue has been added");
		}

		public void Enqueue(string sourceDirectory, string destinationDirectory)
		{
			string currentDate = DateTime.Now.ToString("HH.mm.ss_dd.MM.yyyy");
			string backupDirectoryName = $"{Guid.NewGuid().ToString()}_{currentDate}";
			string backupDirectorypath = Path.Combine(BackupFolder, backupDirectoryName);
			var steps = GetStepsFromDirectory(sourceDirectory, destinationDirectory, backupDirectorypath);
			
			this.EnqueueRange(steps);
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
			try
			{
				IStepLog logM = new LogMessage() {IsSuccess = true, Message = $"Transaction finished with success"};
				IsRunning = true;
				int a = 0;
				while (QueueToBeDone.Count != 0)
				{
					if (a == 8)
						throw new Exception();
					if (cts.IsCancellationRequested)
					{
						QueueToBeDone.Clear();
						throw new Exception($"Transaction cancelled by user");
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
					QueueDone.Enqueue(item);
				}

				IsRunning = false;
				//
				return logM;
			}
			catch (Exception ex)
			{
				IStepLog logM = new LogMessage() { IsSuccess = false, Message = $"{ex.Message}" };
				return logM;
			}			
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

		private List<IStep> GetStepsFromDirectory(string sourceDirectory, string destinationDirectory, string backupDirectory)
		{
			List<IStep> stepListToInsert = new List<IStep>();
			//		
			foreach (string filePath in Directory.GetFiles(sourceDirectory))
			{
				string sourceFileName = Path.GetFileName(filePath);
				ReplaceFileStep rfs = new ReplaceFileStep(sourceFileName, sourceDirectory, destinationDirectory, backupDirectory);
				//
				stepListToInsert.Add(rfs);
			}
			foreach (var d in Directory.GetDirectories(sourceDirectory))
			{
				string directoryName = d.Split('\\').LastOrDefault();
				CreateDirectoryStep cds = new CreateDirectoryStep(directoryName, destinationDirectory, backupDirectory);
				destinationDirectory = Path.Combine(destinationDirectory, directoryName);
				backupDirectory = Path.Combine(backupDirectory, directoryName);
				stepListToInsert.Add(cds);
				var tempList = GetStepsFromDirectory(d, destinationDirectory, backupDirectory);
				stepListToInsert.AddRange(tempList);
			}
			//
			return stepListToInsert;
		}

		#endregion
	}
}
