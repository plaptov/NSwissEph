using System;
using System.Collections.Generic;

namespace NSwissEph
{
	public static class LeapSecond
	{
		private static readonly DateTime[] _yearsWithLeapSecond = new[]
		{
			new DateTime(1972, 06, 30),
			new DateTime(1972, 12, 31),
			new DateTime(1973, 12, 31),
			new DateTime(1974, 12, 31),
			new DateTime(1975, 12, 31),
			new DateTime(1976, 12, 31),
			new DateTime(1977, 12, 31),
			new DateTime(1978, 12, 31),
			new DateTime(1979, 12, 31),
			new DateTime(1981, 06, 30),
			new DateTime(1982, 06, 30),
			new DateTime(1983, 06, 30),
			new DateTime(1985, 06, 30),
			new DateTime(1987, 12, 31),
			new DateTime(1989, 12, 31),
			new DateTime(1990, 12, 31),
			new DateTime(1992, 06, 30),
			new DateTime(1993, 06, 30),
			new DateTime(1994, 06, 30),
			new DateTime(1995, 12, 31),
			new DateTime(1997, 06, 30),
			new DateTime(1998, 12, 31),
			new DateTime(2005, 12, 31),
			new DateTime(2008, 12, 31),
			new DateTime(2012, 06, 30),
			new DateTime(2015, 06, 30),
			new DateTime(2016, 12, 31),
		};

		private static readonly HashSet<DateTime> _yearsSet = new HashSet<DateTime>(_yearsWithLeapSecond);

		public static bool IsDateWith(DateTime date) => _yearsSet.Contains(date.Date);

		public static int CountFor(DateTime date)
		{
			int i = 0;
			foreach (var dt in _yearsWithLeapSecond)
			{
				if (dt <= date)
					i++;
				else
					break;
			}
			return i;
		}

		public static DateTime? LastDateInsertedFor(DateTime date)
		{
			DateTime? last = null;
			foreach (var dt in _yearsWithLeapSecond)
			{
				if (dt <= date)
					last = dt;
				else
					break;
			}
			return last;
		}
	}
}
