using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class HandBeam : Gesture {

    public List<Position> _GesturePositions;
    public Position _PositionTracked;
    
    // Property implementation
    public List<Position> GesturePositions
    {
        get
        {
            return _GesturePositions;
        }
    }
    public Position PositionTracked
    {
        get
        {
            return _PositionTracked;
        }

        set
        {
            _PositionTracked = value;
        }
    }
    
    // Refactor into BodySourceView (Refresh+Gesture tracker)
    public bool Gesture()
    {
        return false;
    }
}
