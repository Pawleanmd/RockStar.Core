using RockStar.Core.Logger;
using RockStar.Core.UpdateConstructor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RockStar.Core.UpdateConstructor.Interfaces;
using RockStar.Core.UpdateConstructor.Steps;

namespace TestApplication
{
	class Program
	{
		static void Main(string[] args)
		{
			//Создаю объект для копирования. Он включает копирование содержимого папки. Папки и файлы.
			//В этом объекте я указываю директорию для бэкапа.
						
			Loggmanager.Action = Console.WriteLine;
			string sourcePath = @"D:\UpdateSlave\updatesfolder";
			string targetPath = @"D:\UpdateSlave\serverfolder";
			string backupPath = @"D:\UpdateSlave\backupdirectory";
			Rick137 Rick = new Rick137(backupPath);
			Rick.Enqueue(sourcePath, targetPath);

			Stopwatch s = Stopwatch.StartNew();
			Thread t = new Thread(async () =>
			{
				await Rick.StartTransactionAsync();
				Console.WriteLine(s.ElapsedMilliseconds);
			});
			t.Start();
			Console.ReadLine();
		}
	
		public static List<IStep> GetStepsFromDirectory(string sourceDirectory, string destinationDirectory,string backupDirectory)
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
				destinationDirectory = Path.Combine(destinationDirectory, directoryName);
				backupDirectory = Path.Combine(backupDirectory, directoryName);
				CreateDirectoryStep cds = new CreateDirectoryStep(directoryName, destinationDirectory, backupDirectory);
				stepListToInsert.Add(cds);				
				var tempList = GetStepsFromDirectory(d, destinationDirectory, backupDirectory);
				stepListToInsert.AddRange(tempList);
			}
			//
			return stepListToInsert;
		}
	}
}
