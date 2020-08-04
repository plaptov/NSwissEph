using System;
using System.Collections.Generic;
using System.Text;

namespace NSwissEph
{
	public class DateCalculator
	{
		private readonly DeltaTMode deltaTMode;

		public DateCalculator(DeltaTMode deltaTMode)
		{
			this.deltaTMode = deltaTMode;
		}

		public double CalcDeltaT(JulianDayNumber julianDay, EphemerisMode? ephemerisMode)
		{
			var tidalAcceleration = ephemerisMode is null
				? TidalAcceleration.Get(JPLDENumber.Default)
				: TidalAcceleration.Calculate(ephemerisMode.Value, julianDay, JPLDENumber.Auto);

			var year = julianDay.GetYear();
			var gregorianYear = julianDay.GetGregorianYear();

			/* Model for epochs before 1955, currently default in Swiss Ephemeris:
			* Stephenson/Morrison/Hohenkerk 2016 
			* (we switch over to Astronomical Almanac K8-K9 and IERS at 1 Jan. 1955. 
			* To make the curve continuous we apply some linear term over 
			* 1000 days before this date.)
			* Delta T according to Stephenson/Morrison/Hohenkerk 2016 is based on
			* ancient, medieval, and modern observations of eclipses and occultations.
			* Values of Deltat T before 1955 depend on this kind of observations.
			* For more recent data we want to use the data provided by IERS 
			* (or Astronomical Almanac K8-K9).
			*/
			if (deltaTMode == DeltaTMode.Stephenson_Etc_2016 && julianDay < JulianDayNumber.J1955)
			{
				double deltaT = DeltaTStephensonEtc2016.Calc(julianDay, tidalAcceleration);
				if (julianDay >= JulianDayNumber.J1952_05_04)
					deltaT += (1.0 - (double)(JulianDayNumber.J1955 - julianDay) / 1000.0) * 0.6610218 / 86400.0;
				return deltaT;
			}

			return 0;
		}

	}
}
