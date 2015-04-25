using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/* Position of the body as a pairing of joints and x/y coords.
 * Each position is a frame of gesture.
 */
public interface Position {

    Dictionary<Vector3, UnityJoint> JointPositions
    {
        get;
    }
}
