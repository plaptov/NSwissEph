﻿/* precession according to Owen 1990:
 * Owen, William M., Jr., (JPL) "A Theory of the Earth's Precession
 * Relative to the Invariable Plane of the Solar System", Ph.D.
 * Dissertation, University of Florida, 1990.
 * Implemented for time range -18000 to 14000. 
 */
/* 
 * p. 177: central time Tc = -160, covering time span -200 <= T <= -120
 * i.e. -14000 +- 40 centuries
 * p. 178: central time Tc = -80, covering time span -120 <= T <= -40 
 * i.e. -6000 +- 40 centuries 
 * p. 179: central time Tc = 0, covering time span -40 <= T <= +40 
 * i.e. 2000 +- 40 centuries 
 * p. 180: central time Tc = 80, covering time span 40 <= T <= 120 
 * i.e. 10000 +- 40 centuries 
 * p. 181: central time Tc = 160, covering time span 120 <= T <= 200
 * i.e. 10000 +- 40 centuries 
 */

using System;

using static NSwissEph.Consts;

namespace NSwissEph.Internals
{
	internal static class OwenPrecession
	{
		private static readonly JulianDayNumber[] t0s = new[]
		{
			JulianDayNumber.FromRaw(-3392455.5),
			JulianDayNumber.FromRaw(-470455.5),
			JulianDayNumber.FromRaw(2451544.5),
			JulianDayNumber.FromRaw(5373544.5),
			JulianDayNumber.FromRaw(8295544.5),
		};

		static readonly double[][] owen_eps0_coef = new[]
		{
			new[] {23.699391439256386, 5.2330816033981775e-1, -5.6259493384864815e-2, -8.2033318431602032e-3, 6.6774163554156385e-4, 2.4931584012812606e-5, -3.1313623302407878e-6, 2.0343814827951515e-7, 2.9182026615852936e-8, -4.1118760893281951e-9,},
			new[] {24.124759551704588, -1.2094875596566286e-1, -8.3914869653015218e-2, 3.5357075322387405e-3, 6.4557467824807032e-4, -2.5092064378707704e-5, -1.7631607274450848e-6, 1.3363622791424094e-7, 1.5577817511054047e-8, -2.4613907093017122e-9,},
			new[] {23.439103144206208, -4.9386077073143590e-1, -2.3965445283267805e-4, 8.6637485629656489e-3, -5.2828151901367600e-5, -4.3951004595359217e-5, -1.1058785949914705e-6, 6.2431490022621172e-8, 3.4725376218710764e-8, 1.3658853127005757e-9,},
			new[] {22.724671295125046, -1.6041813558650337e-1, 7.0646783888132504e-2, 1.4967806745062837e-3, -6.6857270989190734e-4, 5.7578378071604775e-6, 3.3738508454638728e-6, -2.2917813537654764e-7, -2.1019907929218137e-8, 4.3139832091694682e-9,},
			new[] {22.914636050333696, 3.2123508304962416e-1, 3.6633220173792710e-2, -5.9228324767696043e-3, -1.882379107379328e-4, 3.2274552870236244e-5, 4.9052463646336507e-7, -5.9064298731578425e-8, -2.0485712675098837e-8, -6.2163304813908160e-10,},
		};

		static readonly double[][] owen_psia_coef = new[]
		{
			new[] {-218.57864954903122, 51.752257487741612, 1.3304715765661958e-1, 9.2048123521890745e-2, -6.0877528127241278e-3, -7.0013893644531700e-5, -4.9217728385458495e-5, -1.8578234189053723e-6, 7.4396426162029877e-7, -5.9157528981843864e-9,},
			new[] {-111.94350527506128, 55.175558131675861, 4.7366115762797613e-1, -4.7701750975398538e-2, -9.2445765329325809e-3, 7.0962838707454917e-4, 1.5140455277814658e-4, -7.7813159018954928e-7, -2.4729402281953378e-6, -1.0898887008726418e-7,},
			new[] {-2.041452011529441e-1, 55.969995858494106, -1.9295093699770936e-1, -5.6819574830421158e-3, 1.1073687302518981e-2, -9.0868489896815619e-5, -1.1999773777895820e-4, 9.9748697306154409e-6, 5.7911493603430550e-7, -2.3647526839778175e-7,},
			new[] {111.61366860604471, 56.404525305162447, 4.4403302410703782e-1, 7.1490030578883907e-2, -4.9184559079790816e-3, -1.3912698949042046e-3, -6.8490613661884005e-5, 1.2394328562905297e-6, 1.7719847841480384e-6, 2.4889095220628068e-7,},
			new[] {228.40683531269390, 60.056143904919826, 2.9583200718478960e-2, -1.5710838319490748e-1, -7.0017356811600801e-3, 3.3009615142224537e-3, 2.0318123852537664e-4, -6.5840216067828310e-5, -5.9077673352976155e-6, 1.3983942185303064e-6,},
		};

		static readonly double[][] owen_oma_coef = new[]
		{
			new[] {25.541291140949806, 2.377889511272162e-1, -3.7337334723142133e-1, 2.4579295485161534e-2, 4.3840999514263623e-3, -3.1126873333599556e-4, -9.8443045771748915e-6, -7.9403103080496923e-7, 1.0840116743893556e-9, 9.2865105216887919e-9,},
			new[] {24.429357654237926, -9.5205745947740161e-1, 8.6738296270534816e-2, 3.0061543426062955e-2, -4.1532480523019988e-3, -3.7920928393860939e-4, 3.5117012399609737e-5, 4.6811877283079217e-6, -8.1836046585546861e-8, -6.1803706664211173e-8,},
			new[] {23.450465062489337, -9.7259278279739817e-2, 1.1082286925130981e-2, -3.1469883339372219e-2, -1.0041906996819648e-4, 5.6455168475133958e-4, -8.4403910211030209e-6, -3.8269157371098435e-6, 3.1422585261198437e-7, 9.3481729116773404e-9,},
			new[] {22.581778052947806, -8.7069701538602037e-1, -9.8140710050197307e-2, 2.6025931340678079e-2, 4.8165322168786755e-3, -1.906558772193363e-4, -4.6838759635421777e-5, -1.6608525315998471e-6, -3.2347811293516124e-8, 2.8104728109642000e-9,},
			new[] {21.518861835737142, 2.0494789509441385e-1, 3.5193604846503161e-1, 1.5305977982348925e-2, -7.5015367726336455e-3, -4.0322553186065610e-4, 1.0655320434844041e-4, 7.1792339586935752e-6, -1.603874697543020e-6, -1.613563462813512e-7,},
		};

		static readonly double[][] owen_chia_coef = new[]
		{
			new[] {8.2378850337329404e-1, -3.7443109739678667, 4.0143936898854026e-1, 8.1822830214590811e-2, -8.5978790792656293e-3, -2.8350488448426132e-5, -4.2474671728156727e-5, -1.6214840884656678e-6, 7.8560442001953050e-7, -1.032016641696707e-8,},
			new[] {-2.1726062070318606, 7.8470515033132925e-1, 4.4044931004195718e-1, -8.0671247169971653e-2, -8.9672662444325007e-3, 9.2248978383109719e-4, 1.5143472266372874e-4, -1.6387009056475679e-6, -2.4405558979328144e-6, -1.0148113464009015e-7,},
			new[] {-4.8518673570735556e-1, 1.0016737299946743e-1, -4.7074888613099918e-1, -5.8604054305076092e-3, 1.4300208240553435e-2, -6.7127991650300028e-5, -1.3703764889645475e-4, 9.0505213684444634e-6, 6.0368690647808607e-7, -2.2135404747652171e-7,},
			new[] {-2.0950740076326087, -9.4447359463206877e-1, 4.0940512860493755e-1, 1.0261699700263508e-1, -5.3133241571955160e-3, -1.6634631550720911e-3, -5.9477519536647907e-5, 2.9651387319208926e-6, 1.6434499452070584e-6, 2.3720647656961084e-7,},
			new[] {6.3315163285678715e-1, 3.5241082918420464, 2.1223076605364606e-1, -1.5648122502767368e-1, -9.1964075390801980e-3, 3.3896161239812411e-3, 2.1485178626085787e-4, -6.6261759864793735e-5, -5.9257969712852667e-6, 1.3918759086160525e-6,},
		};

		public static double epsiln_owen_1986(JulianDayNumber tjd)
		{
			int i;
			double[] k = new double[10], tau = new double[10];
			var (t0, icof) = get_owen_t0_icof(tjd);
			double eps = 0;
			tau[0] = 0;
			tau[1] = (tjd - t0).Raw / 36525.0 / 40.0;
			for (i = 2; i <= 9; i++)
			{
				tau[i] = tau[1] * tau[i - 1];
			}
			k[0] = 1;
			k[1] = tau[1];
			k[2] = 2 * tau[2] - 1;
			k[3] = 4 * tau[3] - 3 * tau[1];
			k[4] = 8 * tau[4] - 8 * tau[2] + 1;
			k[5] = 16 * tau[5] - 20 * tau[3] + 5 * tau[1];
			k[6] = 32 * tau[6] - 48 * tau[4] + 18 * tau[2] - 1;
			k[7] = 64 * tau[7] - 112 * tau[5] + 56 * tau[3] - 7 * tau[1];
			k[8] = 128 * tau[8] - 256 * tau[6] + 160 * tau[4] - 32 * tau[2] + 1;
			k[9] = 256 * tau[9] - 576 * tau[7] + 432 * tau[5] - 120 * tau[3] + 9 * tau[1];
			for (i = 0; i < 10; i++)
			{
				eps += (k[i] * owen_eps0_coef[icof][i]);
			}
			return eps;
		}

		private static (JulianDayNumber, int) get_owen_t0_icof(JulianDayNumber tjd)
		{
			int j = 0;
			var t0 = t0s[0];
			for (int i = 1; i < 5; i++)
			{
				if (tjd.Raw < (t0s[i - 1] + t0s[i]).Raw / 2)
				{
					;
				}
				else
				{
					t0 = t0s[i];
					j++;
				}
			}
			return (t0, j);
		}

		/// <summary>
		/// precession matrix Owen 1990
		/// </summary>
		public static double[] owen_pre_matrix(JulianDayNumber tjd, SEFLG iflag)
		{
			int i;
			double chia = 0, psia = 0, oma = 0;
			double coseps0, sineps0, coschia, sinchia, cospsia, sinpsia, cosoma, sinoma;
			double[] k = new double[10], tau = new double[10];
			var (t0, icof) = get_owen_t0_icof(tjd);
			tau[0] = 0;
			tau[1] = (tjd - t0) / 36525.0 / 40.0;
			for (i = 2; i <= 9; i++)
			{
				tau[i] = tau[1] * tau[i - 1];
			}
			k[0] = 1;
			k[1] = tau[1];
			k[2] = 2 * tau[2] - 1;
			k[3] = 4 * tau[3] - 3 * tau[1];
			k[4] = 8 * tau[4] - 8 * tau[2] + 1;
			k[5] = 16 * tau[5] - 20 * tau[3] + 5 * tau[1];
			k[6] = 32 * tau[6] - 48 * tau[4] + 18 * tau[2] - 1;
			k[7] = 64 * tau[7] - 112 * tau[5] + 56 * tau[3] - 7 * tau[1];
			k[8] = 128 * tau[8] - 256 * tau[6] + 160 * tau[4] - 32 * tau[2] + 1;
			k[9] = 256 * tau[9] - 576 * tau[7] + 432 * tau[5] - 120 * tau[3] + 9 * tau[1];
			for (i = 0; i < 10; i++)
			{
				//eps += (k[i] * owen_eps0_coef[icof][i]);
				psia += (k[i] * owen_psia_coef[icof][i]);
				oma += (k[i] * owen_oma_coef[icof][i]);
				chia += (k[i] * owen_chia_coef[icof][i]);
			}
			if (iflag.HasFlag(SEFLG.JPLHOR) || iflag.HasFlag(SEFLG.JPLHOR_APPROX))
			{
				/* 
				 * In comparison with JPL Horizons we have an almost constant offset
				 * almost constant offset in ecl. longitude of about -0.000019 deg. 
				 * We fix this as follows: */
				psia += -0.000018560;
			}
			var eps0 = 84381.448 / 3600.0;
			eps0 *= DEGTORAD;
			psia *= DEGTORAD;
			chia *= DEGTORAD;
			oma *= DEGTORAD;
			coseps0 = Math.Cos(eps0);
			sineps0 = Math.Sin(eps0);
			coschia = Math.Cos(chia);
			sinchia = Math.Sin(chia);
			cospsia = Math.Cos(psia);
			sinpsia = Math.Sin(psia);
			cosoma = Math.Cos(oma);
			sinoma = Math.Sin(oma);

			var rp = new double[9];
			rp[0] = coschia * cospsia + sinchia * cosoma * sinpsia;
			rp[1] = (-coschia * sinpsia + sinchia * cosoma * cospsia) * coseps0 + sinchia * sinoma * sineps0;
			rp[2] = (-coschia * sinpsia + sinchia * cosoma * cospsia) * sineps0 - sinchia * sinoma * coseps0;
			rp[3] = -sinchia * cospsia + coschia * cosoma * sinpsia;
			rp[4] = (sinchia * sinpsia + coschia * cosoma * cospsia) * coseps0 + coschia * sinoma * sineps0;
			rp[5] = (sinchia * sinpsia + coschia * cosoma * cospsia) * sineps0 - coschia * sinoma * coseps0;
			rp[6] = sinoma * sinpsia;
			rp[7] = sinoma * cospsia * coseps0 - cosoma * sineps0;
			rp[8] = sinoma * cospsia * sineps0 + cosoma * coseps0;
			return rp;
		}

	}
}