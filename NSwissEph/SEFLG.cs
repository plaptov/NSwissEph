using System;

namespace NSwissEph
{
	/// <summary>
	/// flag bits for parameter iflag in function swe_calc()
	/// The flag bits are defined in such a way that iflag = 0 delivers what one usually wants:
	///     - the default ephemeris(SWISS EPHEMERIS) is used,
	///     - apparent geocentric positions referring to the true equinox of date are returned.
	/// 
	/// If not only coordinates, but also speed values are required, use flag = SEFLG_SPEED.
	/// </summary>
	[Flags]
	public enum SEFLG
	{
		/// <summary>
		/// use JPL ephemeris
		/// </summary>
		JPLEPH = 1,

		/// <summary>
		/// use SWISSEPH ephemeris
		/// </summary>
		SWIEPH = 2,

		/// <summary>
		/// use Moshier ephemeris
		/// </summary>
		MOSEPH = 4,

		/// <summary>
		/// heliocentric position
		/// </summary>
		HELCTR = 8,

		/// <summary>
		/// true/geometric position, not apparent position
		/// </summary>
		TRUEPOS = 16,

		/// <summary>
		/// no precession, i.e. give J2000 equinox
		/// </summary>
		J2000 = 32,

		/// <summary>
		/// no nutation, i.e. mean equinox of date
		/// </summary>
		NONUT = 64,

		/// <summary>
		/// speed from 3 positions (do not use it, <see cref="SPEED"/> is faster and more precise.)
		// </summary>
		[Obsolete("do not use it, SPEED is faster and more precise")]
		SPEED3 = 128,

		/// <summary>
		/// high precision speed
		/// </summary>
		SPEED = 256,

		/// <summary>
		/// turn off gravitational deflection
		/// </summary>
		NOGDEFL = 512,

		/// <summary>
		/// turn off 'annual' aberration of light
		/// </summary>
		NOABERR = 1024,

		/// <summary>
		/// astrometric position, i.e. with light-time, but without aberration and light deflection
		/// </summary>
		ASTROMETRIC = NOABERR | NOGDEFL,

		/// <summary>
		/// equatorial positions are wanted
		/// </summary>
		EQUATORIAL = 2 * 1024,

		/// <summary>
		/// cartesian, not polar, coordinates
		/// </summary>
		XYZ = 4 * 1024,

		/// <summary>
		/// coordinates in radians, not degrees
		/// </summary>
		RADIANS = 8 * 1024,

		/// <summary>
		/// barycentric position
		/// </summary>
		BARYCTR = 16 * 1024,

		/// <summary>
		/// topocentric position
		/// </summary>
		TOPOCTR = 32 * 1024,

		/// <summary>
		/// used for Astronomical Almanac mode in calculation of Kepler elipses
		/// </summary>
		ORBEL_AA = TOPOCTR,

		/// <summary>
		/// sidereal position
		/// </summary>
		SIDEREAL = 64 * 1024,

		/// <summary>
		/// ICRS (DE406 reference frame)
		/// </summary>
		ICRS = 128 * 1024,

		/// <summary>
		/// reproduce JPL Horizons 1962 - today to 0.002 arcsec.
		/// </summary>
		JPLHOR = 256 * 1024,

		/// <summary>
		/// approximate JPL Horizons 1962 - today
		/// </summary>
		JPLHOR_APPROX = 512 * 1024,
	}
}
