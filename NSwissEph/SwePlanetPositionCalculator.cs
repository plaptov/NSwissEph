using System;
using NSwissEph.Internals;

namespace NSwissEph
{
	public class SwePlanetPositionCalculator
	{
		private const double EARTH_MOON_MRAT = 1 / 0.0123000383; // AA 2006, K7

		private readonly SwePositionCalculator _sweph;

		public SwePlanetPositionCalculator(SwePositionCalculator sweph)
		{
			_sweph = sweph;
		}

		/// <see cref="sweplan"/>
		public PlanetPosition CalculatePlanet(int bodyNumber, DateTime dateTimeInUTC) =>
			CalculatePlanetWithSpeed(bodyNumber, dateTimeInUTC).position;

		/// <see cref="sweplan"/>
		public (PlanetPosition position, PlanetPosition speed) CalculatePlanetWithSpeed(int bodyNumber, DateTime dateTimeInUTC)
		{
			if (bodyNumber == (int)InternalPlanets.Earth)
			{
				var (earthPosition, earthSpeed) = _sweph.CalcBodyPositionAndSpeed((int)InternalPlanets.EMB, dateTimeInUTC);
				var (moonPosition, moonSpeed) = _sweph.CalcBodyPositionAndSpeed((int)InternalPlanets.Moon, dateTimeInUTC);
				return (embofs(earthPosition, moonPosition),
						embofs(earthSpeed, moonSpeed));
			}

			var (position, speed) = _sweph.CalcBodyPositionAndSpeed(bodyNumber, dateTimeInUTC);
			if (_sweph.IsPlanetHeliocentric(bodyNumber, dateTimeInUTC))
				(position, speed) = _sweph.ConvertToBarycentric(
					position, speed, dateTimeInUTC, calcSpeed: true);
			
			return (position, speed);
		}

		/* Adjust position from Earth-Moon barycenter to Earth
 		*
 		* xemb = hel./bar. position or velocity vectors of emb (input)
 		*                                                  earth (output)
 		* xmoon= geocentric position or velocity vector of moon
 		*/
		private static PlanetPosition embofs(PlanetPosition xemb, PlanetPosition xmoon)
		{
			static double adjust(double emb, double moon) =>
				emb - moon / (EARTH_MOON_MRAT + 1.0);

			return new PlanetPosition(
				longitude: adjust(xemb.Longitude, xmoon.Longitude),
				latitude: adjust(xemb.Latitude, xmoon.Latitude),
				distance: adjust(xemb.Distance, xmoon.Distance)
			);
		}
	}
}