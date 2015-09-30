// Build Unity Talker and Unity Receiver projects. 
// Drag UnityListener.exe, UnityTalker.exe, and HidDllDrt.dll into the Assets folder of your Unity project.
// Attach this file to Main Camera to receive data from UnityTalker.exe
// Attach UDPSend.cs to Main Camera to send data to UnityListener.exe

using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;

public class UDPReceive : MonoBehaviour 
{
	Worker worker;
	public bool running = false;
	static Process externalTalkListen;

    public void OnEnable()
	{	
		if (init())
		{
			//Thread.Sleep (0);
			running = true;
			worker = new Worker(this);
		}
	}

	static bool init()
	{
		externalTalkListen = new Process ();
		externalTalkListen.StartInfo.FileName = (Application.dataPath + "/UnityTalkAndListen.exe");
		externalTalkListen.Start ();
		return true;
	}

	void EndIt() 
	{ 
		running = false;
		externalTalkListen.Kill ();
		worker.RequestStop ();
		//Camera.main.GetComponent<UDPSend> ().EndIt ();
	} 

	void OnGUI()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			EndIt ();
		}
	}

	public List<string> getLatestUDPPacket()
	{
        var str = worker.GetLastPacket();
        var strSpl = str.Split(',');
        var strings = new List<string>();
        foreach (var chunk in strSpl)
        {
            float val;
            if (float.TryParse(chunk, out val))
               strings.Add(val.ToString("f0"));
        }
        return strings;
	}
}