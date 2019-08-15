using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RockStar.Core.Logger;
using RockStar.Core.UpdateConstructor.Interfaces;

namespace RockStar.Core.UpdateConstructor
{
	public class ReplaceDirectoryStep : IStep, ITransactionManager
	{
		#region Properties

		public string BackupDirectory { get; set; }
		public Queue<IStep> QueueToBeDone { get; private set; }
		public Queue<IStep> QueueDone { get; private set; }
		public bool IsRunning { get; private set; }
		public CancellationTokenSource cts { get; }

		#endregion


		public IStepLog Backup()
		{
			
		}

		public IStepLog Do()
		{
			
		}

		public IStepLog RollBack()
		{
			
		}

		public Task<IStepLog> RollBackTransaction()
		{
			throw new NotImplementedException();
		}

		public Task<IStepLog> StartTransaction()
		{
			throw new NotImplementedException();
		}

		public Task<IStepLog> StopTransaction()
		{
			throw new NotImplementedException();
		}

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

		private LogMessage ReplaceDirectory(string fileName, string sourceDirectory, string destinationDirectory)
		{
			try
			{
				Loggmanager.Log($"REPLACING started the file: {Path.Combine(sourceDirectory, fileName)} to: {Path.Combine(destinationDirectory, fileName)} directory");
				if (!Directory.Exists(sourceDirectory))
				{
					LogMessage l = new LogMessage();
					l.IsSuccess = false;
					l.Message = $"Directory for copying :{sourceDirectory} is not exists";
					return l;
				}
				//
				FullFilePath = Path.Combine(sourceDirectory, fileName);
				if (!File.Exists(FullFilePath))
				{
					LogMessage l = new LogMessage();
					l.IsSuccess = false;
					l.Message = $"File for copying :{FullFilePath} is not exists";
					return l;
				}
				//
				if (!Directory.Exists(destinationDirectory))
				{
					LogMessage l = new LogMessage();
					l.IsSuccess = false;
					l.Message = $"Cant create inexistent folder :{destinationDirectory} is not exists";
					return l;
				}
				FullTargetPath = Path.Combine(destinationDirectory, fileName);
				File.Copy(FullFilePath, FullTargetPath, true);
				Loggmanager.Log($"REPLACING ended the file: {Path.Combine(sourceDirectory, fileName)} to: {Path.Combine(destinationDirectory, fileName)} directory");
				//
				return new LogMessage() { IsSuccess = true };
			}
			catch (Exception ex)
			{
				LogMessage l = new LogMessage();
				l.IsSuccess = false;
				l.Message = $"Exception while : {ex.Message}, Internal exception: {ex.InnerException?.Message}";
				//
				return l;
			}
		}

		private LogMessage Backup_Directory(string fileName, string sourceDirectory, string destinationDirectory)
		{
			if (!Directory.Exists(BackupDirectory))
			{
				Directory.CreateDirectory(BackupDirectory);
			}
			Loggmanager.Log($"==BACKUP the file:{Path.Combine(sourceDirectory, fileName)} to: {Path.Combine(destinationDirectory, fileName)} directory");
			var result = ReplaceDirectory(fileName, sourceDirectory, BackupDirectory);
			Loggmanager.Log(result);
			Loggmanager.Log($"==BACKUP file :{Path.Combine(sourceDirectory, fileName)} to: {Path.Combine(destinationDirectory, fileName)} directory has been completed with state: {result.IsSuccess}, Message: {result.Message}");
			//
			return result;
		}

		private LogMessage RollBackDirectory(string fileName, string backupDirectory, string targetDirectory)
		{
			Loggmanager.Log($"========== ROLLBACK the file:{Path.Combine(backupDirectory, fileName)} to: {Path.Combine(targetDirectory, fileName)} directory");
			var result = ReplaceFile(fileName, backupDirectory, targetDirectory);
			//

			Loggmanager.Log(result);
			Loggmanager.Log($"========== ROLLBACK the file:{Path.Combine(backupDirectory, fileName)} to: {Path.Combine(targetDirectory, fileName)} directory");
			//
			return result;
		}
	}
}
