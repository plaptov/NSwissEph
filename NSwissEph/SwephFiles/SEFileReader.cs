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

		public string Version { get; private set; }

		public string FileName { get; private set; }

		public string Copyright { get; private set; }

		public bool NeedBytesReorder { get; private set; }

		public bool IsLittleEndian { get; private set; }

		public void ReadConsts()
		{
			Version = ReadString();
			FileName = ReadString();
			Copyright = ReadString();
			// orbital elements, if single asteroid
			if (FileType == SEFileType.AdditionalAsteroid)
			{
				var asteroidData = ReadString();
				// TODO Parse data
			}
			ReadBytesOrder();
			CheckFileLength();
		}

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
			var buff = ReadData(4, 1, 4);
			int length = BitConverter.ToInt32(buff, 0);
			var currentPosition = _stream.Position;
			_stream.Seek(0, SeekOrigin.End);
			if (length != _stream.Position)
				throw new FormatException();
			_stream.Seek(currentPosition, SeekOrigin.Begin);
		}

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
	}
}
