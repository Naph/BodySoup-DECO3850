using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class RightArmUp : Position {

    private Dictionary<Vector3, UnityJoint> _JointPositions;

    // Property implementation
    public Dictionary<Vector3, UnityJoint> JointPositions
    {
        get
        {
            return _JointPositions;
        }
    }
}
