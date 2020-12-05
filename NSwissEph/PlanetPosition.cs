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
	}
}
