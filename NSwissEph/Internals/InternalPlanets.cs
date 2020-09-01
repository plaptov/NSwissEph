namespace NSwissEph.Internals
{
	internal enum InternalPlanets
	{
		/// <summary>
		/// Earth-Moon-Barycenter (special value for calculations)
		/// </summary>
		EMB = 0,
		// WARNING! Earth and Sun has same index = 0
		Earth = 0,
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
		BarycentricSun = 10,
		/// <summary>
		/// Any asteroid
		/// </summary>
		AnyBody = 11,
		Chiron = 12,
		Pholus = 13,
		Ceres = 14,
		Pallas = 15,
		Juno = 16,
		Vesta = 17,
	}
}
