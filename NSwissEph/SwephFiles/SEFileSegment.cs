using System;

namespace NSwissEph.SwephFiles
{
	internal class SEFileSegment
	{
		/// <summary>
		/// <c>segp</c>
		/// unpacked cheby coeffs of segment;
		/// the size is 3 x ncoe
		/// </summary>
		private readonly double[] _coefficients;

		public SEFileSegment(JulianDayNumber start, JulianDayNumber size, double[] coefficients, int evaluateCoefficientsCount)
		{
			Start = start;
			Size = size;
			_coefficients = coefficients;

			var partSize = coefficients.Length / 3;
			LongitudeCoefficients = new ArraySegment<double>(_coefficients, 0, evaluateCoefficientsCount);
			LatitudeCoefficients = new ArraySegment<double>(_coefficients, partSize, evaluateCoefficientsCount);
			DistanceCoefficients = new ArraySegment<double>(_coefficients, partSize * 2, evaluateCoefficientsCount);
		}

		/// <summary>
		/// <c>tseg0</c>
		/// </summary>
		public JulianDayNumber Start { get; }

		/// <summary>
		/// Segment size (days covered by a polynomial) <code>dseg</code>
		/// </summary>
		public JulianDayNumber Size { get; }

		/// <summary>
		/// <c>tseg1</c>
		/// </summary>
		public JulianDayNumber End => Start + Size;
		
		public ArraySegment<double> LongitudeCoefficients { get; }

		public ArraySegment<double> LatitudeCoefficients { get; }

		public ArraySegment<double> DistanceCoefficients { get; }
	}
}
