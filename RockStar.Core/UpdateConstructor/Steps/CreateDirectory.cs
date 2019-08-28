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

		public CreateDirectoryStep(string directoryName, string destinationDirectory)
		{
			DirectoryName = directoryName;
			DestinationDirectory = destinationDirectory;
		}

		#endregion


		public IStepLog Backup()
		{
			return new LogMessage() { IsSuccess = true };
		}

		public IStepLog Do()
		{
			IStepLog logM = CreateDirectory(DirectoryName, DestinationDirectory);
			return logM;
		}

		public IStepLog RollBack()
		{
			return new LogMessage(){IsSuccess = true};
		}

		private LogMessage CreateDirectory(string directoryname, string destinationPath)
		{
			if (!Directory.Exists(destinationPath))
			{
				return new LogMessage() {IsSuccess = false, Message = $"Target directory: {DestinationDirectory} is not exists"};
			}
			try
			{
				Directory.CreateDirectory(Path.Combine(destinationPath, directoryname));
				return new LogMessage() { IsSuccess = true, Message = $"Directory: {Path.Combine(destinationPath, directoryname)} was created" };
			}
			catch (Exception ex)
			{
				return new LogMessage() { IsSuccess = false, Message = $"Directory: {Path.Combine(destinationPath, directoryname)} creation was ended with exception: {ex.Message}" };
			}			
		}
	}
}
