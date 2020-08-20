using System;
using System.Collections.Generic;
using System.Text;

namespace NSwissEph
{
	/// <summary>
	/// Calculation context. Like <c>swe_data</c>.
	/// </summary>
	public class SweData
	{
		public SweData()
		{
			TidalAcc = TidalAcceleration.Get(TidalAccelerationMode.Default);
			IsManualTidalAcc = false;
		}

		public double TidalAcc { get; private set; }
		public bool IsManualTidalAcc { get; private set; }
	}
}
