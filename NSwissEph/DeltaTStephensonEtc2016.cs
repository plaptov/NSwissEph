using System;
using System.Collections.Generic;
using System.Text;

namespace NSwissEph
{
	internal class DeltaTStephensonEtc2016
	{
		private static readonly JulianDayNumber JMinus720 = JulianDayNumber.FromRaw(1458085.5);
		private static readonly JulianDayNumber J2016 = JulianDayNumber.FromRaw(2457388.5);

		public static double Calc(JulianDayNumber julianDay, double tidalAcceleration)
		{
			var gregorianYear = julianDay.GetGregorianYear();
			double dt = 0.0;
			if (julianDay >= J2016 || julianDay < JMinus720)
			{
				var t = (gregorianYear - 1825) / 100.0;
				dt = -320 + 32.5 * t * t;
				if (julianDay >= J2016)
					dt += 269.4790417; // to make curve continous on 1 Jan 2016 (D. Koch)
				else
					dt -= 179.7337208; // to make curve continous on 1 Jan -720 (D. Koch)
			}
			else
				for (int i = 0; i < _coefficients.Length; i++)
				{
					ref var data = ref _coefficients[i];
					if (julianDay < data.End)
					{
						var t = (double)(julianDay - data.Begin) / (double)(data.End - data.Begin);
						dt = data.Coef1 + data.Coef2 * t + data.Coef3 * t * t + data.Coef4 * t * t * t;
					}
				}

			/* The parameter adjust_after_1955 must be TRUE here, because the 
			* Stephenson 2016 curve is based on occultation data alone, 
			* not on IERS data.
			* Note, however, the current function deltat_stephenson_etc_2016()
			* is called only for dates before 1 Jan 1955. */
			dt = TidalAcceleration.AdjustForTidalAcceleration(dt, gregorianYear, tidalAcceleration, TidalAccelerationMode.Stephenson_2016, true);
			dt /= 86400.0;
			return dt;
		}

		private readonly struct CoefData
		{
			public CoefData(JulianDayNumber begin, JulianDayNumber end,
				double coef1, double coef2, double coef3, double coef4)
			{
				Begin = begin;
				End = end;
				Coef1 = coef1;
				Coef2 = coef2;
				Coef3 = coef3;
				Coef4 = coef4;
			}
			public readonly JulianDayNumber Begin;
			public readonly JulianDayNumber End;
			public readonly double Coef1;
			public readonly double Coef2;
			public readonly double Coef3;
			public readonly double Coef4;
		}

		private static readonly CoefData[] _coefficients = new[]
		{
			new CoefData(JulianDayNumber.FromRaw(1458085.5), JulianDayNumber.FromRaw(1867156.5), 20550.593,-21268.478, 11863.418, -4541.129), // ybeg=-720, yend= 400
			new CoefData(JulianDayNumber.FromRaw(1867156.5), JulianDayNumber.FromRaw(2086302.5),  6604.404, -5981.266,  -505.093,  1349.609), // ybeg= 400, yend=1000
			new CoefData(JulianDayNumber.FromRaw(2086302.5), JulianDayNumber.FromRaw(2268923.5),  1467.654, -2452.187,  2460.927, -1183.759), // ybeg=1000, yend=1500
			new CoefData(JulianDayNumber.FromRaw(2268923.5), JulianDayNumber.FromRaw(2305447.5),   292.635,  -216.322,   -43.614,    56.681), // ybeg=1500, yend=1600
			new CoefData(JulianDayNumber.FromRaw(2305447.5), JulianDayNumber.FromRaw(2323710.5),    89.380,   -66.754,    31.607,   -10.497), // ybeg=1600, yend=1650
			new CoefData(JulianDayNumber.FromRaw(2323710.5), JulianDayNumber.FromRaw(2349276.5),    43.736,   -49.043,     0.227,    15.811), // ybeg=1650, yend=1720
			new CoefData(JulianDayNumber.FromRaw(2349276.5), JulianDayNumber.FromRaw(2378496.5),    10.730,    -1.321,    62.250,   -52.946), // ybeg=1720, yend=1800
			new CoefData(JulianDayNumber.FromRaw(2378496.5), JulianDayNumber.FromRaw(2382148.5),    18.714,    -4.457,    -1.509,     2.507), // ybeg=1800, yend=1810
			new CoefData(JulianDayNumber.FromRaw(2382148.5), JulianDayNumber.FromRaw(2385800.5),    15.255,     0.046,     6.012,    -4.634), // ybeg=1810, yend=1820
			new CoefData(JulianDayNumber.FromRaw(2385800.5), JulianDayNumber.FromRaw(2389453.5),    16.679,    -1.831,    -7.889,     3.799), // ybeg=1820, yend=1830
			new CoefData(JulianDayNumber.FromRaw(2389453.5), JulianDayNumber.FromRaw(2393105.5),    10.758,    -6.211,     3.509,    -0.388), // ybeg=1830, yend=1840
			new CoefData(JulianDayNumber.FromRaw(2393105.5), JulianDayNumber.FromRaw(2396758.5),     7.668,    -0.357,     2.345,    -0.338), // ybeg=1840, yend=1850
			new CoefData(JulianDayNumber.FromRaw(2396758.5), JulianDayNumber.FromRaw(2398584.5),     9.317,     1.659,     0.332,    -0.932), // ybeg=1850, yend=1855
			new CoefData(JulianDayNumber.FromRaw(2398584.5), JulianDayNumber.FromRaw(2400410.5),    10.376,    -0.472,    -2.463,     1.596), // ybeg=1855, yend=1860
			new CoefData(JulianDayNumber.FromRaw(2400410.5), JulianDayNumber.FromRaw(2402237.5),     9.038,    -0.610,     2.325,    -2.497), // ybeg=1860, yend=1865
			new CoefData(JulianDayNumber.FromRaw(2402237.5), JulianDayNumber.FromRaw(2404063.5),     8.256,    -3.450,    -5.166,     2.729), // ybeg=1865, yend=1870
			new CoefData(JulianDayNumber.FromRaw(2404063.5), JulianDayNumber.FromRaw(2405889.5),     2.369,    -5.596,     3.020,    -0.919), // ybeg=1870, yend=1875
			new CoefData(JulianDayNumber.FromRaw(2405889.5), JulianDayNumber.FromRaw(2407715.5),    -1.126,    -2.312,     0.264,    -0.037), // ybeg=1875, yend=1880
			new CoefData(JulianDayNumber.FromRaw(2407715.5), JulianDayNumber.FromRaw(2409542.5),    -3.211,    -1.894,     0.154,     0.562), // ybeg=1880, yend=1885
			new CoefData(JulianDayNumber.FromRaw(2409542.5), JulianDayNumber.FromRaw(2411368.5),    -4.388,     0.101,     1.841,    -1.438), // ybeg=1885, yend=1890
			new CoefData(JulianDayNumber.FromRaw(2411368.5), JulianDayNumber.FromRaw(2413194.5),    -3.884,    -0.531,    -2.473,     1.870), // ybeg=1890, yend=1895
			new CoefData(JulianDayNumber.FromRaw(2413194.5), JulianDayNumber.FromRaw(2415020.5),    -5.017,     0.134,     3.138,    -0.232), // ybeg=1895, yend=1900
			new CoefData(JulianDayNumber.FromRaw(2415020.5), JulianDayNumber.FromRaw(2416846.5),    -1.977,     5.715,     2.443,    -1.257), // ybeg=1900, yend=1905
			new CoefData(JulianDayNumber.FromRaw(2416846.5), JulianDayNumber.FromRaw(2418672.5),     4.923,     6.828,    -1.329,     0.720), // ybeg=1905, yend=1910
			new CoefData(JulianDayNumber.FromRaw(2418672.5), JulianDayNumber.FromRaw(2420498.5),    11.142,     6.330,     0.831,    -0.825), // ybeg=1910, yend=1915
			new CoefData(JulianDayNumber.FromRaw(2420498.5), JulianDayNumber.FromRaw(2422324.5),    17.479,     5.518,    -1.643,     0.262), // ybeg=1915, yend=1920
			new CoefData(JulianDayNumber.FromRaw(2422324.5), JulianDayNumber.FromRaw(2424151.5),    21.617,     3.020,    -0.856,     0.008), // ybeg=1920, yend=1925
			new CoefData(JulianDayNumber.FromRaw(2424151.5), JulianDayNumber.FromRaw(2425977.5),    23.789,     1.333,    -0.831,     0.127), // ybeg=1925, yend=1930
			new CoefData(JulianDayNumber.FromRaw(2425977.5), JulianDayNumber.FromRaw(2427803.5),    24.418,     0.052,    -0.449,     0.142), // ybeg=1930, yend=1935
			new CoefData(JulianDayNumber.FromRaw(2427803.5), JulianDayNumber.FromRaw(2429629.5),    24.164,    -0.419,    -0.022,     0.702), // ybeg=1935, yend=1940
			new CoefData(JulianDayNumber.FromRaw(2429629.5), JulianDayNumber.FromRaw(2431456.5),    24.426,     1.645,     2.086,    -1.106), // ybeg=1940, yend=1945
			new CoefData(JulianDayNumber.FromRaw(2431456.5), JulianDayNumber.FromRaw(2433282.5),    27.050,     2.499,    -1.232,     0.614), // ybeg=1945, yend=1950
			new CoefData(JulianDayNumber.FromRaw(2433282.5), JulianDayNumber.FromRaw(2434378.5),    28.932,     1.127,     0.220,    -0.277), // ybeg=1950, yend=1953
			new CoefData(JulianDayNumber.FromRaw(2434378.5), JulianDayNumber.FromRaw(2435473.5),    30.002,     0.737,    -0.610,     0.631), // ybeg=1953, yend=1956
			new CoefData(JulianDayNumber.FromRaw(2435473.5), JulianDayNumber.FromRaw(2436569.5),    30.760,     1.409,     1.282,    -0.799), // ybeg=1956, yend=1959
			new CoefData(JulianDayNumber.FromRaw(2436569.5), JulianDayNumber.FromRaw(2437665.5),    32.652,     1.577,    -1.115,     0.507), // ybeg=1959, yend=1962
			new CoefData(JulianDayNumber.FromRaw(2437665.5), JulianDayNumber.FromRaw(2438761.5),    33.621,     0.868,     0.406,     0.199), // ybeg=1962, yend=1965
			new CoefData(JulianDayNumber.FromRaw(2438761.5), JulianDayNumber.FromRaw(2439856.5),    35.093,     2.275,     1.002,    -0.414), // ybeg=1965, yend=1968
			new CoefData(JulianDayNumber.FromRaw(2439856.5), JulianDayNumber.FromRaw(2440952.5),    37.956,     3.035,    -0.242,     0.202), // ybeg=1968, yend=1971
			new CoefData(JulianDayNumber.FromRaw(2440952.5), JulianDayNumber.FromRaw(2442048.5),    40.951,     3.157,     0.364,    -0.229), // ybeg=1971, yend=1974
			new CoefData(JulianDayNumber.FromRaw(2442048.5), JulianDayNumber.FromRaw(2443144.5),    44.244,     3.198,    -0.323,     0.172), // ybeg=1974, yend=1977
			new CoefData(JulianDayNumber.FromRaw(2443144.5), JulianDayNumber.FromRaw(2444239.5),    47.291,     3.069,     0.193,    -0.192), // ybeg=1977, yend=1980
			new CoefData(JulianDayNumber.FromRaw(2444239.5), JulianDayNumber.FromRaw(2445335.5),    50.361,     2.878,    -0.384,     0.081), // ybeg=1980, yend=1983
			new CoefData(JulianDayNumber.FromRaw(2445335.5), JulianDayNumber.FromRaw(2446431.5),    52.936,     2.354,    -0.140,    -0.166), // ybeg=1983, yend=1986
			new CoefData(JulianDayNumber.FromRaw(2446431.5), JulianDayNumber.FromRaw(2447527.5),    54.984,     1.577,    -0.637,     0.448), // ybeg=1986, yend=1989
			new CoefData(JulianDayNumber.FromRaw(2447527.5), JulianDayNumber.FromRaw(2448622.5),    56.373,     1.649,     0.709,    -0.277), // ybeg=1989, yend=1992
			new CoefData(JulianDayNumber.FromRaw(2448622.5), JulianDayNumber.FromRaw(2449718.5),    58.453,     2.235,    -0.122,     0.111), // ybeg=1992, yend=1995
			new CoefData(JulianDayNumber.FromRaw(2449718.5), JulianDayNumber.FromRaw(2450814.5),    60.677,     2.324,     0.212,    -0.315), // ybeg=1995, yend=1998
			new CoefData(JulianDayNumber.FromRaw(2450814.5), JulianDayNumber.FromRaw(2451910.5),    62.899,     1.804,    -0.732,     0.112), // ybeg=1998, yend=2001
			new CoefData(JulianDayNumber.FromRaw(2451910.5), JulianDayNumber.FromRaw(2453005.5),    64.082,     0.675,    -0.396,     0.193), // ybeg=2001, yend=2004
			new CoefData(JulianDayNumber.FromRaw(2453005.5), JulianDayNumber.FromRaw(2454101.5),    64.555,     0.463,     0.184,    -0.008), // ybeg=2004, yend=2007
			new CoefData(JulianDayNumber.FromRaw(2454101.5), JulianDayNumber.FromRaw(2455197.5),    65.194,     0.809,     0.161,    -0.101), // ybeg=2007, yend=2010
			new CoefData(JulianDayNumber.FromRaw(2455197.5), JulianDayNumber.FromRaw(2456293.5),    66.063,     0.828,    -0.142,     0.168), // ybeg=2010, yend=2013
			new CoefData(JulianDayNumber.FromRaw(2456293.5), JulianDayNumber.FromRaw(2457388.5),    66.917,     1.046,     0.360,    -0.282), // ybeg=2013, yend=2016
		};
	}
}
