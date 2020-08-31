namespace NSwissEph.SwephFiles
{
	/*
	 * The following C code (by Rob Warnock rpw3@sgi.com) does CRC-32 in
	 * BigEndian/BigEndian byte/bit order. That is, the data is sent most
	 * significant byte first, and each of the bits within a byte is sent most
	 * significant bit first, as in FDDI. You will need to twiddle with it to do
	 * Ethernet CRC, i.e., BigEndian/LittleEndian byte/bit order.
	 * 
	 * The CRCs this code generates agree with the vendor-supplied Verilog models
	 * of several of the popular FDDI "MAC" chips.
	 */
	internal class Crc32
	{
		private const int CRC32_POLY = 0x04c11db7; // AUTODIN II, Ethernet, & FDDI
		private static readonly uint[] Crc32Table;

		/// <summary>
		/// Build auxiliary table for parallel byte-at-a-time CRC-32.
		/// </summary>
		static Crc32()
		{
			int i, j;
			uint c;
			for (i = 0; i < 256; ++i)
			{
				for (c = (uint)(i << 24), j = 8; j > 0; --j)
					c = (c & 0x80000000) > 0 ? (c << 1) ^ CRC32_POLY : (c << 1);
				Crc32Table[i] = c;
			}
		}

		public static uint CalcCrc(byte[] buf, int len)
		{
			uint crc = 0xffffffff; // preload shift register, per CRC-32 spec
			for (int p = 0; p < len; ++p)
				crc = (crc << 8) ^ Crc32Table[(crc >> 24) ^ buf[p]];
			return ~crc;           // transmit complement, per CRC-32 spec
		}
	}
}
