using System;

namespace NSwissEph.Internals
{
	/// <summary>
	/// functions for precession and ecliptic obliquity according to Vondrák et alii, 2011
	/// </summary>
	internal static class Vondrak
	{
		private const double AS2R = Consts.DEGTORAD / 3600.0;
		private const double EPS0 = 84381.406 * AS2R;
		private const int NPOL_PEPS = 4;
		private const int NPER_PEPS = 10;
		private const int NPOL_PECL = 4;
		private const int NPER_PECL = 8;
		private const int NPOL_PEQU = 4;
		private const int NPER_PEQU = 14;

		/* for pre_peps(): */
		/* polynomials */
		static readonly double[][] pepol = new []
		{
			new[] {+8134.017132, +84028.206305},
			new[] {+5043.0520035, +0.3624445},
			new[] {-0.00710733, -0.00004039},
			new[] {+0.000000271, -0.000000110},
		};

		/* periodics */
		static readonly double[][] peper = new[]
		{
			new[] {+409.90, +396.15, +537.22, +402.90, +417.15, +288.92, +4043.00, +306.00, +277.00, +203.00},
			new[] {-6908.287473, -3198.706291, +1453.674527, -857.748557, +1173.231614, -156.981465, +371.836550, -216.619040, +193.691479, +11.891524},
			new[] {+753.872780, -247.805823, +379.471484, -53.880558, -90.109153, -353.600190, -63.115353, -28.248187, +17.703387, +38.911307},
			new[] {-2845.175469, +449.844989, -1255.915323, +886.736783, +418.887514, +997.912441, -240.979710, +76.541307, -36.788069, -170.964086},
			new[] {-1704.720302, -862.308358, +447.832178, -889.571909, +190.402846, -56.564991, -296.222622, -75.859952, +67.473503, +3.014055}
		};

		/* for pre_pecl(): */
		/* polynomials */
		static readonly double[][] pqpol = new[]
		{
			new[] {+5851.607687, -1600.886300},
			new[] {-0.1189000, +1.1689818},
			new[] {-0.00028913, -0.00000020},
			new[] { +0.000000101, -0.000000437}
		};

		/* periodics */
		static readonly double[][] pqper = new[]
		{
			new[] {708.15, 2309, 1620, 492.2, 1183, 622, 882, 547},
			new[] {-5486.751211, -17.127623, -617.517403, 413.44294, 78.614193, -180.732815, -87.676083, 46.140315},
		// original publication    A&A 534, A22 (2011):
		//        {-684.66156, 2446.28388, 399.671049, -356.652376, -186.387003, -316.80007, 198.296071, 101.135679}, 
		// typo fixed according to A&A 541, C1 (2012)
			new[] {-684.66156, 2446.28388, 399.671049, -356.652376, -186.387003, -316.80007, 198.296701, 101.135679}, 
			new[] {667.66673, -2354.886252, -428.152441, 376.202861, 184.778874, 335.321713, -185.138669, -120.97283},
			new[] { -5523.863691, -549.74745, -310.998056, 421.535876, -36.776172, -145.278396, -34.74445, 22.885731}
		};

		/* for pre_pequ(): */
		/* polynomials */
		static readonly double[][] xypol = new[]
		{
			new[] {+5453.282155, -73750.930350},
			new[] {+0.4252841, -0.7675452},
			new[] {-0.00037173, -0.00018725},
			new[] { -0.000000152, +0.000000231}
		};

		/* periodics */
		static readonly double[][] xyper = new[]
		{
			new[] {256.75, 708.15, 274.2, 241.45, 2309, 492.2, 396.1, 288.9, 231.1, 1610, 620, 157.87, 220.3, 1200},
			new[] {-819.940624, -8444.676815, 2600.009459, 2755.17563, -167.659835, 871.855056, 44.769698, -512.313065, -819.415595, -538.071099, -189.793622, -402.922932, 179.516345, -9.814756},
			new[] {75004.344875, 624.033993, 1251.136893, -1102.212834, -2660.66498, 699.291817, 153.16722, -950.865637, 499.754645, -145.18821, 558.116553, -23.923029, -165.405086, 9.344131},
			new[] {81491.287984, 787.163481, 1251.296102, -1257.950837, -2966.79973, 639.744522, 131.600209, -445.040117, 584.522874, -89.756563, 524.42963, -13.549067, -210.157124, -44.919798},
			new[] { 1558.515853, 7774.939698, -2219.534038, -2523.969396, 247.850422, -846.485643, -1393.124055, 368.526116, 749.045012, 444.704518, 235.934465, 374.049623, -171.33018, -22.899655}
		};

		public static (double dpre, double deps) swi_ldp_peps(JulianDayNumber tjd)
		{
			int i;
			int npol = NPOL_PEPS;
			int nper = NPER_PEPS;
			double t, p, q, w, a, s, c;
			t = (tjd - JulianDayNumber.J2000).Raw / 36525.0;
			p = 0;
			q = 0;
			/* periodic terms */
			for (i = 0; i < nper; i++)
			{
				w = Consts.TwoPi * t;
				a = w / peper[0][i];
				s = Math.Sin(a);
				c = Math.Cos(a);
				p += c * peper[1][i] + s * peper[3][i];
				q += c * peper[2][i] + s * peper[4][i];
			}
			/* polynomial terms */
			w = 1;
			for (i = 0; i < npol; i++)
			{
				p += pepol[i][0] * w;
				q += pepol[i][1] * w;
				w *= t;
			}
			/* both to radians */
			p *= AS2R;
			q *= AS2R;
			return (p, q);
		}

		/*
         * Long term high precision precession, 
         * according to Vondrak/Capitaine/Wallace, "New precession expressions, valid
         * for long time intervals", in A&A 534, A22(2011).
         */
		/* precession of the ecliptic */
		public static double[] pre_pecl(JulianDayNumber tjd)
		{
			int i;
			int npol = NPOL_PECL;
			int nper = NPER_PECL;
			double t, p, q, w, a, s, c, z;
			t = (tjd - JulianDayNumber.J2000).Raw / 36525.0;
			p = 0;
			q = 0;
			/* periodic terms */
			for (i = 0; i < nper; i++)
			{
				w = Consts.TwoPi * t;
				a = w / pqper[0][i];
				s = Math.Sin(a);
				c = Math.Cos(a);
				p += c * pqper[1][i] + s * pqper[3][i];
				q += c * pqper[2][i] + s * pqper[4][i];
			}
			/* polynomial terms */
			w = 1;
			for (i = 0; i < npol; i++)
			{
				p += pqpol[i][0] * w;
				q += pqpol[i][1] * w;
				w *= t;
			}
			/* both to radians */
			p *= AS2R;
			q *= AS2R;
			/* ecliptic pole vector */
			z = 1 - p * p - q * q;
			if (z < 0)
				z = 0;
			else
				z = Math.Sqrt(z);
			s = Math.Sin(EPS0);
			c = Math.Cos(EPS0);
			return new[]
			{
				p,
				-q * c - z * s,
				-q * s + z * c,
			};
		}

		/* precession of the equator */
		public static double[] pre_pequ(JulianDayNumber tjd)
		{
			int i;
			int npol = NPOL_PEQU;
			int nper = NPER_PEQU;
			double t, x, y, w, a, s, c;
			t = (tjd - JulianDayNumber.J2000).Raw / 36525.0;
			x = 0;
			y = 0;
			for (i = 0; i < nper; i++)
			{
				w = Consts.TwoPi * t;
				a = w / xyper[0][i];
				s = Math.Sin(a);
				c = Math.Cos(a);
				x += c * xyper[1][i] + s * xyper[3][i];
				y += c * xyper[2][i] + s * xyper[4][i];
			}
			/* polynomial terms */
			w = 1;
			for (i = 0; i < npol; i++)
			{
				x += xypol[i][0] * w;
				y += xypol[i][1] * w;
				w *= t;
			}
			x *= AS2R;
			y *= AS2R;
			/* equator pole vector */
			w = x * x + y * y;
			return new[]
			{
				x,
				y,
				w < 1 ? Math.Sqrt(1 - w) : 0
			};
		}

		/* precession matrix */
		public static double[] pre_pmat(JulianDayNumber tjd)
		{
			//tjd = 1219339.078000;
			/*equator pole */
			var peqr = pre_pequ(tjd);
			/* ecliptic pole */
			var pecl = pre_pecl(tjd);
			/* equinox */
			var v = Calculus.CrossProduct(peqr, pecl);
			var w = Math.Sqrt(v[0] * v[0] + v[1] * v[1] + v[2] * v[2]);
			double[] eqx = new[]
			{
				v[0] / w,
				v[1] / w,
				v[2] / w,
			};
			v = Calculus.CrossProduct(peqr, eqx);
			return new[]
			{
				eqx[0],
				eqx[1],
				eqx[2],
				v[0],
				v[1],
				v[2],
				peqr[0],
				peqr[1],
				peqr[2],
			};
		}
	}
}
