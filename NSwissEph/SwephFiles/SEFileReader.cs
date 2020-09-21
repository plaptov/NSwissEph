using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using NSwissEph.Internals;

namespace NSwissEph.SwephFiles
{
	public class SEFileReader
	{
		private const int MaxChars = 256;
		private const int FileTestEndian = 0x616263; // abc

		private readonly Stream _stream;

		public SEFileReader(Stream stream, SEFileType fileType)
		{
			this._stream = stream;
			FileType = fileType;
		}

		public SEFileType FileType { get; }

		public bool NeedBytesReorder { get; private set; }

		public bool IsLittleEndian { get; private set; }

		public SEFileData ReadConsts()
		{
			var data = new SEFileData()
			{
				FileType = FileType,
				Version = ReadString(),
				FileName = ReadString(),
				Copyright = ReadString(),
			};
			// orbital elements, if single asteroid
			if (FileType == SEFileType.AdditionalAsteroid)
			{
				var asteroidData = ReadString();
				// TODO Parse data
			}
			ReadBytesOrder();
			CheckFileLength();
			data.DENumber = ReadDENumber();
			(data.StartDate, data.EndDate) = ReadFilePeriod();
			data.PlanetNumbers = ReadPlanetNumbers();
			if (FileType == SEFileType.AdditionalAsteroid)
			{
				ReadAsteroidName();
			}
			CheckCRC();
			ReadGeneralConsts();
			ReadPlanetConstants(data);

			return data;
		}

		#region File parts

		private void ReadBytesOrder()
		{
			// one int32 for test of byte order
			var buff = new byte[4];
			_stream.Read(buff, 0, 4);
			int testEndian = BitConverter.ToInt32(buff, 0);
			if (testEndian == FileTestEndian)
				NeedBytesReorder = false;
			else
			{
				NeedBytesReorder = true;
				Array.Reverse(buff);
				int reversed = BitConverter.ToInt32(buff, 0);
				if (reversed != FileTestEndian)
					throw new FormatException();
			}
			IsLittleEndian = BitConverter.IsLittleEndian ^ NeedBytesReorder;
		}

		private void CheckFileLength()
		{
			int length = ReadInt32();
			var currentPosition = _stream.Position;
			_stream.Seek(0, SeekOrigin.End);
			if (length != _stream.Position)
				throw new FormatException();
			_stream.Seek(currentPosition, SeekOrigin.Begin);
		}

		private JPLDENumber ReadDENumber() => (JPLDENumber)ReadInt32();

		/// <summary>
		/// start and end epoch of file 
		/// </summary>
		private (JulianDayNumber, JulianDayNumber) ReadFilePeriod()
		{
			var buff = ReadDoubles(2);
			double start = buff[0];
			double end = buff[1];
			return (JulianDayNumber.FromRaw(start), JulianDayNumber.FromRaw(end));
		}

		private int[] ReadPlanetNumbers()
		{
			int numberSize = 2;
			var buff = ReadData(2);
			short numberPlanets = BitConverter.ToInt16(buff, 0);
			if (numberPlanets > 256)
			{
				numberSize = 4;
				numberPlanets %= 256;
			}
			if (numberPlanets < 1 || numberPlanets > 20)
				throw new FormatException();

			buff = ReadData(numberSize, numberPlanets, 4);
			var result = new int[numberPlanets];
			int j = 0;
			for (int i = 0; i < buff.Length; i+=4)
			{
				result[j++] = BitConverter.ToInt32(buff, i); 
			}
			return result;
		}

		private void ReadAsteroidName()
		{
			// TODO
			ReadData(30);
		}

		private void CheckCRC()
		{
			var currentPosition = _stream.Position;
			if (currentPosition - 1 > 2 * MaxChars)
				throw new FormatException();

			var buff = ReadData(4);
			uint crc = BitConverter.ToUInt32(buff, 0);

			_stream.Seek(0L, SeekOrigin.Begin);
			buff = new byte[currentPosition];
			_stream.Read(buff, 0, (int)currentPosition);

			if (Crc32.CalcCrc(buff, buff.Length) != crc)
				throw new FormatException();

			_stream.Seek(currentPosition + 4, SeekOrigin.Begin);
		}

		private void ReadGeneralConsts()
		{
			/* clight, aunit, helgravconst, ratme, sunradius 
			* these constants are currently not in use */
			ReadData(8, 5);
		}

		private void ReadPlanetConstants(SEFileData data)
		{
			foreach (var ipli in data.PlanetNumbers)
			{
				var pdp = new PlanetData();

				pdp.InternalBodyNumber = ipli;
				pdp.FileIndexStart = ReadInt32();
				pdp.Flags = (PlanetFlags)_stream.ReadByte();
				pdp.CoefficientsNumber = _stream.ReadByte();
				pdp.NormalizationFactor = ReadInt32() / 1000.0;
				var doubles = ReadDoubles(10);
				pdp.StartDate = JulianDayNumber.FromRaw(doubles[0]);
				pdp.EndDate = JulianDayNumber.FromRaw(doubles[1]);
				pdp.SegmentSize = doubles[2];
				pdp.IndexEntriesCount = (int)((doubles[1] - doubles[0] + 0.1) / doubles[2]);
				pdp.ElementsEpoch = doubles[3];
				pdp.Prot = doubles[4];
				pdp.Dprot = doubles[5];
				pdp.Qrot = doubles[6];
				pdp.Dqrot = doubles[7];
				pdp.Perigee = doubles[8];
				pdp.DPerigee = doubles[9];
				if (pdp.Flags.HasFlag(PlanetFlags.Ellipse))
					pdp.ReferenceEllipseCoefficients = ReadDoubles(2 * pdp.CoefficientsNumber);

				var index = ipli >= SEConsts.AseroidOffset
					? InternalPlanets.AnyBody
					: (InternalPlanets)ipli;
				data.PlanetsData[index] = pdp;
			}
		}

		#endregion

		internal SEFileSegment ReadSegment(PlanetData pdp, JulianDayNumber jd)
		{
			int segmentNumber = (int)((jd - pdp.StartDate).Raw / pdp.SegmentSize);
			var segmentStart = pdp.StartDate + JulianDayNumber.FromRaw(pdp.SegmentSize * segmentNumber);
			var segmentEnd = segmentStart + JulianDayNumber.FromRaw(pdp.SegmentSize);

			var fpos = pdp.FileIndexStart + segmentNumber * 3;
			fpos = ReadInt32From3Bytes(fpos);
			_stream.Seek(fpos, SeekOrigin.Begin);

			var rmax = pdp.NormalizationFactor;
			double[] segp = new double[pdp.CoefficientsNumber * 3];
			// read coefficients for 3 coordinates
			for (var icoord = 0; icoord < 3; icoord++)
			{
				var idbl = icoord * pdp.CoefficientsNumber;
				// first read header
				// first bit indicates number of sizes of packed coefficients
				int nco;
				int nsizes;
				int[] nsize = new int[6];
				var c = ReadData(1, 2);
				if ((c[0] & 128) > 0)
				{
					var c2 = ReadData(1, 2);
					nsizes = 6;
					nsize[0] = c[1] / 16;
					nsize[1] = c[1] % 16;
					nsize[2] = c2[0] / 16;
					nsize[3] = c2[0] % 16;
					nsize[4] = c2[1] / 16;
					nsize[5] = c2[1] % 16;
					nco = nsize[0] + nsize[1] + nsize[2] + nsize[3] + nsize[4] + nsize[5];
				}
				else
				{
					nsizes = 4;
					nsize[0] = c[0] / 16;
					nsize[1] = c[0] % 16;
					nsize[2] = c[1] / 16;
					nsize[3] = c[1] % 16;
					nco = nsize[0] + nsize[1] + nsize[2] + nsize[3];
				}
				// there may not be more coefficients than interpolation order + 1
				if (nco > pdp.CoefficientsNumber)
					throw new FormatException($"Error in ephemeris file: {nco} coefficients instead of {pdp.CoefficientsNumber}");

				// now unpack
				for (var i = 0; i < nsizes; i++)
				{
					if (nsize[i] == 0)
						continue;

					if (i < 4)
					{
						int j = 4 - i;
						int k = nsize[i];
						var longs = ReadUints(j, k);
						for (int m = 0; m < k; m++)
						{
							if ((longs[m] & 1) > 0) // will be negative
								segp[idbl] = -(((longs[m] + 1) / 2) / 1e+9 * rmax / 2);
							else
								segp[idbl] = (longs[m] / 2) / 1e+9 * rmax / 2;
							idbl++;
						}
					}
					else if (i == 4) // half byte packing
					{
						int k = (nsize[i] + 1) / 2;
						var longs = ReadUints(1, k);
						for (int m = 0, j = 0;
							m < k && j < nsize[i];
							m++)
						{
							for (int n = 0, o = 16;
								n < 2 && j < nsize[i];
								n++, j++, idbl++, longs[m] %= (uint)o, o /= 16)
							{
								if ((longs[m] & o) > 0)
									segp[idbl] = -(((longs[m] + o) / o / 2) * rmax / 2 / 1e+9);
								else
									segp[idbl] = (longs[m] / o / 2) * rmax / 2 / 1e+9;
							}
						}
					}
					else if (i == 5) // quarter byte packing
					{
						int k = (nsize[i] + 3) / 4;
						var longs = ReadUints(1, k);
						for (int m = 0, j = 0;
							m < k && j < nsize[i];
							m++)
						{
							for (int n = 0, o = 64;
								n < 4 && j < nsize[i];
								n++, j++, idbl++, longs[m] %= (uint)o, o /= 4)
							{
								if ((longs[m] & o) > 0)
									segp[idbl] = -(((longs[m] + o) / o / 2) * rmax / 2 / 1e+9);
								else
									segp[idbl] = (longs[m] / o / 2) * rmax / 2 / 1e+9;
							}
						}
					}
				}
			}

			return new SEFileSegment(segmentStart, segmentEnd, segp);
		}

		#region Service methods

		private string ReadString()
		{
			var bytes = new List<byte>(MaxChars);
			int count = 0;
			while (count < MaxChars)
			{
				var b = _stream.ReadByte();
				bytes.Add((byte)b);
				if (b == 0x0A)
					return Encoding.UTF8.GetString(bytes.ToArray()).TrimEnd();
				count++;
			}
			throw new FormatException();
		}

		private int ReadInt32() => BitConverter.ToInt32(ReadData(4), 0);

		private int ReadInt32From3Bytes(long? offset) => BitConverter.ToInt32(ReadData(3, 1, 4, offset), 0);

		private double[] ReadDoubles(int count)
		{
			var buff = ReadData(8, count);
			var result = new double[count];
			for (int i = 0; i < count; i++)
				result[i] = BitConverter.ToDouble(buff, i * 8);
			return result;
		}

		private uint[] ReadUints(int size, int count)
		{
			var buff = ReadData(size, count, 4);
			var result = new uint[count];
			for (int i = 0; i < count; i++)
				result[i] = BitConverter.ToUInt32(buff, i * 4);
			return result;
		}

		private byte[] ReadData(int size, int count = 1) =>
			ReadData(size, count, correctedSize: size);

		/// <summary>
		/// Reads from a file and, if necessary, reorders bytes 
		/// </summary>
		/// <param name="size">Size of item to be read</param>
		/// <param name="count">Number of items</param>
		/// <param name="correctedSize">In what size should it be returned (e.g. 3 byte int -> 4 byte int)</param>
		/// <param name="offset">File position (current if null)</param>
		/// <see cref="do_fread"/>
		private byte[] ReadData(int size, int count, int correctedSize, long? offset = null)
		{
			var totalSize = size * count;
			if (offset.HasValue)
				_stream.Seek(offset.Value, SeekOrigin.Begin);

			var data = new byte[totalSize];
			_stream.Read(data, 0, totalSize);

			if (!NeedBytesReorder && size == correctedSize)
				return data;

			var result = size == correctedSize
				? new byte[totalSize]
				: new byte[correctedSize * count];

			for (var i = 0; i < count; i++)
			{
				for (var j = size - 1; j >= 0; j--)
				{
					int k = NeedBytesReorder ? size - j - 1 : j;
					if (size != correctedSize && IsLittleEndian == NeedBytesReorder)
						k += correctedSize - size;
					result[i * correctedSize + k] = data[i * size + j];
				}
			}
			return result;
		}

		#endregion
	}
}
