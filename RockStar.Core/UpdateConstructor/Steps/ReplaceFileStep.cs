using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RockStar.Core.Logger;
using RockStar.Core.UpdateConstructor.Interfaces;

namespace RockStar.Core.UpdateConstructor.Steps
{
	public class ReplaceFileStep : IStep
	{
		//Есть приватные методы - вних весь функционалд
		//Есть публичные - методы интерфейса ISTEP.
		//При старте, существуют три директории. ОТКУДА КОПИРОВАТЬ, КУДА КОПИРОВАТЬ, БЭКАП ДИРЕКТОРИЯ
		//и имя самого файла.
		// Сначала фйал бэкапится, потом из ОТКУДА директории копируется в БЭКАП.
		// Потом из ОТКУДА копируется в КУДА
		// Если появляются ошибки, из БЭКАП директории файт копируется в КУДА директори.
		
		#region Properties

		private string FileName { get; set; }
		private string SourceDirectory { get; set; }
		private string DestinationDirectory { get; set; }
		public string BackupDirectory { get; set; }

		private string FullTargetPath { get; set; }
		private string FullFilePath { get; set; }

		#endregion

		#region Constructors

		public ReplaceFileStep(string fileName, string sourceDirectory, string destinationDirectory)
		{
			Loggmanager.Log("Replace instance is creating");
			FileName = fileName;
			SourceDirectory = sourceDirectory;
			DestinationDirectory = destinationDirectory;
			Loggmanager.Log("Replace instance is created");
		}

		#endregion

		#region Private methods

		private LogMessage ReplaceFile(string fileName, string sourceDirectory, string destinationDirectory)
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

		private LogMessage BackupFile(string fileName, string sourceDirectory, string destinationDirectory)
		{
			if (!Directory.Exists(BackupDirectory))
			{
				Directory.CreateDirectory(BackupDirectory);
			}
			Loggmanager.Log($"==BACKUP the file:{Path.Combine(sourceDirectory, fileName)} to: {Path.Combine(destinationDirectory, fileName)} directory");
			var result = ReplaceFile(fileName, sourceDirectory, BackupDirectory);
			Loggmanager.Log(result);
			Loggmanager.Log($"==BACKUP file :{Path.Combine(sourceDirectory, fileName)} to: {Path.Combine(destinationDirectory, fileName)} directory has been completed with state: {result.IsSuccess}, Message: {result.Message}");
			//
			return result;
		}

		private LogMessage RollBackFile(string fileName, string backupDirectory, string targetDirectory)
		{
			Loggmanager.Log($"========== ROLLBACK the file:{Path.Combine(backupDirectory, fileName)} to: {Path.Combine(targetDirectory, fileName)} directory");
			var result = ReplaceFile(fileName, backupDirectory, targetDirectory);
			//
			
			Loggmanager.Log(result);
			Loggmanager.Log($"========== ROLLBACK the file:{Path.Combine(backupDirectory, fileName)} to: {Path.Combine(targetDirectory, fileName)} directory");
			//
			return result;
		}
		
		#endregion


		public IStepLog Do()
		{
			IStepLog logM = ReplaceFile(FileName, SourceDirectory, DestinationDirectory);
			return logM;
		}

		public IStepLog RollBack()
		{
			IStepLog logM = RollBackFile(FileName, BackupDirectory, DestinationDirectory);
			return logM;
		}

		public IStepLog Backup()
		{
			IStepLog logM = BackupFile(FileName, DestinationDirectory, BackupDirectory);
			return logM;
		}
	}
}
