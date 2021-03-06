namespace NSwissEph
{
	/// <summary>
	/// planet numbers for the ipl parameter in swe_calc()
	/// </summary>
	public enum PlanetNumber
	{
		Sun = 0,

		Moon = 1,

		Mercury = 2,

		Venus = 3,

		Mars = 4,

		Jupiter = 5,

		Saturn = 6,

		Uranus = 7,

		Neptune = 8,

		Pluto = 9,

		MeanNode = 10,

		TrueNode = 11,

		MeanApogee = 12,

		OsculatingApogee = 13,

		Earth = 14,

		Chiron = 15,

		Pholus = 16,

		Ceres = 17,

		Pallas = 18,

		Juno = 19,

		Vesta = 20,

		InterpolatedApogee = 21,

		InterpolatedPerigee = 22,

		NumberOfPlanets = 23,

		FictionalOffset = 40,

		FictionOffsetMinusOne = FictionalOffset - 1,

		// Hamburger or Uranian "planets"

		Cupido = FictionalOffset + FictionalPlanetNumber.Cupido,

		Hades = FictionalOffset + FictionalPlanetNumber.Hades,

		Zeus = FictionalOffset + FictionalPlanetNumber.Zeus,

		Kronos = FictionalOffset + FictionalPlanetNumber.Kronos,

		Apollon = FictionalOffset + FictionalPlanetNumber.Apollon,

		Admetos = FictionalOffset + FictionalPlanetNumber.Admetos,

		Vulkanus = FictionalOffset + FictionalPlanetNumber.Vulkanus,

		Poseidon = FictionalOffset + FictionalPlanetNumber.Poseidon,

		// other fictitious bodies

		Isis = FictionalOffset + FictionalPlanetNumber.Isis,

		Nibiru = FictionalOffset + FictionalPlanetNumber.Nibiru,

		Harrington = FictionalOffset + FictionalPlanetNumber.Harrington,

		NeptuneLeverrier = FictionalOffset + FictionalPlanetNumber.NeptuneLeverrier,

		NeptuneAdams = FictionalOffset + FictionalPlanetNumber.NeptuneAdams,

		PlutoLowell = FictionalOffset + FictionalPlanetNumber.PlutoLowell,

		PlutoPickering = FictionalOffset + FictionalPlanetNumber.PlutoPickering,

		// folowing planets not in FictionalPlanetNumber enum because they don't have
		// built-in values in read_elements_file()

		Vulcan = FictionalOffset + 15,

		WhiteMoon = FictionalOffset + 16,

		Proserpina = FictionalOffset + 17,

		Waldemath = FictionalOffset + 18,

		MaxFinctionalPlanetNumber = 999,

		AsteroidsOffset = 10000,

		Varuna = AsteroidsOffset + 20000,
	}
}