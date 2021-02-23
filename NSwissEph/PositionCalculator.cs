using System;

using NSwissEph.Internals;
using NSwissEph.SwephFiles;

namespace NSwissEph
{
	public class PositionCalculator
	{
		private readonly SweData _sweData;
		private readonly SwephFilesStorage _storage;

		public PositionCalculator(SweData sweData, SwephFilesStorage storage)
		{
			_sweData = sweData;
			_storage = storage;
		}

		public PlanetPosition CalcBodyPosition(int bodyNumber, DateTime dateTimeInUTC) =>
			CalcBodyPositionAndSpeedInternal(bodyNumber, dateTimeInUTC, false).position;

		public (PlanetPosition position, PlanetPosition speed) CalcBodyPositionAndSpeed(int bodyNumber, DateTime dateTimeInUTC) =>
			CalcBodyPositionAndSpeedInternal(bodyNumber, dateTimeInUTC, true);

		/// <see cref="sweph"/>
		private (PlanetPosition position, PlanetPosition speed) CalcBodyPositionAndSpeedInternal(int bodyNumber, DateTime dateTimeInUTC, bool calcSpeed)
		{
			var tjd = JulianDayNumber.FromDate(dateTimeInUTC);
			var file = _storage.GetFile(bodyNumber, tjd);
			var planet = bodyNumber > SEConsts.AseroidOffset
				? InternalPlanets.AnyBody
				: (InternalPlanets)bodyNumber;
			var planetData = file.PlanetsData[planet];
			var segment = file.ReadSegment(planetData, tjd, _sweData.oec2000);
			var (position, speed) = CalcBySegment(tjd, segment, calcSpeed);
			/* if planet wanted is barycentric sun:
			* current sepl* files have do not have barycentric sun,
			* but have heliocentric earth and barycentric earth.
			* So barycentric sun and must be computed
			* from heliocentric earth and barycentric earth: the 
			* computation above gives heliocentric earth, therefore we
			* have to compute barycentric earth and subtract heliocentric
			* earth from it. this may be necessary with calls from 
			* sweplan() and from app_pos_etc_sun() (light-time). */
			if (bodyNumber == (int)InternalPlanets.BarycentricSun && planetData.Flags.HasFlag(PlanetFlags.EmbHeliocentric))
			{
				// sweph() calls sweph() !!! for EMB.
				// because we always call new calculation, warning about EARTH and EMB equality was omitted
				var (embPos, embSpeed) = CalcBodyPositionAndSpeedInternal((int)InternalPlanets.EMB, dateTimeInUTC, calcSpeed);
				position = new PlanetPosition(
					embPos.Longitude - position.Longitude,
					embPos.Latitude - position.Latitude,
					embPos.Distance - position.Distance);
				if (calcSpeed)
					speed = new PlanetPosition(
						embSpeed.Longitude - speed.Longitude,
						embSpeed.Latitude - speed.Latitude,
						embSpeed.Distance - speed.Distance);
			}
			// asteroids are heliocentric.
			// if JPL or SWISSEPH, convert to barycentric - currently always
			if (planet >= InternalPlanets.AnyBody)
				(position, speed) = ConvertToBarycentric(position, speed, dateTimeInUTC, calcSpeed);
			return (position, speed);
		}

		private (PlanetPosition position, PlanetPosition speed) ConvertToBarycentric(
			PlanetPosition position, PlanetPosition speed, DateTime dateTimeInUTC, bool calcSpeed)
		{
			// warning: in original code this value cached
			var (sunPos, sunSpeed) = CalcBodyPositionAndSpeedInternal((int)InternalPlanets.BarycentricSun, dateTimeInUTC, calcSpeed);
			position = new PlanetPosition(
				sunPos.Longitude + position.Longitude,
				sunPos.Latitude + position.Latitude,
				sunPos.Distance + position.Distance);

			if (calcSpeed)
				speed = new PlanetPosition(
					sunSpeed.Longitude + speed.Longitude,
					sunSpeed.Latitude + speed.Latitude,
					sunSpeed.Distance + speed.Distance);

			return (position, speed);
		}

		private (PlanetPosition position, PlanetPosition speed) CalcBySegment(JulianDayNumber tjd, SEFileSegment segment, bool calcSpeed = true)
		{
			var t = (tjd - segment.Start).Raw / segment.Size.Raw;
			t = t * 2 - 1;

			var position = new PlanetPosition(
				longitude: echeb(t, segment.LongitudeCoefficients),
				latitude: echeb(t, segment.LatitudeCoefficients),
				distance: echeb(t, segment.DistanceCoefficients)
			);

			PlanetPosition speed = default;
			if (calcSpeed)
				speed = new PlanetPosition(
					longitude: edcheb(t, segment.LongitudeCoefficients),
					latitude: edcheb(t, segment.LatitudeCoefficients),
					distance: edcheb(t, segment.DistanceCoefficients)
				);

			return (position, speed);
		}

		/// <summary>
		/// Evaluates a given chebyshev series coef[0..ncf-1] 
		/// with ncf terms at x in [-1,1]. Communications of the ACM, algorithm 446,
		/// April 1973 (vol. 16 no.4) by Dr.Roger Broucke.
		/// </summary>
		private double echeb(double x, ArraySegment<double> coef)
		{
			var ncf = coef.Count;
			double x2 = x * 2.0;
			double br = 0.0;
			double brp2 = 0.0;
			double brpp = 0.0;
			for (int j = ncf - 1; j >= 0; j--)
			{
				brp2 = brpp;
				brpp = br;
				br = x2 * brpp - brp2 + coef[j];
			}
			return (br - brp2) / 2.0;
		}

		/// <summary>
		/// evaluates derivative of chebyshev series, <seealso cref="echeb"/>
		/// </summary>
		private double edcheb(double x, ArraySegment<double> coef)
		{
			var ncf = coef.Count;
			double x2 = x * 2.0;
			double bf = 0.0;
			double bj = 0.0;
			double xjp2 = 0.0;
			double xjpl = 0.0;
			double bjp2 = 0.0;
			double bjpl = 0.0;
			for (int j = ncf - 1; j >= 1; j--)
			{
				var xj = coef[j] * (j + j) + xjp2;
				bj = x2 * bjpl - bjp2 + xj;
				bf = bjp2;
				bjp2 = bjpl;
				bjpl = bj;
				xjp2 = xjpl;
				xjpl = xj;
			}
			return (bj - bf) * 2.0;
		}

	}
}
