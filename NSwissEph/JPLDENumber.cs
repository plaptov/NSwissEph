namespace NSwissEph
{
	/// <summary>
	/// JPL DE files release number (not full list, for difference detecting)
	/// </summary>
	/// <seealso cref="https://en.wikipedia.org/wiki/Jet_Propulsion_Laboratory_Development_Ephemeris"/>
	public enum JPLDENumber
	{
		DE200 = 200,
		DE403 = 403,
		DE404 = 404,
		DE405 = 405,
		DE406 = 406,
		DE421 = 421,
		DE422 = 422,
		DE430 = 430,
		DE431 = 431,

		Auto = 0,
		Default = DE431,
	}
}
