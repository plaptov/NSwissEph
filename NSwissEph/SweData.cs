using NSwissEph.Internals;

namespace NSwissEph
{
	/// <summary>
	/// Calculation context. Like <c>swe_data</c>.
	/// </summary>
	public class SweData
	{
		public SweData(SEFLG iflag, JulianDayNumber date)
		{
			Iflag = iflag;
			TidalAcc = TidalAcceleration.Get(TidalAccelerationMode.Default);
			IsManualTidalAcc = false;
			LongtermPrecessionMode = PrecessionModel.Default;
			ShorttermPrecessionMode = PrecessionModel.Default;
			JplHorizonsMode = JplHorizonsMode.Default;
			oec = Epsilon.Calc(date, iflag, this);
			oec2000 = Epsilon.Calc(JulianDayNumber.J2000, iflag, this);
		}

		public double TidalAcc { get; private set; }

		public bool IsManualTidalAcc { get; private set; }

		public PrecessionModel LongtermPrecessionMode { get; private set; }

		public PrecessionModel ShorttermPrecessionMode { get; private set; }

		public JplHorizonsMode JplHorizonsMode { get; private set; }

		public SEFLG Iflag { get; }

		public Epsilon oec { get; }

		public Epsilon oec2000 { get; }
	}
}
