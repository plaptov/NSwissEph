using System;
using System.Collections.Concurrent;
using System.IO;

using NSwissEph.SwephFiles;

namespace NSwissEph
{
	public class SwephFilesStorage
	{
		private readonly ConcurrentDictionary<int, ConcurrentBag<SEFileData>> _files = new ConcurrentDictionary<int, ConcurrentBag<SEFileData>>();

		/// <summary>
		/// Load file with name like <c>semo_18.se1</c>
		/// </summary>
		/// <param name="stream">File readonly stream</param>
		public void LoadMoonFile(Stream stream) => LoadFile(stream, SEFileType.Moon);

		/// <summary>
		/// Load file with name like <c>sepl_18.se1</c>
		/// </summary>
		/// <param name="stream">File readonly stream</param>
		public void LoadPlanetsFile(Stream stream) => LoadFile(stream, SEFileType.Planet);

		/// <summary>
		/// Load file with name like <c>seas_18.se1</c>
		/// </summary>
		/// <param name="stream">File readonly stream</param>
		public void LoadMainAsteroidsFile(Stream stream) => LoadFile(stream, SEFileType.Asteroid);

		/// <summary>
		/// Load file with name like <c>se09999.se1</c> (long file) or <c>se09999s.se1</c> (short file)
		/// </summary>
		/// <param name="stream">File readonly stream</param>
		public void LoadAdditionalAsteroidFile(Stream stream) => LoadFile(stream, SEFileType.AdditionalAsteroid);

		private void LoadFile(Stream stream, SEFileType fileType)
		{
			var data = new SEFileReader(stream, fileType).ReadConsts();
			foreach (var num in data.PlanetNumbers)
			{
				var bag = _files.GetOrAdd(num, _ => new ConcurrentBag<SEFileData>());
				bag.Add(data);
			}
		}

		internal SEFileData GetFile(int bodyNumber, JulianDayNumber jd)
		{
			if (_files.TryGetValue(bodyNumber, out var bag))
				foreach (var item in bag)
					if (item.StartDate >= jd && item.EndDate <= jd)
						return item;
			throw new FileNotFoundException();
		}
	}
}
