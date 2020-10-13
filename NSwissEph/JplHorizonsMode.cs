namespace NSwissEph
{
	/// <summary>
	/// methods of approximation of JPL Horizons (iflag & SEFLG_JPLHORA), without dpsi, deps; see explanations below
	/// </summary>
	/// <remarks>
	/// With <see cref="One"/>, planetary positions are always calculated 
	/// using a recent precession/nutation model.Frame bias matrix is applied
	/// with some correction to RA and another correction added to epsilon.
	/// This provides a very good approximation of JPL Horizons positions. 
	/// 
	/// With <see cref="Two"/>, frame bias as recommended by IERS Conventions 2003
	/// and 2010 is *not* applied. Instead, dpsi_bias and deps_bias are added to
	/// nutation.This procedure is found in some older astronomical software.
	/// Equatorial apparent positions will be close to JPL Horizons
	/// (within a few mas) between 1962 and current years.Ecl.longitude
	/// will be good, latitude bad.
	/// 
	/// With <see cref="Three"/> works like <see cref="Two"/> after 1962, but like
	/// <see cref="SEFLG.JPLHOR"/> before that.This allows EXTREMELY good agreement with JPL
	/// Horizons over its whole time range.
	/// </remarks>
	public enum JplHorizonsMode
	{
		One = 1,
		Two = 2,
		Three = 3,
		Default = Three,
	}
}
