namespace NSwissEph.SwephFiles
{
	internal class SEFileSegment
	{
		public SEFileSegment(JulianDayNumber start, JulianDayNumber end, double[] coefficients)
		{
			Start = start;
			End = end;
			Coefficients = coefficients;
		}

		/// <see cref="tseg0"/>
		public JulianDayNumber Start { get; }

		/// <see cref="tseg1"/>
		public JulianDayNumber End { get; }

		/// <summary>
		/// unpacked cheby coeffs of segment;
		/// the size is 3 x ncoe
		/// </summary>
		/// <see cref="segp"/>
		public double[] Coefficients { get; }
	}
}
