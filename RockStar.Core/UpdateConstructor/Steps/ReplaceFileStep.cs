using System;
using System.Collections.Generic;
using System.Data;
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
		private string BackupDirectory { get; set; }

		#endregion

		#region Constructors

		public ReplaceFileStep(string sourceFileName, string sourceDirectory, string destinationDirectory,string backupDirectory)
		{
			FileName = sourceFileName;
			SourceDirectory = sourceDirectory;
			DestinationDirectory = destinationDirectory;
			BackupDirectory = backupDirectory;		
		}

		#endregion

		#region Private methods

		private LogMessage ReplaceFile(string fileName, string sourceDirectory, string destinationDirectory)
		{				
			if (!Directory.Exists(sourceDirectory))
			{
				throw new DirectoryNotFoundException($"Directory for copying :{sourceDirectory} is not exists");
			}
			//
			string fullFilePath = Path.Combine(sourceDirectory, fileName);
			if (!File.Exists(fullFilePath))
			{
				throw new FileNotFoundException($"File for copying :{fullFilePath} is not exists");
			}
			//
			if (!Directory.Exists(destinationDirectory))
			{
				throw new DirectoryNotFoundException($"Cant create inexistent folder :{destinationDirectory} is not exists");
			}
			//
			string fullTargetPath = Path.Combine(destinationDirectory, fileName);
			File.Copy(fullFilePath, fullTargetPath, true);

			//
			return new LogMessage() { IsSuccess = true };
		}

		private LogMessage BackupFile(string fileName, string sourceDirectory, string destinationDirectory)
		{
			if (!Directory.Exists(BackupDirectory))
			{
				Directory.CreateDirectory(BackupDirectory);
			}
			var result = ReplaceFile(fileName, destinationDirectory, BackupDirectory);
			Loggmanager.Log(result);
			//
			return result;
		}

		private LogMessage RollBackFile(string fileName, string backupDirectory, string targetDirectory)
		{
			
			var result = ReplaceFile(fileName, backupDirectory, targetDirectory);
			//			
			Loggmanager.Log(result);
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
