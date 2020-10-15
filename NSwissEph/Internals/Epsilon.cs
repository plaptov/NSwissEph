/* Obliquity of the ecliptic at Julian date J
 *
 * IAU Coefficients are from:
 * J. H. Lieske, T. Lederle, W. Fricke, and B. Morando,
 * "Expressions for the Precession Quantities Based upon the IAU
 * (1976) System of Astronomical Constants,"  Astronomy and Astrophysics
 * 58, 1-16 (1977).
 *
 * Before or after 200 years from J2000, the formula used is from:
 * J. Laskar, "Secular terms of classical planetary theories
 * using the results of general theory," Astronomy and Astrophysics
 * 157, 59070 (1986).
 *
 * Bretagnon, P. et al.: 2003, "Expressions for Precession Consistent with 
 * the IAU 2000A Model". A&A 400,785
 *B03  	84381.4088  	-46.836051*t  	-1667*10-7*t2  	+199911*10-8*t3  	-523*10-9*t4  	-248*10-10*t5  	-3*10-11*t6
 *C03   84381.406  	-46.836769*t  	-1831*10-7*t2  	+20034*10-7*t3  	-576*10-9*t4  	-434*10-10*t5
 *
 *  See precess and page B18 of the Astronomical Almanac.
 */

using System;
using System.Linq;

using static NSwissEph.Consts;

namespace NSwissEph.Internals
{
	public class Epsilon
	{
		private const double OFFSET_EPS_JPLHORIZONS = 35.95;
		private static readonly JulianDayNumber DCOR_EPS_JPL_TJD0 = JulianDayNumber.FromRaw(2437846.5);
		private static readonly double[] dcor_eps_jpl = new[]
		{
			36.726, 36.627, 36.595, 36.578, 36.640, 36.659, 36.731, 36.765,
			36.662, 36.555, 36.335, 36.321, 36.354, 36.227, 36.289, 36.348, 36.257, 36.163,
			35.979, 35.896, 35.842, 35.825, 35.912, 35.950, 36.093, 36.191, 36.009, 35.943,
			35.875, 35.771, 35.788, 35.753, 35.822, 35.866, 35.771, 35.732, 35.543, 35.498,
			35.449, 35.409, 35.497, 35.556, 35.672, 35.760, 35.596, 35.565, 35.510, 35.394,
			35.385, 35.375, 35.415,
		};

		private Epsilon(JulianDayNumber date, double eps)
		{
			Date = date;
			Eps = eps;
			SinEps = Math.Sin(eps);
			CosEps = Math.Cos(eps);
		}

		public JulianDayNumber Date { get; }

		public double Eps { get; }

		public double SinEps { get; }

		public double CosEps { get; }

		public static Epsilon Calc(JulianDayNumber J, SEFLG iflag, SweData swed)
		{
			var prec_model = swed.LongtermPrecessionMode;
			var prec_model_short = swed.ShorttermPrecessionMode;
			var jplhora_model = swed.JplHorizonsMode;

			bool is_jplhor = false;
			if (iflag.HasFlag(SEFLG.JPLHOR))
				is_jplhor = true;

			if (iflag.HasFlag(SEFLG.JPLHOR_APPROX)
				&& jplhora_model == JplHorizonsMode.Three
				&& J <= HORIZONS_TJD0_DPSI_DEPS_IAU1980)
				is_jplhor = true;

			double T = (J - JulianDayNumber.J2000).Raw / 36525.0;

			double eps;
			if (is_jplhor)
			{
				if (J > JulianDayNumber.J1799_01_01 && J < JulianDayNumber.J2202_01_01)
				{ // between 1.1.1799 and 1.1.2202
					eps = (((1.813e-3 * T - 5.9e-4) * T - 46.8150) * T + 84381.448) * DEGTORAD / 3600;
				}
				else
				{
					eps = OwenPrecession.epsiln_owen_1986(J);
					eps *= DEGTORAD;
				}
			}
			else if (iflag.HasFlag(SEFLG.JPLHOR_APPROX) && jplhora_model == JplHorizonsMode.Two)
			{
				eps = (((1.813e-3 * T - 5.9e-4) * T - 46.8150) * T + 84381.448) * DEGTORAD / 3600;
			}
			else if (prec_model_short == PrecessionModel.IAU_1976 && Math.Abs(T) <= PREC_IAU_1976_CTIES)
			{
				eps = (((1.813e-3 * T - 5.9e-4) * T - 46.8150) * T + 84381.448) * DEGTORAD / 3600;
			}
			else if (prec_model == PrecessionModel.IAU_1976)
			{
				eps = (((1.813e-3 * T - 5.9e-4) * T - 46.8150) * T + 84381.448) * DEGTORAD / 3600;
			}
			else if (prec_model_short == PrecessionModel.IAU_2000 && Math.Abs(T) <= PREC_IAU_2000_CTIES)
			{
				eps = (((1.813e-3 * T - 5.9e-4) * T - 46.84024) * T + 84381.406) * DEGTORAD / 3600;
			}
			else if (prec_model == PrecessionModel.IAU_2000)
			{
				eps = (((1.813e-3 * T - 5.9e-4) * T - 46.84024) * T + 84381.406) * DEGTORAD / 3600;
			}
			else if (prec_model_short == PrecessionModel.IAU_2006 && Math.Abs(T) <= PREC_IAU_2006_CTIES)
			{
				eps = (((((-4.34e-8 * T - 5.76e-7) * T + 2.0034e-3) * T - 1.831e-4) * T - 46.836769) * T + 84381.406) * DEGTORAD / 3600.0;
			}
			else if (prec_model == PrecessionModel.IAU_2006)
			{
				eps = (((((-4.34e-8 * T - 5.76e-7) * T + 2.0034e-3) * T - 1.831e-4) * T - 46.836769) * T + 84381.406) * DEGTORAD / 3600.0;
			}
			else if (prec_model == PrecessionModel.BRETAGNON_2003)
			{
				eps = ((((((-3e-11 * T - 2.48e-8) * T - 5.23e-7) * T + 1.99911e-3) * T - 1.667e-4) * T - 46.836051) * T + 84381.40880) * DEGTORAD / 3600.0;
			}
			else if (prec_model == PrecessionModel.SIMON_1994)
			{
				eps = (((((2.5e-8 * T - 5.1e-7) * T + 1.9989e-3) * T - 1.52e-4) * T - 46.80927) * T + 84381.412) * DEGTORAD / 3600.0;/* */
			}
			else if (prec_model == PrecessionModel.WILLIAMS_1994)
			{
				eps = ((((-1.0e-6 * T + 2.0e-3) * T - 1.74e-4) * T - 46.833960) * T + 84381.409) * DEGTORAD / 3600.0;/* */
			}
			else if (prec_model == PrecessionModel.LASKAR_1986 || prec_model == PrecessionModel.WILL_EPS_LASK)
			{
				T /= 10.0;
				eps = (((((((((2.45e-10 * T + 5.79e-9) * T + 2.787e-7) * T
				+ 7.12e-7) * T - 3.905e-5) * T - 2.4967e-3) * T
				- 5.138e-3) * T + 1.99925) * T - 0.0155) * T - 468.093) * T
				+ 84381.448;
				eps *= DEGTORAD / 3600.0;
			}
			else if (prec_model == PrecessionModel.OWEN_1990)
			{
				eps = OwenPrecession.epsiln_owen_1986(J);
				eps *= DEGTORAD;
			}
			else
			{ /* SEMOD_PREC_VONDRAK_2011 */
				(_, eps) = Vondrak.swi_ldp_peps(J);
				if (iflag.HasFlag(SEFLG.JPLHOR_APPROX) && jplhora_model != JplHorizonsMode.Two)
				{
					double tofs = (J - DCOR_EPS_JPL_TJD0).Raw / 365.25;
					double dofs;
					if (tofs < 0)
					{
						dofs = dcor_eps_jpl[0];
					}
					else if (tofs >= dcor_eps_jpl.Length)
					{
						dofs = dcor_eps_jpl.Last();
					}
					else
					{
						int t0 = (int)tofs;
						int t1 = t0 + 1;
						dofs = (tofs - t0) * (dcor_eps_jpl[t0] - dcor_eps_jpl[t1]) + dcor_eps_jpl[t0];
					}
					dofs /= (1000.0 * 3600.0);
					eps += dofs * DEGTORAD;
				}
			}
			return new Epsilon(J, eps);
		}

	}
}
