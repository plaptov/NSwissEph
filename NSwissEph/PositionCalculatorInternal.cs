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
			/* length of position and speed vectors;
			 * the height above sea level must be taken into account.
			 * with the moon, an altitude of 3000 m makes a difference 
			 * of about 2 arc seconds.
			 * height is referred to the average sea level. however, 
			 * the spheroid (geoid), which is defined by the average 
			 * sea level (or rather by all points of same gravitational
			 * potential), is of irregular shape and cannot easily
			 * be taken into account. therefore, we refer height to 
			 * the surface of the ellipsoid. the resulting error 
			 * is below 500 m, i.e. 0.2 - 0.3 arc seconds with the moon.
			 */
			cosfi = cos(swed.topd.geolat * DEGTORAD);
			sinfi = sin(swed.topd.geolat * DEGTORAD);
			cc = 1 / sqrt(cosfi * cosfi + (1 - f) * (1 - f) * sinfi * sinfi);
			ss = (1 - f) * (1 - f) * cc;
			/* neglect polar motion (displacement of a few meters), as long as 
			 * we use the earth ellipsoid */
			/* ... */
			/* add sidereal time */
			cosl = cos((swed.topd.geolon + sidt) * DEGTORAD);
			sinl = sin((swed.topd.geolon + sidt) * DEGTORAD);
			h = swed.topd.geoalt;
			xobs[0] = (re * cc + h) * cosfi * cosl;
			xobs[1] = (re * cc + h) * cosfi * sinl;
			xobs[2] = (re * ss + h) * sinfi;
			/* polar coordinates */
			swi_cartpol(xobs, xobs);
			/* speed */
			xobs[3] = EARTH_ROT_SPEED;
			xobs[4] = xobs[5] = 0;
			swi_polcart_sp(xobs, xobs);
			/* to AUNIT */
			for (i = 0; i <= 5; i++)
				xobs[i] /= AUNIT;
			/* subtract nutation, set backward flag */
			if (!(iflag & SEFLG_NONUT))
			{
				swi_coortrf2(xobs, xobs, -swed.nut.snut, swed.nut.cnut);
				/* speed of xobs is always required, namely for aberration!!! */
				/*if (iflag & SEFLG_SPEED)*/
				swi_coortrf2(xobs + 3, xobs + 3, -swed.nut.snut, swed.nut.cnut);
				swi_nutate(xobs, iflag | SEFLG_SPEED, TRUE);
			}
			/* precess to J2000 */
			swi_precess(xobs, tjd, iflag, J_TO_J2000);
			/*if (iflag & SEFLG_SPEED)*/
			swi_precess_speed(xobs, tjd, iflag, J_TO_J2000);
			/* neglect frame bias (displacement of 45cm) */
			/* ... */

		}
	}
}