using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using RockStar.Core.UpdateConstructor.Interfaces;

namespace RockStar.Core.UpdateConstructor.Steps
{
	public class CreateDirectoryStep : IStep
	{
		#region Properties

		public string DirectoryName { get; set; }
		public string DestinationDirectory { get; set; }
		public string BackupDirectory { get; set; }

		#endregion

		#region Constructors

		public CreateDirectoryStep(string directoryName, string destinationDirectory,string backupDirectory)
		{
			DirectoryName = directoryName;
			DestinationDirectory = destinationDirectory;
			BackupDirectory = backupDirectory;
		}

		#endregion


		public IStepLog Backup()
		{			
			return new LogMessage() { IsSuccess = true };
		}

		public IStepLog Do()
		{
			throw new Exception("Vasyan");
			IStepLog logM = CreateDirectory(DirectoryName, DestinationDirectory);
			return logM;
		}

		public IStepLog RollBack()
		{
			return new LogMessage(){IsSuccess = true};
		}

		private LogMessage CreateDirectory(string directoryname, string destinationPath)
		{
			string fullDirectoryName = Path.Combine(BackupDirectory, DirectoryName);
			Directory.CreateDirectory(fullDirectoryName);

			return new LogMessage() { IsSuccess = true, Message = $"Directory: {Path.Combine(destinationPath, directoryname)} was created" };			
		}
	}
}
