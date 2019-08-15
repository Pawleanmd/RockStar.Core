using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RockStar.Core.UpdateConstructor;

namespace RockStar.Core.Logger
{
	public static class Loggmanager
	{
		public static Action<string>  Action { get; set; }

		public static void Log(string message)
		{
			Action.Invoke(message);
		}
		public static void Log(LogMessage message)
		{
			Action.Invoke($"State: {message.IsSuccess}, Message: {message.Message}");
		}

	}
}
