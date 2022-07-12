using System;
using System.Collections.Generic;
using System.Text;

using NSwissEph.Internals;

using static NSwissEph.Consts;
using static NSwissEph.JulianDayNumber;

namespace NSwissEph
{
	/* Precession of the equinox and ecliptic
	 * from epoch Julian date J to or from J2000.0
	 *
	 * Original program by Steve Moshier.
	 * Changes in program structure and implementation of IAU 2003 (P03) and
	 * Vondrak 2011 by Dieter Koch.
	 *
	 * SEMOD_PREC_VONDRAK_2011 
	 * J. Vondrák, N. Capitaine, and P. Wallace, "New precession expressions,
	 * valid for long time intervals", A&A 534, A22 (2011)
	 *
	 * SEMOD_PREC_IAU_2006 
	 * N. Capitaine, P.T. Wallace, and J. Chapront, "Expressions for IAU 2000
	 * precession quantities", 2003, A&A 412, 567-586 (2003).
	 * This is a "short" term model, that can be combined with other models
	 *
	 * SEMOD_PREC_WILLIAMS_1994 
	 * James G. Williams, "Contributions to the Earth's obliquity rate,
	 * precession, and nutation,"  Astron. J. 108, 711-724 (1994).
	 *
	 * SEMOD_PREC_SIMON_1994 
	 * J. L. Simon, P. Bretagnon, J. Chapront, M. Chapront-Touze', G. Francou,
	 * and J. Laskar, "Numerical Expressions for precession formulae and
	 * mean elements for the Moon and the planets," Astronomy and Astrophysics
	 * 282, 663-683 (1994).  
	 *
	 * SEMOD_PREC_IAU_1976 
	 * IAU Coefficients are from:
	 * J. H. Lieske, T. Lederle, W. Fricke, and B. Morando,
	 * "Expressions for the Precession Quantities Based upon the IAU
	 * (1976) System of Astronomical Constants,"  Astronomy and
	 * Astrophysics 58, 1-16 (1977).
	 * This is a "short" term model, that can be combined with other models
	 *
	 * SEMOD_PREC_LASKAR_1986 
	 * Newer formulas that cover a much longer time span are from:
	 * J. Laskar, "Secular terms of classical planetary theories
	 * using the results of general theory," Astronomy and Astrophysics
	 * 157, 59070 (1986).
	 *
	 * See also:
	 * P. Bretagnon and G. Francou, "Planetary theories in rectangular
	 * and spherical variables. VSOP87 solutions," Astronomy and
	 * Astrophysics 202, 309-315 (1988).
	 *
	 * Bretagnon and Francou's expansions for the node and inclination
	 * of the ecliptic were derived from Laskar's data but were truncated
	 * after the term in T**6. I have recomputed these expansions from
	 * Laskar's data, retaining powers up to T**10 in the result.
	 *
	 */

	public class Precession
	{
		private static readonly JulianDayNumber B1850 = JulianDayNumber.FromRaw(2396758.2035810); /* 1850 January 16:53 */

		static double[] precess_1(double[] R, JulianDayNumber J, int direction, PrecessionModel prec_method)
		{
			double Z = 0, z = 0, TH = 0;
			double sinth, costh, sinZ, cosZ, sinz, cosz, A, B;
			if (J == J2000)
				return R;
			double T = (J - J2000) / 36525.0;
			if (prec_method == PrecessionModel.IAU_1976)
			{
				Z = ((0.017998 * T + 0.30188) * T + 2306.2181) * T * DEGTORAD / 3600;
				z = ((0.018203 * T + 1.09468) * T + 2306.2181) * T * DEGTORAD / 3600;
				TH = ((-0.041833 * T - 0.42665) * T + 2004.3109) * T * DEGTORAD / 3600;
				/*
				 * precession relative to ecliptic of start epoch is:
				 * pn = (5029.0966 + 2.22226*T-0.000042*T*T) * t + (1.11161-0.000127*T) * t * t - 0.000113*t*t*t;
				 * with: t = (tstart - tdate) / 36525.0
				 *       T = (tstart - J2000) / 36525.0
				 */
			}
			else if (prec_method == PrecessionModel.IAU_2000)
			{
				/* AA 2006 B28:*/
				Z = (((((-0.0000002 * T - 0.0000327) * T + 0.0179663) * T + 0.3019015) * T + 2306.0809506) * T + 2.5976176) * DEGTORAD / 3600;
				z = (((((-0.0000003 * T - 0.000047) * T + 0.0182237) * T + 1.0947790) * T + 2306.0803226) * T - 2.5976176) * DEGTORAD / 3600;
				TH = ((((-0.0000001 * T - 0.0000601) * T - 0.0418251) * T - 0.4269353) * T + 2004.1917476) * T * DEGTORAD / 3600;
			}
			else if (prec_method == PrecessionModel.IAU_2006)
			{
				T = (J - J2000) / 36525.0;
				Z = (((((-0.0000003173 * T - 0.000005971) * T + 0.01801828) * T + 0.2988499) * T + 2306.083227) * T + 2.650545) * DEGTORAD / 3600;
				z = (((((-0.0000002904 * T - 0.000028596) * T + 0.01826837) * T + 1.0927348) * T + 2306.077181) * T - 2.650545) * DEGTORAD / 3600;
				TH = ((((-0.00000011274 * T - 0.000007089) * T - 0.04182264) * T - 0.4294934) * T + 2004.191903) * T * DEGTORAD / 3600;
			}
			else if (prec_method == PrecessionModel.BRETAGNON_2003)
			{
				Z = ((((((-0.00000000013 * T - 0.0000003040) * T - 0.000005708) * T + 0.01801752) * T + 0.3023262) * T + 2306.080472) * T + 2.72767) * DEGTORAD / 3600;
				z = ((((((-0.00000000005 * T - 0.0000002486) * T - 0.000028276) * T + 0.01826676) * T + 1.0956768) * T + 2306.076070) * T - 2.72767) * DEGTORAD / 3600;
				TH = ((((((0.000000000009 * T + 0.00000000036) * T - 0.0000001127) * T - 0.000007291) * T - 0.04182364) * T - 0.4266980) * T + 2004.190936) * T * DEGTORAD / 3600;
#if false
  } else if (prec_method == SEMOD_PREC_NEWCOMB) {
    double t1 = (J2000 - 2415020.3135) / 36524.2199;
    double T = (J - J2000) / 36524.2199;
    double T2 = T * T; double T3 = T2 * T;
    Z = (2304.250 + 1.396 * t1) * T + 0.302 * T2 + 0.0179 * T3;
    z = (2304.250 + 1.396 * t1) * T + 1.093 * T2 + 0.0192 * T3;
    TH =(2004.682 - 0.853 * t1) * T - 0.426 * T2 - 0.0416 * T3;
    Z *= (DEGTORAD/3600.0);
    z *= (DEGTORAD/3600.0);
    TH *= (DEGTORAD/3600.0);
#endif
#if false
  // from Newcomb, "Compendium" (1906), pp. 245f., relative to 1850
/* } else if (prec_method == SEMOD_PREC_NEWCOMB) {
    double cties = 36524.2198782; // trop. centuries
    double T = (J - B1850) / cties;
    double T2 = T * T; double T3 = T2 * T;
    double Z1 = 2303.56;
    Z = 2303.56 * T + 0.3023 * T2 + 0.018 * T3;
    z = 2303.55 * T + 1.094 * T2 + 0.018 * T3;
    TH = 2005.11 * T - 0.43 * T2 - 0.041 * T3;
    Z *= (DEGTORAD/3600.0);
    z *= (DEGTORAD/3600.0);
    TH *= (DEGTORAD/3600.0);
*/
#endif
#if false
  // Newcomb from Expl. supp. 61 pg. 38 
  // "Andoyar (Woolard and Clemence) expressions":
  } else if (prec_method == SEMOD_PREC_NEWCOMB) {
    double mills = 365242.198782; // trop. millennia
    double t1 = (J2000 - B1850) / mills;
    double t2 = (J - B1850) / mills;
    double T = t2 - t1;
    double T2 = T * T; double T3 = T2 * T;
    double Z1 = 23035.545 + 139.720 * t1 + 0.060 * t1 * t1;
    Z = Z1 * T + (30.240 - 0.270 * t1) * T2 + 17.995 * T3;
    z = Z1 * T + (109.480 - 0.390 * t1) * T2 + 18.325 * T3;
    TH = (20051.12 - 85.29 * t1 - 0.37 * t1 * t1) * T + (-42.65 - 0.37 * t1) * T2 - 41.80 * T3;
    Z *= (DEGTORAD/3600.0);
    z *= (DEGTORAD/3600.0);
    TH *= (DEGTORAD/3600.0);
#endif
#if true
				// Newcomb according to Kinoshita 1975, very close to ExplSuppl/Andoyer;
				// one additional digit.
			}
			else if (prec_method == PrecessionModel.NEWCOMB)
			{
				double mills = 365242.198782; // trop. millennia
				double t1 = (J2000 - B1850) / mills;
				double t2 = (J - B1850) / mills;
				T = t2 - t1;
				double T2 = T * T; double T3 = T2 * T;
				double Z1 = 23035.5548 + 139.720 * t1 + 0.069 * t1 * t1;
				Z = Z1 * T + (30.242 - 0.269 * t1) * T2 + 17.996 * T3;
				z = Z1 * T + (109.478 - 0.387 * t1) * T2 + 18.324 * T3;
				TH = (20051.125 - 85.294 * t1 - 0.365 * t1 * t1) * T + (-42.647 - 0.365 * t1) * T2 - 41.802 * T3;
				Z *= (DEGTORAD / 3600.0);
				z *= (DEGTORAD / 3600.0);
				TH *= (DEGTORAD / 3600.0);
#endif
#if false
  // from Lieske, "Expressions for the Precession Quantities..." (1967), p. 20
  } else if (prec_method == SEMOD_PREC_NEWCOMB) {
    double cties = 36524.2198782; // trop. centuries
    double t1 = (J2000 - J1900) / cties;
    double t2 = (J - J1900) / cties;
    double T = t2 - t1;
    double T2 = T * T; double T3 = T2 * T;
    double Z1 = 2304.253 + 1.3972 * t1 + 0.000125 * t1 * t1;
    Z = Z1 * T + (0.3023 - 0.000211 * t1) * T2 + 0.0180 * T3;
    z = Z1 * T + (1.0949 - 0.00046 * t1) * T2 + 0.0183 * T3;
    TH = (2004.684 - 0.8532 * t1 - 0.000317 * t1 * t1) * T + (-0.4266 - 0.00032 * t1) * T2 - 0.0418 * T3;
    Z *= (DEGTORAD/3600.0);
    z *= (DEGTORAD/3600.0);
    TH *= (DEGTORAD/3600.0);
#endif
			}
			else
			{
				return R;
			}
			sinth = Math.Sin(TH);
			costh = Math.Cos(TH);
			sinZ = Math.Sin(Z);
			cosZ = Math.Cos(Z);
			sinz = Math.Sin(z);
			cosz = Math.Cos(z);
			A = cosZ * costh;
			B = sinZ * costh;
			double[] x = new double[3];
			if (direction < 0)
			{ /* From J2000.0 to J */
				x[0] = (A * cosz - sinZ * sinz) * R[0]
					- (B * cosz + cosZ * sinz) * R[1]
						  - sinth * cosz * R[2];
				x[1] = (A * sinz + sinZ * cosz) * R[0]
					- (B * sinz - cosZ * cosz) * R[1]
						  - sinth * sinz * R[2];
				x[2] = cosZ * sinth * R[0]
						  - sinZ * sinth * R[1]
						  + costh * R[2];
			}
			else
			{ /* From J to J2000.0 */
				x[0] = (A * cosz - sinZ * sinz) * R[0]
					+ (A * sinz + sinZ * cosz) * R[1]
						  + cosZ * sinth * R[2];
				x[1] = -(B * cosz + cosZ * sinz) * R[0]
					- (B * sinz - cosZ * cosz) * R[1]
						  - sinZ * sinth * R[2];
				x[2] = -sinth * cosz * R[0]
						  - sinth * sinz * R[1]
								  + costh * R[2];
			}
			return x;
		}

		/* In WILLIAMS and SIMON, Laskar's terms of order higher than t^4
		   have been retained, because Simon et al mention that the solution
		   is the same except for the lower order terms.  */

		/* SEMOD_PREC_WILLIAMS_1994 */
		static readonly double[] pAcof_williams = new[] {
			-8.66e-10, -4.759e-8, 2.424e-7, 1.3095e-5, 1.7451e-4, -1.8055e-3,
			-0.235316, 0.076, 110.5407, 50287.70000 };
		static readonly double[] nodecof_williams = new[] {
			6.6402e-16, -2.69151e-15, -1.547021e-12, 7.521313e-12, 1.9e-10,
			-3.54e-9, -1.8103e-7,  1.26e-7,  7.436169e-5,
			-0.04207794833,  3.052115282424};
		static readonly double[] inclcof_williams = new[] {
			1.2147e-16, 7.3759e-17, -8.26287e-14, 2.503410e-13, 2.4650839e-11,
			-5.4000441e-11, 1.32115526e-9, -6.012e-7, -1.62442e-5,
			0.00227850649, 0.0 };

		/* SEMOD_PREC_SIMON_1994 */
		/* Precession coefficients from Simon et al: */
		static readonly double[] pAcof_simon = new[] {
			-8.66e-10, -4.759e-8, 2.424e-7, 1.3095e-5, 1.7451e-4, -1.8055e-3,
			-0.235316, 0.07732, 111.2022, 50288.200 };
		static readonly double[] nodecof_simon = new[] {
			6.6402e-16, -2.69151e-15, -1.547021e-12, 7.521313e-12, 1.9e-10,
			-3.54e-9, -1.8103e-7, 2.579e-8, 7.4379679e-5,
			-0.0420782900, 3.0521126906};
		static readonly double[] inclcof_simon = new[] {
			1.2147e-16, 7.3759e-17, -8.26287e-14, 2.503410e-13, 2.4650839e-11,
			-5.4000441e-11, 1.32115526e-9, -5.99908e-7, -1.624383e-5,
			0.002278492868, 0.0 };

		/* SEMOD_PREC_LASKAR_1986 */
		/* Precession coefficients taken from Laskar's paper: */
		static readonly double[] pAcof_laskar = new[] {
			-8.66e-10, -4.759e-8, 2.424e-7, 1.3095e-5, 1.7451e-4, -1.8055e-3,
			-0.235316, 0.07732, 111.1971, 50290.966 };
		/* Node and inclination of the earth's orbit computed from
		 * Laskar's data as done in Bretagnon and Francou's paper.
		 * Units are radians.
		 */
		static readonly double[] nodecof_laskar = new[] {
			6.6402e-16, -2.69151e-15, -1.547021e-12, 7.521313e-12, 6.3190131e-10,
			-3.48388152e-9, -1.813065896e-7, 2.75036225e-8, 7.4394531426e-5,
			-0.042078604317, 3.052112654975 };
		static readonly double[] inclcof_laskar = new[] {
			1.2147e-16, 7.3759e-17, -8.26287e-14, 2.503410e-13, 2.4650839e-11,
			-5.4000441e-11, 1.32115526e-9, -5.998737027e-7, -1.6242797091e-5,
			0.002278495537, 0.0 };

		static double[] precess_2(double[] R, JulianDayNumber J, SEFLG iflag, int direction, PrecessionModel prec_method, SweData swed)
		{
			int i;
			double T, z;
			double eps, sineps, coseps;
			double[] x = new double[3];
			double A, B, pA, W;
			double[] pAcof, inclcof, nodecof;
			if (J == J2000)
				return R;
			if (prec_method == PrecessionModel.LASKAR_1986)
			{
				pAcof = pAcof_laskar;
				nodecof = nodecof_laskar;
				inclcof = inclcof_laskar;
			}
			else if (prec_method == PrecessionModel.SIMON_1994)
			{
				pAcof = pAcof_simon;
				nodecof = nodecof_simon;
				inclcof = inclcof_simon;
			}
			else if (prec_method == PrecessionModel.WILLIAMS_1994)
			{
				pAcof = pAcof_williams;
				nodecof = nodecof_williams;
				inclcof = inclcof_williams;
			}
			else
			{   /* default, to satisfy compiler */
				pAcof = pAcof_laskar;
				nodecof = nodecof_laskar;
				inclcof = inclcof_laskar;
			}
			T = (J - J2000) / 36525.0;
			/* Implementation by elementary rotations using Laskar's expansions.
			 * First rotate about the x axis from the initial equator
			 * to the ecliptic. (The input is equatorial.)
			 */
			if (direction == 1)
				eps = Epsilon.Calc(J, iflag, swed).Eps; /* To J2000 */
			else
				eps = Epsilon.Calc(J2000, iflag, swed).Eps; /* From J2000 */
			sineps = Math.Sin(eps);
			coseps = Math.Cos(eps);
			x[0] = R[0];
			z = coseps * R[1] + sineps * R[2];
			x[2] = -sineps * R[1] + coseps * R[2];
			x[1] = z;
			/* Precession in longitude */
			T /= 10.0; /* thousands of years */
			pA = pAcof[0];
			for (i = 0; i < 9; i++)
			{
				pA = pA * T + pAcof[i + 1];
			}
			pA *= DEGTORAD / 3600 * T;
			/* Node of the moving ecliptic on the J2000 ecliptic.
			 */
			W = nodecof[0];
			for (i = 0; i < 10; i++)
				W = W * T + nodecof[i + 1];
			/* Rotate about z axis to the node.
			 */
			if (direction == 1)
				z = W + pA;
			else
				z = W;
			B = Math.Cos(z);
			A = Math.Sin(z);
			z = B * x[0] + A * x[1];
			x[1] = -A * x[0] + B * x[1];
			x[0] = z;
			/* Rotate about new x axis by the inclination of the moving
			 * ecliptic on the J2000 ecliptic.
			 */
			z = inclcof[0];
			for (i = 0; i < 10; i++)
				z = z * T + inclcof[i + 1];
			if (direction == 1)
				z = -z;
			B = Math.Cos(z);
			A = Math.Sin(z);
			z = B * x[1] + A * x[2];
			x[2] = -A * x[1] + B * x[2];
			x[1] = z;
			/* Rotate about new z axis back from the node.
			 */
			if (direction == 1)
				z = -W;
			else
				z = -W - pA;
			B = Math.Cos(z);
			A = Math.Sin(z);
			z = B * x[0] + A * x[1];
			x[1] = -A * x[0] + B * x[1];
			x[0] = z;
			/* Rotate about x axis to final equator.
			 */
			if (direction == 1)
				eps = Epsilon.Calc(J2000, iflag, swed).Eps;
			else
				eps = Epsilon.Calc(J, iflag, swed).Eps;
			sineps = Math.Sin(eps);
			coseps = Math.Cos(eps);
			z = coseps * x[1] - sineps * x[2];
			x[2] = sineps * x[1] + coseps * x[2];
			x[1] = z;
			return x;
		}

		static double[] precess_3(double[] R, JulianDayNumber J, int direction, SEFLG iflag, PrecessionModel prec_meth)
		{
			int i, j;
			if (J == J2000)
				return R;
			/* Each precession angle is specified by a polynomial in
			 * T = Julian centuries from J2000.0.  See AA page B18.
			 */
			//T = (J - J2000)/36525.0;
			double[] pmat = new double[9];
			if (prec_meth == PrecessionModel.OWEN_1990)
				owen_pre_matrix(J, pmat, iflag);
			else
				pre_pmat(J, pmat);
			double[] x = new double[3];
			if (direction == -1)
			{
				for (i = 0, j = 0; i <= 2; i++, j = i * 3)
				{
					x[i] = R[0] * pmat[j + 0] +
						R[1] * pmat[j + 1] +
						R[2] * pmat[j + 2];
				}
			}
			else
			{
				for (i = 0, j = 0; i <= 2; i++, j = i * 3)
				{
					x[i] = R[0] * pmat[i + 0] +
						R[1] * pmat[i + 3] +
						R[2] * pmat[i + 6];
				}
			}
			return x;
		}

		/* Subroutine arguments:
		 *
		 * R = rectangular equatorial coordinate vector to be precessed.
		 *     The result is written back into the input vector.
		 * J = Julian date
		 * direction =
		 *      Precess from J to J2000: direction = 1
		 *      Precess from J2000 to J: direction = -1
		 * Note that if you want to precess from J1 to J2, you would
		 * first go from J1 to J2000, then call the program again
		 * to go from J2000 to J2.
		 */
		double[] swi_precess(double[] R, JulianDayNumber J, SEFLG iflag, int direction, SweData swed)
		{
			double T = (J - J2000) / 36525.0;
			PrecessionModel prec_model = swed.LongtermPrecessionMode;
			PrecessionModel prec_model_short = swed.ShorttermPrecessionMode;
			JplHorizonsMode jplhora_model = swed.JplHorizonsMode;
			bool is_jplhor = false;
			if (iflag.HasFlag(SEFLG.JPLHOR))
				is_jplhor = true;
			if (iflag.HasFlag(SEFLG.JPLHOR_APPROX)
				&& jplhora_model == JplHorizonsMode.Three
				&& J <= HORIZONS_TJD0_DPSI_DEPS_IAU1980)
				is_jplhor = true;
			/* JPL Horizons uses precession IAU 1976 and nutation IAU 1980 plus
			 * some correction to nutation, arriving at extremely high precision */
			if (is_jplhor)
			{
				if (J > J1799_01_01 && J < J2202_01_01)
				{ // between 1.1.1799 and 1.1.2202
					return precess_1(R, J, direction, PrecessionModel.IAU_1976);
				}
				else
				{
					return precess_3(R, J, direction, iflag, PrecessionModel.OWEN_1990);
				}
				/* Use IAU 1976 formula for a few centuries.  */
			}
			else if (prec_model_short == PrecessionModel.IAU_1976 && Math.Abs(T) <= PREC_IAU_1976_CTIES)
			{
				return precess_1(R, J, direction, PrecessionModel.IAU_1976);
			}
			else if (prec_model == PrecessionModel.IAU_1976)
			{
				return precess_1(R, J, direction, PrecessionModel.IAU_1976);
				/* Use IAU 2000 formula for a few centuries.  */
			}
			else if (prec_model_short == PrecessionModel.IAU_2000 && Math.Abs(T) <= PREC_IAU_2000_CTIES)
			{
				return precess_1(R, J, direction, PrecessionModel.IAU_2000);
			}
			else if (prec_model == PrecessionModel.IAU_2000)
			{
				return precess_1(R, J, direction, PrecessionModel.IAU_2000);
				/* Use IAU 2006 formula for a few centuries.  */
			}
			else if (prec_model_short == PrecessionModel.IAU_2006 && Math.Abs(T) <= PREC_IAU_2006_CTIES)
			{
				return precess_1(R, J, direction, PrecessionModel.IAU_2006);
			}
			else if (prec_model == PrecessionModel.IAU_2006)
			{
				return precess_1(R, J, direction, PrecessionModel.IAU_2006);
			}
			else if (prec_model == PrecessionModel.BRETAGNON_2003)
			{
				return precess_1(R, J, direction, PrecessionModel.BRETAGNON_2003);
			}
			else if (prec_model == PrecessionModel.NEWCOMB)
			{
				return precess_1(R, J, direction, PrecessionModel.NEWCOMB);
			}
			else if (prec_model == PrecessionModel.LASKAR_1986)
			{
				return precess_2(R, J, iflag, direction, PrecessionModel.LASKAR_1986, swed);
			}
			else if (prec_model == PrecessionModel.SIMON_1994)
			{
				return precess_2(R, J, iflag, direction, PrecessionModel.SIMON_1994, swed);
			}
			else if (prec_model == PrecessionModel.WILLIAMS_1994 || prec_model == PrecessionModel.WILL_EPS_LASK)
			{
				return precess_2(R, J, iflag, direction, PrecessionModel.WILLIAMS_1994, swed);
			}
			else if (prec_model == PrecessionModel.OWEN_1990)
			{
				return precess_3(R, J, direction, iflag, PrecessionModel.OWEN_1990);
			}
			else
			{ /* SEMOD_PREC_VONDRAK_2011 */
				return precess_3(R, J, direction, iflag, PrecessionModel.VONDRAK_2011);
			}
		}

	}
}
