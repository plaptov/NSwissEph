using System;
using static NSwissEph.Consts;

namespace NSwissEph.Internals
{
	public static class Calculus
	{
		/// <summary>
		/// Reduce x modulo 360 degrees
		/// </summary>
		public static double swe_degnorm(double x)
		{
			double y = x % 360.0;
			if (Math.Abs(y) < 1e-13) /* Alois fix 11-dec-1999 */
				y = 0;
			if (y < 0.0)
				y += 360.0;
			return y;
		}

		/// <summary>
		/// Reduce x modulo TWOPI degrees
		/// </summary>
		public static double swe_radnorm(double x)
		{
			double y = x % TwoPi;
			if (Math.Abs(y) < 1e-13) /* Alois fix 11-dec-1999 */
				y = 0;
			if (y < 0.0)
				y += TwoPi;
			return y;
		}

		public static double swe_deg_midp(double x1, double x0)
		{
			double d, y;
			d = swe_difdeg2n(x1, x0);   /* arc from x0 to x1 */
			y = swe_degnorm(x0 + d / 2);
			return (y);
		}

		public static double swe_rad_midp(double x1, double x0)
		{
			return DEGTORAD * swe_deg_midp(x1 * RADTODEG, x0 * RADTODEG);
		}

		/// <summary>
		/// Reduce x modulo 2*PI
		/// </summary>
		public static double swi_mod2PI(double x)
		{
			double y = x % TwoPi;
			if (y < 0.0)
				y += TwoPi;
			return y;
		}

		public static double swi_angnorm(double x)
		{
			if (x < 0.0)
				return x + TwoPi;
			else if (x >= TwoPi)
				return x - TwoPi;
			else
				return x;
		}

		public static double[] CrossProduct(double[] a, double[] b)
		{
			return new[]
			{
				a[1] * b[2] - a[2] * b[1],
				a[2] * b[0] - a[0] * b[2],
				a[0] * b[1] - a[1] * b[0],
			};
		}

		public static double swe_difdeg2n(double p1, double p2)
		{
			double dif = swe_degnorm(p1 - p2);
			if (dif >= 180.0)
				return dif - 360.0;
			return dif;
		}

		public static double swe_difrad2n(double p1, double p2)
		{
			double dif = swe_radnorm(p1 - p2);
			if (dif >= TwoPi / 2)
				return dif - TwoPi;
			return dif;
		}

	}
}
