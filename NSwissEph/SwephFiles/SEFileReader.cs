using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
			ReadPlanetConstants();

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
			var buff = ReadData(4);
			int length = BitConverter.ToInt32(buff, 0);
			var currentPosition = _stream.Position;
			_stream.Seek(0, SeekOrigin.End);
			if (length != _stream.Position)
				throw new FormatException();
			_stream.Seek(currentPosition, SeekOrigin.Begin);
		}

		private JPLDENumber ReadDENumber()
		{
			var buff = ReadData(4);
			var i = BitConverter.ToInt32(buff, 0);
			return (JPLDENumber)i;
		}

		/// <summary>
		/// start and end epoch of file 
		/// </summary>
		private (JulianDayNumber, JulianDayNumber) ReadFilePeriod()
		{
			var buff = ReadData(8, 2);
			double start = BitConverter.ToDouble(buff, 0);
			double end = BitConverter.ToDouble(buff, 8);
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

		private void ReadPlanetConstants()
		{
			// TBD
		}

		#endregion

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
					return Encoding.UTF8.GetString(bytes.ToArray());
				count++;
			}
			throw new FormatException();
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
