using Sres.Net.EEIP;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wrapper
{
	public class ImplicitConnection
	{
		private static SocketListener _listener = new SocketListener();
		private const double TIMEOUT_ms = 250;

		public EEIPClient this[uint id] => _listener[id];

		public void StartListener(string ip)
		{
			_listener.CreateReceiveSocket(ip);
		}

		public void CloseListener()
		{
			_listener.CloseSocket();
		}

		//public EEIPClient this[uint id] => _listener[id];

		// Return the dictionary key stored in the listener
		public uint CreateConnection(ImplicitProfile profile)
		{
			var eip = new EEIPClient {
				IPAddress = profile.IpAddress,
			};
			//Ip-Address of the Ethernet-IP Device (In this case Allen-Bradley 1734-AENT Point I/O)
			//A Session has to be registered before any communication can be established
			//var port = (ushort)(7000 + ushort.Parse(ip.Split('.')[3]));
			eip.RegisterSession();
			eip.OriginatorUDPPort = profile.Port;
			eip.TargetUDPPort = 2048;
			//_eeipClient.TCPPort = port;

			eip.AssemblyObjectClass = profile.AssemblyObjectClass;
			eip.ConfigurationAssemblyInstanceID = profile.ConfigurationAssemblyInstanceID;

			//Parameters from Originator -> Target
			eip.O_T_InstanceID = profile.O_T.Instance_ID; //Instance ID of the Output Assembly
			eip.O_T_Length = profile.O_T.Length; //The Method "Detect_O_T_Length" detect the Length using an UCMM Message
			eip.O_T_RealTimeFormat = RealTimeFormat.Header32Bit; //Header Format
			eip.O_T_OwnerRedundant = profile.O_T.OwnerRedundant;
			eip.O_T_Priority = Priority.Scheduled;
			eip.O_T_VariableLength = false;
			eip.O_T_ConnectionType = profile.O_T.ConnectionType;
			eip.RequestedPacketRate_O_T = (uint)(profile.PollRate_ms * 100);//.O_T.PacketRate_us; //500ms is the Standard value

			//Parameters from Target -> Originator
			eip.T_O_InstanceID = profile.T_O.Instance_ID;
			eip.T_O_Length = profile.T_O.Length;
			eip.T_O_RealTimeFormat = RealTimeFormat.Modeless;
			eip.T_O_OwnerRedundant = profile.T_O.OwnerRedundant;
			eip.T_O_Priority = Priority.Scheduled;
			eip.T_O_VariableLength = false;
			eip.T_O_ConnectionType = profile.T_O.ConnectionType;
			eip.RequestedPacketRate_T_O = (uint)(profile.PollRate_ms * 100);//.T_O.PacketRate_us; //RPI in  500ms is the Standard value

			//Forward open initiates the Implicit Messaging
			var id = eip.ForwardOpen();
			if (!_listener.TryAddClient(id, eip))
			{
				eip.ForwardClose();
				return 0;
			}
			return id;
		}

		public void CloseConnection(uint id)
		{
			var eip = _listener[id];
			_listener.RemoveClient(id);
			eip.ForwardClose();
		}

		public byte[] GetInputs(uint id)
		{
			return _listener[id].T_O_IOData;
		}

		public byte GetInputs(uint id, byte register)
		{
			return _listener[id].T_O_IOData[register];
		}

		// Use a zero based bit, not a bit mask
		public bool GetInputs(uint id, byte register, byte bit)
		{
			return (_listener[id].T_O_IOData[register] & (1 << bit)) > 0;
		}

		public void SetOutputs(uint id, byte[] registers)
		{
			var eip = _listener[id];
			for (int i = 0; i < registers.Length; i++)
			{
				eip.O_T_IOData[i] = registers[i];
			}
		}

		public void SetOutputs(uint id, byte register, byte value)
		{
			_listener[id].O_T_IOData[register] = value;
		}

		// Use a zero based bit, not a bit mask
		public void SetOutputs(uint id, byte register, byte bit, bool value)
		{
			var mask = (byte)(1 << bit);
			var eip = _listener[id];
			var currentValue = (eip.O_T_IOData[register] & mask) > 0;
			if (currentValue && !value)
			{
				eip.O_T_IOData[register] ^= mask;
			}
			if (!currentValue && value)
			{
				eip.O_T_IOData[register] |= mask;
			}
		}
	}
}
