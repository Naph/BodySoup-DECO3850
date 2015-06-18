using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;

public class NetworkManager : MonoBehaviour {

    public GameObject ConnectPanel;
    public InputField ipField;
    public InputField portField;

    private TcpClient tcpClient;
    private NetworkStream ns;

    private Dictionary<string, float[]> dataToSend;

    private bool connected = false;

    private bool upToDate = true;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (connected)
        {
            if (ns.DataAvailable)
            {
                ns.ReadByte();
                upToDate = true;
            }
        }
	}

    public void Connect()
    {
        string host = ipField.text;
        int port = int.Parse(portField.text);

        tcpClient = new TcpClient();
        tcpClient.Connect(host, port);

        ns = tcpClient.GetStream();
        ConnectPanel.SetActive(false);
        connected = true;
    }

    public bool Connected
    {
        get { return connected; }
    }

    public void SendBodyUpdate(Dictionary<string, float[]> bodyData)
    {
        if (dataToSend == null && upToDate == true)
        {
            dataToSend = bodyData;
            upToDate = false;
        }
        else
        {
            return;
        }

        Debug.Log("SendData");

        int[] bodyCount = new int[1];

        bodyCount[0] = bodyData.Count;

        byte[] byteBodyData = new byte[1 * sizeof(int)];
        Buffer.BlockCopy(bodyCount, 0, byteBodyData, 0, byteBodyData.Length);

        if (bodyData.Count == 0)
        {
            ns.BeginWrite(byteBodyData, 0, byteBodyData.Length, new AsyncCallback(DoEndSendData), ns);
        }
        else
        {
            List<KeyValuePair<string, float[]>> values = new List<KeyValuePair<string,float[]>>();
            int[] stringSize = new int[bodyData.Count];


            foreach (KeyValuePair<string, float[]> body in bodyData)
            {
                //Debug.Log("size of name: " + values.Count + ": " + body.Key.Length * sizeof(char));
                stringSize[values.Count] = body.Key.Length * sizeof(char);
                //Debug.Log(stringSize[values.Count] + " namesSize");
                values.Add(body);
            }

            byte[] byteStringSize = new byte[stringSize.Length * sizeof(int)];
            Buffer.BlockCopy(stringSize, 0, byteStringSize, 0, byteStringSize.Length);
            
            byte[] byteData;

            switch (bodyData.Count)
            {
                case 1:
                    //Debug.Log("1 person");
                    //Debug.Log(values[0].Key + ": " + values[0].Value);
                    byteData = Combine(values[0]);
                    break;
                case 2:
                    byteData = Combine(values[0], values[1]);
                    break;
                case 3:
                    byteData = Combine(values[0], values[1], values[2]);
                    break;
                case 4:
                    byteData = Combine(values[0], values[1], values[2], values[3]);
                    break;
                case 5:
                    byteData = Combine(values[0], values[1], values[2], values[3], values[4]);
                    break;
                case 6:
                    byteData = Combine(values[0], values[1], values[2], values[3], values[4], values[5]);
                    break;
                default:
                    byteData = new byte[1];
                    break;
            }

            byte[] writing = Combine(byteBodyData, byteStringSize, byteData);

            //Debug.Log("Send Body - " + bodyCount[0]);




            ns.BeginWrite(writing, 0, writing.Length, new AsyncCallback(DoEndSendData), ns);

            //ns.Write(writing, 0, writing.Length);
            //dataToSend = null;
        }

    }

    private static byte[] Combine(byte[] first, byte[] second, byte[] third)
    {
        byte[] ret = new byte[first.Length + second.Length + third.Length];
        Buffer.BlockCopy(first, 0, ret, 0, first.Length);
        Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
        Buffer.BlockCopy(third, 0, ret, first.Length + second.Length, third.Length);
        return ret;
    }

    private static byte[] Combine(KeyValuePair<string, float[]> v1)
    {
        byte[] nameBytes = GetBytes(v1.Key);
        byte[] floatBytes = GetBytes(v1.Value);

        byte[] ret = new byte[nameBytes.Length + floatBytes.Length];
        Buffer.BlockCopy(nameBytes, 0, ret, 0, nameBytes.Length);
        Buffer.BlockCopy(floatBytes, 0, ret, nameBytes.Length, floatBytes.Length);

        return ret;
    }

    private static byte[] Combine(KeyValuePair<string, float[]> v1, KeyValuePair<string, float[]> v2)
    {
        byte[] nameBytes1 = GetBytes(v1.Key);
        byte[] floatBytes1 = GetBytes(v1.Value);
        byte[] nameBytes2 = GetBytes(v2.Key);
        byte[] floatBytes2 = GetBytes(v2.Value);

        byte[] ret = new byte[(nameBytes1.Length + nameBytes2.Length) + floatBytes1.Length * 2];
        Buffer.BlockCopy(nameBytes1, 0, ret, 0, nameBytes1.Length);
        Buffer.BlockCopy(nameBytes2, 0, ret, nameBytes1.Length, nameBytes2.Length);
        Buffer.BlockCopy(floatBytes1, 0, ret, nameBytes1.Length + nameBytes2.Length, floatBytes1.Length);
        Buffer.BlockCopy(floatBytes2, 0, ret, nameBytes1.Length + nameBytes2.Length + floatBytes1.Length, floatBytes2.Length);

        return ret;
    }

    private static byte[] Combine(KeyValuePair<string, float[]> v1, KeyValuePair<string, float[]> v2, KeyValuePair<string, float[]> v3)
    {
        byte[] nameBytes1 = GetBytes(v1.Key);
        byte[] floatBytes1 = GetBytes(v1.Value);
        byte[] nameBytes2 = GetBytes(v2.Key);
        byte[] floatBytes2 = GetBytes(v2.Value);
        byte[] nameBytes3 = GetBytes(v3.Key);
        byte[] floatBytes3 = GetBytes(v3.Value);

        byte[] ret = new byte[(nameBytes1.Length + nameBytes2.Length + nameBytes3.Length) + floatBytes1.Length * 3];
        Buffer.BlockCopy(nameBytes1, 0, ret, 0, nameBytes1.Length);
        Buffer.BlockCopy(nameBytes2, 0, ret, nameBytes1.Length, nameBytes2.Length);
        Buffer.BlockCopy(nameBytes3, 0, ret, nameBytes1.Length + nameBytes2.Length, nameBytes3.Length);
        Buffer.BlockCopy(floatBytes1, 0, ret, nameBytes1.Length + nameBytes2.Length + nameBytes3.Length, floatBytes1.Length);
        Buffer.BlockCopy(floatBytes2, 0, ret, nameBytes1.Length + nameBytes2.Length + nameBytes3.Length + floatBytes1.Length, floatBytes2.Length);
        Buffer.BlockCopy(floatBytes3, 0, ret, nameBytes1.Length + nameBytes2.Length + nameBytes3.Length + floatBytes1.Length + floatBytes2.Length, floatBytes3.Length);

        return ret;
    }

    private static byte[] Combine(KeyValuePair<string, float[]> v1, KeyValuePair<string, float[]> v2, KeyValuePair<string, float[]> v3, KeyValuePair<string, float[]> v4)
    {
        byte[] nameBytes1 = GetBytes(v1.Key);
        byte[] floatBytes1 = GetBytes(v1.Value);
        byte[] nameBytes2 = GetBytes(v2.Key);
        byte[] floatBytes2 = GetBytes(v2.Value);
        byte[] nameBytes3 = GetBytes(v3.Key);
        byte[] floatBytes3 = GetBytes(v3.Value);
        byte[] nameBytes4 = GetBytes(v4.Key);
        byte[] floatBytes4 = GetBytes(v4.Value);

        byte[] ret = new byte[(nameBytes1.Length + nameBytes2.Length + nameBytes3.Length + nameBytes4.Length) + floatBytes1.Length * 4];
        Buffer.BlockCopy(nameBytes1, 0, ret, 0, nameBytes1.Length);
        Buffer.BlockCopy(nameBytes2, 0, ret, nameBytes1.Length, nameBytes2.Length);
        Buffer.BlockCopy(nameBytes3, 0, ret, nameBytes1.Length + nameBytes2.Length, nameBytes3.Length);
        Buffer.BlockCopy(nameBytes4, 0, ret, nameBytes1.Length + nameBytes2.Length + nameBytes3.Length, nameBytes4.Length);
        Buffer.BlockCopy(floatBytes1, 0, ret, nameBytes1.Length + nameBytes2.Length + nameBytes3.Length + nameBytes4.Length, floatBytes1.Length);
        Buffer.BlockCopy(floatBytes2, 0, ret, nameBytes1.Length + nameBytes2.Length + nameBytes3.Length + nameBytes4.Length + floatBytes1.Length, floatBytes2.Length);
        Buffer.BlockCopy(floatBytes3, 0, ret, nameBytes1.Length + nameBytes2.Length + nameBytes3.Length + nameBytes4.Length + floatBytes1.Length + floatBytes2.Length, floatBytes3.Length);
        Buffer.BlockCopy(floatBytes4, 0, ret, nameBytes1.Length + nameBytes2.Length + nameBytes3.Length + nameBytes4.Length + floatBytes1.Length + floatBytes2.Length + floatBytes3.Length, floatBytes4.Length);

        return ret;
    }

    private static byte[] Combine(KeyValuePair<string, float[]> v1, KeyValuePair<string, float[]> v2, KeyValuePair<string, float[]> v3, KeyValuePair<string, float[]> v4, KeyValuePair<string, float[]> v5)
    {
        byte[] nameBytes1 = GetBytes(v1.Key);
        byte[] floatBytes1 = GetBytes(v1.Value);
        byte[] nameBytes2 = GetBytes(v2.Key);
        byte[] floatBytes2 = GetBytes(v2.Value);
        byte[] nameBytes3 = GetBytes(v3.Key);
        byte[] floatBytes3 = GetBytes(v3.Value);
        byte[] nameBytes4 = GetBytes(v4.Key);
        byte[] floatBytes4 = GetBytes(v4.Value);
        byte[] nameBytes5 = GetBytes(v5.Key);
        byte[] floatBytes5 = GetBytes(v5.Value);

        byte[] ret = new byte[(nameBytes1.Length + nameBytes2.Length + nameBytes3.Length + nameBytes4.Length + nameBytes5.Length) + floatBytes1.Length * 5];
        Buffer.BlockCopy(nameBytes1, 0, ret, 0, nameBytes1.Length);
        Buffer.BlockCopy(nameBytes2, 0, ret, nameBytes1.Length, nameBytes2.Length);
        Buffer.BlockCopy(nameBytes3, 0, ret, nameBytes1.Length + nameBytes2.Length, nameBytes3.Length);
        Buffer.BlockCopy(nameBytes4, 0, ret, nameBytes1.Length + nameBytes2.Length + nameBytes3.Length, nameBytes4.Length);
        Buffer.BlockCopy(nameBytes5, 0, ret, nameBytes1.Length + nameBytes2.Length + nameBytes3.Length + nameBytes4.Length, nameBytes5.Length);
        Buffer.BlockCopy(floatBytes1, 0, ret, nameBytes1.Length + nameBytes2.Length + nameBytes3.Length + nameBytes4.Length + nameBytes5.Length, floatBytes1.Length);
        Buffer.BlockCopy(floatBytes2, 0, ret, nameBytes1.Length + nameBytes2.Length + nameBytes3.Length + nameBytes4.Length + nameBytes5.Length + floatBytes1.Length, floatBytes2.Length);
        Buffer.BlockCopy(floatBytes3, 0, ret, nameBytes1.Length + nameBytes2.Length + nameBytes3.Length + nameBytes4.Length + nameBytes5.Length + floatBytes1.Length + floatBytes2.Length, floatBytes3.Length);
        Buffer.BlockCopy(floatBytes4, 0, ret, nameBytes1.Length + nameBytes2.Length + nameBytes3.Length + nameBytes4.Length + nameBytes5.Length + floatBytes1.Length + floatBytes2.Length + floatBytes3.Length, floatBytes4.Length);
        Buffer.BlockCopy(floatBytes4, 0, ret, nameBytes1.Length + nameBytes2.Length + nameBytes3.Length + nameBytes4.Length + nameBytes5.Length + floatBytes1.Length + floatBytes2.Length + floatBytes3.Length + floatBytes4.Length, floatBytes5.Length);

        return ret;
    }

    private static byte[] Combine(KeyValuePair<string, float[]> v1, KeyValuePair<string, float[]> v2, KeyValuePair<string, float[]> v3, KeyValuePair<string, float[]> v4, KeyValuePair<string, float[]> v5, KeyValuePair<string, float[]> v6)
    {
        byte[] nameBytes1 = GetBytes(v1.Key);
        byte[] floatBytes1 = GetBytes(v1.Value);
        byte[] nameBytes2 = GetBytes(v2.Key);
        byte[] floatBytes2 = GetBytes(v2.Value);
        byte[] nameBytes3 = GetBytes(v3.Key);
        byte[] floatBytes3 = GetBytes(v3.Value);
        byte[] nameBytes4 = GetBytes(v4.Key);
        byte[] floatBytes4 = GetBytes(v4.Value);
        byte[] nameBytes5 = GetBytes(v5.Key);
        byte[] floatBytes5 = GetBytes(v5.Value);
        byte[] nameBytes6 = GetBytes(v6.Key);
        byte[] floatBytes6 = GetBytes(v6.Value);

        byte[] ret = new byte[(nameBytes1.Length + nameBytes2.Length + nameBytes3.Length + nameBytes4.Length + nameBytes5.Length + nameBytes6.Length) + floatBytes1.Length * 6];
        Buffer.BlockCopy(nameBytes1, 0, ret, 0, nameBytes1.Length);
        Buffer.BlockCopy(nameBytes2, 0, ret, nameBytes1.Length, nameBytes2.Length);
        Buffer.BlockCopy(nameBytes3, 0, ret, nameBytes1.Length + nameBytes2.Length, nameBytes3.Length);
        Buffer.BlockCopy(nameBytes4, 0, ret, nameBytes1.Length + nameBytes2.Length + nameBytes3.Length, nameBytes4.Length);
        Buffer.BlockCopy(nameBytes5, 0, ret, nameBytes1.Length + nameBytes2.Length + nameBytes3.Length + nameBytes4.Length, nameBytes5.Length);
        Buffer.BlockCopy(nameBytes5, 0, ret, nameBytes1.Length + nameBytes2.Length + nameBytes3.Length + nameBytes4.Length + nameBytes5.Length, nameBytes6.Length);
        Buffer.BlockCopy(floatBytes1, 0, ret, nameBytes1.Length + nameBytes2.Length + nameBytes3.Length + nameBytes4.Length + nameBytes5.Length + nameBytes6.Length, floatBytes1.Length);
        Buffer.BlockCopy(floatBytes2, 0, ret, nameBytes1.Length + nameBytes2.Length + nameBytes3.Length + nameBytes4.Length + nameBytes5.Length + nameBytes6.Length + floatBytes1.Length, floatBytes2.Length);
        Buffer.BlockCopy(floatBytes3, 0, ret, nameBytes1.Length + nameBytes2.Length + nameBytes3.Length + nameBytes4.Length + nameBytes5.Length + nameBytes6.Length + floatBytes1.Length + floatBytes2.Length, floatBytes3.Length);
        Buffer.BlockCopy(floatBytes4, 0, ret, nameBytes1.Length + nameBytes2.Length + nameBytes3.Length + nameBytes4.Length + nameBytes5.Length + nameBytes6.Length + floatBytes1.Length + floatBytes2.Length + floatBytes3.Length, floatBytes4.Length);
        Buffer.BlockCopy(floatBytes4, 0, ret, nameBytes1.Length + nameBytes2.Length + nameBytes3.Length + nameBytes4.Length + nameBytes5.Length + nameBytes6.Length + floatBytes1.Length + floatBytes2.Length + floatBytes3.Length + floatBytes4.Length, floatBytes5.Length);
        Buffer.BlockCopy(floatBytes4, 0, ret, nameBytes1.Length + nameBytes2.Length + nameBytes3.Length + nameBytes4.Length + nameBytes5.Length + nameBytes6.Length + floatBytes1.Length + floatBytes2.Length + floatBytes3.Length + floatBytes4.Length, floatBytes6.Length);

        return ret;
    }

    private void DoEndSendData(IAsyncResult iar)
    {
        NetworkStream ns = (NetworkStream)iar.AsyncState;
        ns.EndWrite(iar);

        dataToSend = null;
    }

    static byte[] GetBytes(float[] floats)
    {
        byte[] bytes = new byte[floats.Length * sizeof(float)];
        System.Buffer.BlockCopy(floats, 0, bytes, 0, bytes.Length);
        return bytes;
    }

    static byte[] GetBytes(string str)
    {
        byte[] bytes = new byte[str.Length * sizeof(char)];
        System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
        return bytes;
    }

    static string GetString(byte[] bytes)
    {
        char[] chars = new char[bytes.Length / sizeof(char)];
        System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
        return new string(chars);
    }

    void OnApplicationQuit()
    {
        if (ns != null)
        {
            ns.Close();
            ns = null;
        }
        if (tcpClient != null)
        {
            if (tcpClient.Connected)
            {
                tcpClient.Close();
            }
        }
    }
}
