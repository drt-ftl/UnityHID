// Build Unity Talker and Unity Receiver projects. 
// Drag UnityListener.exe, UnityTalker.exe, and HidDllDrt.dll into the Assets folder of your Unity project.
// Attach this file to Main Camera to send data to UnityListener.exe
// Attach UDPReceive.cs to Main Camera to receive data from UnityTalker.exe

using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading;

public class UDPSend : MonoBehaviour
{
	private static int localPort;
	private string IP;
	public int port;
	IPEndPoint remoteEndPoint;
	UdpClient client;
	byte[] byteMessage = new byte[28];

	public void Start()
	{
		byteMessage [0] = 254;
		byteMessage [1] = 0x39;
		byteMessage [2] = 129;
		for (int i = 3; i < 28; i++)
		{
			byteMessage[i] = 0;
		}
		IP="10.0.0.2";
		port=8051;
		remoteEndPoint = new IPEndPoint(IPAddress.Any, port);
		client = new UdpClient(port);
	}

	void Update()
	{
		// strMessage is the data Unity will send out to UnityListener.exe
		// I've used comma delimited text, so UnityListener.exe will split it up on the other end.


		sendBytes(byteMessage);   
	}

	private void sendBytes(byte[] message)
	{
		try
		{
			client.Send(message, message.Length, remoteEndPoint);
		}
		catch {}
	}

	public void EndIt() 
	{ 
		client.Close();
		Thread.Sleep (1);
		Application.Quit ();
	} 
}