using System;
using System.Linq;
using System.Threading.Tasks;
using Sres.Net.EEIP;

namespace Wrapper
{
	public abstract class Profile
	{
		public string IpAddress { get; set; }
		public abstract ushort Port { get; set; }
		public abstract int PollRate_ms { get; }
        public abstract byte AssemblyObjectClass { get; }
        public abstract byte ConfigurationAssemblyInstanceID { get; }
	}
}
