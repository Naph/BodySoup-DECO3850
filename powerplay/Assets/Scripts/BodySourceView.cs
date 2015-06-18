using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Windows.Kinect;
using KinectJoint = Windows.Kinect.Joint;

public class BodySourceView : MonoBehaviour 
{
    public GameObject BodySourceManager;

    private Dictionary<ulong, PlayerBody> _Bodies;
    private List<Gesture> _Gestures;
    private BodySourceManager _BodyManager;
    private Body[] kinectBodies;

    private Dictionary<string, PlayerBody> _AlphaBodies;

    private int interval = 1;

    public Material redMaterial;
    public Material greenMaterial;
    public Material whiteMaterial;
    public float kinectDisplacement;

    public NetworkReader nr;

    public PlayerBody getBody(ulong id)
    {
        if (_Bodies.ContainsKey(id))
        {
            return _Bodies[id];
        }
        else
        {
            return null;
        }
    }
    public PlayerBody getBody(string id)
    {
        if (_AlphaBodies.ContainsKey(id))
        {
            return _AlphaBodies[id];
        }
        else
        {
            return null;
        }
    }

    void Start()
    {
        _Bodies = new Dictionary<ulong, PlayerBody>();
        _AlphaBodies = new Dictionary<string, PlayerBody>();
        _Gestures = new List<Gesture>();
        

        // WHITEBEARD CRACK
        Gesture.SubGesture crackFirst =
            new Gesture.SubGesture(Position.RightArmCrossOverHeart, JointType.HandRight, true, Effect.Instance.CastingCrack, false, 0.45f, 1.5f);
        Gesture.SubGesture crackSecond =
            new Gesture.SubGesture(Position.RightArmOutwardMid, JointType.HandRight, false, Effect.Instance.Crack, true, 0.60f, 2.5f);

        Gesture.SubGesture[] crackGesture = new Gesture.SubGesture[] {
           crackFirst, crackSecond};
        Gesture crack = new Gesture(crackGesture);
        _Gestures.Add(crack);


        // FIRE SHIELD
        Gesture.SubGesture fireshieldFirst =
            new Gesture.SubGesture(Position.HandsCrossed, JointType.SpineMid, true, Effect.Instance.CastingShield, false, 0.50f, 1f);
        Gesture.SubGesture watershieldFirst =
            new Gesture.SubGesture(Position.HandsOutRelaxed, JointType.SpineMid, true, Effect.Instance.Shield, true, 1.5f, 4f);

        Gesture.SubGesture[] fireshieldGesture = new Gesture.SubGesture[] {
            fireshieldFirst, watershieldFirst };
        Gesture fireshield = new Gesture(fireshieldGesture);
        _Gestures.Add(fireshield);


        // KAMEHAMEHA
        Gesture.SubGesture kamehamehaFirst =
            new Gesture.SubGesture(Position.ChargingKamehamehaMid, JointType.HandLeft, true, Effect.Instance.CastingKame, false, 0.35f, 3f);
        
        Gesture.SubGesture kamehamehaSecondRight =
            new Gesture.SubGesture(Position.KamehamehaLeft, JointType.HandLeft, JointType.SpineMid, false, Effect.Instance.Kame, true, 0.50f, 3f);
        
        Gesture.SubGesture kamehamehaSecondLeft =
            new Gesture.SubGesture(Position.KamehamehaRight, JointType.HandRight, JointType.SpineMid, false, Effect.Instance.Kame, true, 0.50f, 3f);
        
        Gesture.SubGesture[] kamehamehaGesture = new Gesture.SubGesture[] {
            kamehamehaFirst, kamehamehaSecondLeft, kamehamehaSecondRight };
        Gesture kamehameha = new Gesture(kamehamehaGesture);
        _Gestures.Add(kamehameha);
        

        // WINGS - Flapping arms
        Gesture.SubGesture WingsFirst =
            new Gesture.SubGesture(Position.TPoseLower, JointType.SpineShoulder, false, Effect.Instance.None, false, 0.35f, 2f);
        Gesture.SubGesture WingsSecond =
            new Gesture.SubGesture(Position.TPoseUpper, JointType.SpineShoulder, false, Effect.Instance.Feathers, false, 0.35f, 2f);
        Gesture.SubGesture WingsLast =
            new Gesture.SubGesture(Position.TPoseLower, JointType.SpineShoulder, true, Effect.Instance.Wings, true, 0.35f, 8f);
        
        Gesture.SubGesture[] wingGesture = new Gesture.SubGesture[] {
           WingsFirst, WingsSecond, WingsLast};
        Gesture wings = new Gesture(wingGesture);
        _Gestures.Add(wings);


        // LIGHTNING STORM - Y pose
        Gesture.SubGesture Lightning =
            new Gesture.SubGesture(Position.YPose, JointType.Neck, false, Effect.Instance.Lightning, true, 0.35f, 5f);

        Gesture.SubGesture[] lightningGesture = new Gesture.SubGesture[] {
            Lightning};
        Gesture lightning = new Gesture(lightningGesture);
        _Gestures.Add(lightning);

        
        // Mario Block - Right hand fist pump 
        Gesture.SubGesture marioFirst =
            new Gesture.SubGesture(Position.FistPumpRightLower, JointType.SpineMid, true, Effect.Instance.None, false, 0.45f, .8f);
        Gesture.SubGesture marioLast =
            new Gesture.SubGesture(Position.FirstPumpRightUpper, JointType.WristRight, false, Effect.Instance.MarioBlock, true, 0.45f, 2f);
        Gesture.SubGesture ghostLast =
            new Gesture.SubGesture(Position.RightHandHighWaveRight, JointType.HandRight, true, Effect.Instance.Wave, true, 0.35f, 3f);

        Gesture.SubGesture[] marioGesture = new Gesture.SubGesture[] {
            marioFirst, marioLast, ghostLast };
        Gesture mario = new Gesture(marioGesture);
        _Gestures.Add(mario);

        
     // Right Big Bang Shoot
     Gesture.SubGesture RightBigBang =
         new Gesture.SubGesture(Position.RightBigBang, JointType.HandRight, false, Effect.Instance.RightBigBang, true, 0.45f, 6f);

     Gesture.SubGesture[] rightbigbangGesture = new Gesture.SubGesture[] {
         RightBigBang };
     Gesture rightbigbang = new Gesture(rightbigbangGesture);
     _Gestures.Add(rightbigbang);
     

        
        // Left Explosion
        Gesture.SubGesture LeftExplosion =
            new Gesture.SubGesture(Position.LeftExplosion, JointType.HandLeft, false, Effect.Instance.LeftExplosion, true, 0.45f, 6f);

        Gesture.SubGesture[] leftexplosionGesture = new Gesture.SubGesture[] {
            LeftExplosion };
        Gesture leftexplosion = new Gesture(leftexplosionGesture);
        _Gestures.Add(leftexplosion);
      

        /*
        // SPAWN GHOST - Right hand waving 
        Gesture.SubGesture ghostFirst =
            new Gesture.SubGesture(Position.RightHandHighWaveLeft, JointType.HandRight, true, Effect.Instance.Wave, false, 0.45f, .2f);
        Gesture.SubGesture ghostLast =
            new Gesture.SubGesture(Position.RightHandHighWaveRight, JointType.HandRight, true, Effect.Instance.Wave, true, 0.45f, 4f);

        Gesture.SubGesture[] ghostGesture = new Gesture.SubGesture[] {
            ghostFirst, ghostLast };
        Gesture ghost = new Gesture(ghostGesture);
        _Gestures.Add(ghost);


        // Mario Block - Right hand fist pump 
        Gesture.SubGesture marioFirst =
            new Gesture.SubGesture(Position.FistPumpRightLower, JointType.SpineMid, true, Effect.Instance.None, false, 0.45f, .8f);
        Gesture.SubGesture marioLast =
            new Gesture.SubGesture(Position.FirstPumpRightUpper, JointType.WristRight, false, Effect.Instance.MarioBlock, true, 0.45f, 2f);

        Gesture.SubGesture[] marioGesture = new Gesture.SubGesture[] {
            marioFirst, marioLast };
        Gesture mario = new Gesture(marioGesture);
        _Gestures.Add(mario);
        */


        /* Stomp
        Gesture.SubGesture Stomp =
            new Gesture.SubGesture(Position.LowStomp, JointType.FootRight, false, Effect.Instance.Stomp, true, 0.45f, 5f);

        Gesture.SubGesture[] stompGesture = new Gesture.SubGesture[] {
            Stomp };
        Gesture stomp = new Gesture(stompGesture);
        _Gestures.Add(stomp);
        */

        /* FireAura
        Gesture.SubGesture FireAura =
            new Gesture.SubGesture(Position.FireAura, JointType.FootRight, false, Effect.Instance.FireAura, true, 0.45f, 7f);

        Gesture.SubGesture[] fireauraGesture = new Gesture.SubGesture[] {
            FireAura };
        Gesture fireaura = new Gesture(fireauraGesture);
        _Gestures.Add(fireaura);
        */
 
        /*
        // Cyclone
        Gesture.SubGesture Cyclone =
            new Gesture.SubGesture(Position.Cyclone, JointType.FootRight, true, Effect.Instance.Cyclone, true, 0.45f, 7f);

        Gesture.SubGesture[] cycloneGesture = new Gesture.SubGesture[] {
            Cyclone };
        Gesture cyclone = new Gesture(cycloneGesture);
        _Gestures.Add(cyclone);
        */

        /*
        // MeteorStorm
        Gesture.SubGesture MeteorStorm =
            new Gesture.SubGesture(Position.MeteorStorm, JointType.HandRight, false, Effect.Instance.MeteorStorm, true, 0.45f, 6f);

        Gesture.SubGesture[] meteorstormGesture = new Gesture.SubGesture[] {
            MeteorStorm };
        Gesture meteorstorm = new Gesture(meteorstormGesture);
        _Gestures.Add(meteorstorm);
        */

     

        /*
        // Fire Burst
        Gesture.SubGesture FireBurst =
            new Gesture.SubGesture(Position.FireBurst, JointType.FootRight, false, Effect.Instance.FireBurst, true, 0.45f, 6f);

        Gesture.SubGesture[] fireburstGesture = new Gesture.SubGesture[] {
            FireBurst };
        Gesture fireburst = new Gesture(fireburstGesture);
        _Gestures.Add(fireburst);
        */

        /*
        // Left Holy Shine
        Gesture.SubGesture LeftHolyShine =
            new Gesture.SubGesture(Position.LeftHolyShine, JointType.HandLeft, false, Effect.Instance.LeftHolyShine, true, 0.45f, 6f);

        Gesture.SubGesture[] leftholyshineGesture = new Gesture.SubGesture[] {
            LeftHolyShine };
        Gesture leftholyshine = new Gesture(leftholyshineGesture);
        _Gestures.Add(leftholyshine);
        */

        /*
        // Right Snow Storm
        Gesture.SubGesture RightSnowStorm =
            new Gesture.SubGesture(Position.RightSnowStorm, JointType.Neck, false, Effect.Instance.RightSnowStorm, true, 0.45f, 6f);

        Gesture.SubGesture[] rightsnowstormGesture = new Gesture.SubGesture[] {
            RightSnowStorm };
        Gesture rightsnowstorm = new Gesture(rightsnowstormGesture);
        _Gestures.Add(rightsnowstorm);
        */

    }


    void Update()
    {
        UpdateULongBodies();
        UpdateAlphaBodies();
    }

    void UpdateAlphaBodies()
    {
        if (!nr.Connected)
        {
            return;
        }

        if (nr.ReadOnce)
        {
            if (nr.NumBodies > 0)
            {
                List<string> notFound = new List<string>();

                List<string> names = nr.Names;
                List<float[]> floats = nr.Floats;

                foreach (string name in _AlphaBodies.Keys)
                {
                    if (!names.Contains(name))
                    {
                        notFound.Add(name);
                    }
                }

                foreach (string name in notFound)
                {
                    _AlphaBodies[name].DestroyBody();
                    _AlphaBodies.Remove(name);
                }

                for (int i = 0; i < names.Count; i++)
                {
                    if (!_AlphaBodies.ContainsKey(names[i]))
                    {
                        _AlphaBodies[names[i]] = CreateBodyObject(names[i]);
                    }

                    RefreshBody(floats[i], _AlphaBodies[names[i]]);
                    GestureListener(_AlphaBodies[names[i]]);
                }
            }
            else
            {
                foreach (KeyValuePair<string, PlayerBody> zombie in _AlphaBodies)
                {
                    zombie.Value.DestroyBody();
                }
                _AlphaBodies.Clear();
            }
        }
    }

    void UpdateULongBodies()
    {
        _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();

        kinectBodies = _BodyManager.getBodies();

        List<ulong> trackedIds = new List<ulong>();

        List<ulong> knownIds = new List<ulong>(_Bodies.Keys);


        if (BodySourceManager == null)
        {
            return;
        }

        if (_BodyManager == null)
        {
            return;
        }

        if (kinectBodies == null)
        {
            return;
        }


        /* Store ID of each tracked Kinect.Body
         */
        for (int i = 0; i < kinectBodies.Length; i++)
        {
            var body = kinectBodies[i];

            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                trackedIds.Add(body.TrackingId);
            }
        }


        /* Remove ID of untracked Bodies
         */
        foreach (ulong trackingId in knownIds)
        {
            if (!trackedIds.Contains(trackingId))
            {
                _Bodies[trackingId].DestroyBody();
                _Bodies.Remove(trackingId);
            }
        }


        /* CreateBodyObject for each tracked Kinect.Body while
         * refreshing and listening for Gesture
         */
        for (int i = 0; i < kinectBodies.Length; i++)
        {

            var body = kinectBodies[i];

            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                if (!_Bodies.ContainsKey(body.TrackingId))
                {
                    _Bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                }

                RefreshBody(body, _Bodies[body.TrackingId]);
                GestureListener(_Bodies[body.TrackingId]);
            }
        }
	}


    /**
     * Instantiates new PlayerBody for each tracked Kinect.Body Id
     */
    private PlayerBody CreateBodyObject(ulong id)
    {
        return new PlayerBody(id, greenMaterial, redMaterial, whiteMaterial, kinectDisplacement);
    }

    private PlayerBody CreateBodyObject(string name)
    {
        return new PlayerBody(name, greenMaterial, redMaterial, whiteMaterial, kinectDisplacement);
    }


    /**
     * Refreshes PlayerBody to evaluate Kinect.Body positions
     */
    private void RefreshBody(Body body, PlayerBody player)
    {
        player.RefreshBody(body);
    }

    private void RefreshBody(float[] floats, PlayerBody player)
    {
        player.RefreshBody(floats);
    }


    /**
     * Informs PlayerBody of player if they have started a new gesture.
     */
    private void GestureListener(PlayerBody player)
    {
        for (int i = 0; i < _Gestures.Count; i++)
        {
            if (player.ComparePosition(_Gestures[i].first))
            {
                player.StartGesture(_Gestures[i]);
                break;
            }
        }
    }


    void OnGUI()
    {
        // SHIFT + RETURN
        if (Event.current.Equals(Event.KeyboardEvent("#return")))
        {
            InvokeRepeating("CaptureBodyFrame", 1.0f, 1.0f);
        }
    }


    /**
     * Captures frame of ligament orientations and appends to file
     * Called by SHIFT + RETURN at runtime
     */
    private void CaptureBodyFrame()
    {
        string s = new string('.', interval * 5);

        if (interval < 5)
        {
            Debug.Log("Capturing" + s);
            interval++;
        }
        else
        {
            string output = "";

            for (int i = 0; i < kinectBodies.Length; i++)
            {
                var body = kinectBodies[i];

                if (body == null)
                {
                    continue;
                }

                if (body.IsTracked)
                {
                    PlayerBody playerBody = _Bodies[body.TrackingId];
                    output = playerBody.ToFileString();
                }
            }
            File.AppendAllText(@"../CapturedBodyFrames.txt", output);

            Debug.Log("Captured");
            interval = 1;
            CancelInvoke("CaptureBodyFrame");
        }
    }
}
