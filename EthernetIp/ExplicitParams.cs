using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wrapper
{
	public class ExplicitParams
	{
		public byte Length { get; set; }
		public byte Class { get; set; } = 0x04;
		public byte Instance { get; set; } = 0x65;
		public byte Attribute { get; set; } = 0x03;
	}
}
