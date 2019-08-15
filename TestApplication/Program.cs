using RockStar.Core.Logger;
using RockStar.Core.UpdateConstructor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using RockStar.Core.UpdateConstructor.Steps;

namespace TestApplication
{
	class Program
	{
		static void Main(string[] args)
		{
			//Создаю объект для копирования. Он включает копирование содержимого папки. Папки и файлы.
			//В этом объекте я указываю директорию для бэкапа.
			//
			//
			//
			Rick137 Rick = new Rick137();
			Loggmanager.Action = Console.WriteLine;
			ReplaceFileStep step = new ReplaceFileStep("testFile.txt", "C:\\Alexei", "C:\\Alexei\\targetdirectory");
			Rick.Enqueue(step);
			Console.WriteLine("Transaction started");
			Rick.StartTransaction();
			Console.WriteLine("Transaction stopped");

			Console.ReadLine();
		}
	}
}
