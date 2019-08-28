using RockStar.Core.Logger;
using RockStar.Core.UpdateConstructor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
			
			Rick137 Rick = new Rick137();
			Loggmanager.Action = Console.WriteLine;
			string path = "C:\\Alexei\\TaxiServices\\TaxiService1\\Local";
			//DirSearch(path);
			var steps = GetStepsFromDirectory(path, "C:\\test");

			Rick.EnqueueRange(steps);
			Stopwatch s = Stopwatch.StartNew();
			Thread t = new Thread(async () =>
			{
				await Rick.StartTransactionAsync();
				Console.WriteLine(s.ElapsedMilliseconds);
			});
			t.Start();
			Console.ReadLine();
		}
	
		public static List<IStep> GetStepsFromDirectory(string sourceDirectory, string destinationDirectory)
		{
			List<IStep> stepListToInsert = new List<IStep>();
			//
			foreach (var f in Directory.GetFiles(sourceDirectory))
			{
				string fileName = Path.GetFileName(f);
				string directory = Path.GetDirectoryName(f);
				ReplaceFileStep rfs = new ReplaceFileStep(fileName, directory, destinationDirectory);
				//
				stepListToInsert.Add(rfs);
			}
			foreach (var d in Directory.GetDirectories(sourceDirectory))
			{				
				string[] parts = d.Split('\\');
				CreateDirectoryStep cds = new CreateDirectoryStep(parts.Last(), destinationDirectory);
				stepListToInsert.Add(cds);
				var tempList = GetStepsFromDirectory(d, Path.Combine(destinationDirectory,d));
				stepListToInsert.AddRange(tempList);
			}
			//
			return stepListToInsert;
		}
	}
}
