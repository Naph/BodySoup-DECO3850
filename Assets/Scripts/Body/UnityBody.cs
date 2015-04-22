using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;

public class UnityBody : MonoBehaviour {
    public ulong trackingId;

    public Dictionary<int, UnityJoint> joints;

    public bool isTracked;

    public UnityBody()
    {
        joints = new Dictionary<int, UnityJoint>();
        for (int i = 0; i < 25; i++)
        {
            joints[i] = new UnityJoint((JointType)i);
        }

        isTracked = false;
    }
}
