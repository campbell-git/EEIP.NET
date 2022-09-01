using PropertyChanged;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Profiles.Generic
{
	[AddINotifyPropertyChangedInterface]
	public class OutputObject : Wrapper.OutputMapper
	{
		public override Task<byte[]> GetBytes()
		{
			return Task.Run(() =>
			{
				if (IO.Count == 0) return new byte[0];
				var length = IO.Select(io => io.Register).Max() + 1;
				var barry = new byte[length];
				foreach (var io in IO)
				{
					barry[io.Register] += io.GetValue();
				}
				return barry;
			});
		}

		public ConcurrentBag<DigitalIO> IO { get; private set; } = new ConcurrentBag<DigitalIO>();
	}
}
