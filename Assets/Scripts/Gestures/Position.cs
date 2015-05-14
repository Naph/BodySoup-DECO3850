using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;

/**
 * 
 */
public class Position {

    public static Dictionary<LigDir, Vector3> HandsOnHead =
        new Dictionary<LigDir, Vector3>()
    {
        { LigDir.LeftUpperArm,  new Vector3(-0.6963681f, 0.7176848f, 0f) },
        { LigDir.LeftLowerArm,  new Vector3(0.8570663f, 0.5152063f, 0f) },
        { LigDir.LeftHand,      new Vector3(0.9936339f, 0.1126572f, 0f) },
        { LigDir.RightUpperArm, new Vector3(0.6964172f, 0.7176371f, 0f) },
        { LigDir.RightLowerArm, new Vector3(-0.8371778f, 0.5469309f, 0f) },
        { LigDir.RightHand,     new Vector3(-0.9806817f, 0.1956107f, 0f) }
    };

    public static Dictionary<LigDir, Vector3> TPose =
        new Dictionary<LigDir, Vector3>()
    {
        { LigDir.LeftUpperArm,  new Vector3(-0.9980345f, -0.06266659f, 0f) },
        { LigDir.LeftLowerArm,  new Vector3(-0.9980772f, -0.06198315f, 0f) },
        { LigDir.LeftHand,      new Vector3(-0.9832519f, 0.1822518f, 0f) },
        { LigDir.RightUpperArm, new Vector3(0.9935167f, -0.1136861f, 0f) },
        { LigDir.RightLowerArm, new Vector3(0.9999539f, 0.009609748f, 0f) },
        { LigDir.RightHand,     new Vector3(0.9619886f, 0.2730895f, 0f) }
    };

    public static Dictionary<LigDir, Vector3> RightArmCrossOverHeart =
        new Dictionary<LigDir, Vector3>()
    {  
		{ LigDir.LeftUpperArm,  new Vector3(-0.1799714f, -0.9836719f, 0f) },
		{ LigDir.LeftLowerArm,  new Vector3(-0.1238129f, -0.9923055f, 0f) },
		{ LigDir.LeftHand,      new Vector3(0.05572656f, -0.998446f, 0f) },
		{ LigDir.RightUpperArm, new Vector3(-0.1484051f, -0.9889266f, 0f) },
		{ LigDir.RightLowerArm, new Vector3(-0.9436026f, 0.3310801f, 0f) },
		{ LigDir.RightHand,     new Vector3(-0.5363953f, 0.8439669f, 0f) }
    };

    public static Dictionary<LigDir, Vector3> RightArmOutwardMid =
        new Dictionary<LigDir, Vector3>()
    {  
        { LigDir.LeftUpperArm,  Vector3.zero },
        { LigDir.LeftLowerArm,  Vector3.zero },
        { LigDir.LeftHand,      Vector3.zero },
        { LigDir.RightUpperArm, new Vector3(0.8912934f, -0.4534271f, 0f) },
        { LigDir.RightLowerArm, new Vector3(0.9877541f, -0.1560187f, 0f) },
        { LigDir.RightHand,     new Vector3(0.9980134f, 0.06300332f, 0f) }
    };

    public static Dictionary<LigDir, Vector3> ChargingKamehamehaRight =
        new Dictionary<LigDir, Vector3>()
    {  
		{ LigDir.LeftUpperArm,  new Vector3(-0.6790665f, -0.7340768f, 0f) },
		{ LigDir.LeftLowerArm,  new Vector3(0.5636994f, -0.82598f, 0f) },
		{ LigDir.LeftHand,      new Vector3(0.7708623f, -0.6370018f, 0f) },
		{ LigDir.RightUpperArm, new Vector3(0.01651556f, -0.9998636f, 0f) },
		{ LigDir.RightLowerArm, new Vector3(-0.8565479f, -0.5160677f, 0f) },
		{ LigDir.RightHand,     new Vector3(-0.891721f, -0.4525856f, 0f) }
    };

    public static Dictionary<LigDir, Vector3> ShootingKamehamehaRight =
        new Dictionary<LigDir, Vector3>()
    {  
		{ LigDir.LeftUpperArm,  Vector3.zero },
		{ LigDir.LeftLowerArm,  Vector3.zero },
		{ LigDir.LeftHand,      new Vector3(0.9589835f, -0.2834618f, 0f) },
		{ LigDir.RightUpperArm, Vector3.zero },
		{ LigDir.RightLowerArm, Vector3.zero },
		{ LigDir.RightHand,     new Vector3(0.7370645f, -0.6758224f, 0f) }
    };

    public static Dictionary<LigDir, Vector3> ChargingKamehamehaLeft =
        new Dictionary<LigDir, Vector3>()
    {  
		{ LigDir.LeftUpperArm,  new Vector3(0.04830567f, -0.9988326f, 0f) },
		{ LigDir.LeftLowerArm,  new Vector3(0.9240014f, -0.382389f, 0f) },
		{ LigDir.LeftHand,      new Vector3(0.9893546f, -0.1455246f, 0f) },
		{ LigDir.RightUpperArm, new Vector3(0.8613438f, -0.5080224f, 0f) },
		{ LigDir.RightLowerArm, new Vector3(-0.3197216f, -0.9475116f, 0f) },
		{ LigDir.RightHand,     new Vector3(-0.775146f, -0.6317821f, 0f) }
    };

    public static Dictionary<LigDir, Vector3> ShootingKamehamehaLeft =
        new Dictionary<LigDir, Vector3>()
    {  
        { LigDir.LeftUpperArm,  Vector3.zero },
		{ LigDir.LeftLowerArm,  Vector3.zero },
		{ LigDir.LeftHand,      new Vector3(0.9711809f, -0.2383438f, 0f) },
		{ LigDir.RightUpperArm, Vector3.zero },
		{ LigDir.RightLowerArm, Vector3.zero },
		{ LigDir.RightHand,     new Vector3(-0.9072307f, -0.4206336f, 0f) }
    };
}