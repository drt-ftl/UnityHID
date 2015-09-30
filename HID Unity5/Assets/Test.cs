using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		var client = new UdpClient();
		IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8052); // endpoint where server is listening
		client.Connect(ep);
		
		// send data
		client.Send(new byte[] { 1, 2, 3, 4, 5 }, 5);
		
		// then receive data
		var receivedData = client.Receive(ref ep);
		
		print("receive data from " + ep.ToString());
	}
}
