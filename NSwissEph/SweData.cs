namespace NSwissEph
{
	/// <summary>
	/// Calculation context. Like <c>swe_data</c>.
	/// </summary>
	public class SweData
	{
		public SweData()
		{
			TidalAcc = TidalAcceleration.Get(TidalAccelerationMode.Default);
			IsManualTidalAcc = false;
			LongtermPrecessionMode = PrecessionModel.Default;
			ShorttermPrecessionMode = PrecessionModel.Default;
			JplHorizonsMode = JplHorizonsMode.Default;
		}

		public double TidalAcc { get; private set; }

		public bool IsManualTidalAcc { get; private set; }

		public PrecessionModel LongtermPrecessionMode { get; private set; }

		public PrecessionModel ShorttermPrecessionMode { get; private set; }

		public JplHorizonsMode JplHorizonsMode { get; private set; }
	}
}
