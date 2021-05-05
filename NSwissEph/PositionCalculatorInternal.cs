using System;
using NSwissEph.Internals;

namespace NSwissEph
{
	internal class PositionCalculatorInternal
	{
		private readonly SweData _sweData;
		private readonly SwephFilesStorage _swephFiles;

		public PositionCalculatorInternal(SweData sweData, SwephFilesStorage storage)
		{
			_sweData = sweData;
			_swephFiles = storage;
		}

		public void Calculate(PlanetNumber ipl, SEFLG iflag)
		{
			iflag = iflag.PlausIflag(_sweData, ipl);
			var epheflag = iflag.GetEphemerisMode();
			if (iflag.HasFlag(SEFLG.BARYCTR) && epheflag == EphemerisMode.Moshier)
				throw new InvalidOperationException("barycentric Moshier positions are not supported.");

			//if (iflag.HasFlag(SEFLG.SIDEREAL) && !swed.ayana_is_set)
			//	swe_set_sid_mode(SE_SIDM_FAGAN_BRADLEY, 0, 0);
		}
	}
}