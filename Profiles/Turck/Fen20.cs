using Wrapper;

namespace Profiles.Turck
{
	public class Fen20 : ImplicitDevice<Generic.InputObject, Generic.OutputObject>
	{
		public class Profile : ImplicitProfile
		{
			public const byte INPUT_LENGTH = 8;
			public const byte OUTPUT_LENGTH = 4;
			public const int DEFAULT_REGISTER = 2;

			public override int PollRate_ms => 200;
			public override ImplicitParams T_O { get; set; } = new ImplicitParams
			{
				Instance_ID = 0x67,
				Length = INPUT_LENGTH,
			};
			public override ImplicitParams O_T { get; set; } = new ImplicitParams
			{
				Instance_ID = 0x68,
				Length = OUTPUT_LENGTH,
			};
		}

		public Fen20(int id, Profile profile, ImplicitConnection connection)
			: base(id, profile, connection) { }
	}
}
