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

		/// <summary>
		/// <c>tnut</c>
		/// </summary>
		public JulianDayNumber Date { get; }
		/// <summary>
		/// <c>nutlo[0]</c>
		/// </summary>
		public double Longitude { get; }
		/// <summary>
		/// <c>nutlo[1]</c>
		/// </summary>
		public double Obliquity { get; }
		/// <summary>
		/// <c>snut</c>
		/// </summary>
		public double Sin { get; }
		/// <summary>
		/// <c>cnut</c>
		/// </summary>
		public double Cos { get; }
		/// <summary>
		/// <c>matrix[3,3]</c>
		/// </summary>
		public double[,] Matrix { get; }

	}
}