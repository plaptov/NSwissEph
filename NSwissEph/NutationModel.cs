namespace NSwissEph
{
	public enum NutationModel
	{
		IAU_1980 = 1,

		/// <summary>
		/// Herring's (1987) corrections to IAU 1980 nutation series. AA (1996) neglects them.
		/// </summary>
		IAU_CORR_1987 = 2,

		/// <summary>
		/// very time consuming!
		/// </summary>
		IAU_2000A = 3,

		/// <summary>
		/// fast, but precision of milli-arcsec
		/// </summary>
		IAU_2000B = 4,

		Default = IAU_2000B,
	}
}