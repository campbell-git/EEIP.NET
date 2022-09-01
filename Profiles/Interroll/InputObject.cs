using PropertyChanged;
using System;

namespace Profiles.Interroll
{
	[AddINotifyPropertyChangedInterface]
	public class InputObject : Wrapper.InputMapper
	{
		public InputObject()
		{
			_barry = new byte[MultiControl.Profile.INPUT_LENGTH];
		}

		public override void ParseBytes(byte[] barry)
		{
			Array.Copy(barry, 0, _barry, 0, MultiControl.Profile.INPUT_LENGTH);
			var sensors = new bool[8];
			var speed = new sbyte[4];
			VoltageMotor = GetShort(barry, 16);
			VoltageLogic = GetShort(barry, 18);
			Temperature = GetShort(barry, 20);
			SystemUptime = (uint)GetInt(barry, 22);
			Decision = barry[27];
			Start = barry[35].Bit(0);
			StartInverse = barry[35].Bit(1);
			NormalStop = barry[35].Bit(2);
			EmergencyStop = barry[35].Bit(3);
			AlternateSpeed = barry[35].Bit(4);
			InverseDirection = barry[35].Bit(5);
			GlobalError = barry[35].Bit(6);
			for (int i = 0; i < 8; i++)
			{
				if (i < 4)
				{
					IO[i] = barry[1].Bit(i);
					Error[i] = barry[2].Bit(i);
					speed[i] = (sbyte)barry[3 + i];
					Current[i] = (ushort)GetShort(barry, 8 + (i << 1));
					HandshakeIn[i] = barry[29].Bit(i);
					HandshakeOut[i] = barry[29].Bit(i + 4);
					ZoneBusy[i] = barry[30].Bit(i);
					ZoneError[i] = barry[31 + i];
				}
				sensors[i] = barry[0].Bit(i);
				ControlInput[i] = barry[26].Bit(i);
				ControlOutput[i] = barry[28].Bit(i);
			}
			Sensors = sensors;
			Speed = speed;
		}

		private short GetShort(byte[] barry, int register)
		{
			short myShort = 0;
			for (int i = 0; i < 2; i++)
			{
				myShort += (short)(barry[register + i] << (i * 8));
			}
			return myShort;
		}

		private int GetInt(byte[] barry, int register)
		{
			int myInt = 0;
			for (int i = 0; i < 4; i++)
			{
				myInt += (barry[register + i] << (i * 8));
			}
			return myInt;
		}

		public bool[] Sensors { get; private set; } = new bool[8];
		public bool[] IO { get; private set; } = new bool[4];
		public bool[] Error { get; private set; } = new bool[4];
		public sbyte[] Speed { get; private set; } = new sbyte[4];
		public ushort[] Current { get; private set; } = new ushort[4];
		public short VoltageMotor { get; private set; }
		public short VoltageLogic { get; private set; }
		public short Temperature { get; private set; }
		public uint SystemUptime { get; private set; }
		public byte Decision { get; private set; }
		public bool[] ControlInput { get; private set; } = new bool[8];
		public bool[] ControlOutput { get; private set; } = new bool[8];
		public bool[] HandshakeIn { get; private set; } = new bool[4];
		public bool[] HandshakeOut { get; private set; } = new bool[4];
		public bool[] ZoneBusy { get; private set; } = new bool[4];
		public byte[] ZoneError { get; private set; } = new byte[4];
		public bool Start { get; private set; }
		public bool StartInverse { get; private set; }
		public bool NormalStop { get; private set; }
		public bool EmergencyStop { get; private set; }
		public bool AlternateSpeed { get; private set; }
		public bool InverseDirection { get; private set; }
		public bool GlobalError { get; private set; }
	}
}
