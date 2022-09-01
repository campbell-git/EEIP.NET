using System;

namespace Wrapper
{
	public abstract class ImplicitProfile : Profile
	{
		public override ushort Port { get; set; } = 2222;

		/// <summary>
		/// Inputs to device
		/// </summary>
		public abstract ImplicitParams T_O { get; set; }

		/// <summary>
		/// Outputs to device
		/// </summary>
		public abstract ImplicitParams O_T { get; set; }
	}
}
