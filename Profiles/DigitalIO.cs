using PropertyChanged;
using System;

namespace Profiles
{
	[AddINotifyPropertyChangedInterface]
	public class DigitalIO
	{
		public int Register { get; set; }
		public int Bit { get; set; }
		public bool Value { get; set; }

		public Action<bool> EdgeTrigger;

		// Set the value from the proper bit register
		public void SetValue(byte[] barry)
		{
			var value = barry[Register].Bit(Bit);
			if (Value != value)
			{
				Value = value;
				EdgeTrigger?.Invoke(Value);
			}
		}

		// Get the bit value for setting the proper register
		public byte GetValue() => Value.Bit(Bit);

		public DigitalIO((int reg, int bit) set)
		{
			Register = set.reg;
			Bit = set.bit;
		}
	}
}
