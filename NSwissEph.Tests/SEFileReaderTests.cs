using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using NSwissEph.SwephFiles;

using NUnit.Framework;

namespace NSwissEph.Tests
{
	[TestFixture]
	public class SEFileReaderTests
	{
		[Test]
		public void ReadPlanetsFile()
		{
			using var stream = SEFiles.GetPlanetsFileStream();
			var reader = new SEFileReader(stream, SEFileType.Planet);
			var data = reader.ReadConsts();
		}

		[Test]
		public void ReadMoonFile()
		{
			using var stream = SEFiles.GetMoonFileStream();
			var reader = new SEFileReader(stream, SEFileType.Moon);
			var data = reader.ReadConsts();
		}

		[Test]
		public void ReadAsteroidsFile()
		{
			using var stream = SEFiles.GetAsteroidsFileStream();
			var reader = new SEFileReader(stream, SEFileType.Asteroid);
			var data = reader.ReadConsts();
		}
	}
}
