namespace NSwissEph
{
	public enum PrecessionModel
	{
		/// <summary>
		/// IAU 1976 (Lieske)
		/// </summary>
		IAU_1976 = 1,

		/// <summary>
		/// Laskar 1986
		/// </summary>
		LASKAR_1986 = 2,

		/// <summary>
		/// Williams 1994 / Epsilon Laskar 1986
		/// </summary>
		WILL_EPS_LASK = 3,

		/// <summary>
		/// Williams 1994
		/// </summary>
		WILLIAMS_1994 = 4,

		/// <summary>
		/// Simon 1994
		/// </summary>
		SIMON_1994 = 5,

		/// <summary>
		/// IAU 2000 (Lieske 1976, Mathews 2002)
		/// </summary>
		IAU_2000 = 6,

		/// <summary>
		/// Bretagnon 2003
		/// </summary>
		BRETAGNON_2003 = 7,

		/// <summary>
		/// IAU 2006 (Capitaine & alii)
		/// </summary>
		IAU_2006 = 8,

		/// <summary>
		/// Vondrák 2011
		/// </summary>
		VONDRAK_2011 = 9,

		/// <summary>
		/// Owen 1990
		/// </summary>
		OWEN_1990 = 10,

		Default = VONDRAK_2011,
	}
}
