using System;

namespace NSwissEph
{
	public readonly struct Nutation
	{
		public Nutation(
			JulianDayNumber date,
			double longitude,
			double obliquity,
			double[,] matrix
		)
		{
			Date = date;
			Longitude = longitude;
			Obliquity = obliquity;
			Matrix = matrix;
			Sin = Math.Sin(obliquity);
			Cos = Math.Cos(obliquity);
		}

		public JulianDayNumber Date { get; }
		public double Longitude { get; }
		public double Obliquity { get; }
		public double Sin { get; }
		public double Cos { get; }
		public double[,] Matrix { get; }

	}
}