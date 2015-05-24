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

    public static Dictionary<LigDir, Vector3> YPose =
        new Dictionary<LigDir, Vector3>()
    {
        { LigDir.LeftUpperArm, new Vector3(-0.567504f, 0.8233706f, 0f) },
		{ LigDir.LeftLowerArm, new Vector3(-0.4474594f, 0.8943042f, 0f) },
		{ LigDir.LeftHand, new Vector3(-0.2222516f, 0.9749893f, 0f) },
		{ LigDir.RightUpperArm, new Vector3(0.5471132f, 0.8370587f, 0f) },
		{ LigDir.RightLowerArm, new Vector3(0.3302624f, 0.9438891f, 0f) },
		{ LigDir.RightHand, new Vector3(0.08692314f, 0.9962151f, 0f) }
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

    public static Dictionary<LigDir, Vector3> ChargingKamehamehaMid =
      new Dictionary<LigDir, Vector3>()
    {  
		{ LigDir.LeftUpperArm,  new Vector3(-0.3339442f, -0.9425929f, 0f) },
		{ LigDir.LeftLowerArm,  new Vector3(0.9454747f, 0.3256955f, 0f) },
		{ LigDir.LeftHand,      Vector3.zero },
		{ LigDir.RightUpperArm, new Vector3(0.4051847f, -0.9142349f, 0f) },
		{ LigDir.RightLowerArm, new Vector3(-0.9389423f, 0.3440746f, 0f) },
		{ LigDir.RightHand,     Vector3.zero }
    };

    public static Dictionary<LigDir, Vector3> ChargingKamehamehaRL =
        new Dictionary<LigDir, Vector3>()
    {  
		{ LigDir.LeftUpperArm, new Vector3(0.183956f, -0.9829345f, 0f) },
		{ LigDir.LeftLowerArm, new Vector3(0.8922045f, -0.4516315f, 0f) },
		{ LigDir.LeftHand, new Vector3(0.9846293f, -0.1746572f, 0f) },
		{ LigDir.RightUpperArm, new Vector3(-0.1390942f, -0.9902791f, 0f) },
		{ LigDir.RightLowerArm, new Vector3(0.5323825f, -0.8465039f, 0f) },
		{ LigDir.RightHand, new Vector3(0.7386277f, -0.6741135f, 0f) }
    };

    public static Dictionary<LigDir, Vector3> ChargingKamehamehaLR =
        new Dictionary<LigDir, Vector3>()
    {  
		{ LigDir.LeftUpperArm, new Vector3(-0.08942757f, -0.9959933f, 0f) },
		{ LigDir.LeftLowerArm, new Vector3(-0.3259355f, -0.9453921f, 0f) },
		{ LigDir.LeftHand, new Vector3(-0.7384217f, -0.6743392f, 0f) },
		{ LigDir.RightUpperArm, new Vector3(-0.2081425f, -0.9780986f, 0f) },
		{ LigDir.RightLowerArm, new Vector3(-0.7399271f, -0.6726871f, 0f) },
		{ LigDir.RightHand, new Vector3(-0.8409937f, -0.541045f, 0f) }
    };

    public static Dictionary<LigDir, Vector3> KamehamehaRight =
        new Dictionary<LigDir, Vector3>()
    {  
	    { LigDir.LeftUpperArm,  new Vector3(0.9879123f, -0.155014f, 0f) },
		{ LigDir.LeftLowerArm,  new Vector3(0.9984961f, 0.05482337f, 0f) },
		{ LigDir.LeftHand,      new Vector3(0.9053686f, -0.4246267f, 0f) },
		{ LigDir.RightUpperArm, Vector3.zero },
		{ LigDir.RightLowerArm, Vector3.zero },
		{ LigDir.RightHand,     Vector3.zero }	
		//{ LigDir.RightHand,     new Vector3(0.7370645f, -0.6758224f, 0f) }
    };

    public static Dictionary<LigDir, Vector3> KamehamehaLeft =
        new Dictionary<LigDir, Vector3>()
    {  
        { LigDir.LeftUpperArm,  Vector3.zero },
		{ LigDir.LeftLowerArm,  Vector3.zero },
		{ LigDir.LeftHand,      Vector3.zero },
		//{ LigDir.LeftHand,    new Vector3(0.9711809f, -0.2383438f, 0f) },
		{ LigDir.RightUpperArm, new Vector3(-0.9997495f, -0.02238094f, 0f) },
		{ LigDir.RightLowerArm, new Vector3(-0.996089f, 0.0883546f, 0f) },
		{ LigDir.RightHand,     new Vector3(-0.9878862f, 0.1551802f, 0f) }
    };

    public static Dictionary<LigDir, Vector3> IronmanLeftRF =
        new Dictionary<LigDir, Vector3>()
    {  
        { LigDir.LeftUpperArm, new Vector3(-0.374613f, -0.9271813f, 0f) },
		{ LigDir.LeftLowerArm, new Vector3(-0.988156f, 0.1534528f, 0f) },
		{ LigDir.LeftHand, new Vector3(-0.9969546f, 0.0779838f, 0f) },
		{ LigDir.RightUpperArm, new Vector3(-0.9827826f, -0.1847656f, 0f) },
		{ LigDir.RightLowerArm, new Vector3(-0.9663712f, 0.2571509f, 0f) },
		{ LigDir.RightHand, new Vector3(-0.7193612f, 0.6946362f, 0f) }
    };

    public static Dictionary<LigDir, Vector3> IronmanLeftLF =
        new Dictionary<LigDir, Vector3>()
    {  
        { LigDir.LeftUpperArm, new Vector3(-0.7202339f, -0.6937312f, 0f) },
		{ LigDir.LeftLowerArm, new Vector3(-0.980965f, -0.1941848f, 0f) },
		{ LigDir.LeftHand, new Vector3(-0.9883741f, 0.1520416f, 0f) },
		{ LigDir.RightUpperArm, new Vector3(-0.9554021f, -0.2953082f, 0f) },
		{ LigDir.RightLowerArm, new Vector3(-0.9064786f, 0.4222518f, 0f) },
		{ LigDir.RightHand, new Vector3(-0.5860792f, 0.8102537f, 0f) }
    };

    public static Dictionary<LigDir, Vector3> ZenMeditation =
        new Dictionary<LigDir, Vector3>()
    {  
        { LigDir.LeftUpperArm, new Vector3(-0.9769526f, -0.2134561f, 0f) },
		{ LigDir.LeftLowerArm, new Vector3(0.6099243f, 0.7924596f, 0f) },
		{ LigDir.LeftHand, new Vector3(0.8715143f, 0.49037f, 0f) },
		{ LigDir.RightUpperArm, new Vector3(-0.8250238f, 0.565098f, 0f) },
		{ LigDir.RightLowerArm, new Vector3(-0.781472f, 0.6239403f, 0f) },
		{ LigDir.RightHand, new Vector3(-0.2422982f, 0.9702018f, 0f) }
    };

    public static Dictionary<LigDir, Vector3> HandsAtShoulders =
        new Dictionary<LigDir, Vector3>()
    {  
        { LigDir.LeftUpperArm, new Vector3(-0.4531104f, -0.8914545f, 0f) },
		{ LigDir.LeftLowerArm, new Vector3(0.3911757f, 0.920316f, 0f) },
		{ LigDir.LeftHand, new Vector3(0.132154f, 0.9912292f, 0f) },
		{ LigDir.RightUpperArm, new Vector3(0.3823312f, -0.9240253f, 0f) },
		{ LigDir.RightLowerArm, new Vector3(-0.3831514f, 0.9236855f, 0f) },
		{ LigDir.RightHand, new Vector3(-0.3193555f, 0.947635f, 0f) }
    };

}