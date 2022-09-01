using Sres.Net.EEIP;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static Sres.Net.EEIP.EEIPClient;

namespace Sres.Net.EEIP
{
	public class SocketListener
	{
		private ConcurrentDictionary<uint, EEIPClient> _clients = new ConcurrentDictionary<uint, EEIPClient>();
		public EEIPClient this[uint i] => _clients.TryGetValue(i, out var client) ? client : null;
        System.Net.Sockets.UdpClient udpClientReceive;
        bool udpClientReceiveClosed = false;

		public IEnumerable<uint> GetClientIds() => _clients.Select(kvp => kvp.Key);

		/// <summary>
		/// Used Real-Time Format Target -> Originator for Implicit Messaging (Default: RealTimeFormat.Modeless)
		/// Possible Values: RealTimeFormat.Header32Bit; RealTimeFormat.Heartbeat; RealTimeFormat.ZeroLength; RealTimeFormat.Modeless
		/// </summary>
		public RealTimeFormat T_O_RealTimeFormat { get; set; } = RealTimeFormat.Modeless;

		public void CreateReceiveSocket(string ip)
		{
			//System.Net.IPAddress.TryParse("192.168.0.4", out var ip);
			System.Net.IPAddress.TryParse(ip, out var parsed);
			System.Net.IPEndPoint endPointReceive = new System.Net.IPEndPoint(parsed, 2222);
			udpClientReceive = new UdpClient(endPointReceive);
			UdpState udpState = new UdpState();
			udpState.e = endPointReceive;
			udpState.u = udpClientReceive;
			/*
			if (multicastAddress != 0)
			{
				System.Net.IPAddress multicast = (new System.Net.IPAddress(multicastAddress));
				udpClientReceive.JoinMulticastGroup(multicast);

			}
			*/
			var asyncResult = udpClientReceive.BeginReceive(new AsyncCallback(ReceiveCallback), udpState);
		}

		public void CloseSocket()
		{

            //Close the Socket for Receive
            udpClientReceiveClosed = true;
            udpClientReceive?.Close();
		}

        private void ReceiveCallback(IAsyncResult ar)
        {          
            UdpClient u = (UdpClient)((UdpState)(ar.AsyncState)).u;
            if (udpClientReceiveClosed)
                return;

            u.BeginReceive(new AsyncCallback(ReceiveCallback), (UdpState)ar.AsyncState);
            System.Net.IPEndPoint e = ((UdpState)ar.AsyncState).e;


            Byte[] receiveBytes = u.EndReceive(ar, ref e);

            // EndReceive worked and we have received data and remote endpoint

            if (receiveBytes.Length > 20)
            {
                //Get the connection ID
                uint connectionID = (uint)(receiveBytes[6] | receiveBytes[7] << 8 | receiveBytes[8] << 16 | receiveBytes[9] << 24);


                if (_clients.ContainsKey(connectionID))
                {
                    ushort headerOffset = 0;
                    if (T_O_RealTimeFormat == RealTimeFormat.Header32Bit)
                        headerOffset = 4;
                    if (T_O_RealTimeFormat == RealTimeFormat.Heartbeat)
                        headerOffset = 0;
					var offset = 20 + headerOffset;
					Array.Copy(receiveBytes, offset, _clients[connectionID].T_O_IOData, 0, receiveBytes.Length - offset);
					//Console.WriteLine(string.Join("-", _clients[connectionID].T_O_IOData.Take(receiveBytes.Length).Select(b => $"[{b:X2}]")));
					_clients[connectionID].LastReceivedImplicitMessage = DateTime.Now;
                    //for (int i = 0; i < receiveBytes.Length-20-headerOffset; i++)
                    //{
                    //    T_O_IOData[i] = receiveBytes[20 + i + headerOffset];
                    //}
                    //Console.WriteLine(T_O_IOData[0]);


                }
            }
        }

		public void RemoveClient(uint id)
		{
			if (_clients.ContainsKey(id))
			{
				_clients.TryRemove(id, out var eip);
			}
		}

		public bool TryAddClient(uint id, EEIPClient eip)
		{
			if (!_clients.ContainsKey(id))
			{
				return _clients.TryAdd(id, eip);
			}
			else return false;
		}
	}
}
