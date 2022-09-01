using PropertyChanged;
using System.Threading.Tasks;

namespace Profiles.Interroll
{
	[AddINotifyPropertyChangedInterface]
	public class OutputObject : Wrapper.OutputMapper
	{
		public override Task<byte[]> GetBytes()
		{
			return Task.Run(() =>
			{
				var barry = new byte[MultiControl.Profile.OUTPUT_LENGTH];
				barry[6] = Decision;
				barry[9] = Start.Bit(0);
				barry[9] = StartInverse.Bit(1);
				barry[9] = NormalStop.Bit(2);
				barry[9] = EmergencyStop.Bit(3);
				barry[9] = AlternateSpeed.Bit(4);
				barry[9] = InverseDirection.Bit(5);
				barry[9] = GlobalError.Bit(6);
				for (int i = 0; i < 8; i++)
				{
					if (i < 4)
					{
						/*
						if (MaxSpeedForward[i])
						{
							Speed[i] = 100;
						}
						else if (MaxSpeedReverse[i])
						{
							Speed[i] = -100;
						}
						else
						{
							Speed[i] = 0;
						}
						*/
						barry[0] += IO[i].Bit(i);
						barry[1 + i] = (byte)Speed[i];
						barry[8] += HandshakeIn[i].Bit(i);
						barry[8] += HandshakeOut[i].Bit(4 + i);
					}
					barry[5] += ControlInput[i].Bit(i);
					barry[7] += ControlOutput[i].Bit(i);
				}
				return barry;
			});
		}

		//public bool[] MaxSpeedForward { get; set; } = new bool[4];
		//public bool[] MaxSpeedReverse { get; set; } = new bool[4];
		public bool[] IO { get; set; } = new bool[4];
		public sbyte[] Speed { get; set; } = new sbyte[4];
		public bool[] ControlInput { get; set; } = new bool[8];
		public byte Decision { get; set; }
		public bool[] ControlOutput { get; set; } = new bool[8];
		public bool[] HandshakeIn { get; set; } = new bool[4];
		public bool[] HandshakeOut { get; set; } = new bool[4];
		public bool Start { get; set; }
		public bool StartInverse { get; set; }
		public bool NormalStop { get; set; }
		public bool EmergencyStop { get; set; }
		public bool AlternateSpeed { get; set; }
		public bool InverseDirection { get; set; }
		public bool GlobalError { get; set; }
	}
}
