namespace NSwissEph
{
	/// <summary>
	/// <c>xreturn double[24]</c>
	/// </summary>
	public struct FullBodyPosition
	{
		/// <summary>
		/// <c>xreturn + 0</c>
		/// </summary>
		public PlanetPosition EclipticPolarPosition { get; set; }
		/// <summary>
		/// <c>xreturn + 3</c>
		/// </summary>
		public PlanetPosition EclipticPolarSpeed { get; set; }
		/// <summary>
		/// <c>xreturn + 6</c>
		/// </summary>
		public PlanetPosition EclipticCartesianPosition { get; set; }
		/// <summary>
		/// <c>xreturn + 9</c>
		/// </summary>
		public PlanetPosition EclipticCartesianSpeed { get; set; }
		/// <summary>
		/// <c>xreturn + 12</c>
		/// </summary>
		public PlanetPosition EquatorialPolarPosition { get; set; }
		/// <summary>
		/// <c>xreturn + 15</c>
		/// </summary>
		public PlanetPosition EquatorialPolarSpeed { get; set; }
		/// <summary>
		/// <c>xreturn + 18</c>
		/// </summary>
		public PlanetPosition EquatorialCartesianPosition { get; set; }
		/// <summary>
		/// <c>xreturn + 21</c>
		/// </summary>
		public PlanetPosition EquatorialCartesianSpeed { get; set; }
	}
}
