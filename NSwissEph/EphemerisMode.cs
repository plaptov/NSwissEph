namespace NSwissEph
{
	public enum EphemerisMode
	{
		/// <summary>
		/// NASA's JPL Files
		/// </summary>
		JPL = 1,
		/// <summary>
		/// SWISS EPHEMERIS files (default)
		/// </summary>
		SWISSEPH = 2,
		/// <summary>
		/// Steve Moshier's approximation with no storage (less precision)
		/// </summary>
		Moshier = 4,

		Default = SWISSEPH,
	}
}
