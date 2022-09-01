using Wrapper;

namespace Profiles.Interroll
{
	public class MultiControl : ExplicitDevice<InputObject, OutputObject>
	{
		public class Profile : ExplicitProfile
		{
			public const byte INPUT_LENGTH = 36;
			public const byte OUTPUT_LENGTH = 10;
			public override int PollRate_ms => 250;
			public override ExplicitParams Inputs { get; } = new ExplicitParams
			{
				Length = INPUT_LENGTH,
				Instance = 0x65
			};
			public override ExplicitParams Outputs { get; } = new ExplicitParams
			{
				Length = OUTPUT_LENGTH,
				Instance = 0x64
			};
		}

		public MultiControl(int id, Profile profile)
			: base(id, profile)
		{
		}
	}
}
