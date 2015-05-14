using UnityEngine;
using System.Collections.Generic;

public class HandsOnHead : Position
{
    public Dictionary<LigDir, Vector3> jointPositions
    {
        get
        {
            return new Dictionary<LigDir, Vector3>()
            {
                { LigDir.LeftUpperArm, new Vector3(-0.8246481f, 0.5656461f, 0) },
                { LigDir.LeftLowerArm, new Vector3(0f, 0f, 0) },
                { LigDir.LeftHand, new Vector3(0.8958837f, 0.4442886f, 0) },
                { LigDir.RightUpperArm, new Vector3(0.7573851f, 0.6529684f, 0) },
                { LigDir.RightLowerArm, new Vector3(0f, 0f, 0) },
                { LigDir.RightHand, new Vector3(-0.8720062f, 0.4894949f, 0) }
            };
        }
    }
}
