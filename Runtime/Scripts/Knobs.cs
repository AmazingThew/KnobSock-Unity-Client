using System;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using System.Net;

namespace Thew.KnobSock
{
	public class Knobs
	{
		public const int NUM_KNOBS = 64;

		public enum ConnectionState
		{
			DISCONNECTED,
			CONNECTING,
			CONNECTED,
		}

		public static ConnectionState connectionState = ConnectionState.DISCONNECTED;

		public static int[] knobs = new int[NUM_KNOBS];

		public static bool knobsUpdated;

		static Socket _sock;

		static Thread _worker;

		public static void Connect()
		{
			if (connectionState != ConnectionState.DISCONNECTED)
				return;

			SocketAsyncEventArgs args = new SocketAsyncEventArgs();
			args.RemoteEndPoint = new IPEndPoint(IPAddress.Loopback, 8008);
			args.Completed += ConnectionCallback;

			try
			{
				_sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				if (_sock.ConnectAsync(args))
				{
					connectionState = ConnectionState.CONNECTING;
				}
				else
				{
					Debug.LogError($"ConnectAsync failed: {args.SocketError}");
					connectionState = ConnectionState.DISCONNECTED;
				}
			}
			catch (Exception e)
			{
				Debug.LogError($"Exception connecting to KnobServer: {e}");
				connectionState = ConnectionState.DISCONNECTED;
				throw;
			}
		}

		private static void ConnectionCallback(object sender, SocketAsyncEventArgs e)
		{
			if (e.SocketError == SocketError.Success)
			{
				_worker = new Thread(new ThreadStart(ReadFromServer));
				_worker.Start();

				connectionState = ConnectionState.CONNECTED;
			}
			else
			{
				Debug.LogError($"KnobServer connection failed: {e.SocketError}");
				connectionState = ConnectionState.DISCONNECTED;
			}
		}

		public static void Disconnect()
		{
			connectionState = ConnectionState.DISCONNECTED;
			_sock.Close();
		}

		public static bool HasNewValues()
		{
			bool updated = knobsUpdated;
			knobsUpdated = false;
			return updated;
		}

		public static float Get(int index)
		{
			return Get(index, 0, 1);
		}

		public static float Get(int index, float max)
		{
			return Get(index, 0, max);
		}

		public static float Get(int index, float min, float max)
		{
			Connect();

			float value = (float) knobs[index] / 127.0f;

			return (1 - value) * min + value * max;
		}


		static void ReadFromServer()
		{
			byte[] sockBuffer = new byte[NUM_KNOBS];

			try
			{
				while (connectionState == ConnectionState.CONNECTED)
				{
					if (_sock.Receive(sockBuffer, NUM_KNOBS, SocketFlags.None) > 0)
					{
						for (int i = 0; i < NUM_KNOBS; i++)
						{
							knobs[i] = sockBuffer[i];
						}
					}
					else
					{
						Debug.LogError("Socket connection lost; aborting");
						Disconnect();
						break;
					}

					knobsUpdated = true;
				}
			}
			catch (SocketException e)
			{
				Debug.LogError(e);
				Disconnect();
			}
		}
	}
}