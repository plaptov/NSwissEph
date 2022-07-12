using System;

using NSwissEph.DeltaT;
using NSwissEph.Internals;
using static NSwissEph.Consts;

namespace NSwissEph
{
	internal class PositionCalculatorInternal
	{
		private readonly SweData _sweData;
		private readonly SwePlanetPositionCalculator _planetPositionCalculator;

		public PositionCalculatorInternal(SweData sweData, SwephFilesStorage storage)
		{
			_sweData = sweData;
			_planetPositionCalculator = new SwePlanetPositionCalculator(new SwePositionCalculator(sweData, storage));
		}

		public double[] GetEclipticAndNutation()
		{
			var x = new[]
			{
				_sweData.oec.Eps + _sweData.Nutation.Obliquity, // true ecliptic
				_sweData.oec.Eps,                               // mean ecliptic
				_sweData.Nutation.Longitude,                    // nutation in longitude
				_sweData.Nutation.Obliquity,                    // nutation in obliquity
			};
			for (int i = 0; i < x.Length; i++)
				x[i] *= RADTODEG;
			return x;
		}

		/// <summary>
		/// <c>swecalc</c>
		/// </summary>
		public FullBodyPosition Calculate(PlanetNumber ipl, DateTime dt, SEFLG iflag)
		{
			iflag = iflag.PlausIflag(_sweData, ipl);
			var epheflag = iflag.GetEphemerisMode();
			if (iflag.HasFlag(SEFLG.BARYCTR) && epheflag == EphemerisMode.Moshier)
				throw new InvalidOperationException("barycentric Moshier positions are not supported.");

			//if (iflag.HasFlag(SEFLG.SIDEREAL) && !swed.ayana_is_set)
			//	swe_set_sid_mode(SE_SIDM_FAGAN_BRADLEY, 0, 0);

			if (ipl == PlanetNumber.Moon)
			{
				switch (epheflag)
				{
					case EphemerisMode.JPL:
						throw new NotImplementedException("JPL");
					case EphemerisMode.SWISSEPH:
						var (position, speed) = _planetPositionCalculator.CalculatePlanetWithSpeed((int)InternalPlanets.Moon, dt);
						break;
					default:
						break;
				}
			}

			return default;
		}

		/// <summary>
		/// <see cref="app_pos_etc_moon"/><br/>
		/// transforms the position of the moon:
		/// heliocentric position
		/// barycentric position
		/// astrometric position
		/// apparent position
		/// precession and nutation
		/// note: 
		/// for apparent positions, we consider the earth-moon
		/// system as independant.
		/// for astrometric positions (SEFLG_NOABERR), we 
		/// consider the motions of the earth and the moon 
		/// related to the solar system barycenter.
		/// </summary>
		private void TransformMoonPosition(PlanetPosition position, PlanetPosition speed, DateTime dt)
		{
			// the conversions will be done with xx[].
			var xx = position;
			var xx3 = speed;
			var xxm = xx;
			var xxm3 = xx3;

			var (earthPos, earthSpeed) = _planetPositionCalculator.CalculatePlanetWithSpeed((int)InternalPlanets.Earth, dt);
			// to solar system barycentric
			xx += earthPos;
			xx3 += earthSpeed;
			// observer
			// swi_get_observer TODO
		}

		/// <summary>
		/// Get geocentric position of observer
		/// <see cref="swi_get_observer"/>
		/// </summary>
		public void GetObserver(DateTime dt, SweData swed)
		{
			double f = EARTH_OBLATENESS;
			double re = EARTH_RADIUS;
			/* geocentric position of observer depends on sidereal time,
			* which depends on UT. 
			* compute UT from ET. this UT will be slightly different
			* from the user's UT, but this difference is extremely small.
			*/
			JulianDayNumber tjd = JulianDayNumber.FromDate(dt);
			var delt = DeltaTCalculator.Calc(tjd, swed.DeltaTMode, swed.Iflag.GetEphemerisMode());
			var tjd_ut = tjd.AddDays(-delt);
			var eps = (swed.CalculationDate == dt
				? swed.oec
				: Epsilon.Calc(tjd, swed.Iflag, swed)).Eps;
			var nutlo = swed.CalculationDate == dt
				? swed.Nutation
				: NutationCalculator.Calculate(tjd, swed);
			double nut = 0;
			if (!swed.Iflag.HasFlag(SEFLG.NONUT))
			{
				eps += nutlo.Obliquity;
				nut = nutlo.Longitude;
			}
			/* mean or apparent sidereal time, depending on whether or
			* not SEFLG_NONUT is set */
			var sidt = SiderealTime.Calc(tjd_ut, eps * RADTODEG, nut * RADTODEG, swed);
			sidt *= 15; /* in degrees */
			TODO
		}
	}
}