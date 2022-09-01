using PropertyChanged;
using Sres.Net.EEIP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wrapper
{
	public abstract class ExplicitDevice
	{
		protected readonly ExplicitProfile _profile;
		public ExplicitDevice(ExplicitProfile profile)
		{
			_profile = profile;
		}
	}

	[AddINotifyPropertyChangedInterface]
	public class ExplicitDevice<I, O> : ExplicitDevice
		where I : InputMapper, new()
		where O : OutputMapper, new()
	{
		private readonly EEIPClient _eeip = new EEIPClient();
		private int _pollRate_ms;
		protected bool _tryToConnect = false;

		public readonly int ID;
		public readonly int IP; //Short-hand IpAddress; last byte
		public I Inputs {get; private set;}

		public O Outputs { get; private set; }

		public bool IsConnected { get; private set; }
		public bool DataReceived { get; private set; }

		public event Action<ExplicitDevice<I, O>> ConnectionPulse;

		public ExplicitDevice(int id, ExplicitProfile profile)
			: base(profile)
		{
			ID = id;
			if (int.TryParse(profile.IpAddress.Split('.').Last(), out var ip))
			{
				IP = ip;
			}
			_pollRate_ms = profile.PollRate_ms;
			Inputs = new I();
			Outputs = new O();
			Inputs.ID = id;
			Outputs.ID = id;
		}

		public string IpAddress
		{
			get => _profile.IpAddress;
			set => _profile.IpAddress = value;
		}

		public void StartConnection()
		{
			if (!_tryToConnect)
			{
				Console.WriteLine($"Starting connection monitor for explicit device {IpAddress}");
				_tryToConnect = true;
				ConnectionLoop();
			}
		}

		public void StopConnection()
		{
			Console.WriteLine($"Stopping connection monitor for explicit device {IpAddress}");
			_tryToConnect = false;
		}

		protected virtual void Connect()
		{
			if (!IsConnected && System.Net.IPAddress.TryParse(IpAddress, out var addr))
			{
				try
				{
					Console.WriteLine($"Connecting explicit device {IpAddress}");
					_eeip.RegisterSession(_profile.IpAddress);
					IsConnected = true;
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}
			}
		}

		protected void Disconnect()
		{
			Console.WriteLine($"Disconnecting explicit device {IpAddress}");
			DataReceived = false;
			IsConnected = false;
			try
			{
				// I think this was catching and
				// preventing the socket from closing
				_eeip.UnRegisterSession();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
			try
			{
				_eeip.CloseSocket();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}

		private async void ConnectionLoop()
		{
			while (_tryToConnect)
			{
				await Task.Delay(_pollRate_ms);
				try
				{
					await Task.Run(() => Connect());
					if (IsConnected)
					{
						var connected = await Task.Run(() => _eeip.IsTcpClientConnected());
						if (connected)
						{
							var input = await Read();
							if (input.Length > 0)
							{
								Inputs.ParseBytes(input);
							}
							var output = await Outputs.GetBytes();
							if (output.Length > 0)
							{
								Write(output);
							}
							DataReceived = true;
							ConnectionPulse?.Invoke(this);
						}
						else
						{
							await Task.Run(() => Disconnect());
							await Task.Delay(250);
							Console.WriteLine($"Explicit device {_profile.IpAddress} dropped connection.");
						}
					}
				}
				catch (Exception e)
				{
					await Task.Run(() => Disconnect());
					Console.WriteLine(e);
				}
			}
			await Task.Run(() => Disconnect());
		}

		private async Task<byte[]> Read()
		{
			return await Task.Run(() => _eeip.AssemblyObject.getInstance(_profile.Inputs.Instance));
		}

		private async void Write(byte[] barry)
		{
			await Task.Run(() => _eeip.AssemblyObject.setInstance(_profile.Outputs.Instance, barry));
		}

		public void SetPollRate(int rate)
		{
			_pollRate_ms = rate;
			Console.WriteLine($"Setting poll rate to {rate}ms for address {IpAddress}");
		}
	}
}
