using System;
using System.Collections.Generic;

namespace NSwissEph
{
	public static class TidalAcceleration
	{
		private static readonly Dictionary<TidalAccelerationMode, double> _accs =
			new Dictionary<TidalAccelerationMode, double>()
			{
				{ TidalAccelerationMode.DE200, -23.8946 },
				{ TidalAccelerationMode.DE403, -25.580  }, // was (-25.8) until V. 1.76.2
				{ TidalAccelerationMode.DE404, -25.580  }, // was (-25.8) until V. 1.76.2
				{ TidalAccelerationMode.DE405, -25.826  }, // was (-25.7376) until V. 1.76.2
				{ TidalAccelerationMode.DE406, -25.826  }, // was (-25.7376) until V. 1.76.2
				{ TidalAccelerationMode.DE421, -25.85   }, // JPL Interoffice Memorandum 14-mar-2008 on DE421 Lunar Orbit
				{ TidalAccelerationMode.DE422, -25.85   }, // JPL Interoffice Memorandum 14-mar-2008 on DE421 (sic!) Lunar Orbit
				{ TidalAccelerationMode.DE430, -25.82   }, // JPL Interoffice Memorandum 9-jul-2013 on DE430 Lunar Orbit
				{ TidalAccelerationMode.DE431, -25.80   }, // IPN Progress Report 42-196 • February 15, 2014, p. 15; was (-25.82) in V. 2.00.00
				{ TidalAccelerationMode.Const26, -26.0 },
			};

		public static double Get(TidalAccelerationMode mode) => _accs[mode];

		public static double Get(JPLDENumber denum)
		{
			TidalAccelerationMode mode;
			switch (denum)
			{
				case JPLDENumber.DE200:
					mode = TidalAccelerationMode.DE200;
					break;
				case JPLDENumber.DE403:
					mode = TidalAccelerationMode.DE403;
					break;
				case JPLDENumber.DE404:
					mode = TidalAccelerationMode.DE404;
					break;
				case JPLDENumber.DE405:
					mode = TidalAccelerationMode.DE405;
					break;
				case JPLDENumber.DE406:
					mode = TidalAccelerationMode.DE406;
					break;
				case JPLDENumber.DE421:
					mode = TidalAccelerationMode.DE421;
					break;
				case JPLDENumber.DE422:
					mode = TidalAccelerationMode.DE422;
					break;
				case JPLDENumber.DE430:
					mode = TidalAccelerationMode.DE430;
					break;
				case JPLDENumber.DE431:
					mode = TidalAccelerationMode.DE431;
					break;
				case JPLDENumber.Auto:
					mode = TidalAccelerationMode.Default;
					break;
				default:
					mode = TidalAccelerationMode.Default;
					break;
			}
			return Get(mode);
		}

		/// <see cref="swi_get_tid_acc"/>
		public static double Calculate(EphemerisMode ephemerisMode, JulianDayNumber julianDay, JPLDENumber denum)
		{
			if (denum == JPLDENumber.Auto)
			{
				switch (ephemerisMode)
				{
					case EphemerisMode.Moshier:
						denum = JPLDENumber.DE404;
						break;

					case EphemerisMode.JPL:
						throw new NotImplementedException();

					case EphemerisMode.SWISSEPH:
						/*
						tjd_et = tjd_ut; // + swe_deltat_ex(tjd_ut, 0, NULL); we do not add delta t, because it would result in a recursive call of swi_set_tid_acc()
						if (swed.fidat[SEI_FILE_MOON].fptr == NULL ||
							tjd_et < swed.fidat[SEI_FILE_MOON].tfstart + 1 ||
							  tjd_et > swed.fidat[SEI_FILE_MOON].tfend - 1)
						{
							iflag = SEFLG_SWIEPH | SEFLG_J2000 | SEFLG_TRUEPOS | SEFLG_ICRS;
							iflag = swe_calc(tjd_et, SE_MOON, iflag, xx, serr);
						}
						if (swed.fidat[SEI_FILE_MOON].fptr != NULL)
						{
							denum = swed.fidat[SEI_FILE_MOON].sweph_denum;
						}
						// Moon ephemeris file is not available, default to Moshier ephemeris
						else
						*/
							denum = JPLDENumber.DE404; // DE number of Moshier ephemeris
						break;
				}
			}

			return Get(denum);
		}

		/// <summary>
		/// Astronomical Almanac table is corrected by adding the expression
		///     -0.000091 (ndot + 26)(year-1955)^2  seconds
		/// to entries prior to 1955 (AA page K8), where ndot is the secular tidal term in the mean motion of the Moon.
		/// Entries after 1955 are referred to atomic time standards and are not affected by errors in Lunar or planetary theory.
		/// </summary>
		/// <see cref="adjust_for_tidacc"/>
		public static double AdjustForTidalAcceleration(double ans, double year, double tidalAcceleration, TidalAccelerationMode mode, bool adjustAfter1955)
		{
			if (year < 1955.0 || adjustAfter1955)
			{
				double B = year - 1955.0;
				ans += -0.000091 * (tidalAcceleration - Get(mode)) * B * B;
			}
			return ans;
		}
	}

	public enum TidalAccelerationMode
	{
		DE200,
		DE403,
		DE404,
		DE405,
		DE406,
		DE421,
		DE422,
		DE430,
		DE431,
		Const26,
		Stephenson_2016 = DE421,
		Default = DE431,
	}
}
