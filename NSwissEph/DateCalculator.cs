using System;
using System.Collections.Generic;
using System.Text;

using NSwissEph.DeltaT;

namespace NSwissEph
{
	public class DateCalculator
	{
		private readonly DeltaTMode deltaTMode;

		public DateCalculator(DeltaTMode deltaTMode)
		{
			this.deltaTMode = deltaTMode;
		}

	}
}
