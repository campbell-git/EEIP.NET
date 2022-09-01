namespace Profiles
{
	static class Extensions
	{
		public static bool Bit(this byte b, int bit) => (b & (1 << bit)) > 0;
		public static byte Bit(this bool b, int bit) => (byte)(b ? 1 << bit : 0);
	}
}
