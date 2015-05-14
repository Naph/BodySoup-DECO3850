using UnityEngine;
using System.Collections.Generic;

public class ArmsHorizontal : Position
{
    public Dictionary<LigDir, Vector3> jointPositions
    {
        get
        {
            return new Dictionary<LigDir, Vector3>()
            {
                {LigDir.LeftUpperArm, new Vector3(-1.0f, 0f, 0f)},
                {LigDir.LeftLowerArm, new Vector3(0f, 0f, 0f)},
                {LigDir.LeftHand, new Vector3(-1.0f, 0f, 0f)},
                {LigDir.RightUpperArm, new Vector3(1f, 0f, 0f)},
                {LigDir.RightLowerArm, new Vector3(0f, 0f, 0f)},
                {LigDir.RightHand, new Vector3(1f, 0f, 0f)}
            };
        }
    }
}
