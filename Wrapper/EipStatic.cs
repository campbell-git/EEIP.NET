using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wrapper
{
	public static class EipStatic
	{
		public static string DiscoverDevices()
		{
			return string.Join("\n", Sres.Net.EEIP.EEIPClient.DiscoverDevices());
		}
	}
}
