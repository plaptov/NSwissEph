using System;
using System.Diagnostics;

namespace NSwissEph
{
	[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
	public readonly struct JulianDayNumber
	{
		/// <summary>
		/// 2000 January 1.5
		/// </summary>
		public static readonly JulianDayNumber J2000 = new JulianDayNumber(2451545.0);

		/// <summary>
		/// 1950 January 0.923
		/// </summary>
		public static readonly JulianDayNumber B1950 = new JulianDayNumber(2433282.42345905);

		/// <summary>
		/// 1900 January 0.5
		/// </summary>
		public static readonly JulianDayNumber J1900 = new JulianDayNumber(2415020.0);

		/// <summary>
		/// 1955 January 1
		/// </summary>
		public static readonly JulianDayNumber J1955 = new JulianDayNumber(2435108.5);

		/// <summary>
		/// 1952 April 5
		/// </summary>
		public static readonly JulianDayNumber J1952_05_04 = new JulianDayNumber(2434108.5);

		/// <summary>
		/// 1799 January 1
		/// </summary>
		public static readonly JulianDayNumber J1799_01_01 = new JulianDayNumber(2378131.5);

		/// <summary>
		/// 2202 January 1
		/// </summary>
		public static readonly JulianDayNumber J2202_01_01 = new JulianDayNumber(2525323.5);

		private JulianDayNumber(double jd)
		{
			Raw = jd;
		}

		internal double Raw { get; }

		/// <summary>
		/// This function returns the absolute Julian day number (JD) for a given calendar date.
		/// </summary>
		/// <param name="isGregorian">if true, Gregorian calendar is assumed, else Julian calendar is assumed</param>
		/// <see cref="swe_julday"/>
		/// <remarks>
		/// The Julian day number is a system of numbering all days continously
		/// within the time range of known human history. It should be familiar
		/// to every astrological or astronomical programmer. The time variable
		/// in astronomical theories is usually expressed in Julian days or
		/// Julian centuries (36525 days per century) relative to some start day;
		/// the start day is called 'the epoch'.
		/// The Julian day number is a double representing the number of
		/// days since JD = 0.0 on 1 Jan -4712, 12:00 noon(in the Julian calendar).
		/// 
		/// Midnight has always a JD with fraction .5, because traditionally
		/// the astronomical day started at noon.This was practical because
		/// then there was no change of date during a night at the telescope.
		/// From this comes also the fact the noon ephemerides were printed
		/// before midnight ephemerides were introduced early in the 20th century.
		/// 
		/// NOTE: The Julian day number must not be confused with the Julian calendar system.
		/// 
		/// Be aware the we always use astronomical year numbering for the years
		/// before Christ, not the historical year numbering.
		/// Astronomical years are done with negative numbers, historical
		/// years with indicators BC or BCE (before common era).
		/// Year 0 (astronomical)  	= 1 BC
		/// year -1 (astronomical) 	= 2 BC
		/// etc.
		/// 
		/// Original author: Marc Pottenger, Los Angeles.
		/// with bug fix for year< -4711   15-aug-88 by Alois Treindl
		/// 
		/// References: Oliver Montenbruck, Grundlagen der Ephemeridenrechnung,
		/// Verlag Sterne und Weltraum (1987), p.49 ff</remarks>
		public static JulianDayNumber FromDate(DateTime dt, bool isGregorian = true)
		{
			double year = dt.Year;
			double month = dt.Month + 1.0;
			if (dt.Month < 3)
			{
				year -= 1.0;
				month += 12.0;
			}
			double julianYear = year + 4712.0;
			double jd = Math.Floor(julianYear * 365.25)
				+ Math.Floor(30.6 * month + 0.000001)
				+ dt.Day
				+ dt.TimeOfDay.TotalHours / 24.0
				- 63.5;
			if (isGregorian)
			{
				double leapYears = Math.Floor(Math.Abs(year) / 100.0) - Math.Floor(Math.Abs(year) / 400.0);
				if (year < 0.0)
					leapYears = -leapYears;
				jd = jd - leapYears + 2;
				if ((year < 0.0) && (year / 100.0 == Math.Floor(year / 100.0)) && (year / 400.0 != Math.Floor(year / 400.0)))
					jd -= 1;
			}
			return new JulianDayNumber(jd);
		}

		/// <summary>
		/// This function returns the UTC date for a current absolute Julian day number (JD).
		/// </summary>
		/// <param name="isGregorian">if true, Gregorian calendar is assumed, else Julian calendar is assumed</param>
		/// <see cref="swe_revjul"/>
		/// <remarks>
		/// Be aware the we use astronomical year numbering for the years
		/// before Christ, not the historical year numbering.
		/// Astronomical years are done with negative numbers, historical
		/// years with indicators BC or BCE (before common era).
		/// Year  0 (astronomical)  	= 1 BC historical year
		/// year -1 (astronomical) 	= 2 BC historical year
		/// year -234 (astronomical) 	= 235 BC historical year
		/// etc.
		/// 
		/// Original author Mark Pottenger, Los Angeles.
		/// with bug fix for year< -4711 16-aug-88 Alois Treindl</remarks>
		public DateTime ToDate(bool isGregorian = true)
		{
			double allDays = Raw + 32082.5;
			if (isGregorian)
			{
				double leapYears = allDays + Math.Floor(allDays / 36525.0) - Math.Floor(allDays / 146100.0) - 38.0;
				if (Raw >= 1830691.5)
					leapYears += 1;
				allDays = allDays + Math.Floor(leapYears / 36525.0) - Math.Floor(leapYears / 146100.0) - 38.0;
			}
			allDays = Math.Floor(allDays + 123.0);
			double years = Math.Floor((allDays - 122.2) / 365.25);
			double months = Math.Floor((allDays - Math.Floor(365.25 * years)) / 30.6001);
			var month = (int)(months - 1.0);
			if (month > 12)
				month -= 12;
			var day = (int)(allDays - Math.Floor(365.25 * years) - Math.Floor(30.6001 * months));
			var year = (int)(years + Math.Floor((months - 2.0) / 12.0) - 4800);
			var time = (Raw - Math.Floor(Raw + 0.5) + 0.5) * 24.0;
			var t = TimeSpan.FromHours(time);

			return new DateTime(year, month, day, t.Hours, t.Minutes, t.Seconds, DateTimeKind.Utc);
		}

		public double GetYear() => 2000.0 + (this - J2000).Raw / 365.25;

		public double GetGregorianYear() => 2000.0 + (this - J2000).Raw / 365.2425;

		public double GetJulianYear() => 2000 + (Raw - 2451557.5) / 365.25;

		public static JulianDayNumber operator +(JulianDayNumber a, JulianDayNumber b) =>
			new JulianDayNumber(a.Raw + b.Raw);

		public static JulianDayNumber operator -(JulianDayNumber a, JulianDayNumber b) =>
			new JulianDayNumber(a.Raw - b.Raw);

		public static bool operator <(JulianDayNumber a, JulianDayNumber b) =>
			a.Raw < b.Raw;

		public static bool operator >(JulianDayNumber a, JulianDayNumber b) =>
			a.Raw > b.Raw;

		public static bool operator <=(JulianDayNumber a, JulianDayNumber b) =>
			a.Raw <= b.Raw;

		public static bool operator >=(JulianDayNumber a, JulianDayNumber b) =>
			a.Raw >= b.Raw;

		public static explicit operator double (JulianDayNumber d) => d.Raw;

		internal static JulianDayNumber FromRaw(double value) => new JulianDayNumber(value);

		public override string ToString()
		{
			if (Raw == 0.0)
				return "(zero)";
			return ToDate().ToString();
		}
	}
}
