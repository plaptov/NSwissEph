namespace NSwissEph.Internals
{
	internal class InterpolatedNutation
	{
		public JulianDayNumber Time0 { get; set; }

		public JulianDayNumber Time2 { get; set; }

		public double DPsi0 { get; set; }

		public double DPsi1 { get; set; }

		public double DPsi2 { get; set; }

		public double DEps0 { get; set; }

		public double DEps1 { get; set; }

		public double DEps2 { get; set; }
	}
}
