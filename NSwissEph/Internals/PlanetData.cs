using System;
using System.Collections.Generic;
using System.Text;

namespace NSwissEph.Internals
{
	internal class PlanetData
	{
		/// <summary>
		/// </summary>
		/// <see cref="ibdy"/>
		public int InternalBodyNumber { get; set; }

		/// <summary>
		/// </summary>
		/// <see cref="iflg"/>
		public PlanetFlags Flags { get; set; }

		/// <summary>
		/// # of coefficients of ephemeris polynomial, is polynomial order + 1
		/// </summary>
		/// <see cref="ncoe"/>
		public int CoefficientsNumber { get; set; }

		#region where is the segment index on the file

		/// <summary>
		/// file position of begin of planet's index
		/// </summary>
		/// <see cref="lndx0"/>
		public int FileIndexStart { get; set; }

		/// <summary>
		/// number of index entries on file: computed
		/// </summary>
		/// <see cref="nndx"/>
		public int IndexEntriesCount { get; set; }

		/// <summary>
		/// file contains ephemeris for <see cref="StartDate"/> thru <see cref="EndDate"/> for this particular planet
		/// </summary>
		/// <see cref="tfstart"/>
		public JulianDayNumber StartDate { get; set; }

		/// <summary>
		/// file contains ephemeris for <see cref="StartDate"/> thru <see cref="EndDate"/> for this particular planet
		/// </summary>
		/// <see cref="tfend"/>
		public JulianDayNumber EndDate { get; set; }

		/// <summary>
		/// segment size (days covered by a polynomial)
		/// </summary>
		/// <see cref="dseg"/>
		public double SegmentSize { get; set; }

		#endregion

		#region orbital elements:

		/// <see cref="telem"/>
		public double ElementsEpoch { get; set; }

		/// <see cref="prot"/>
		public double Prot { get; set; }

		/// <see cref="qrot"/>
		public double Qrot { get; set; }

		/// <see cref="dprot"/>
		public double Dprot { get; set; }

		/// <see cref="dqrot"/>
		public double Dqrot { get; set; }

		/// <summary>
		/// normalisation factor of chebyshew coefficients
		/// </summary>
		/// <see cref="rmax"/>
		public double NormalizationFactor { get; set; }

		#endregion

		#region in addition, if reference ellipse is used:

		/// <see cref="peri"/>
		public double Perigee { get; set; }

		/// <see cref="dperi"/>
		public double DPerigee { get; set; }

		/// <summary>
		/// cheby coeffs of reference ellipse,
		/// size of data is 2 x ncoe
		/// </summary>
		/// <see cref="refep"/>
		public double[] ReferenceEllipseCoefficients { get; set; }

		#endregion

		#region unpacked segment information, only updated when a segment is read:

		/// <see cref="tseg0"/>
		public JulianDayNumber SegmentStart { get; set; }

		/// <see cref="tseg1"/>
		public JulianDayNumber SegmentEnd { get; set; }

		/// <summary>
		/// unpacked cheby coeffs of segment;
		/// the size is 3 x ncoe
		/// </summary>
		/// <see cref="segp"/>
		public double[] SegmentCoefficients { get; set; }

		/// <summary>
		/// how many coefficients to evaluate. this may be less than <see cref="CoefficientsNumber"/>
		/// </summary>
		/// <see cref="neval"/>
		public int EvaluateCoefficientsCount { get; set; }

		#endregion

		#region result of most recent data evaluation for this body:

		/// <summary>
		/// time for which previous computation was made
		/// </summary>
		/// <see cref="teval"/>
		public JulianDayNumber EvaluatedTime { get; set; }

		/// <summary>
		/// which ephemeris was used
		/// </summary>
		/// <see cref="iephe"/>
		public int EphemerisIndex { get; set; }

		/// <summary>
		/// position and speed vectors equatorial J2000;
		/// 6 elements in array
		/// </summary>
		/// <see cref="x"/>
		public double[] Data { get; set; }

		/// <summary>
		/// hel., light-time, aberr., prec. flags etc.
		/// </summary>
		/// <see cref="xflgs"/>
		public int AdditinalFlags { get; set; }

		/// <summary>
		/// return positions:
		/// * xreturn+0	ecliptic polar coordinates
		/// * xreturn+6	ecliptic cartesian coordinates
		/// * xreturn+12	equatorial polar coordinates
		/// * xreturn+18	equatorial cartesian coordinates
		/// </summary>
		/// <see cref="xreturn"/>
		public double[] ReturnPositions { get; set; }

		#endregion
	}
}
