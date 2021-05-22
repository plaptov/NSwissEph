using System.IO;

namespace NSwissEph.Tests
{
	public static class SEFiles
	{
		public static Stream GetPlanetsFileStream() => new MemoryStream(TestFiles.sepl_18, writable: false);

		public static Stream GetMoonFileStream() => new MemoryStream(TestFiles.semo_18, writable: false);

		public static Stream GetAsteroidsFileStream() => new MemoryStream(TestFiles.seas_18, writable: false);

		public static Stream GetEopC04File() => new MemoryStream(TestFiles.EOP_14_C04_IAU1980_one_file_1962_now, writable: false);

		public static Stream GetEopFinalsFile() => new MemoryStream(TestFiles.finals_all_iau1980, writable: false);
	}
}
