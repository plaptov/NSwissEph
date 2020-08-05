using System;

namespace NSwissEph.DeltaT
{
	internal static class DeltaTTabulated
	{
		public const int TabStart = 1620;
		public const int TabEnd = 2027;
		public const int TabSize = TabEnd - TabStart + 1;

		public const int Tab2Size = 27;
		public const int Tab2Start = -1000;
		public const int Tab2End = 1600;
		public const int Tab2Step = 100;

		/// <summary>
		/// Last update of table dt[]: Dieter Koch, 18 dec 2013.
		/// ATTENTION: Whenever updating this table, do not forget to adjust the TABEND!
		/// </summary>
		private static readonly double[] dt = new[]
		{
			/* 1620.0 - 1659.0 */
			124.00, 119.00, 115.00, 110.00, 106.00, 102.00, 98.00, 95.00, 91.00, 88.00,
			85.00, 82.00, 79.00, 77.00, 74.00, 72.00, 70.00, 67.00, 65.00, 63.00,
			62.00, 60.00, 58.00, 57.00, 55.00, 54.00, 53.00, 51.00, 50.00, 49.00,
			48.00, 47.00, 46.00, 45.00, 44.00, 43.00, 42.00, 41.00, 40.00, 38.00,
			/* 1660.0 - 1699.0 */
			37.00, 36.00, 35.00, 34.00, 33.00, 32.00, 31.00, 30.00, 28.00, 27.00,
			26.00, 25.00, 24.00, 23.00, 22.00, 21.00, 20.00, 19.00, 18.00, 17.00,
			16.00, 15.00, 14.00, 14.00, 13.00, 12.00, 12.00, 11.00, 11.00, 10.00,
			10.00, 10.00, 9.00, 9.00, 9.00, 9.00, 9.00, 9.00, 9.00, 9.00,
			/* 1700.0 - 1739.0 */
			9.00, 9.00, 9.00, 9.00, 9.00, 9.00, 9.00, 9.00, 10.00, 10.00,
			10.00, 10.00, 10.00, 10.00, 10.00, 10.00, 10.00, 11.00, 11.00, 11.00,
			11.00, 11.00, 11.00, 11.00, 11.00, 11.00, 11.00, 11.00, 11.00, 11.00,
			11.00, 11.00, 11.00, 11.00, 12.00, 12.00, 12.00, 12.00, 12.00, 12.00,
			/* 1740.0 - 1779.0 */
			12.00, 12.00, 12.00, 12.00, 13.00, 13.00, 13.00, 13.00, 13.00, 13.00,
			13.00, 14.00, 14.00, 14.00, 14.00, 14.00, 14.00, 14.00, 15.00, 15.00,
			15.00, 15.00, 15.00, 15.00, 15.00, 16.00, 16.00, 16.00, 16.00, 16.00,
			16.00, 16.00, 16.00, 16.00, 16.00, 17.00, 17.00, 17.00, 17.00, 17.00,
			/* 1780.0 - 1799.0 */
			17.00, 17.00, 17.00, 17.00, 17.00, 17.00, 17.00, 17.00, 17.00, 17.00,
			17.00, 17.00, 16.00, 16.00, 16.00, 16.00, 15.00, 15.00, 14.00, 14.00,
			/* 1800.0 - 1819.0 */
			13.70, 13.40, 13.10, 12.90, 12.70, 12.60, 12.50, 12.50, 12.50, 12.50,
			12.50, 12.50, 12.50, 12.50, 12.50, 12.50, 12.50, 12.40, 12.30, 12.20,
			/* 1820.0 - 1859.0 */
			12.00, 11.70, 11.40, 11.10, 10.60, 10.20, 9.60, 9.10, 8.60, 8.00,
			7.50, 7.00, 6.60, 6.30, 6.00, 5.80, 5.70, 5.60, 5.60, 5.60,
			5.70, 5.80, 5.90, 6.10, 6.20, 6.30, 6.50, 6.60, 6.80, 6.90,
			7.10, 7.20, 7.30, 7.40, 7.50, 7.60, 7.70, 7.70, 7.80, 7.80,
			/* 1860.0 - 1899.0 */
			7.88, 7.82, 7.54, 6.97, 6.40, 6.02, 5.41, 4.10, 2.92, 1.82,
			1.61, .10, -1.02, -1.28, -2.69, -3.24, -3.64, -4.54, -4.71, -5.11,
			-5.40, -5.42, -5.20, -5.46, -5.46, -5.79, -5.63, -5.64, -5.80, -5.66,
			-5.87, -6.01, -6.19, -6.64, -6.44, -6.47, -6.09, -5.76, -4.66, -3.74,
			/* 1900.0 - 1939.0 */
			-2.72, -1.54, -.02, 1.24, 2.64, 3.86, 5.37, 6.14, 7.75, 9.13,
			10.46, 11.53, 13.36, 14.65, 16.01, 17.20, 18.24, 19.06, 20.25, 20.95,
			21.16, 22.25, 22.41, 23.03, 23.49, 23.62, 23.86, 24.49, 24.34, 24.08,
			24.02, 24.00, 23.87, 23.95, 23.86, 23.93, 23.73, 23.92, 23.96, 24.02,
			/* 1940.0 - 1949.0 */
			24.33, 24.83, 25.30, 25.70, 26.24, 26.77, 27.28, 27.78, 28.25, 28.71,
			/* 1950.0 - 1959.0 */
			29.15, 29.57, 29.97, 30.36, 30.72, 31.07, 31.35, 31.68, 32.18, 32.68,
			/* 1960.0 - 1969.0 */
			33.15, 33.59, 34.00, 34.47, 35.03, 35.73, 36.54, 37.43, 38.29, 39.20,
			/* 1970.0 - 1979.0 */
			/* from 1974 on values (with 4-digit precision) were calculated from IERS data */
			40.18, 41.17, 42.23, 43.37, 44.4841, 45.4761, 46.4567, 47.5214, 48.5344, 49.5862,
			/* 1980.0 - 1989.0 */
			50.5387, 51.3808, 52.1668, 52.9565, 53.7882, 54.3427, 54.8713, 55.3222, 55.8197, 56.3000,
			/* 1990.0 - 1999.0 */
			56.8553, 57.5653, 58.3092, 59.1218, 59.9845, 60.7854, 61.6287, 62.2951, 62.9659, 63.4673,
			/* 2000.0 - 2009.0 */
			63.8285, 64.0908, 64.2998, 64.4734, 64.5736, 64.6876, 64.8452, 65.1464, 65.4574, 65.7768,
			/* 2010.0 - 2018.0 */
			66.0699, 66.3246, 66.6030, 66.9069, 67.2810, 67.6439, 68.1024, 68.5927, 68.9676, 69.2202,
			/* Extrapolated values: 
			 * 2020 - 2027 */
			69.4456, 70.00,   70.50,   71.00,   71.50,   72.00,   72.50,   73.00,
		};

		/// <summary>
		/// Table for -1000 through 1600, from Morrison & Stephenson (2004).
		/// </summary>
		internal static readonly short[] dt2 = new short[]
		{
			/*-1000  -900  -800  -700  -600  -500  -400  -300  -200  -100*/
			25400, 23700, 22000, 21000, 19040, 17190, 15530, 14080, 12790, 11640,
			/*    0   100   200   300   400   500   600   700   800   900*/
			10580, 9600, 8640, 7680, 6700, 5710, 4740, 3810, 2960, 2200,
			/* 1000  1100  1200  1300  1400  1500  1600,                 */
			1570, 1090,  740,  490,  320,  200,  120,
		};

		public static double CalcInterpolated(double year, double gregorianYear, double tidalAcceleration)
		{
			var B = TabStart - Tab2End;
			var iy = (Tab2End - Tab2Start) / Tab2Step;
			var dd = (year - Tab2End) / B;
			var ans = dt2[iy] + dd * (dt[0] - dt2[iy]);
			ans = TidalAcceleration.AdjustForTidalAcceleration(ans, gregorianYear, tidalAcceleration, TidalAccelerationMode.Const26, false);
			return ans / 86400.0;
		}

		/// <summary>
		/// The tabulated values of deltaT, in hundredths of a second,
		/// were taken from The Astronomical Almanac 1997etc., pp.K8-K9.
		/// Some more recent values are taken from IERS
		/// http://maia.usno.navy.mil/ser7/deltat.data .
		/// </summary>
		/// <remarks>
		/// Bessel's interpolation formula is implemented to obtain fourth 
		/// order interpolated values at intermediate times.
		/// The values are adjusted depending on the ephemeris used
		/// and its inherent value of secular tidal acceleration ndot.
		/// Note by Dieter Jan. 2017:
		/// Bessel interpolation assumes equidistant sampling points. However the
		/// sampling points are not equidistant, because they are for first January of
		/// every year and years can have either 365 or 366 days.The interpolation uses
		/// a step width of 365.25 days.As a consequence, in three out of four years
		/// the interpolation does not reproduce the exact values of the sampling points
		/// on the days they refer to.
		/// </remarks>
		/// <see cref="deltat_aa"/>
		public static double Calc(JulianDayNumber tjd, double tid_acc, DeltaTMode mode)
		{
			double ans, ans2 = 0, ans3;
			double p, B, B2, Y, dd;
			double[] d = new double[6];
			int i, iy, k;
			Y = tjd.GetYear();
			if (Y <= TabEnd)
			{
				// Index into the table.
				p = Math.Floor(Y);
				iy = (int)(p - TabStart);
				/* Zeroth order estimate is value at start of year */
				ans = dt[iy];
				k = iy + 1;
				if (k >= TabSize)
					goto done; /* No data, can't go on. */
				/* The fraction of tabulation interval */
				p = Y - p;
				/* First order interpolated value */
				ans += p * (dt[k] - dt[iy]);
				if ((iy - 1 < 0) || (iy + 2 >= TabSize))
					goto done; /* can't do second differences */
				/* Make table of first differences */
				k = iy - 2;
				for (i = 0; i < 5; i++)
				{
					if ((k < 0) || (k + 1 >= TabSize))
						d[i] = 0;
					else
						d[i] = dt[k + 1] - dt[k];
					k += 1;
				}
				/* Compute second differences */
				for (i = 0; i < 4; i++)
					d[i] = d[i + 1] - d[i];
				B = 0.25 * p * (p - 1.0);
				ans += B * (d[1] + d[2]);
				if (iy + 2 >= TabSize)
					goto done;
				/* Compute third differences */
				for (i = 0; i < 3; i++)
					d[i] = d[i + 1] - d[i];
				B = 2.0 * B / 3.0;
				ans += (p - 0.5) * B * d[1];
				if ((iy - 2 < 0) || (iy + 3 > TabSize))
					goto done;
				/* Compute fourth differences */
				for (i = 0; i < 2; i++)
					d[i] = d[i + 1] - d[i];
				B = 0.125 * B * (p + 1.0) * (p - 2.0);
				ans += B * (d[0] + d[1]);
			done:
				ans = TidalAcceleration.AdjustForTidalAcceleration(ans, Y, tid_acc, TidalAccelerationMode.Const26, false);
				return ans / 86400.0;
			}
			/* today - future: 
			 * 3rd degree polynomial based on data given by 
			 * Stephenson/Morrison/Hohenkerk 2016 here:
			 * http://astro.ukho.gov.uk/nao/lvm/
			 */
			if (mode == DeltaTMode.Stephenson_Etc_2016)
			{
				B = Y - 2000;
				if (Y < 2500)
				{
					ans = B * B * B * 121.0 / 30000000.0 + B * B / 1250.0 + B * 521.0 / 3000.0 + 64.0;
					/* for slow transition from tablulated data */
					B2 = TabEnd - 2000;
					ans2 = B2 * B2 * B2 * 121.0 / 30000000.0 + B2 * B2 / 1250.0 + B2 * 521.0 / 3000.0 + 64.0;
					/* we use a parable after 2500 */
				}
				else
				{
					B = 0.01 * (Y - 2000);
					ans = B * B * 32.5 + 42.5;
				}
				/* 
				 * Formula Stephenson (1997; p. 507),
				 * with modification to avoid jump at end of AA table,
				 * similar to what Meeus 1998 had suggested.
				 * Slow transition within 100 years.
				 */
			}
			else
			{
				B = 0.01 * (Y - 1820);
				ans = -20 + 31 * B * B;
				/* for slow transition from tablulated data */
				B2 = 0.01 * (TabEnd - 1820);
				ans2 = -20 + 31 * B2 * B2;
			}
			/* slow transition from tabulated values to Stephenson formula: */
			if (Y <= TabEnd + 100)
			{
				ans3 = dt[TabSize - 1];
				dd = (ans2 - ans3);
				ans += dd * (Y - (TabEnd + 100)) * 0.01;
			}
			return ans / 86400.0;
		}
	}
}
