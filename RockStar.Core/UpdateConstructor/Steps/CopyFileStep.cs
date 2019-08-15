using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RockStar.Core.UpdateConstructor.Interfaces;

namespace RockStar.Core.UpdateConstructor.Steps
{
	public class CopyFileStep : IStep
	{
		#region Properties

		public string TempFolder { get; set; }
		private string UID { get; set; }
		public string BackupDirectory { get; set; }

		#endregion

		#region Constructors

		public CopyFileStep()
		{
			TempFolder = "C:\\temp\\";
			UID = Guid.NewGuid().ToString();
		}

		#endregion

		#region Private Methods

		private LogMessage CopyFile(string fileName, string fileDirectory, string targetDirectory)
		{
			try
			{
				if (!Directory.Exists(fileDirectory))
				{
					LogMessage l = new LogMessage();
					l.IsSuccess = false;
					l.Message = $"Directory for copying :{fileDirectory} is not exists";
				}
				//
				string fullFilePath = Path.Combine(fileDirectory, fileName);
				if (!File.Exists(fullFilePath))
				{
					LogMessage l = new LogMessage();
					l.IsSuccess = false;
					l.Message = $"File for copying :{fullFilePath} is not exists";
				}
				//
				if (!Directory.Exists(targetDirectory))
				{
					Directory.CreateDirectory(targetDirectory);
				}
				//
				if (!Directory.Exists(targetDirectory))
				{
					LogMessage l = new LogMessage();
					l.IsSuccess = false;
					l.Message = $"Cant create inexistent folder :{targetDirectory} is not exists";
				}
				//
				string fullTargetPath = Path.Combine(fileDirectory, fileName);
				File.Copy(fullFilePath, fullTargetPath, true);
				//
				return new LogMessage() { IsSuccess = true };
			}
			catch (Exception ex)
			{
				LogMessage l = new LogMessage();
				l.IsSuccess = false;
				l.Message = $"Exception while :{ex.Message}, Internal exception: {ex.InnerException?.Message}";
				//
				return l;
			}
		}


		private LogMessage RollBack(string fileName, string fileDirectory, string targetDirectory)
		{
			try
			{

				//
				return new LogMessage() { IsSuccess = true };
			}
			catch (Exception ex)
			{
				LogMessage l = new LogMessage();
				l.IsSuccess = false;
				l.Message = $"Exception while :{ex.Message}, Internal exception: {ex.InnerException?.Message}";
				//
				return l;
			}
		}

		#endregion

		public IStepLog Do()
		{
			return null;
		}

		public IStepLog RollBack()
		{
			return null;
		}

		public IStepLog Backup()
		{
			return null;
		}
	}
}
