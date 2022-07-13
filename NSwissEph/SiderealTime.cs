using System;

using NSwissEph.DeltaT;
using NSwissEph.Internals;

using static NSwissEph.Consts;
using static NSwissEph.Internals.Calculus;

namespace NSwissEph
{
	public static class SiderealTime
	{
		private static readonly JulianDayNumber SIDT_LTERM_T0 = JulianDayNumber.FromRaw(2396758.5); /* 1 Jan 1850  */
		private static readonly JulianDayNumber SIDT_LTERM_T1 = JulianDayNumber.FromRaw(2469807.5); /* 1 Jan 2050  */
		private static readonly double SIDT_LTERM_OFS0 = 0.000378172 / 15.0;
		private static readonly double SIDT_LTERM_OFS1 = 0.001385646 / 15.0;

		/// <summary>
		/// <see cref="swe_sidtime"/>
		/// </summary>
		public static double Calc(JulianDayNumber tjd_ut, SweData swed)
		{
			var tjde = tjd_ut.AddDays(DeltaTCalculator.Calc(tjd_ut, DeltaTMode.None, null));
			var eps = Epsilon.Calc(tjde, SEFLG.None, swed).Eps * RADTODEG;
			var nutlo = NutationCalculator.Calculate(tjde, swed);
			return Calc(tjd_ut, eps + nutlo.Obliquity, nutlo.Longitude, swed);
		}

		/// <summary>
		/// <see cref="swe_sidtime0"/>
		/// </summary>
		public static double Calc(JulianDayNumber tjd, double eps, double nut, SweData swed)
		{
			double jd0;     /* Julian day at midnight Universal Time */
			double secs;    /* Time of day, UT seconds since UT midnight */
			double eqeq, jd, tu, tt, msday, jdrel;
			double gmst, dadd;
			var prec_model_short = swed.ShorttermPrecessionMode;
			var sidt_model = swed.SiderealTimeMode;
			if (prec_model_short == 0) prec_model_short = PrecessionModel.DefaultShort;
			if (sidt_model == 0) sidt_model = SiderealTimeMode.Default;
			if (sidt_model == SiderealTimeMode.LONGTERM)
			{
				if (tjd <= SIDT_LTERM_T0 || tjd >= SIDT_LTERM_T1)
				{
					gmst = sidtime_long_term(tjd, eps, nut, swed);
					if (tjd <= SIDT_LTERM_T0)
						gmst -= SIDT_LTERM_OFS0;
					else if (tjd >= SIDT_LTERM_T1)
						gmst -= SIDT_LTERM_OFS1;
					if (gmst >= 24) gmst -= 24;
					if (gmst < 0) gmst += 24;
					return gmst;
				}
			}
			/* Julian day at given UT */
			jd = tjd.Raw;
			jd0 = Math.Floor(jd);
			secs = tjd.Raw - jd0;
			if (secs < 0.5)
			{
				jd0 -= 0.5;
				secs += 0.5;
			}
			else
			{
				jd0 += 0.5;
				secs -= 0.5;
			}
			secs *= 86400.0;
			tu = (jd0 - JulianDayNumber.J2000.Raw) / 36525.0; /* UT1 in centuries after J2000 */
			if (sidt_model == SiderealTimeMode.IERS_CONV_2010 || sidt_model == SiderealTimeMode.LONGTERM)
			{
				/*  ERA-based expression for Greenwich Sidereal Time (GST) based 
				 *  on the IAU 2006 precession */
				jdrel = (tjd - JulianDayNumber.J2000).Raw;
				tt = (tjd.Raw + DeltaTCalculator.Calc(tjd, DeltaTMode.None, null) - JulianDayNumber.J2000.Raw) / 36525.0;
				gmst = swe_degnorm((0.7790572732640 + 1.00273781191135448 * jdrel) * 360);
				gmst += (0.014506 + tt * (4612.156534 + tt * (1.3915817 + tt * (-0.00000044 + tt * (-0.000029956 + tt * -0.0000000368))))) / 3600.0;
				dadd = sidtime_non_polynomial_part(tt);
				gmst = swe_degnorm(gmst + dadd);
				/*printf("gmst iers=%f \n", gmst);*/
				gmst = gmst / 15.0 * 3600.0;
				/* sidt_model == SEMOD_SIDT_IAU_2006, older standards according to precession model */
			}
			else if (sidt_model == SiderealTimeMode.IAU_2006)
			{
				tt = (jd0 + DeltaTCalculator.Calc(JulianDayNumber.FromRaw(jd0), DeltaTMode.None, null) - JulianDayNumber.J2000.Raw) / 36525.0; /* TT in centuries after J2000 */
				gmst = (((-0.000000002454 * tt - 0.00000199708) * tt - 0.0000002926) * tt + 0.092772110) * tt * tt + 307.4771013 * (tt - tu) + 8640184.79447825 * tu + 24110.5493771;
				/* mean solar days per sidereal day at date tu;
				 * for the derivative of gmst, we can assume UT1 =~ TT */
				msday = 1 + ((((-0.000000012270 * tt - 0.00000798832) * tt - 0.0000008778) * tt + 0.185544220) * tt + 8640184.79447825) / (86400.0 * 36525.0);
				gmst += msday * secs;
				/* SEMOD_SIDT_IAU_1976 */
			}
			else
			{  /* IAU 1976 formula */
				/* Greenwich Mean Sidereal Time at 0h UT of date */
				gmst = ((-6.2e-6 * tu + 9.3104e-2) * tu + 8640184.812866) * tu + 24110.54841;
				/* mean solar days per sidereal day at date tu, = 1.00273790934 in 1986 */
				msday = 1.0 + ((-1.86e-5 * tu + 0.186208) * tu + 8640184.812866) / (86400.0 * 36525.0);
				gmst += msday * secs;
			}
			/* Local apparent sidereal time at given UT at Greenwich */
			eqeq = 240.0 * nut * Math.Cos(eps * DEGTORAD);
			gmst = gmst + eqeq  /* + 240.0*tlong */;
			/* Sidereal seconds modulo 1 sidereal day */
			gmst = gmst - 86400.0 * Math.Floor(gmst / 86400.0);
			/* return in hours */
			gmst /= 3600;
			return gmst;
		}

		/*
		 * The time range of DE431 requires a new calculation of sidereal time that 
		 * gives sensible results for the remote past and future.
		 * The algorithm is based on the formula of the mean earth by Simon & alii,
		 * "Precession formulae and mean elements for the Moon and the Planets",
		 * A&A 282 (1994), p. 675/678.
		 * The longitude of the mean earth relative to the mean equinox J2000
		 * is calculated and then precessed to the equinox of date, using the
		 * default precession model of the Swiss Ephmeris. Afte that,
		 * sidereal time is derived.
		 * The algoritm provides exact agreement for epoch 1 Jan. 2003 with the 
		 * definition of sidereal time as given in the IERS Convention 2010.
		 */
		static double sidtime_long_term(JulianDayNumber tjd_ut, double eps, double nut, SweData swed)
		{
			const double dlt = AUNIT / CLIGHT / 86400.0;
			var tjd_et = tjd_ut.AddDays(DeltaTCalculator.Calc(tjd_ut, DeltaTMode.None, null));
			var t = (tjd_et - JulianDayNumber.J2000).Raw / 365250.0;
			var t2 = t * t;
			var t3 = t * t2;
			/* mean longitude of earth J2000 */
			double dlon = 100.46645683 + (1295977422.83429 * t - 2.04411 * t2 - 0.00523 * t3) / 3600.0;
			/* light time sun-earth */
			dlon = swe_degnorm(dlon - dlt * 360.0 / 365.2425);
			var xs_pol = new PolarCoordinates(dlon * DEGTORAD, 0, 1);
			/* to mean equator J2000, cartesian */
			var lat = Epsilon.Calc(JulianDayNumber.J2000.AddDays(DeltaTCalculator.Calc(JulianDayNumber.J2000)), SEFLG.None, swed).Eps * RADTODEG;
			var xs_cart = xs_pol.ToCartesian().Transform(-lat * DEGTORAD);
			/* precess to mean equinox of date */
			xs_cart = Precession.swi_precess(xs_cart, tjd_et, SEFLG.None, -1, swed);
			/* to mean equinox of date */
			lat = Epsilon.Calc(tjd_et, SEFLG.None, swed).Eps * RADTODEG;
			var nutlo = NutationCalculator.Calculate(tjd_et, swed);
			var lon = lat + nutlo.Obliquity * RADTODEG;
			var dist = nutlo.Longitude * RADTODEG;
			xs_cart = xs_cart.Transform(lat * DEGTORAD);
			var az = xs_cart.ToPolar().Azimuth;
			az *= RADTODEG;
			var dhour = ((tjd_ut.Raw - 0.5) % 1) * 360;
			/* mean to true (if nut != 0) */
			if (eps == 0)
				az += dist * Math.Cos(lon * DEGTORAD);
			else
				az += nut * Math.Cos(eps * DEGTORAD);
			/* add hour */
			az = swe_degnorm(az + dhour);
			return az / 15.0;
		}

		private const int SIDTNTERM = 33;
		private const int SIDTNARG = 14;

		private static readonly double[] stcf = new double[SIDTNTERM * 2]
		{
			2640.96,-0.39,
			63.52,-0.02,
			11.75,0.01,
			11.21,0.01,
			-4.55,0.00,
			2.02,0.00,
			1.98,0.00,
			-1.72,0.00,
			-1.41,-0.01,
			-1.26,-0.01,
			-0.63,0.00,
			-0.63,0.00,
			0.46,0.00,
			0.45,0.00,
			0.36,0.00,
			-0.24,-0.12,
			0.32,0.00,
			0.28,0.00,
			0.27,0.00,
			0.26,0.00,
			-0.21,0.00,
			0.19,0.00,
			0.18,0.00,
			-0.10,0.05,
			0.15,0.00,
			-0.14,0.00,
			0.14,0.00,
			-0.14,0.00,
			0.14,0.00,
			0.13,0.00,
			-0.11,0.00,
			0.11,0.00,
			0.11,0.00,
		};

		private static readonly int[] stfarg = new int[SIDTNTERM * SIDTNARG]
		{
		 // l    l'   F    D   Om   L_Me L_Ve L_E  L_Ma L_J  L_Sa L_U  L_Ne p_A
			0,   0,   0,   0,   1,   0,   0,   0,   0,   0,   0,   0,   0,   0,
			0,   0,   0,   0,   2,   0,   0,   0,   0,   0,   0,   0,   0,   0,
			0,   0,   2,  -2,   3,   0,   0,   0,   0,   0,   0,   0,   0,   0,
			0,   0,   2,  -2,   1,   0,   0,   0,   0,   0,   0,   0,   0,   0,
			0,   0,   2,  -2,   2,   0,   0,   0,   0,   0,   0,   0,   0,   0,
			0,   0,   2,   0,   3,   0,   0,   0,   0,   0,   0,   0,   0,   0,
			0,   0,   2,   0,   1,   0,   0,   0,   0,   0,   0,   0,   0,   0,
			0,   0,   0,   0,   3,   0,   0,   0,   0,   0,   0,   0,   0,   0,
			0,   1,   0,   0,   1,   0,   0,   0,   0,   0,   0,   0,   0,   0,
			0,   1,   0,   0,  -1,   0,   0,   0,   0,   0,   0,   0,   0,   0,
			1,   0,   0,   0,  -1,   0,   0,   0,   0,   0,   0,   0,   0,   0,
			1,   0,   0,   0,   1,   0,   0,   0,   0,   0,   0,   0,   0,   0,
			0,   1,   2,  -2,   3,   0,   0,   0,   0,   0,   0,   0,   0,   0,
			0,   1,   2,  -2,   1,   0,   0,   0,   0,   0,   0,   0,   0,   0,
			0,   0,   4,  -4,   4,   0,   0,   0,   0,   0,   0,   0,   0,   0,
			0,   0,   1,  -1,   1,   0,  -8,  12,   0,   0,   0,   0,   0,   0,
			0,   0,   2,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,
			0,   0,   2,   0,   2,   0,   0,   0,   0,   0,   0,   0,   0,   0,
			1,   0,   2,   0,   3,   0,   0,   0,   0,   0,   0,   0,   0,   0,
			1,   0,   2,   0,   1,   0,   0,   0,   0,   0,   0,   0,   0,   0,
			0,   0,   2,  -2,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,
			0,   1,  -2,   2,  -3,   0,   0,   0,   0,   0,   0,   0,   0,   0,
			0,   1,  -2,   2,  -1,   0,   0,   0,   0,   0,   0,   0,   0,   0,
			0,   0,   0,   0,   0,   0,   8, -13,   0,   0,   0,   0,   0,  -1,
			0,   0,   0,   2,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,
			2,   0,  -2,   0,  -1,   0,   0,   0,   0,   0,   0,   0,   0,   0,
			1,   0,   0,  -2,   1,   0,   0,   0,   0,   0,   0,   0,   0,   0,
			0,   1,   2,  -2,   2,   0,   0,   0,   0,   0,   0,   0,   0,   0,
			1,   0,   0,  -2,  -1,   0,   0,   0,   0,   0,   0,   0,   0,   0,
			0,   0,   4,  -2,   4,   0,   0,   0,   0,   0,   0,   0,   0,   0,
			0,   0,   2,  -2,   4,   0,   0,   0,   0,   0,   0,   0,   0,   0,
			1,   0,  -2,   0,  -3,   0,   0,   0,   0,   0,   0,   0,   0,   0,
			1,   0,  -2,   0,  -1,   0,   0,   0,   0,   0,   0,   0,   0,   0,
		};

		static double sidtime_non_polynomial_part(double tt)
		{
			double[] delm = new double[SIDTNARG];
			/* L Mean anomaly of the Moon.*/
			delm[0] = swe_radnorm(2.35555598 + 8328.6914269554 * tt);
			/* LSU Mean anomaly of the Sun.*/
			delm[1] = swe_radnorm(6.24006013 + 628.301955 * tt);
			/* F Mean argument of the latitude of the Moon. */
			delm[2] = swe_radnorm(1.627905234 + 8433.466158131 * tt);
			/* D Mean elongation of the Moon from the Sun. */
			delm[3] = swe_radnorm(5.198466741 + 7771.3771468121 * tt);
			/* OM Mean longitude of the ascending node of the Moon. */
			delm[4] = swe_radnorm(2.18243920 - 33.757045 * tt);
			/* Planetary longitudes, Mercury through Neptune (Souchay et al. 1999). 
			 * LME, LVE, LEA, LMA, LJU, LSA, LUR, LNE */
			delm[5] = swe_radnorm(4.402608842 + 2608.7903141574 * tt);
			delm[6] = swe_radnorm(3.176146697 + 1021.3285546211 * tt);
			delm[7] = swe_radnorm(1.753470314 + 628.3075849991 * tt);
			delm[8] = swe_radnorm(6.203480913 + 334.0612426700 * tt);
			delm[9] = swe_radnorm(0.599546497 + 52.9690962641 * tt);
			delm[10] = swe_radnorm(0.874016757 + 21.3299104960 * tt);
			delm[11] = swe_radnorm(5.481293871 + 7.4781598567 * tt);
			delm[12] = swe_radnorm(5.321159000 + 3.8127774000 * tt);
			/* PA General accumulated precession in longitude. */
			delm[13] = (0.02438175 + 0.00000538691 * tt) * tt;
			double dadd = -0.87 * Math.Sin(delm[4]) * tt;
			for (int i = 0; i < SIDTNTERM; i++)
			{
				double darg = 0;
				for (int j = 0; j < SIDTNARG; j++)
				{
					darg += stfarg[i * SIDTNARG + j] * delm[j];
				}
				dadd += stcf[i * 2] * Math.Sin(darg) + stcf[i * 2 + 1] * Math.Cos(darg);
			}
			return dadd / (3600.0 * 1000000.0);
		}
	}
}
