using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace NSwissEph
{
	public class EarthOrientationParameters
	{
		private EarthOrientationParameters(
			JulianDayNumber beginDate, JulianDayNumber endDate,
			IReadOnlyList<double> dPsi, IReadOnlyList<double> dEps)
		{
			BeginDate = beginDate;
			EndDate = endDate;
			BeginDateHorizons = JulianDayNumber.FromRaw(2437684.5);
			this.dPsi = dPsi;
			this.dEps = dEps;
		}

		/// <summary>
		/// <see cref="eop_tjd_beg"/>
		/// </summary>
		public JulianDayNumber BeginDate { get; }

		/// <summary>
		/// <see cref="eop_tjd_end"/>
		/// </summary>
		public JulianDayNumber EndDate { get; }

		/// <summary>
		/// <see cref="eop_tjd_beg_horizons"/>
		/// </summary>
		public JulianDayNumber BeginDateHorizons { get; }

		/// <summary>
		/// <see cref="swed.dpsi"/>
		/// </summary>
		public IReadOnlyList<double> dPsi { get; }

		/// <summary>
		/// <see cref="swed.deps"/>
		/// </summary>
		public IReadOnlyList<double> dEps { get; }

		/// <summary>
		/// Load from EOP files (see https://datacenter.iers.org/eop.php)
		/// <parameter name="eopC04File">File from https://datacenter.iers.org/data/latestVersion/EOP_14_C04_IAU1980_one_file_1962-now.txt</parameter>
		/// <parameter name="eopFinalsFile">File from https://datacenter.iers.org/data/latestVersion/finals.all.iau1980.txt</parameter>
		/// </summary>
		public static EarthOrientationParameters LoadFrom(
			Stream eopC04File,
			Stream? eopFinalsFile)
		{
			const double TJDOFS = 2400000.5;
			string s;
			JulianDayNumber beginDate = default;
			var dpsi = new List<double>();
			var deps = new List<double>();
			int mjdsv = 0;
			using (var reader = new StreamReader(eopC04File))
			{
				while ((s = reader.ReadLine()) != null)
				{
					if (!s.StartsWith("1962"))
						continue;
					var values = s.Split(' ', StringSplitOptions.RemoveEmptyEntries);
					var mjd = int.Parse(values[3]);
					// is file in one-day steps?
					if (mjdsv > 0 && mjd - mjdsv != 1)
						throw new FormatException("File must be in one-day steps");
					if (mjdsv == 0)
						beginDate = JulianDayNumber.FromRaw(mjd + TJDOFS);
					dpsi.Add(double.Parse(values[8], CultureInfo.InvariantCulture));
					deps.Add(double.Parse(values[9], CultureInfo.InvariantCulture));
					mjdsv = mjd;
				}
			}
			var endDate = JulianDayNumber.FromRaw(mjdsv + TJDOFS);

			// file finals.all may have some more data, and especially estimations for the near future
			if (eopFinalsFile != null)
			{
				mjdsv = 0;
				using var reader = new StreamReader(eopFinalsFile);
				while ((s = reader.ReadLine()) != null)
				{
					var mjd = int.Parse(GetDigits(s, 7));
					var jd = JulianDayNumber.FromRaw(mjd + TJDOFS);
					if (jd <= endDate)
						continue;
					// are data in one-day steps?
					if (mjdsv > 0 && mjd - mjdsv != 1)
						throw new FormatException("File must be in one-day steps");

					// dpsi, deps Bulletin B
					var dPsi = GetDoubleFromStr(s, 168);
					var dEps = GetDoubleFromStr(s, 178);
					if (dPsi == 0.0)
					{
						// try dpsi, deps Bulletin A
						dPsi = GetDoubleFromStr(s, 99);
						dEps = GetDoubleFromStr(s, 118);
					}
					if (dPsi == 0.0)
						break;
					dpsi.Add(dPsi / 1000.0);
					deps.Add(dEps / 1000.0);
					mjdsv = mjd;
				}
			}

			return new EarthOrientationParameters(beginDate, endDate, dpsi, deps);
		}

		private static string? GetDigits(string source, int startIndex)
		{
			if (source.Length <= startIndex)
				return null;
			while (source[startIndex] == ' ')
				startIndex++;
			var end = startIndex;
			while (char.IsDigit(source[end]))
				end++;
			return source[startIndex..end];
		}

		private static double GetDoubleFromStr(string source, int startIndex)
		{
			if (source.Length <= startIndex)
				return 0.0;
			while (startIndex < source.Length && source[startIndex] == ' ')
				startIndex++;
			if (startIndex == source.Length)
				return 0.0;
			var end = startIndex;
			while (char.IsDigit(source[end]) || source[end] == '.' || source[end] == '-')
				end++;
			return double.Parse(source[startIndex..end], CultureInfo.InvariantCulture);
		}
	}
}