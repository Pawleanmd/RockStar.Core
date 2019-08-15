using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RockStar.Core.UpdateConstructor.Interfaces;

namespace RockStar.Core.UpdateConstructor
{
	public class LogMessage: IStepLog
	{
		public LogMessage()
		{
				
		}
		public bool IsSuccess { get; set; }
		public string Message { get; set; }
	}
}
