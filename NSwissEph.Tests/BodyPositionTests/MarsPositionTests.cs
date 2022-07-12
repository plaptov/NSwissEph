using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

namespace NSwissEph.Tests.BodyPositionTests
{
	[TestFixture]
	public class MarsPositionTests
	{
		private SwephFilesStorage _filesStorage;
		private EarthOrientationParameters _eop;

		[OneTimeSetUp]
		public void Setup()
		{
			_filesStorage = new SwephFilesStorage();
			_filesStorage.LoadPlanetsFile(SEFiles.GetPlanetsFileStream());
			_eop = EarthOrientationParameters.LoadFrom(SEFiles.GetEopC04File(), SEFiles.GetEopFinalsFile());
		}

		[Test]
		public void Test1()
		{
			var date = new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			var jd = JulianDayNumber.FromDate(date);
			var iflag = SEFLG.SWIEPH | SEFLG.ICRS | SEFLG.J2000;
			var sweData = new SweData(iflag, date, _eop);
			var calculator = new SwePositionCalculator(sweData, _filesStorage);

			var result = calculator.CalcBodyPosition(4, date);

			TestContext.WriteLine($"Lon: {result.Longitude}");
			TestContext.WriteLine($"Lat: {result.Latitude}");
		}
	}
}
