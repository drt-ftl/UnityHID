using System;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;

public class Worker
{

	public static UdpClient listener;
	private string IP = "10.0.0.2";
	public static string dataString = "N";
	public static int i = 0;
	public static bool shouldStop = false;
	Thread thread;
	private const int listenPort = 8052;

	public Worker(UDPReceive _udpReceive)
	{
		thread = new Thread (DoWork);
		thread.Start ();
	}

	static void DoWork()
	{
		listener = new UdpClient (listenPort);
		var groupEP = new IPEndPoint(IPAddress.Any, listenPort);
		string received_data;
		byte[] receive_byte_array;

		while (!shouldStop)
		{
			try
			{
				receive_byte_array = listener.Receive(ref groupEP);
				received_data = Encoding.ASCII.GetString(receive_byte_array, 0, receive_byte_array.Length);
				dataString = received_data;
			}
			catch
			{
			}
            Thread.Sleep(0);
		}
	}

	public string GetLastPacket()
	{
		return dataString;
	}

	public void RequestStop ()
	{
		shouldStop = true;
		thread.Abort ();
		listener.Close ();
	}
}