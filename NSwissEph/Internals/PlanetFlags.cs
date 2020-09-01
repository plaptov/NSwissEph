using System;

namespace NSwissEph.Internals
{
	[Flags]
	internal enum PlanetFlags
	{
		None = 0,
		/// <summary>
		/// Heliocentric if set, else baryocentric
		/// </summary>
		Heliocentric = 1,
		/// <summary>
		/// Set if coefficients are referred to coordinate system of orbital plane
		/// </summary>
		Rotate = 2,
		/// <summary>
		/// Set if reference ellipse
		/// </summary>
		Ellipse = 4,
		/// <summary>
		/// Set if heliocentric earth is given instead of barycentric sun
		/// i.e. bary sun is computed from barycentric and heliocentric earth
		/// <seealso cref="InternalPlanets.EMB"/>
		/// </summary>
		EmbHeliocentric = 8,
	}
}
