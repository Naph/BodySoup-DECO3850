using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;

public class UnityBody : MonoBehaviour {
    public ulong trackingId;

    public Dictionary<int, UnityJoint> joints;
    public Transform transform;
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

    public static Dictionary<JointType, List<JointType>> boneMap = new Dictionary<JointType, List<JointType>>()
    {
        
        //{ JointType.FootLeft, new List<JointType>(){JointType.AnkleLeft} },
        //{ JointType.AnkleLeft, JointType.KneeLeft },
        //{ JointType.KneeLeft, JointType.HipLeft },
        { JointType.HipLeft, new List<JointType>(){JointType.SpineBase} },
        
        //{ JointType.FootRight, new List<JointType>(){JointType.AnkleRight} },
        //{ JointType.AnkleRight, JointType.KneeRight },
        //{ JointType.KneeRight, JointType.HipRight },
        { JointType.HipRight, new List<JointType>(){JointType.SpineBase} },
        
        { JointType.HandTipLeft, new List<JointType>(){JointType.HandLeft} },
        { JointType.ThumbLeft, new List<JointType>(){JointType.HandLeft} },
        { JointType.HandLeft, new List<JointType>(){JointType.WristLeft} },
        //{ JointType.WristLeft, JointType.ElbowLeft },
        //{ JointType.ElbowLeft, JointType.ShoulderLeft },
        { JointType.ShoulderLeft, new List<JointType>(){JointType.SpineShoulder, JointType.HipLeft } },
        
        { JointType.HandTipRight, new List<JointType>(){JointType.HandRight} },
        { JointType.ThumbRight, new List<JointType>(){JointType.HandRight} },
        { JointType.HandRight, new List<JointType>(){JointType.WristRight} },
        //{ JointType.WristRight, JointType.ElbowRight },
        //{ JointType.ElbowRight, JointType.ShoulderRight },
        { JointType.ShoulderRight, new List<JointType>(){JointType.SpineShoulder, JointType.HipRight} },
        
        { JointType.SpineBase, new List<JointType>(){JointType.SpineMid} },
        { JointType.SpineMid, new List<JointType>(){JointType.SpineShoulder} },
        //{ JointType.SpineShoulder, JointType.Neck },
        { JointType.Neck, new List<JointType>(){JointType.Neck} }
    };
}
