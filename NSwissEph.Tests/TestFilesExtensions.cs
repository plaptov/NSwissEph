using System.IO;

namespace NSwissEph.Tests
{
	public static class SEFiles
	{
		public static Stream GetPlanetsFileStream() => new MemoryStream(TestFiles.sepl_18, writable: false);

		public static Stream GetMoonFileStream() => new MemoryStream(TestFiles.semo_18, writable: false);

		public static Stream GetAsteroidsFileStream() => new MemoryStream(TestFiles.seas_18, writable: false);
	}
}
