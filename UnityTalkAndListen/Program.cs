// UNITY TALK AND LISTEN
// This app sends data to a listener in Unity (UDPReceive.cs) and receives data from a talker in Unity (UDPSend.cs)
// Build this project 
// Drag UnityTalkAndListen.exe and HidDllDrt.dll into the Assets folder of your Unity project or the data folder of your build.
// Attach UDPReceive.cs to Main Camera to receive data from here
// Attach UDPSend.cs to Main Camera to send data to here

using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using HidDllDrt;

public class UnityTalkAndListen
{
    HidDllDrt.HidDll usbI = new HidDllDrt.HidDll("Vid_04d8&Pid_003f");
    uint[] sensors = new uint[28];
    public int talkPort;
    //static UdpClient talkClient;
    UdpClient listenClient;

    Thread receiveThread;
    public int listenPort;
    public string lastReceivedUDPPacket = "";

    private static void Main()
    {
        AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit); 
        UnityTalkAndListen sendObj = new UnityTalkAndListen();
        sendObj.init();
        UnityTalkAndListen receiveObj = new UnityTalkAndListen();
        receiveObj.init();
    }
    public void init()
    {
        listenPort = 8051;
        //receiveThread = new Thread(new ThreadStart(ReceiveData));
        //receiveThread.IsBackground = true;
        //receiveThread.Start();
        inputFromPIC();
    }

    #region SEND

    private void inputFromPIC()
    {
        try
        {
            string textToSend;
            do
            {
                textToSend = "";
                sensors = usbI.getSensors();
                var x = 0;
                foreach (var sensor in sensors)
                {
                    if (x == 0)
                        textToSend = sensor.ToString();
                    else if (x < 5)
                        textToSend += ("," + sensor.ToString());
                    x++;
                }
                x = 0;
                if (textToSend != "")
                {
                    sendMessage(textToSend);
                    Console.WriteLine(textToSend);
                }
                Thread.Sleep(10);
            } while (textToSend != "");
        }
        catch (Exception err)
        {
        }
    }

    void sendMessage (string message)
    {
        Socket sending_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
        ProtocolType.Udp);
        IPAddress send_to_address = IPAddress.Parse("10.0.0.4");
        IPEndPoint sending_end_point = new IPEndPoint(send_to_address, 8052);
        byte[] send_buffer = Encoding.ASCII.GetBytes(message);
        try
        {
            sending_socket.SendTo(send_buffer, sending_end_point);            
        }
        catch { }
    }

    #endregion

    #region RECEIVE

    private void ReceiveData()
    {
        listenClient = new UdpClient(listenPort);
        while (true)
        {
            List<byte> commandsToSend = new List<byte>();
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, listenPort);
                // receivedData is the information that UnityTalker.exe has sent (Data from microcontroller?)
                // I've made it comma delimited, so the variable splitText is just string array sent from Unity
                byte[] receivedData = listenClient.Receive(ref anyIP);
                string receivedText = Encoding.UTF8.GetString(receivedData);
                
                Console.WriteLine(receivedData[1].ToString());
                lastReceivedUDPPacket = receivedText;
                commandsToSend.Add(0x00);
                commandsToSend.Add(0x37);
                if (receivedData.Length > 0)
                {
                    for (var i = 2; i < (receivedData.Length + 2); i++)
                    {
                        commandsToSend.Add(receivedData[i - 2]);
                    }
                    outputToPIC(commandsToSend);
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(err.ToString());
            }
        }
    }

    private void outputToPIC(List<byte> _dataToSend)
    {
        //SEND DATA TO PIC HERE
    }

    public string getLatestUDPPacket()
    {
        return lastReceivedUDPPacket;
    }

    static void OnProcessExit(object sender, EventArgs e)
    {
        //talkClient.Close();
    }
    #endregion

}

