﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RockStar.Core.UpdateConstructor.Interfaces
{
	public interface IStepLog
	{
		bool IsSuccess { get; set; }
		string Message { get; set; }
	}
}
