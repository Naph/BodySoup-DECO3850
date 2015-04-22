using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;

public class UnityJoint : MonoBehaviour {

    public JointType jointType;
    public TrackingState trackingState;

    public UnityJoint(JointType jointType)
    {
        this.jointType = jointType;
        trackingState = TrackingState.NotTracked;
    }

}
