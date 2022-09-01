using PropertyChanged;

namespace Wrapper
{
	[AddINotifyPropertyChangedInterface]
	public abstract class OutputMapper
	{
		public int ID { get; set; }

		public string IoType => "Outputs";

		public string PrintProperties()
		{
			var s = "";
			foreach (var prop in GetType().GetProperties())
			{
				s += $"{prop.Name} = {prop.GetValue(this)}\n";
			}

			return s;
		}

		public abstract System.Threading.Tasks.Task<byte[]> GetBytes();

		protected void AddBit(byte bit, bool value, ref byte[] barry)
		{
			--bit;
			var by = bit / 8;
			var bi = bit % 8;
			barry[by] += (byte)(value ? 1 << bi : 0);
		}
	}
}