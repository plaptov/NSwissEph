/* DeltaT = Ephemeris Time - Universal Time, in days.
 * 
 * Before 1955 we use the data developed by 
 * Stephenson, Morrison, and Hohenkerk (2016),
 *
 * 1955 - today + a couple of years:
 * ---------------------------------
 * The tabulated values of deltaT from the Astronomical
 * Alamanc (AA 1997 etc. pp. K8-K9) are used. Some 
 * more recent values have been taken from IERS
 * (http://maia.usno.navy.mil/ser7/deltat.data).
 * Bessel's interpolation formula is implemented to obtain fourth 
 * order interpolated values at intermediate times.
 * The values are adjusted depending on the ephemeris used
 * and its inherent value of secular tidal acceleration ndot.
 *
 * future:
 * ---------------------------------
 * For the time after the last tabulated value, we use the formula
 * of Stephenson (1997; p. 507), with a modification that avoids a jump
 * at the end of the tabulated period. A linear term is added that
 * makes a slow transition from the table to the formula over a period
 * of 100 years. (Need not be updated, when table will be enlarged.)
 *
 * References:
 *
 * Stephenson, F. R., and L. V. Morrison, "Long-term changes
 * in the rotation of the Earth: 700 B.C. to A.D. 1980,"
 * Philosophical Transactions of the Royal Society of London
 * Series A 313, 47-70 (1984)
 *
 * Borkowski, K. M., "ELP2000-85 and the Dynamical Time
 * - Universal Time relation," Astronomy and Astrophysics
 * 205, L8-L10 (1988)
 * Borkowski's formula is derived from partly doubtful eclipses 
 * going back to 2137 BC and uses lunar position based on tidal 
 * coefficient of -23.9 arcsec/cy^2.
 *
 * Chapront-Touze, Michelle, and Jean Chapront, _Lunar Tables
 * and Programs from 4000 B.C. to A.D. 8000_, Willmann-Bell 1991
 * Their table agrees with the one here, but the entries are
 * rounded to the nearest whole second.
 *
 * Stephenson, F. R., and M. A. Houlden, _Atlas of Historical
 * Eclipse Maps_, Cambridge U. Press (1986)
 *
 * Stephenson, F.R. & Morrison, L.V., "Long-Term Fluctuations in 
 * the Earth's Rotation: 700 BC to AD 1990", Philosophical 
 * Transactions of the Royal Society of London, 
 * Ser. A, 351 (1995), 165-202. 
 *
 * Stephenson, F. Richard, _Historical Eclipses and Earth's 
 * Rotation_, Cambridge U. Press (1997)
 *
 * Morrison, L. V., and F.R. Stephenson, "Historical Values of the Earth's 
 * Clock Error DT and the Calculation of Eclipses", JHA xxxv (2004), 
 * pp.327-336
 *
 * Stephenson, F.R., Morrison, L.V., and Hohenkerk, C.Y., "Measurement of the
 * Earth's Rotation: 720 BC to AD 2015", Royal Society Proceedings A 
 * 7 Dec 2016,
 * http://rspa.royalsocietypublishing.org/lookup/doi/10.1098/rspa.2016.0404
 * 
 * Table from AA for 1620 through today
 * Note, Stephenson and Morrison's table starts at the year 1630.
 * The Chapronts' table does not agree with the Almanac prior to 1630.
 * The actual accuracy decreases rapidly prior to 1780.
 *
 * Jean Meeus, Astronomical Algorithms, 2nd edition, 1998.
 * 
 * For a comprehensive collection of publications and formulae, see:
 * http://www.phys.uu.nl/~vgent/deltat/deltat_modern.htm
 * http://www.phys.uu.nl/~vgent/deltat/deltat_old.htm
 * 
 * For future values of delta t, the following data from the 
 * Earth Orientation Department of the US Naval Observatory can be used:
 * (TAI-UTC) from: ftp://maia.usno.navy.mil/ser7/tai-utc.dat
 * (UT1-UTC) from: ftp://maia.usno.navy.mil/ser7/finals.all (cols. 59-68)
 *             or: ftp://ftp.iers.org/products/eop/rapid/standard/finals.data
 * file description in: ftp://maia.usno.navy.mil/ser7/readme.finals
 * Delta T = TAI-UT1 + 32.184 sec = (TAI-UTC) - (UT1-UTC) + 32.184 sec
 *
 * Also, there is the following file: 
 * http://maia.usno.navy.mil/ser7/deltat.data, but it is about 3 months
 * behind (on 3 feb 2009); and predictions:
 * http://maia.usno.navy.mil/ser7/deltat.preds
 */

namespace NSwissEph.DeltaT
{
	public static class DeltaT
	{
		private static readonly JulianDayNumber J1633 = JulianDayNumber.FromRaw(2317746.13090277789);

		/// <summary>
		/// Calculate DeltaT
		/// </summary>
		/// <remarks>
		/// delta t is adjusted to the tidal acceleration that is compatible 
		/// with the ephemeris mode contained in ephemerisMode and with the ephemeris
		/// files made accessible through swe_set_ephe_path() or swe_set_jplfile().
		/// If ephemerisMode is null, then the default tidal acceleration is ussed(i.e. that of DE431).
		/// </remarks>
		/// <see cref="calc_deltat"/>
		/// <returns>DeltaT (ET - UT) in days</returns>
		public static double Calc(JulianDayNumber julianDay, DeltaTMode deltaTMode, EphemerisMode? ephemerisMode)
		{
			var tidalAcceleration = ephemerisMode is null
				? TidalAcceleration.Get(JPLDENumber.Default)
				: TidalAcceleration.Calculate(ephemerisMode.Value, julianDay, JPLDENumber.Auto);

			if (deltaTMode == DeltaTMode.Stephenson_Etc_2016 && julianDay < JulianDayNumber.J1955)
			{
				double deltaT = DeltaTStephensonEtc2016.Calc(julianDay, tidalAcceleration);
				if (julianDay >= JulianDayNumber.J1952_05_04)
					deltaT += (1.0 - (double)(JulianDayNumber.J1955 - julianDay) / 1000.0) * 0.6610218 / 86400.0;
				return deltaT;
			}

			if (deltaTMode == DeltaTMode.Espenak_Meeus_2006 && julianDay < J1633)
			{
				return DeltaTEspenakMeeus1620.Calc(julianDay, tidalAcceleration);
			}

			var year = julianDay.GetYear();

			// delta t model used in SE 1.72 - 1.76: Stephenson & Morrison 2004; before 1620
			if (deltaTMode == DeltaTMode.Stephenson_Morrison_2004 && year < DeltaTTabulated.TabStart)
			{
				// before 1600:
				if (year < DeltaTTabulated.Tab2End)
					return DeltaTStephensonMorrison2004_1600.Calc(julianDay, tidalAcceleration);
				else
					// between 1600 and 1620:
					// linear interpolation between end of table dt2 and start of table dt
					return DeltaTTabulated.CalcInterpolated(year, julianDay.GetGregorianYear(), tidalAcceleration);
			}

			/* delta t model used before SE 1.64: 
			* Stephenson/Morrison 1984 with Borkowski 1988; 
			* before 1620 */
			if (deltaTMode == DeltaTMode.Stephenson_Morrison_1984 && year < DeltaTTabulated.TabStart)
			{
				double B;
				double ans;
				if (year >= 948.0)
				{
					/* Stephenson and Morrison, stated domain is 948 to 1600:
					 * 25.5(centuries from 1800)^2 - 1.9159(centuries from 1955)^2 */
					B = 0.01 * (year - 2000.0);
					ans = (23.58 * B + 100.3) * B + 101.6;
				}
				else
				{
					/* Borkowski, before 948 and between 1600 and 1620 */
					B = 0.01 * (year - 2000.0) + 3.75;
					ans = 35.0 * B * B + 40.0;
				}
				return ans / 86400.0;
			}

			/* 1620 - today + a few years (tabend):
			* Tabulated values of deltaT from Astronomical Almanac 
			* (AA 1997etc., pp. K8-K9) and from IERS  
			* (http://maia.usno.navy.mil/ser7/deltat.data).
			*/
			if (year >= DeltaTTabulated.TabStart)
				return DeltaTTabulated.Calc(julianDay, tidalAcceleration, deltaTMode);

			return 0;
		}
	}
}
