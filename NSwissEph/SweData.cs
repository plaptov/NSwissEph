using System;
using NSwissEph.Internals;

namespace NSwissEph
{
	/// <summary>
	/// Calculation context. Like <c>swe_data</c>.
	/// </summary>
	public class SweData
	{
		public SweData(
			SEFLG iflag,
			DateTime dateTimeInUTC,
			CartesianCoordinates observerPosition,
			EarthOrientationParameters eop,
			bool interpolateNutation = false)
		{
			Iflag = iflag;
			CalculationDate = dateTimeInUTC;
			ObserverPosition = observerPosition;
			Eop = eop;
			InterpolateNutation = interpolateNutation;
			CalculationJulianDayNumber = JulianDayNumber.FromDate(dateTimeInUTC);
			TidalAcc = TidalAcceleration.Get(TidalAccelerationMode.Default);
			IsManualTidalAcc = false;
			LongtermPrecessionMode = PrecessionModel.Default;
			ShorttermPrecessionMode = PrecessionModel.Default;
			JplHorizonsMode = JplHorizonsMode.Default;
			NutationModel = NutationModel.Default;
			DeltaTMode = DeltaTMode.Default;
			SiderealTimeMode = SiderealTimeMode.Default;
			oec = Epsilon.Calc(CalculationJulianDayNumber, iflag, this);
			oec2000 = Epsilon.Calc(JulianDayNumber.J2000, iflag, this);
			Nutation = NutationCalculator.Calculate(CalculationJulianDayNumber, this);
			SpeedNutation = NutationCalculator.CalculateForSpeed(CalculationJulianDayNumber, this);
		}

		public double TidalAcc { get; private set; }

		public bool IsManualTidalAcc { get; private set; }

		public PrecessionModel LongtermPrecessionMode { get; private set; }

		public PrecessionModel ShorttermPrecessionMode { get; private set; }

		public JplHorizonsMode JplHorizonsMode { get; private set; }

		public NutationModel NutationModel { get; private set; }

		public DeltaTMode DeltaTMode { get; private set; }

		public SiderealTimeMode SiderealTimeMode { get; private set; }

		public SEFLG Iflag { get; }

		public DateTime CalculationDate { get; }
		public CartesianCoordinates ObserverPosition { get; }
		public EarthOrientationParameters Eop { get; }

		public bool InterpolateNutation { get; }

		public JulianDayNumber CalculationJulianDayNumber { get; }

		public Epsilon oec { get; }

		public Epsilon oec2000 { get; }

		/// <summary>
		/// <c>nut</c>
		/// </summary>
		internal Nutation Nutation { get; }
		/// <summary>
		/// <c>nutv</c>
		/// </summary>
		internal Nutation SpeedNutation { get; }

		internal InterpolatedNutation? InterpolatedNutation { get; set; }
	}
}
