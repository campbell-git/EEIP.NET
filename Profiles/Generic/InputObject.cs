using PropertyChanged;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Profiles.Generic
{
	[AddINotifyPropertyChangedInterface]
	public class InputObject : Wrapper.InputMapper
	{
		public override void ParseBytes(byte[] barry)
		{
			var length = IO.Select(io => io.Register).Max() + 1;
			_barry = new byte[length];
			Array.Copy(barry, 0, _barry, 0, length);
			foreach (var io in IO)
			{
				io.SetValue(barry);
			}
			ByteToString = ToString();
		}

		public string ByteToString { get; set; }

		public override string ToString()
		{
			if (_barry == null) return "";
			var s = "";
			var i = 0;
			foreach (var b in _barry)
			{
				s += $"{i}: {_barry[i++]}\n";
			}
			return s;
		}

		public ConcurrentBag<DigitalIO> IO { get; private set; } = new ConcurrentBag<DigitalIO>();
	}
}
