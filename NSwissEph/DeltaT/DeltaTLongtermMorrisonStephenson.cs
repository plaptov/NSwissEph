namespace NSwissEph.DeltaT
{
	internal class DeltaTLongtermMorrisonStephenson
	{
		/// <see cref="deltat_longterm_morrison_stephenson"/>
		public static double Calc(JulianDayNumber tjd)
		{
			var gregorianYear = tjd.GetGregorianYear();
			double u = (gregorianYear - 1820) / 100.0;
			return (-20 + 32 * u * u);
		}
	}
}
