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
			using var stream = new MemoryStream(TestFiles.sepl_18, writable: false);
			var reader = new SEFileReader(stream, SEFileType.Planet);
			var data = reader.ReadConsts();
			
		}
	}
}
