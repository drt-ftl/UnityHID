using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class guiWindow : MonoBehaviour
{
    public GUISkin skin;
    Rect rect;
    Rect innerRect;
    UDPReceive udpR;

    void Start()
    {
        rect = new Rect(5, 5, 250, 570);
        innerRect = new Rect(5, 30, 240, 535);
        udpR = Camera.main.GetComponent<UDPReceive>();
    }
	void OnGUI()
    {
        GUI.skin = skin;
        GUI.Window(0, rect, Display, "Inspector");
    }

    void Display(int _ii)
    {
        GUILayout.BeginArea(innerRect);
        {
            foreach (var str in udpR.getLatestUDPPacket())
                GUILayout.Box("Last Packet: " + str);
            GUILayout.Box(Worker.i.ToString());
        }
        GUILayout.EndArea();
    }
}
