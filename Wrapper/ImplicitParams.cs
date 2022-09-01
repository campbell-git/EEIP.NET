using System;
using System.Linq;
using System.Threading.Tasks;
using Sres.Net.EEIP;

namespace Wrapper
{
	public class ImplicitParams
	{
		public byte Instance_ID { get; set; }
		public byte Length { get; set; }
		public ConnectionType ConnectionType { get; set; } = ConnectionType.Point_to_Point;
		public bool OwnerRedundant { get; set; } = true;
	}
}
