using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;

public class UnityJoint : MonoBehaviour {

    public LigDir ligament;
    public Vector3 direction;

    public UnityJoint(LigDir ligament, Vector3 direction)
    {
        this.ligament = ligament;
        this.direction = direction;
    }

    public bool Equals(UnityJoint joint)
    {
        if (joint.direction == this.direction && joint.ligament == this.ligament)
            return true;
        else
            return false;
    }

}
