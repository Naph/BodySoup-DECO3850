using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public interface Gesture {

    List<Position> GesturePositions
    {
        get;
    }

    Position PositionTracked
    {
        get;
        set;
    }

    // y% Toleration for Vector3
    // Check for x time hold to trigger gesture
    bool Gesture();
}
