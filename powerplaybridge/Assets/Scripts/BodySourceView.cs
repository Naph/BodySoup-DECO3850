using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;

public class BodySourceView : MonoBehaviour {

    public BodySourceManager bsm;
    public NetworkManager nm;

    private Dictionary<ulong, string> ulongReference;

    private static int current = 1;

    // Use this for initialization
	void Start () {
        ulongReference = new Dictionary<ulong, string>();
	}
	
	// Update is called once per frame
	void Update () {
        if (bsm == null)
        {
            return;
        }

        if (nm == null)
        {
            return;
        }

        if (nm.Connected == false)
        {
            return;
        }

        Dictionary<string, float[]> bodyDictionary = new Dictionary<string, float[]>();

        Kinect.Body[] bodies = bsm.GetData();

        foreach (var body in bodies)
        {
            if (body == null)
            {
                continue;
            }
            if (body.IsTracked)
            {
                string name = getReference(body.TrackingId);

                float[] positionData = new float[75];

                int pos = 0;

                for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
                {
                    positionData[pos++] = body.Joints[jt].Position.X;
                    positionData[pos++] = body.Joints[jt].Position.Y;
                    positionData[pos++] = body.Joints[jt].Position.Z;
                }

                bodyDictionary.Add(name, positionData);
            }
        }

        nm.SendBodyUpdate(bodyDictionary);
	}

    private string getReference(ulong newId)
    {
        if (ulongReference.ContainsKey(newId))
        {
            return ulongReference[newId];
        }

        ulongReference.Add(newId, GenerateNew());
        return ulongReference[newId];
    }

    private string GenerateNew()
    {
        int dividend = current;
        string name = string.Empty;
        int modulo;

        while (dividend > 0)
        {
            modulo = (dividend - 1) % 26;
            name = Convert.ToChar(65 + modulo).ToString() + name;
            dividend = (int)((dividend - modulo) / 26);
        }

        current++;

        return name;
    }
}