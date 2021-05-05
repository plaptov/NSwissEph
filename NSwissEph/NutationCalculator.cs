using System;
using System.Collections.Generic;
using NSwissEph.Internals;
using static NSwissEph.Consts;
using static NSwissEph.Internals.Calculus;
using static NSwissEph.Internals.NutationTables;

namespace NSwissEph
{
	public static class NutationCalculator
	{
		private const double DPSI_IAU1980_TJD0 = 64.284 / 1000.0; // arcsec
		private const double DEPS_IAU1980_TJD0 = 6.151 / 1000.0; // arcsec
		private const double NUT_SPEED_INTV = 0.0001;
		private static readonly JulianDayNumber DPSI_DEPS_IAU1980_TJD0_HORIZONS = JulianDayNumber.FromRaw(2437684.5);

		/// <summary>
		/// <see cref="swi_check_nutation"/>
		/// </summary>
		internal static Nutation Calculate(JulianDayNumber tjd, SweData swed)
		{
			var (longitude, obliquity) = swi_nutation(tjd, swed);
			var matrix = nut_matrix(longitude, obliquity, swed.oec);
			return new Nutation(tjd, longitude, obliquity, matrix);
		}

		internal static Nutation CalculateForSpeed(JulianDayNumber tjd, SweData swed) =>
			Calculate(tjd.AddDays(-NUT_SPEED_INTV), swed);

		private static (double longitude, double obliquity) swi_nutation(JulianDayNumber tjd, SweData swed)
		{
			// from interpolation, with three data points in 1-day steps;
			// maximum error is about 3 mas
			if (swed.InterpolateNutation)
			{
				// precalculated data points available
				if (swed.InterpolatedNutation != null && tjd < swed.InterpolatedNutation.Time2 && tjd > swed.InterpolatedNutation.Time0)
				{
					var dx = (tjd - swed.InterpolatedNutation.Time0).Raw - 1.0;
					var longitude = quadratic_intp(swed.InterpolatedNutation.DPsi0, swed.InterpolatedNutation.DPsi1, swed.InterpolatedNutation.DPsi2, dx);
					var obliquity = quadratic_intp(swed.InterpolatedNutation.DEps0, swed.InterpolatedNutation.DEps1, swed.InterpolatedNutation.DEps2, dx);
					return (longitude, obliquity);
				}
				else
				{
					var interpol = new InterpolatedNutation
					{
						Time0 = tjd.AddDays(-1.0), // one day earlier
						Time2 = tjd.AddDays(1.0) // one day later
					};

					(interpol.DPsi0, interpol.DEps0) = calc_nutation(interpol.Time0, swed);
					(interpol.DPsi2, interpol.DEps2) = calc_nutation(interpol.Time2, swed);
					(interpol.DPsi1, interpol.DEps1) = calc_nutation(tjd, swed);

					swed.InterpolatedNutation = interpol;
					return (interpol.DPsi1, interpol.DEps1);
				}
			}
			return calc_nutation(tjd, swed);
		}

		private static double[,] nut_matrix(double longitude, double obliquity, Epsilon oe)
		{
			var psi = longitude;
			var eps = oe.Eps + obliquity;
			var sinpsi = Math.Sin(psi);
			var cospsi = Math.Cos(psi);
			var sineps0 = oe.SinEps;
			var coseps0 = oe.CosEps;
			var sineps = Math.Sin(eps);
			var coseps = Math.Cos(eps);
			var matrix = new double[3, 3];
			matrix[0, 0] = cospsi;
			matrix[0, 1] = sinpsi * coseps;
			matrix[0, 2] = sinpsi * sineps;
			matrix[1, 0] = -sinpsi * coseps0;
			matrix[1, 1] = cospsi * coseps * coseps0 + sineps * sineps0;
			matrix[1, 2] = cospsi * sineps * coseps0 - coseps * sineps0;
			matrix[2, 0] = -sinpsi * sineps0;
			matrix[2, 1] = cospsi * coseps * sineps0 - sineps * coseps0;
			matrix[2, 2] = cospsi * sineps * sineps0 + coseps * coseps0;
			return matrix;
		}

		private static double quadratic_intp(double ym, double y0, double yp, double x)
		{
			double c = y0;
			double b = (yp - ym) / 2.0;
			double a = (yp + ym) / 2.0 - c;
			return a * x * x + b * x + c;
		}

		private static double bessel(IReadOnlyList<double> v, int n, double t)
		{
			if (t <= 0)
				return v[0];
			if (t >= n - 1)
				return v[n - 1];
			double p = Math.Floor(t);
			int iy = (int)t;
			/* Zeroth order estimate is value at start of year */
			double ans = v[iy];
			int k = iy + 1;
			if (k >= n)
				return ans;
			/* The fraction of tabulation interval */
			p = t - p;
			ans += p * (v[k] - v[iy]);
			if ((iy - 1 < 0) || (iy + 2 >= n))
				return ans; /* can't do second differences */
			/* Make table of first differences */
			Span<double> d = stackalloc double[6];
			k = iy - 2;
			for (int i = 0; i < 5; i++)
			{
				if ((k < 0) || (k + 1 >= n))
					d[i] = 0;
				else
					d[i] = v[k + 1] - v[k];
				k += 1;
			}
			/* Compute second differences */
			for (int i = 0; i < 4; i++)
				d[i] = d[i + 1] - d[i];
			double B = 0.25 * p * (p - 1.0);
			ans += B * (d[1] + d[2]);
			if (iy + 2 >= n)
				return ans;
			/* Compute third differences */
			for (int i = 0; i < 3; i++)
				d[i] = d[i + 1] - d[i];
			B = 2.0 * B / 3.0;
			ans += (p - 0.5) * B * d[1];
			if ((iy - 2 < 0) || (iy + 3 > n))
				return ans;
			/* Compute fourth differences */
			for (int i = 0; i < 2; i++)
				d[i] = d[i + 1] - d[i];
			B = 0.125 * B * (p + 1.0) * (p - 2.0);
			ans += B * (d[0] + d[1]);
			return ans;
		}

		private static (double longitude, double obliquity) calc_nutation(JulianDayNumber J, SweData swed)
		{
			int n;
			double dpsi, deps;
			double longitude = 0.0, obliquity = 0.0;
			var jplhora_model = swed.JplHorizonsMode;
			var nut_model = swed.NutationModel;
			bool is_jplhor = false;
			if (swed.Iflag.HasFlag(SEFLG.JPLHOR))
				is_jplhor = true;
			if (swed.Iflag.HasFlag(SEFLG.JPLHOR_APPROX) &&
				jplhora_model == JplHorizonsMode.Three
				&& J <= HORIZONS_TJD0_DPSI_DEPS_IAU1980)
				is_jplhor = true;

			if (is_jplhor)
			{
				(longitude, obliquity) = calc_nutation_iau1980(J, nut_model);
				if (swed.Iflag.HasFlag(SEFLG.JPLHOR))
				{
					n = (int)(swed.Eop.EndDate.Raw - swed.Eop.BeginDate.Raw + 0.000001);
					var J2 = J;
					if (J < DPSI_DEPS_IAU1980_TJD0_HORIZONS)
						J2 = DPSI_DEPS_IAU1980_TJD0_HORIZONS;
					var t = (J2 - swed.Eop.BeginDate).Raw;
					dpsi = bessel(swed.Eop.dPsi, n + 1, t);
					deps = bessel(swed.Eop.dEps, n + 1, t);
					longitude += dpsi / 3600.0 * DEGTORAD;
					obliquity += deps / 3600.0 * DEGTORAD;
				}
				else
				{
					longitude += DPSI_IAU1980_TJD0 / 3600.0 * DEGTORAD;
					obliquity += DEPS_IAU1980_TJD0 / 3600.0 * DEGTORAD;
				}
			}
			else if (nut_model == NutationModel.IAU_1980 || nut_model == NutationModel.IAU_CORR_1987)
			{
				(longitude, obliquity) = calc_nutation_iau1980(J, nut_model);
			}
			else if (nut_model == NutationModel.IAU_2000A || nut_model == NutationModel.IAU_2000B)
			{
				(longitude, obliquity) = calc_nutation_iau2000ab(J, nut_model);
				if (swed.Iflag.HasFlag(SEFLG.JPLHOR_APPROX) && jplhora_model == JplHorizonsMode.Two)
				{
					longitude += -41.7750 / 3600.0 / 1000.0 * DEGTORAD;
					obliquity += -6.8192 / 3600.0 / 1000.0 * DEGTORAD;
				}
			}
			return (longitude, obliquity);
		}

		private static (double longitude, double obliquity) calc_nutation_iau1980(JulianDayNumber J, NutationModel nut_model)
		{
			/* arrays to hold sines and cosines of multiple angles */
			double[,] ss = new double[5, 8];
			double[,] cc = new double[5, 8];
			double arg;
			double[] args = new double[5];
			double f, g, T, T2;
			double MM, MS, FF, DD, OM;
			double cu, su, cv, sv, sw, s;
			double C, D;
			int i, j, k, k1, m, n;
			int[] ns = new int[5];
			/* Julian centuries from 2000 January 1.5,
			 * barycentric dynamical time
			 */
			T = (J - JulianDayNumber.J2000).Raw / 36525.0;
			T2 = T * T;
			/* Fundamental arguments in the FK5 reference system.
			 * The coefficients, originally given to 0.001",
			 * are converted here to degrees.
			 */
			/* longitude of the mean ascending node of the lunar orbit
			 * on the ecliptic, measured from the mean equinox of date
			 */
			OM = -6962890.539 * T + 450160.280 + (0.008 * T + 7.455) * T2;
			OM = swe_degnorm(OM / 3600) * DEGTORAD;
			/* mean longitude of the Sun minus the
			 * mean longitude of the Sun's perigee
			 */
			MS = 129596581.224 * T + 1287099.804 - (0.012 * T + 0.577) * T2;
			MS = swe_degnorm(MS / 3600) * DEGTORAD;
			/* mean longitude of the Moon minus the
			 * mean longitude of the Moon's perigee
			 */
			MM = 1717915922.633 * T + 485866.733 + (0.064 * T + 31.310) * T2;
			MM = swe_degnorm(MM / 3600) * DEGTORAD;
			/* mean longitude of the Moon minus the
			 * mean longitude of the Moon's node
			 */
			FF = 1739527263.137 * T + 335778.877 + (0.011 * T - 13.257) * T2;
			FF = swe_degnorm(FF / 3600) * DEGTORAD;
			/* mean elongation of the Moon from the Sun.
			 */
			DD = 1602961601.328 * T + 1072261.307 + (0.019 * T - 6.891) * T2;
			DD = swe_degnorm(DD / 3600) * DEGTORAD;
			args[0] = MM;
			ns[0] = 3;
			args[1] = MS;
			ns[1] = 2;
			args[2] = FF;
			ns[2] = 4;
			args[3] = DD;
			ns[3] = 4;
			args[4] = OM;
			ns[4] = 2;
			/* Calculate Math.Sin( i*MM ), etc. for needed multiple angles
			 */
			for (k = 0; k <= 4; k++)
			{
				arg = args[k];
				n = ns[k];
				su = Math.Sin(arg);
				cu = Math.Cos(arg);
				ss[k, 0] = su;          /* Math.Sin(L) */
				cc[k, 0] = cu;          /* Math.Cos(L) */
				sv = 2.0 * su * cu;
				cv = cu * cu - su * su;
				ss[k, 1] = sv;          /* Math.Sin(2L) */
				cc[k, 1] = cv;
				for (i = 2; i < n; i++)
				{
					s = su * cv + cu * sv;
					cv = cu * cv - su * sv;
					sv = s;
					ss[k, i] = sv;      /* Math.Sin( i+1 L ) */
					cc[k, i] = cv;
				}
			}
			/* first terms, not in table: */
			C = (-0.01742 * T - 17.1996) * ss[4, 0];    /* Math.Sin(OM) */
			D = (0.00089 * T + 9.2025) * cc[4, 0];  /* Math.Cos(OM) */
			foreach (short[] p in nt)
			{
				if (nut_model != NutationModel.IAU_CORR_1987 && (p[0] == 101 || p[0] == 102))
					continue;
				/* argument of sine and cosine */
				k1 = 0;
				cv = 0.0;
				sv = 0.0;
				for (m = 0; m < 5; m++)
				{
					j = p[m];
					if (j > 100)
						j = 0; /* p[0] is a flag */
					if (j > 0)
					{
						k = j;
						if (j < 0)
							k = -k;
						su = ss[m, k - 1]; /* Math.Sin(k*angle) */
						if (j < 0)
							su = -su;
						cu = cc[m, k - 1];
						if (k1 == 0)
						{ /* set first angle */
							sv = su;
							cv = cu;
							k1 = 1;
						}
						else
						{       /* combine angles */
							sw = su * cv + cu * sv;
							cv = cu * cv - su * sv;
							sv = sw;
						}
					}
				}
				/* longitude coefficient, in 0.0001" */
				f = p[5] * 0.0001;
				if (p[6] != 0)
					f += 0.00001 * T * p[6];
				/* obliquity coefficient, in 0.0001" */
				g = p[7] * 0.0001;
				if (p[8] != 0)
					g += 0.00001 * T * p[8];
				if (p[0] >= 100)
				{   /* coefficients in 0.00001" */
					f *= 0.1;
					g *= 0.1;
				}
				/* accumulate the terms */
				if (p[0] != 102)
				{
					C += f * sv;
					D += g * cv;
				}
				else
				{       /* Math.Cos for nutl and Math.Sin for nuto */
					C += f * cv;
					D += g * sv;
				}
			}
			/* Save answers, expressed in radians */
			var longitude = DEGTORAD * C / 3600.0;
			var obliquity = DEGTORAD * D / 3600.0;
			return (longitude, obliquity);
		}

		/// <summary
		/// 0.1 microarcsecond to degrees
		/// </summary>
		private const double O1MAS2DEG = 1 / 3600.0 / 10000000.0;

		/* Number of terms in the luni-solar nutation model */
		private const int NLS = 678;
		private const int NLS_2000B = 77;

		/* Number of terms in the planetary nutation model */
		private const int NPL = 687;

		private static (double longitude, double obliquity) calc_nutation_iau2000ab(JulianDayNumber J, NutationModel nut_model)
		{
			int i, j, k, inls;
			double M, SM, F, D, OM;
			double AL, ALSU, AF, AD, AOM, APA;
			double ALME, ALVE, ALEA, ALMA, ALJU, ALSA, ALUR, ALNE;
			double darg, sinarg, cosarg;
			double dpsi = 0, deps = 0;
			double T = (J - JulianDayNumber.J2000).Raw / 36525.0;
			/* luni-solar nutation */
			/* Fundamental arguments, Simon & al. (1994) */
			/* Mean anomaly of the Moon. */
			M = swe_degnorm((485868.249036 +
					T * (1717915923.2178 +
					T * (31.8792 +
					T * (0.051635 +
					T * (-0.00024470))))) / 3600.0) * DEGTORAD;
			/* Mean anomaly of the Sun */
			SM = swe_degnorm((1287104.79305 +
					T * (129596581.0481 +
					T * (-0.5532 +
					T * (0.000136 +
					T * (-0.00001149))))) / 3600.0) * DEGTORAD;
			/* Mean argument of the latitude of the Moon. */
			F = swe_degnorm((335779.526232 +
					T * (1739527262.8478 +
					T * (-12.7512 +
					T * (-0.001037 +
					T * (0.00000417))))) / 3600.0) * DEGTORAD;
			/* Mean elongation of the Moon from the Sun. */
			D = swe_degnorm((1072260.70369 +
					T * (1602961601.2090 +
					T * (-6.3706 +
					T * (0.006593 +
					T * (-0.00003169))))) / 3600.0) * DEGTORAD;
			/* Mean longitude of the ascending node of the Moon. */
			OM = swe_degnorm((450160.398036 +
					T * (-6962890.5431 +
					T * (7.4722 +
					T * (0.007702 +
					T * (-0.00005939))))) / 3600.0) * DEGTORAD;
			/* luni-solar nutation series, in reverse order, starting with small terms */
			if (nut_model == NutationModel.IAU_2000B)
				inls = NLS_2000B;
			else
				inls = NLS;
			for (i = inls - 1; i >= 0; i--)
			{
				j = i * 5;
				darg = swe_radnorm(
					nls[j + 0] * M +
					nls[j + 1] * SM +
					nls[j + 2] * F +
					nls[j + 3] * D +
					nls[j + 4] * OM);
				sinarg = Math.Sin(darg);
				cosarg = Math.Cos(darg);
				k = i * 6;
				dpsi += (cls[k + 0] + cls[k + 1] * T) * sinarg + cls[k + 2] * cosarg;
				deps += (cls[k + 3] + cls[k + 4] * T) * cosarg + cls[k + 5] * sinarg;
			}
			var longitude = dpsi * O1MAS2DEG;
			var obliquity = deps * O1MAS2DEG;
			if (nut_model == NutationModel.IAU_2000A)
			{
				/* planetary nutation 
				 * note: The MHB2000 code computes the luni-solar and planetary nutation
				 * in different routines, using slightly different Delaunay
				 * arguments in the two cases.  This behaviour is faithfully
				 * reproduced here.  Use of the Simon et al. expressions for both
				 * cases leads to negligible changes, well below 0.1 microarcsecond.*/
				/* Mean anomaly of the Moon.*/
				AL = swe_radnorm(2.35555598 + 8328.6914269554 * T);
				/* Mean anomaly of the Sun.*/
				ALSU = swe_radnorm(6.24006013 + 628.301955 * T);
				/* Mean argument of the latitude of the Moon. */
				AF = swe_radnorm(1.627905234 + 8433.466158131 * T);
				/* Mean elongation of the Moon from the Sun. */
				AD = swe_radnorm(5.198466741 + 7771.3771468121 * T);
				/* Mean longitude of the ascending node of the Moon. */
				AOM = swe_radnorm(2.18243920 - 33.757045 * T);
				/* Planetary longitudes, Mercury through Neptune (Souchay et al. 1999). */
				ALME = swe_radnorm(4.402608842 + 2608.7903141574 * T);
				ALVE = swe_radnorm(3.176146697 + 1021.3285546211 * T);
				ALEA = swe_radnorm(1.753470314 + 628.3075849991 * T);
				ALMA = swe_radnorm(6.203480913 + 334.0612426700 * T);
				ALJU = swe_radnorm(0.599546497 + 52.9690962641 * T);
				ALSA = swe_radnorm(0.874016757 + 21.3299104960 * T);
				ALUR = swe_radnorm(5.481293871 + 7.4781598567 * T);
				ALNE = swe_radnorm(5.321159000 + 3.8127774000 * T);
				/* General accumulated precession in longitude. */
				APA = (0.02438175 + 0.00000538691 * T) * T;
				/* planetary nutation series (in reverse order).*/
				dpsi = 0;
				deps = 0;
				for (i = NPL - 1; i >= 0; i--)
				{
					j = i * 14;
					darg = swe_radnorm(
						npl[j + 0] * AL +
						npl[j + 1] * ALSU +
						npl[j + 2] * AF +
						npl[j + 3] * AD +
						npl[j + 4] * AOM +
						npl[j + 5] * ALME +
						npl[j + 6] * ALVE +
						npl[j + 7] * ALEA +
						npl[j + 8] * ALMA +
						npl[j + 9] * ALJU +
						npl[j + 10] * ALSA +
						npl[j + 11] * ALUR +
						npl[j + 12] * ALNE +
						npl[j + 13] * APA);
					k = i * 4;
					sinarg = Math.Sin(darg);
					cosarg = Math.Cos(darg);
					dpsi += icpl[k + 0] * sinarg + icpl[k + 1] * cosarg;
					deps += icpl[k + 2] * sinarg + icpl[k + 3] * cosarg;
				}
				longitude += dpsi * O1MAS2DEG;
				obliquity += deps * O1MAS2DEG;
				/* changes required by adoption of P03 precession 
				 * according to Capitaine et al. A & A 412, 366 (2005) = IAU 2006 */
				dpsi = -8.1 * Math.Sin(OM) - 0.6 * Math.Sin(2 * F - 2 * D + 2 * OM);
				dpsi += T * (47.8 * Math.Sin(OM) + 3.7 * Math.Sin(2 * F - 2 * D + 2 * OM) + 0.6 * Math.Sin(2 * F + 2 * OM) - 0.6 * Math.Sin(2 * OM));
				deps = T * (-25.6 * Math.Cos(OM) - 1.6 * Math.Cos(2 * F - 2 * D + 2 * OM));
				longitude += dpsi / (3600.0 * 1000000.0);
				obliquity += deps / (3600.0 * 1000000.0);
			}
			longitude *= DEGTORAD;
			obliquity *= DEGTORAD;
			return (longitude, obliquity);
		}

	}
}