namespace NSwissEph.Internals
{
	internal struct Epsilon
	{
		public Epsilon(JulianDayNumber date, double eps, double sinEps, double cosEps)
		{
			Date = date;
			Eps = eps;
			SinEps = sinEps;
			CosEps = cosEps;
		}

		public JulianDayNumber Date { get; }

		public double Eps { get; }

		public double SinEps { get; }

		public double CosEps { get; }
	}
}
