using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;

public class NetworkReader : MonoBehaviour {

    private TcpListener tcpListener;

    private NetworkStream ns;

    private bool connected = false;

    private int numBodies;
    private List<string> names;
    private List<float[]> floats;
    private int[] nameLengths;

    private byte[] nameBuffer;
    private int nameBufferOffset;

    private byte[] floatBuffer;
    private int floatBufferOffset;

    private List<string> validNames;
    private List<float[]> validFloats;
    private int validNumBodies;

    private bool reading = false;
    private bool readOnce = false;

	// Use this for initialization
	void Start () {
        tcpListener = new TcpListener(IPAddress.Any, 41724);
        tcpListener.Start();
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClient), tcpListener);
	}

    public int NumBodies
    {
        get { return validNumBodies; }
    }

    public List<string> Names
    {
        get { return validNames; }
    }

    public List<float[]> Floats
    {
        get { return validFloats; }
    }

    public bool ReadOnce
    {
        get
        {
            if (readOnce == true)
            {
                readOnce = false;
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    private void DoAcceptTcpClient(IAsyncResult iar)
    {
        TcpListener listener = (TcpListener)iar.AsyncState;
        TcpClient client = listener.EndAcceptTcpClient(iar);

        Debug.Log("Stream Connected");

        ns = client.GetStream();
        connected = true;
    }

	// Update is called once per frame
	void Update () {
        if (connected)
        {
            if (ns.DataAvailable && reading == false)
            {
                reading = true;
                byte[] numBodyData = new byte[1 * sizeof(int)];
                ns.Read(numBodyData, 0, numBodyData.Length);

                int[] numBodies = new int[1];
                Buffer.BlockCopy(numBodyData, 0, numBodies, 0, numBodyData.Length);

                if (numBodies[0] == 0)
                {
                    this.numBodies = 0;
                    names = null;
                    floats = null;
                    Debug.Log("No Bodies Seen");
                    reading = false;
                }
                else
                {
                    this.numBodies = numBodies[0];
                    this.names = new List<string>();
                    this.floats = new List<float[]>();

                    byte[] numCharsData = new byte[this.numBodies * sizeof(int)];
                    Debug.Log(numCharsData.Length + "sizeof numCharsData");
                    ns.Read(numCharsData, 0, numCharsData.Length);

                    int[] numChars = new int[this.numBodies];
                    Debug.Log(numCharsData);
                    Buffer.BlockCopy(numCharsData, 0, numChars, 0, numCharsData.Length);
                    Debug.Log("nameChars[0]: " + numChars[0]);
                    this.nameLengths = numChars;

                    this.nameBuffer = new byte[this.nameLengths[0]];

                    this.nameBufferOffset = 0;
                    ns.BeginRead(this.nameBuffer, 0, this.nameBuffer.Length, new AsyncCallback(DoEndReadName), 0);

                    /*
                    List<string> names = new List<string>();

                    for (int i = 0; i < numBodies[0]; i++)
                    {
                        byte[] nameData = new byte[numChars[i] * sizeof(char)];

                        ns.Read(nameData, 0, nameData.Length);

                        names.Add(GetString(nameData));

                        Debug.Log(names[i]);
                    }

                    List<float[]> floats = new List<float[]>();

                    for (int i = 0; i < numBodies[0]; i++)
                    {
                        byte[] floatByteData = new byte[75 * sizeof(float)];
                        ns.Read(floatByteData, 0, floatByteData.Length);

                        float[] floatData = new float[75];
                        Buffer.BlockCopy(floatByteData, 0, floatData, 0, floatByteData.Length);
                        floats.Add(floatData);
                    }

                    */
                }
            }
        }
	}

    private void DoEndReadName(IAsyncResult iar)
    {
        int namePos = (int)iar.AsyncState;
        int numRead = ns.EndRead(iar);

        if (numRead + this.nameBufferOffset == this.nameBuffer.Length)
        {
            this.names.Add(GetString(nameBuffer));
            Debug.Log("Name: " + this.names[this.names.Count - 1]);

            if (this.names.Count == this.numBodies)
            {
                Debug.Log("Starting float read");

                this.floatBuffer = new byte[75 * sizeof(float)];
                this.floatBufferOffset = 0;
                ns.BeginRead(this.floatBuffer, 0, this.floatBuffer.Length, new AsyncCallback(DoEndReadFloats), 0);
            }
            else
            {
                this.nameBuffer = new byte[this.nameLengths[namePos + 1]];
                this.nameBufferOffset = 0;
                ns.BeginRead(this.nameBuffer, 0, this.nameBuffer.Length, new AsyncCallback(DoEndReadName), namePos + 1);
            }
        }
        else
        {
            this.nameBufferOffset += numRead;
            ns.BeginRead(this.nameBuffer, this.nameBufferOffset, this.nameBuffer.Length - this.nameBufferOffset, new AsyncCallback(DoEndReadName), namePos);
        }
    }

    private void DoEndReadFloats(IAsyncResult iar)
    {
        int floatPos = (int)iar.AsyncState;
        int numRead = ns.EndRead(iar);
        Debug.Log("EndFloatRead: " + numRead);
        if (numRead + this.floatBufferOffset == this.floatBuffer.Length)
        {
            Debug.Log("Yes");
            float[] floatNums = new float[75];
            Buffer.BlockCopy(this.floatBuffer, 0, floatNums, 0, this.floatBuffer.Length);

            this.floats.Add(floatNums);

            if (this.floats.Count == this.numBodies)
            {
                Debug.Log(this.floats[0][0] + " " + this.floats[0][1] + " " + this.floats[0][2]);

                this.validNumBodies = this.numBodies;
                this.validNames = this.names;
                this.validFloats = this.floats;
                this.readOnce = true;

                reading = false;
                return;
            }
            else
            {
                this.floatBuffer = new byte[75 * sizeof(float)];
                this.floatBufferOffset = 0;
                ns.BeginRead(this.floatBuffer, 0, this.floatBuffer.Length, new AsyncCallback(DoEndReadFloats), floatPos + 1);
            }
        }
        else
        {
            Debug.Log("No");
            this.floatBufferOffset += numRead;
            ns.BeginRead(this.floatBuffer, this.floatBufferOffset, this.floatBuffer.Length - this.floatBufferOffset, new AsyncCallback(DoEndReadFloats), floatPos);
        }
    }

    static string GetString(byte[] bytes)
    {
        Debug.Log(bytes.Length + ": " + (bytes.Length / sizeof(char)));
        char[] chars = new char[bytes.Length / sizeof(char)];
        System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
        return new string(chars);
    }
}
