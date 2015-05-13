using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;
using KinectJoint = Windows.Kinect.Joint;
using System.Text;

public class UnityBody : MonoBehaviour {
    
    public Dictionary<LigDir, Vector3> jointPositions
    {
        get;
        set;
    }

    private GameObject bodyObject;
    
    public UnityBody(GameObject bodyObject)
    {
        this.bodyObject = bodyObject;

        // Local positions of left joints
        Vector3 ShoulderLeft = bodyObject.transform.FindChild("ShoulderLeft").localPosition;
        Vector3 ElbowLeft = bodyObject.transform.FindChild("ElbowLeft").localPosition;
        Vector3 WristLeft = bodyObject.transform.FindChild("ElbowLeft").localPosition;
        Vector3 HandLeft = bodyObject.transform.FindChild("HandLeft").localPosition;

        // Left Orientations
        Vector3 LeftUpperArm = (ElbowLeft - ShoulderLeft).normalized;
        Vector3 LeftLowerArm = (WristLeft - ElbowLeft).normalized;
        Vector3 LeftHand = (HandLeft - WristLeft).normalized;

        // Local positions of right joints
        Vector3 ShoulderRight = bodyObject.transform.FindChild("ShoulderRight").localPosition;
        Vector3 ElbowRight = bodyObject.transform.FindChild("ElbowRight").localPosition;
        Vector3 WristRight = bodyObject.transform.FindChild("ElbowRight").localPosition;
        Vector3 HandRight = bodyObject.transform.FindChild("HandRight").localPosition;

        // Right Orientations
        Vector3 RightUpperArm = (ElbowRight - ShoulderRight).normalized;
        Vector3 RightLowerArm = (WristRight - ElbowRight).normalized;
        Vector3 RightHand = (HandRight - WristRight).normalized;

        // Construct joints
        jointPositions = new Dictionary<LigDir, Vector3>()
        {
            {LigDir.LeftUpperArm, LeftUpperArm},
            {LigDir.LeftLowerArm, LeftLowerArm},
            {LigDir.LeftHand, LeftHand},
            {LigDir.RightUpperArm, RightUpperArm},
            {LigDir.RightLowerArm, RightLowerArm},
            {LigDir.RightHand, RightHand},
        };
    }


    public bool CompareJoints(Dictionary<LigDir, Vector3> position)
    {
        foreach (KeyValuePair<LigDir, Vector3> pair in this.jointPositions)
        {
            if (position[pair.Key] == null)
            {
                continue;
            }

            if (!pair.Value.Equals(position[pair.Key]))
            {
                return false;
            }
        }
        return true;
    }


    public string ToFileString()
    {
        string output = "";

        foreach (KeyValuePair<LigDir, Vector3> pair in jointPositions)
        {
            if (pair.Key != LigDir.RightHand)
            {
                output +=
                    "{ LigDir." + pair.Key + 
                    ", new Vector3(" + pair.Value.x + "f, " + pair.Value.y + "f, " + pair.Value.z + ") }," + Environment.NewLine;
            }
            else
            {
                output +=
                    "{ LigDir." + pair.Key +
                    ", new Vector3(" + pair.Value.x + "f, " + pair.Value.y + "f, " + pair.Value.z + ") }" + Environment.NewLine;
            }
        }

        return output;
    }
}
