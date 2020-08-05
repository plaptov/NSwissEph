using System;

namespace NSwissEph.DeltaT
{
	internal static class DeltaTStephensonMorrison2004_1600
	{
		/// <summary>
		/// Stephenson & Morrison (2004)
		/// </summary>
		public static double Calc(JulianDayNumber tjd, double tid_acc)
		{
			double ans = 0, ans2, ans3;
			double p, B, dd;
			double tjd0;
			int iy;
			double Y = tjd.GetGregorianYear();
			/* before -1000:
             * formula by Stephenson & Morrison (2004; p. 335) but adjusted to fit the 
             * starting point of table dt2. */
			if (Y < DeltaTTabulated.Tab2Start)
			{
				// before -1000
				ans = DeltaTLongtermMorrisonStephenson.Calc(tjd);
				ans = TidalAcceleration.AdjustForTidalAcceleration(ans, Y, tid_acc, TidalAccelerationMode.Const26, false);
				/* transition from formula to table over 100 years */
				if (Y >= DeltaTTabulated.Tab2Start - 100)
				{
					/* starting value of table dt2: */
					ans2 = TidalAcceleration.AdjustForTidalAcceleration(DeltaTTabulated.dt2[0], DeltaTTabulated.Tab2Start, tid_acc, TidalAccelerationMode.Const26, false);
					/* value of formula at epoch TAB2_START */
					/* B = (TAB2_START - LTERM_EQUATION_YSTART) * 0.01;
                    ans3 = -20 + LTERM_EQUATION_COEFF * B * B;*/
					tjd0 = (DeltaTTabulated.Tab2Start - 2000) * 365.2425 + (double)JulianDayNumber.J2000;
					ans3 = DeltaTLongtermMorrisonStephenson.Calc(JulianDayNumber.FromRaw(tjd0));
					ans3 = TidalAcceleration.AdjustForTidalAcceleration(ans3, Y, tid_acc, TidalAccelerationMode.Const26, false);
					dd = ans3 - ans2;
					B = (Y - (DeltaTTabulated.Tab2Start - 100)) * 0.01;
					/* fit to starting point of table dt2. */
					ans -= dd * B;
				}
			}
			/* between -1000 and 1600: 
             * linear interpolation between values of table dt2 (Stephenson & Morrison 2004) */
			if (Y >= DeltaTTabulated.Tab2Start && Y < DeltaTTabulated.Tab2End)
			{
				double Yjul = tjd.GetJulianYear();
				p = Math.Floor(Yjul);
				iy = (int)((p - DeltaTTabulated.Tab2Start) / DeltaTTabulated.Tab2Step);
				dd = (Yjul - (DeltaTTabulated.Tab2Start + DeltaTTabulated.Tab2Step * iy)) / DeltaTTabulated.Tab2Step;
				ans = DeltaTTabulated.dt2[iy] + (DeltaTTabulated.dt2[iy + 1] - DeltaTTabulated.dt2[iy]) * dd;
				/* correction for tidal acceleration used by our ephemeris */
				ans = TidalAcceleration.AdjustForTidalAcceleration(ans, Y, tid_acc, TidalAccelerationMode.Const26, false);
			}
			ans /= 86400.0;
			return ans;
		}
	}
}
