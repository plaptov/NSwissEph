using System;

namespace NSwissEph
{
	public class Consts
	{
		public const double TwoPi = Math.PI * 2.0;

		public static readonly JulianDayNumber HORIZONS_TJD0_DPSI_DEPS_IAU1980 = JulianDayNumber.FromRaw(2437684.5);

		public const double RADTODEG = 180.0 / Math.PI;
		public const double DEGTORAD = Math.PI / 180.0;

		public const double PREC_IAU_1976_CTIES = 2.0; /* J2000 +/- two centuries */
		public const double PREC_IAU_2000_CTIES = 2.0; /* J2000 +/- two centuries */
		/* we use P03 for whole ephemeris */
		public const double PREC_IAU_2006_CTIES = 75.0; /* J2000 +/- 75 centuries */

		public const double EARTH_RADIUS = 6378136.6; /* AA 2006 K6 */
		public const double EARTH_OBLATENESS = 1.0 / 298.25642; /* AA 2006 K6 */

		public const double AUNIT = 1.49597870700e+11; /* au in meters, DE431 */
		public const double CLIGHT = 2.99792458e+8; /* m/s, AA 1996 K6 / DE431 */
	}
}
