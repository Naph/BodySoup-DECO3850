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

    public static Dictionary<LigDir, Vector3> KamehamehaLeft =
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

    public static Dictionary<LigDir, Vector3> KamehamehaRight =
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

    public static Dictionary<LigDir, Vector3> HandsOutRelaxed =
        new Dictionary<LigDir, Vector3>() 
    {
		{ LigDir.LeftUpperArm, new Vector3(-0.2361802f, -0.9717093f, 0f) },
		{ LigDir.LeftLowerArm, new Vector3(-0.9931731f, 0.1166507f, 0f) },
		{ LigDir.LeftHand, new Vector3(-0.7975165f, 0.6032971f, 0f) },
		{ LigDir.RightUpperArm, new Vector3(0.1929874f, -0.9812012f, 0f) },
		{ LigDir.RightLowerArm, new Vector3(0.956741f, 0.2909411f, 0f) },
		{ LigDir.RightHand, new Vector3(0.790009f, 0.6130952f, 0f) }
    };

    public static Dictionary<LigDir, Vector3> RightHandHighWaveLeft =
        new Dictionary<LigDir, Vector3>() 
    {
		{ LigDir.LeftUpperArm, new Vector3(-0.1323263f, -0.9912062f, 0f) },
		{ LigDir.LeftLowerArm, new Vector3(0.02781733f, -0.999613f, 0f) },
		{ LigDir.LeftHand, new Vector3(0.1456975f, -0.9893292f, 0f) },
		{ LigDir.RightUpperArm, new Vector3(0.9997599f, 0.02191218f, 0f) },
		{ LigDir.RightLowerArm, new Vector3(-0.1402469f, 0.9901165f, 0f) },
		{ LigDir.RightHand, new Vector3(-0.6672141f, 0.744866f, 0f) }
    };

    public static Dictionary<LigDir, Vector3> RightHandHighWaveRight =
        new Dictionary<LigDir, Vector3>() 
    {
		{ LigDir.LeftUpperArm, new Vector3(-0.07367232f, -0.9972825f, 0f) },
		{ LigDir.LeftLowerArm, new Vector3(0.01774124f, -0.9998426f, 0f) },
		{ LigDir.LeftHand, new Vector3(0.1131744f, -0.9935752f, 0f) },
		{ LigDir.RightUpperArm, new Vector3(0.9386668f, -0.3448256f, 0f) },
		{ LigDir.RightLowerArm, new Vector3(0.5731013f, 0.8194846f, 0f) },
		{ LigDir.RightHand, new Vector3(0.5441839f, 0.838966f, 0f) }
    };

    public static Dictionary<LigDir, Vector3> TPoseLower =
        new Dictionary<LigDir, Vector3>() 
    {
		{ LigDir.LeftUpperArm, new Vector3(-0.9265025f, -0.3762887f, 0f) },
		{ LigDir.LeftLowerArm, new Vector3(-0.9537491f, -0.3006038f, 0f) },
		{ LigDir.LeftHand, new Vector3(-0.9619535f, -0.2732131f, 0f) },
		{ LigDir.RightUpperArm, new Vector3(0.9568611f, -0.2905457f, 0f) },
		{ LigDir.RightLowerArm, new Vector3(0.9452727f, -0.3262813f, 0f) },
		{ LigDir.RightHand, new Vector3(0.9684856f, -0.2490694f, 0f) }
    };

    public static Dictionary<LigDir, Vector3> TPoseUpper =
        new Dictionary<LigDir, Vector3>() 
    {
		{ LigDir.LeftUpperArm, new Vector3(-0.9332597f, 0.3592027f, 0f) },
		{ LigDir.LeftLowerArm, new Vector3(-0.9096364f, 0.4154054f, 0f) },
		{ LigDir.LeftHand, new Vector3(-0.8889737f, 0.4579584f, 0f) },
		{ LigDir.RightUpperArm, new Vector3(0.9391102f, 0.3436162f, 0f) },
		{ LigDir.RightLowerArm, new Vector3(0.9422033f, 0.3350419f, 0f) },
		{ LigDir.RightHand, new Vector3(0.9167677f, 0.3994208f, 0f) }
    };

    public static Dictionary<LigDir, Vector3> FistPumpRightLower =
        new Dictionary<LigDir, Vector3>() 
    {
		{ LigDir.LeftUpperArm, new Vector3(-0.2434673f, -0.9699091f, 0f) },
		{ LigDir.LeftLowerArm, new Vector3(0.05665678f, -0.9983938f, 0f) },
		{ LigDir.LeftHand, new Vector3(0.153444f, -0.9881574f, 0f) },
		{ LigDir.RightUpperArm, new Vector3(0.5436894f, -0.8392866f, 0f) },
		{ LigDir.RightLowerArm, new Vector3(0.551622f, 0.8340942f, 0f) },
		{ LigDir.RightHand, new Vector3(0.1645804f, 0.9863637f, 0f) }
    };

    public static Dictionary<LigDir, Vector3> FirstPumpRightUpper =
        new Dictionary<LigDir, Vector3>() 
    {
		{ LigDir.LeftUpperArm, new Vector3(-0.02262265f, -0.9997441f, 0f) },
		{ LigDir.LeftLowerArm, new Vector3(-0.1134907f, -0.9935391f, 0f) },
		{ LigDir.LeftHand, new Vector3(0.162934f, -0.986637f, 0f) },
		{ LigDir.RightUpperArm, new Vector3(0.5640047f, 0.8257716f, 0f) },
		{ LigDir.RightLowerArm, new Vector3(0.2145575f, 0.9767114f, 0f) },
		{ LigDir.RightHand, new Vector3(0.058141f, 0.9983084f, 0f) }
    };

    /*** UNTESTED ***/
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

    public static Dictionary<LigDir, Vector3> HandsCrossed =
        new Dictionary<LigDir, Vector3>()
    {  
        { LigDir.LeftUpperArm, new Vector3(0.2567344f, -0.966482f, 0f) },
		{ LigDir.LeftLowerArm, new Vector3(0.9580926f, 0.2864588f, 0f) },
		{ LigDir.LeftHand, new Vector3(0.9252563f, -0.3793427f, 0f) },
		{ LigDir.RightUpperArm, new Vector3(-0.3769011f, -0.9262535f, 0f) },
		{ LigDir.RightLowerArm, new Vector3(-0.935698f, 0.3528019f, 0f) },
		{ LigDir.RightHand, new Vector3(-0.9850407f, -0.1723219f, 0f) }
    };

    public static Dictionary<LigDir, Vector3> LowStomp =
        new Dictionary<LigDir, Vector3>()
    { 
		{ LigDir.LeftUpperArm, new Vector3(-0.01494402f, 0.9998884f, 0f) },
		{ LigDir.LeftLowerArm, new Vector3(-0.6660757f, 0.7458842f, 0f) },
		{ LigDir.LeftHand, new Vector3(-0.2789885f, 0.9602944f, 0f) },
		{ LigDir.RightUpperArm, new Vector3(0.2855784f, -0.9583554f, 0f) },
		{ LigDir.RightLowerArm, new Vector3(-0.2115364f, -0.9773701f, 0f) },
		{ LigDir.RightHand, new Vector3(-0.2559824f, -0.9666814f, 0f) }
    };

    public static Dictionary<LigDir, Vector3> FireAura =
        new Dictionary<LigDir, Vector3>()
    { 
		{ LigDir.LeftUpperArm, new Vector3(-0.644475f, -0.7646254f, 0f) },
		{ LigDir.LeftLowerArm, new Vector3(-0.6855222f, 0.7280517f, 0f) },
		{ LigDir.LeftHand, new Vector3(-0.6252823f, 0.7803986f, 0f) },
		{ LigDir.RightUpperArm, new Vector3(-0.7834527f, 0.6214515f, 0f) },
		{ LigDir.RightLowerArm, new Vector3(-0.9835154f, 0.1808246f, 0f) },
		{ LigDir.RightHand, new Vector3(-0.9078873f, 0.4192142f, 0f) }
     };

    public static Dictionary<LigDir, Vector3> Cyclone =
        new Dictionary<LigDir, Vector3>()
    { 
    	{ LigDir.LeftUpperArm, new Vector3(-0.2523338f, -0.9676402f, 0f) },
		{ LigDir.LeftLowerArm, new Vector3(0.2100201f, -0.9776971f, 0f) },
		{ LigDir.LeftHand, new Vector3(0.0322083f, -0.9994812f, 0f) },
		{ LigDir.RightUpperArm, new Vector3(0.1055829f, 0.9944105f, 0f) },
		{ LigDir.RightLowerArm, new Vector3(0.05628535f, 0.9984148f, 0f) },
		{ LigDir.RightHand, new Vector3(0.2230994f, 0.9747957f, 0f) }
     };


    public static Dictionary<LigDir, Vector3> MeteorStorm =
        new Dictionary<LigDir, Vector3>()
    { 
		{ LigDir.LeftUpperArm, new Vector3(0.6737086f, 0.7389972f, 0f) },
		{ LigDir.LeftLowerArm, new Vector3(0.8297056f, 0.5582012f, 0f) },
		{ LigDir.LeftHand, new Vector3(0.9720002f, 0.2349801f, 0f) },
		{ LigDir.RightUpperArm, new Vector3(0.7748074f, -0.6321974f, 0f) },
		{ LigDir.RightLowerArm, new Vector3(0.9822664f, 0.1874905f, 0f) },
		{ LigDir.RightHand, new Vector3(0.972346f, 0.2335451f, 0f) }
     };


    public static Dictionary<LigDir, Vector3> RightBigBang =
    new Dictionary<LigDir, Vector3>()
    { 
		{ LigDir.LeftUpperArm, new Vector3(-0.9256945f, 0.3782719f, 0f) },
		{ LigDir.LeftLowerArm, new Vector3(0.7120745f, 0.7021039f, 0f) },
		{ LigDir.LeftHand, new Vector3(0.9813865f, 0.1920433f, 0f) },
		{ LigDir.RightUpperArm, new Vector3(0.9770211f, 0.2131425f, 0f) },
		{ LigDir.RightLowerArm, new Vector3(0.9994137f, 0.0342379f, 0f) },
		{ LigDir.RightHand, new Vector3(0.9990355f, 0.04390891f, 0f) }
     };

    public static Dictionary<LigDir, Vector3> LeftExplosion =
    new Dictionary<LigDir, Vector3>()
    { 
		{ LigDir.LeftUpperArm, new Vector3(-0.9662385f, 0.2576492f, 0f) },
		{ LigDir.LeftLowerArm, new Vector3(-0.9923923f, 0.1231165f, 0f) },
		{ LigDir.LeftHand, new Vector3(-0.9817407f, 0.1902246f, 0f) },
		{ LigDir.RightUpperArm, new Vector3(0.9492865f, 0.3144124f, 0f) },
		{ LigDir.RightLowerArm, new Vector3(-0.9367174f, 0.3500865f, 0f) },
		{ LigDir.RightHand, new Vector3(-0.9832837f, -0.1820801f, 0f) }
     };

    public static Dictionary<LigDir, Vector3> FireBurst =
    new Dictionary<LigDir, Vector3>()
    { 
		{ LigDir.LeftUpperArm, new Vector3(-0.5389234f, -0.8423548f, 0f) },
		{ LigDir.LeftLowerArm, new Vector3(0.6048513f, -0.7963385f, 0f) },
		{ LigDir.LeftHand, new Vector3(0.7385942f, 0.6741502f, 0f) },
		{ LigDir.RightUpperArm, new Vector3(0.445213f, -0.8954247f, 0f) },
		{ LigDir.RightLowerArm, new Vector3(-0.6152833f, -0.7883061f, 0f) },
		{ LigDir.RightHand, new Vector3(-0.9993532f, 0.03595952f, 0f) }
     };

    public static Dictionary<LigDir, Vector3> LeftHolyShine =
    new Dictionary<LigDir, Vector3>()
    { 
		{ LigDir.LeftUpperArm, new Vector3(-0.9982188f, -0.05965921f, 0f) },
		{ LigDir.LeftLowerArm, new Vector3(-0.9967913f, 0.08004424f, 0f) },
		{ LigDir.LeftHand, new Vector3(-0.02944523f, 0.9995664f, 0f) },
		{ LigDir.RightUpperArm, new Vector3(-0.9970986f, 0.07612134f, 0f) },
		{ LigDir.RightLowerArm, new Vector3(-0.9935632f, 0.1132792f, 0f) },
		{ LigDir.RightHand, new Vector3(-0.9818414f, -0.1897039f, 0f) }
     };
    

    public static Dictionary<LigDir, Vector3> RightSnowStorm =
    new Dictionary<LigDir, Vector3>()
    { 
    	{ LigDir.LeftUpperArm, new Vector3(0.9956904f, -0.09274018f, 0f) },
		{ LigDir.LeftLowerArm, new Vector3(0.9990427f, 0.04374611f, 0f) },
		{ LigDir.LeftHand, new Vector3(0.9986389f, 0.05215694f, 0f) },
		{ LigDir.RightUpperArm, new Vector3(0.999254f, 0.03861853f, 0f) },
		{ LigDir.RightLowerArm, new Vector3(0.9989367f, 0.04610391f, 0f) },
		{ LigDir.RightHand, new Vector3(0.8364449f, 0.5480511f, 0f) }
     };

}