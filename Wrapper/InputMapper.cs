using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wrapper
{
	[AddINotifyPropertyChangedInterface]
	public abstract class InputMapper
	{
		protected byte[] _barry;

		public int ID { get; set; }

		public InputMapper() { }

		public virtual string PrintProperties()
		{
			var s = "";
			foreach (var prop in GetType().GetProperties())
			{
				if (prop.PropertyType.IsArray)
				{
					var i = 0;
					foreach (var value in (Array)prop.GetValue(this))
					{
						s += $"{prop.Name}[{i++}] = {value}\n";
					}
				}
				else
				{
					s += $"{prop.Name} = {prop.GetValue(this)}\n";
				}
			}

			return s;
		}

		public abstract void ParseBytes(byte[] barry);

		protected bool GetBit(int bit)
		{
			--bit;
			var by = bit / 8;
			var bi = bit % 8;
			return (_barry[by] & (1 << bi)) > 0;
		}

		// Start and end are the bits
		protected int ParseBits(int start, int end)
		{
			--start;
			--end;
			var startByte = start / 8;
			var startBit = start % 8;

			var endByte = end / 8;
			var endBit = end % 8;

			var returnInt = 0;
			var myBit = 0;
			var shift = -startBit;
			for (int i = startByte; i <= endByte; i++)
			{
				while ((startBit < 8 && i < endByte) || (startBit <= endBit && i == endByte))
				{
					var b = (_barry[i] & (1 << startBit));
					if (shift < 0)
					{
						b >>= -shift;
					}
					else
					{
						b <<= shift;
					}
					returnInt += b;
					++startBit;
					if (++myBit < 8) continue;
					myBit = 0;
					shift += 8;
				}

				startBit = 0;
			}

			return returnInt;
		}
	}
}
