using System.Collections.Generic;

using NSwissEph.Internals;

namespace NSwissEph.SwephFiles
{
	public class SEFileData
	{
		private readonly SEFileReader _reader;

		public SEFileData(SEFileReader reader)
		{
			PlanetsData = new Dictionary<InternalPlanets, PlanetData>();
			_reader = reader;
		}

		public SEFileType FileType { get; set; }

		public string Version { get; set; }

		public string FileName { get; set; }

		public string Copyright { get; set; }

		public JulianDayNumber StartDate { get; set; }

		public JulianDayNumber EndDate { get; set; }

		public JPLDENumber DENumber { get; set; }

		// TODO Make enum
		public IReadOnlyCollection<int> PlanetNumbers { get; set; }

		internal Dictionary<InternalPlanets, PlanetData> PlanetsData { get; }

		/// <summary>
		/// Fetch chebyshew coefficients from sweph file
		/// </summary>
		/// <param name="pdp">Planet</param>
		/// <param name="jd">Time</param>
		/// <param name="epsilonJ2000">Epsilon for J2000</param>
		/// <see cref="get_new_segment"/>
		internal SEFileSegment ReadSegment(PlanetData pdp, JulianDayNumber jd, Epsilon epsilonJ2000) =>
			_reader.ReadSegment(pdp, jd, epsilonJ2000);
	}
}
