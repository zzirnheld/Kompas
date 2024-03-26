using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Godot;
using Newtonsoft.Json;

namespace Kompas.Networking
{
	/// <summary>
	/// Handles serializing, sending, deserializing, and receiving packets.
	/// Usage: Call Update() each frame, or each interval at which you want to check packets
	/// </summary>
	public abstract class Networker
	{
		public const int port = 8448;

		public readonly Queue<(string, string)> packets = new();

		private bool awaitingInt = true;
		private int numBytesToRead;
		private int numBytesRead = 0;
		private byte[] bytesRead = new byte[sizeof(int)];

		private readonly TcpClient tcpClient;

		protected Networker(TcpClient tcpClient)
		{
			this.tcpClient = tcpClient;
		}

		public abstract Task ProcessPacket((string command, string json) packetInfo);

		public virtual Task Tick()
		{
			if (tcpClient == null || !tcpClient.Connected) return Task.CompletedTask;
			NetworkStream networkStream = tcpClient.GetStream();
			//if there's nothing to be read, return
			if (networkStream == null || !networkStream.DataAvailable) return Task.CompletedTask;

			if (awaitingInt) ReadInt(networkStream);
			else ReadPacket(networkStream);

			return Task.CompletedTask;
		}

		#region serialization
		protected static byte[] Serialize(Packet packet)
		{
			string json = JsonConvert.SerializeObject(packet, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
			byte[] bytes = Encoding.UTF8.GetBytes(json);
			return bytes;
		}

		protected static (string, string) Deserialize(byte[] bytes)
		{
			//TODO consider alternate encoding to handle +- and chars like it if those stop working
			string json = Encoding.UTF8.GetString(bytes);
			try
			{
				Packet? p = JsonConvert.DeserializeObject<Packet>(json);
				return (p?.command ?? Packet.Invalid, json);
			}
			catch (System.ArgumentException argEx)
			{
				//Catch JSON parse error
				Logger.Err($"Failed to deserialize packet from json \"{json}\", " +
					$"argument exception with message {argEx.Message}");
				return (Packet.Invalid, string.Empty);
			}
		}
		#endregion serialization

		#region writing
		public void SendPacket(Packet? packet)
		{
			if (packet == null) return;
			if (tcpClient == null) return;

			NetworkStream networkStream = tcpClient.GetStream();
			// we won't use a binary writer, because the endianness is unhelpful

			// turn the string message into a byte[] (encode)
			byte[] messageBytes = Serialize(packet);
			// determine length of message
			int length = messageBytes.Length;

			// convert the length into bytes using BitConverter (encode)
			byte[] lengthBytes = System.BitConverter.GetBytes(length);

			// flip the bytes if we are a little-endian system: reverse the bytes in lengthBytes to do so
			if (System.BitConverter.IsLittleEndian) System.Array.Reverse(lengthBytes);

			// send length
			networkStream.Write(lengthBytes, 0, lengthBytes.Length);
			// send message
			networkStream.Write(messageBytes, 0, length);
		}
		#endregion writing

		#region reading
		private void ReadInt(NetworkStream networkStream)
		{
			numBytesRead += networkStream.Read(bytesRead, numBytesRead, sizeof(int) - numBytesRead);
			if (numBytesRead == sizeof(int))
			{
				//if this system is little-endian, reverse the bytes read
				if (System.BitConverter.IsLittleEndian) System.Array.Reverse(bytesRead);
				// get length from bytes
				numBytesToRead = System.BitConverter.ToInt32(bytesRead, 0);
				awaitingInt = false;
				bytesRead = new byte[numBytesToRead];
				numBytesRead = 0;
			}
			else if (numBytesRead == 0)
			{
				throw new System.IO.IOException("Lost Connection during read");
			}
		}

		private void ReadPacket(NetworkStream networkStream)
		{
			numBytesRead += networkStream.Read(bytesRead, numBytesRead, numBytesToRead - numBytesRead);
			if (numBytesRead == numBytesToRead)
			{
				var (p, json) = Deserialize(bytesRead);
				if (p != Packet.Invalid) packets.Enqueue((p, json));
				awaitingInt = true;
				bytesRead = new byte[sizeof(int)];
				numBytesRead = 0;
			}
			else if (numBytesRead == 0)
			{
				throw new System.IO.IOException("Lost Connection during read");
			}
		}
		#endregion reading
	}
}