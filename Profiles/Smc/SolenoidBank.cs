using Wrapper;

namespace Profiles.Smc
{
	public class SolenoidBank : ImplicitDevice<Generic.InputObject, Generic.OutputObject>
	{
		public class Profile : ImplicitProfile
		{
			public const byte INPUT_LENGTH = 6;
			public const byte OUTPUT_LENGTH = 4;
			public override int PollRate_ms => 150;
			public override ImplicitParams T_O { get; set; } = new ImplicitParams
			{
				Instance_ID = 0x64,
				Length = INPUT_LENGTH,
				OwnerRedundant = false,
			};
			public override ImplicitParams O_T { get; set; } = new ImplicitParams
			{
				Instance_ID = 0x96,
				Length = OUTPUT_LENGTH,
				OwnerRedundant = false,
			};
		}

		// Bytes count from the inside out, e.g. input bank 2, bit 1 is found at array [0, 5]
		public SolenoidBank(int id, Profile profile, ImplicitConnection connection)
			: base(id, profile, connection) { }
	}
}
