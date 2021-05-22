namespace NSwissEph
{
	public readonly struct PlanetPosition
	{
		public PlanetPosition(double longitude, double latitude, double distance)
		{
			Longitude = longitude;
			Latitude = latitude;
			Distance = distance;
		}

		public double Longitude { get; }

		public double Latitude { get; }

		public double Distance { get; }

		public static PlanetPosition operator +(PlanetPosition a, PlanetPosition b)
		{
			return new PlanetPosition(a.Longitude + b.Longitude, a.Latitude + b.Latitude, a.Distance + b.Distance);
		}

		public static PlanetPosition operator -(PlanetPosition a, PlanetPosition b)
		{
			return new PlanetPosition(a.Longitude - b.Longitude, a.Latitude - b.Latitude, a.Distance - b.Distance);
		}
	}
}
