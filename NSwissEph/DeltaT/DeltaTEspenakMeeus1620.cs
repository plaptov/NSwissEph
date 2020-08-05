namespace NSwissEph.DeltaT
{
	internal class DeltaTEspenakMeeus1620
	{
		/// <summary>
		/// Model used SE 1.77 - 2.05.01, for epochs before 1633:
		/// Polynomials by Espenak & Meeus 2006, derived from Stephenson & Morrison 2004.
		/// deltat_model == SEMOD_DELTAT_ESPENAK_MEEUS_2006: 
		/// This method is used only for epochs before 1633.
		/// (For more recent epochs, we use the data provided by Astronomical Almanac K8-K9.)
		/// </summary>
		/// <see cref="deltat_espenak_meeus_1620"/>
		public static double Calc(JulianDayNumber tjd, double tidalAcceleration)
		{
			double ans = 0;
			double u;
			double Ygreg = tjd.GetGregorianYear();
			if (Ygreg < -500)
			{
				ans = DeltaTLongtermMorrisonStephenson.Calc(tjd);
			}
			else if (Ygreg < 500)
			{
				u = Ygreg / 100.0;
				ans = (((((0.0090316521 * u + 0.022174192) * u - 0.1798452) * u - 5.952053) * u + 33.78311) * u - 1014.41) * u + 10583.6;
			}
			else if (Ygreg < 1600)
			{
				u = (Ygreg - 1000) / 100.0;
				ans = (((((0.0083572073 * u - 0.005050998) * u - 0.8503463) * u + 0.319781) * u + 71.23472) * u - 556.01) * u + 1574.2;
			}
			else if (Ygreg < 1700)
			{
				u = Ygreg - 1600;
				ans = 120 - 0.9808 * u - 0.01532 * u * u + u * u * u / 7129.0;
			}
			else if (Ygreg < 1800)
			{
				u = Ygreg - 1700;
				ans = (((-u / 1174000.0 + 0.00013336) * u - 0.0059285) * u + 0.1603) * u + 8.83;
			}
			else if (Ygreg < 1860)
			{
				u = Ygreg - 1800;
				ans = ((((((0.000000000875 * u - 0.0000001699) * u + 0.0000121272) * u - 0.00037436) * u + 0.0041116) * u + 0.0068612) * u - 0.332447) * u + 13.72;
			}
			else if (Ygreg < 1900)
			{
				u = Ygreg - 1860;
				ans = ((((u / 233174.0 - 0.0004473624) * u + 0.01680668) * u - 0.251754) * u + 0.5737) * u + 7.62;
			}
			else if (Ygreg < 1920)
			{
				u = Ygreg - 1900;
				ans = (((-0.000197 * u + 0.0061966) * u - 0.0598939) * u + 1.494119) * u - 2.79;
			}
			else if (Ygreg < 1941)
			{
				u = Ygreg - 1920;
				ans = 21.20 + 0.84493 * u - 0.076100 * u * u + 0.0020936 * u * u * u;
			}
			else if (Ygreg < 1961)
			{
				u = Ygreg - 1950;
				ans = 29.07 + 0.407 * u - u * u / 233.0 + u * u * u / 2547.0;
			}
			else if (Ygreg < 1986)
			{
				u = Ygreg - 1975;
				ans = 45.45 + 1.067 * u - u * u / 260.0 - u * u * u / 718.0;
			}
			else if (Ygreg < 2005)
			{
				u = Ygreg - 2000;
				ans = ((((0.00002373599 * u + 0.000651814) * u + 0.0017275) * u - 0.060374) * u + 0.3345) * u + 63.86;
			}
			ans = TidalAcceleration.AdjustForTidalAcceleration(ans, Ygreg, tidalAcceleration, TidalAccelerationMode.Const26, false);
			ans /= 86400.0;
			return ans;
		}
	}
}
