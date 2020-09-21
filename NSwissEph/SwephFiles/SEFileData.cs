using System;
using System.Linq;
using System.Collections.Generic;

using NSwissEph.Internals;

namespace NSwissEph.SwephFiles
{
	public class SEFileData
	{
		public SEFileData()
		{
			PlanetsData = new Dictionary<InternalPlanets, PlanetData>();
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
	}
}
