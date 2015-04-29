using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;

/* Position of the body as a pairing of joints and x/y coords.
 * Each position is a frame of gesture.
 */
public interface Position {

    Dictionary<LigDir, Vector3> jointPositions
    {
        get;
    }
}
