using System;

namespace NSwissEph
{
	public readonly struct PolarCoordinates
	{
		public PolarCoordinates(double azimuth, double inclination, double radius)
		{
			Azimuth = azimuth;
			Inclination = inclination;
			Radius = radius;
		}

		/// <summary>
		/// φ <c>(x[0])</c>
		/// </summary>
		public double Azimuth { get; }
		/// <summary>
		/// θ <c>(x[1])</c>
		/// </summary>
		public double Inclination { get; }
		/// <summary>
		/// r <c>(x[2])</c>
		/// </summary>
		public double Radius { get; }

		/// <summary>
		/// <see cref="swi_polcart"/>
		/// </summary>
		public CartesianCoordinates ToCartesian()
		{
			var cosl1 = Math.Cos(Inclination);
			var lon = Radius * cosl1 * Math.Cos(Azimuth);
			var lat = Radius * cosl1 * Math.Sin(Azimuth);
			var alt = Radius * Math.Sin(Inclination);
			return new CartesianCoordinates(lon, lat, alt);
		}
	}
}
