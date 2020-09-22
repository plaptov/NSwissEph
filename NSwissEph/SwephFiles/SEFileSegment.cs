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

		/// <summary>
		/// <c>tseg0</c>
		/// </summary>
		public JulianDayNumber Start { get; }

		/// <summary>
		/// <c>tseg1</c>
		/// </summary>
		public JulianDayNumber End { get; }

		/// <summary>
		/// <c>segp</c>
		/// unpacked cheby coeffs of segment;
		/// the size is 3 x ncoe
		/// </summary>
		public double[] Coefficients { get; }
	}
}
