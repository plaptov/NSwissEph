using System;

namespace NSwissEph
{
	public readonly struct CartesianCoordinates
	{
		public CartesianCoordinates(double longitude, double latitude, double altitude)
		{
			Longitude = longitude;
			Latitude = latitude;
			Altitude = altitude;
		}

		/// <summary>
		/// X <c>(x[0])</c>
		/// </summary>
		public double Longitude { get; }
		/// <summary>
		/// Y <c>(x[1])</c>
		/// </summary>
		public double Latitude { get; }
		/// <summary>
		/// Z <c>(x[2])</c>
		/// </summary>
		public double Altitude { get; }

		/// <summary>
		/// <see cref="swi_cartpol"/>
		/// </summary>
		public PolarCoordinates ToPolar()
		{
			if (Longitude == 0 && Latitude == 0 && Altitude == 0)
				return default;

			double rxy = Longitude * Longitude + Latitude * Latitude;
			var r = Math.Sqrt(rxy + Altitude * Altitude);
			rxy = Math.Sqrt(rxy);
			var azm = Math.Atan2(Latitude, Longitude);
			if (azm < 0.0)
				azm += Consts.TwoPi;
			double inc;
			if (rxy == 0)
				inc = Altitude >= 0 ? Math.PI / 2 : -(Math.PI / 2);
			else
				inc = Math.Atan(Altitude / rxy);

			return new PolarCoordinates(azm, inc, r);
		}

		/// <summary>
		/// conversion between ecliptical and equatorial cartesian coordinates<br/>
		/// for ecl. to equ.  eps must be negative<br/>
		/// for equ. to ecl.  eps must be positive<br/>
		/// <c>swi_coortrf</c>
		/// </summary>
		/// <param name="eps"></param>
		public CartesianCoordinates Transform(double eps) =>
			Transform(Math.Sin(eps), Math.Cos(eps));

		/// <summary>
		/// conversion between ecliptical and equatorial cartesian coordinates<br/>
		/// for ecl. to equ.  sineps must be -sin(eps)<br/>
		/// <c>swi_coortrf2</c>
		/// </summary>
		/// <param name="sineps">sin(eps)</param>
		/// <param name="coseps">cos(eps)</param>
		public CartesianCoordinates Transform(double sineps, double coseps)
		{
			var lat = Latitude * coseps + Altitude * sineps;
			var alt = Latitude * sineps + Altitude * coseps;
			return new CartesianCoordinates(Longitude, lat, alt);
		}
	}
}
