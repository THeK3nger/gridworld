using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System;

public class GridWorldClient : MonoBehaviour
{
    public string Host = "127.0.0.1";
    public Int32 Port = 10253;

    private TcpClient m_Socket;
    private byte[] bytesReceived;
    private int bytes;

    NetworkStream theStream;
    StreamWriter theWriter;
    StreamReader theReader;
    private bool socketReady;

    private GameObject bot;
    private NetDeliberator netDelib;

    void Awake()
    {
        try
        {
            m_Socket = new TcpClient(Host, Port);
            theStream = m_Socket.GetStream();
            theWriter = new StreamWriter(theStream);
            theReader = new StreamReader(theStream);
            socketReady = true;
        }
        catch (Exception e)
        {
            Debug.Log("Socket error: " + e);
        }
        //InvokeRepeating("emptySocket", 3, 1);
    }

    void Update()
    {
        if (bot == null)
        {
            bot = GameObject.Find("BotDiag(Clone)");
            if (bot == null) return;
            netDelib = bot.GetComponent<NetDeliberator>();
        }
        string receivedText = readSocket();
        if (receivedText != "")
        {
            string[] foo = receivedText.Split('(');
            string[] bar = foo[1].Split(',');
            float x = float.Parse(bar[0]);
            float y = float.Parse(bar[1].Remove(bar[1].Length-1));
            netDelib.newPosition(x, y);
        }
    }

    public String readSocket()
    {
        if (!socketReady)
            return "";
        if (theStream.DataAvailable)
            return theReader.ReadLine();
        return "";
    }

    public void writeSocket(string theLine)
    {
        if (!socketReady)
            return;
        String foo = theLine + "\r\n";
        theWriter.Write(foo);
        theWriter.Flush();
    }

    public void emptySocket()
    {

    }

    void OnApplicationQuit()
    {
        if (!socketReady)
            return;
        theWriter.Close();
        theReader.Close();
        m_Socket.Close();
        socketReady = false;
    }

}
