﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RockStar.Core.UpdateConstructor.Interfaces
{
	public interface ITransactionManager: IBaseTransactionManager
	{
		IStepLog StartTransaction();
		IStepLog StopTransaction();
		IStepLog RollBackTransaction();
	}
}
