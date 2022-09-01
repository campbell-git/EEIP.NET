namespace Wrapper
{
	public abstract class ExplicitProfile : Profile
	{
		public override ushort Port { get; set; } = 0xAF12;
		public abstract ExplicitParams Inputs { get; }
		public abstract ExplicitParams Outputs { get; }
	}
}
