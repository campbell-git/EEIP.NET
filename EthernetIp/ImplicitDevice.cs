using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wrapper
{
	public abstract class ImplicitDevice
	{
		protected readonly ImplicitProfile _profile;
		protected readonly ImplicitConnection _connection;
		public ImplicitDevice(ImplicitProfile profile, ImplicitConnection connection)
		{
			_profile = profile;
			_connection = connection;
		}
	}

	[AddINotifyPropertyChangedInterface]
	public class ImplicitDevice<I, O> : ImplicitDevice
		where I : InputMapper, new()
		where O : OutputMapper, new()
	{
		private bool _tryToConnect = false;
		public readonly int ID;
		public readonly int IP; //Short-hand IpAddress; last byte
		public uint ConnectionId { get; private set; }
		public I Inputs {get; private set;}
		public O Outputs { get; private set; }
		public bool IsConnected { get; private set; }
		public event Action ConnectionPulse;

		public ImplicitDevice(int id, ImplicitProfile profile, ImplicitConnection connection)
			: base(profile, connection)
		{
			if (int.TryParse(profile.IpAddress.Split('.').Last(), out var ip))
			{
				IP = ip;
			}
			ID = id;
			Inputs = new I();
			Outputs = new O();
		}

		public string IpAddress
		{
			get => _profile.IpAddress;
			set => _profile.IpAddress = value;
		}
		public bool DataReceived { get; private set; }

		public void StartConnection()
		{
			if (!_tryToConnect)
			{
				Console.WriteLine($"Starting connection monitor for implicit device {IpAddress}");
				_tryToConnect = true;
				ConnectionLoop();
			}
		}

		public void StopConnection()
		{
			Console.WriteLine($"Stopping connection monitor for implicit device {IpAddress}");
			_tryToConnect = false;
		}

		protected virtual void Connect()
		{
			if (!IsConnected && System.Net.IPAddress.TryParse(IpAddress, out var addr))
			{
				try
				{
					Console.WriteLine($"Connecting implicit device {IpAddress}");
					ConnectionId = _connection.CreateConnection(_profile);
					if (ConnectionId > 0)
					{
						IsConnected = true;
					}
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}
			}
		}

		protected void Disconnect()
		{
			DataReceived = false;
			if (IsConnected)
			{
				try
				{
					Console.WriteLine($"Disconnecting implicit device {IpAddress}");
					_connection.CloseConnection(ConnectionId);
				}
				catch (Sres.Net.EEIP.CIPException ce)
				{
					Console.WriteLine(ce);
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}
				finally
				{
					IsConnected = false;
				}
			}
		}

		private bool TimedOut()
		{
			var time = _connection[ConnectionId].LastReceivedImplicitMessage;
			var limit = _profile.PollRate_ms * 10;
			return DateTime.Now.Subtract(time).TotalMilliseconds > limit;
		}

		private async void ConnectionLoop()
		{
			while (_tryToConnect)
			{
				try
				{
					if (IsConnected)
					{
						if (TimedOut())
						{
							await Task.Run(() => Disconnect());
							await Task.Delay(250);
							Console.WriteLine($"Implicit connection {_profile.IpAddress} timed out.");
						}
						else
						{
							var input = _connection.GetInputs(ConnectionId);
							if (input.Length > 0)
							{
								Inputs.ParseBytes(input);
							}
							var output = await Outputs.GetBytes();
							if (output.Length > 0)
							{
								_connection.SetOutputs(ConnectionId, output);
							}
							DataReceived = true;
							ConnectionPulse?.Invoke();
						}
					}
					else
					{
						await Task.Run(() => Connect());
					}
				}
				catch (Exception e)
				{
					await Task.Run(() => Disconnect());
					Console.WriteLine(e);
				}
				await Task.Delay(_profile.PollRate_ms);
			}
			await Task.Run(() => Disconnect());
		}
	}
}
