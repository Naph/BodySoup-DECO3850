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

    public NetworkReader nr;

    void Start()
    {
        _Bodies = new Dictionary<ulong, PlayerBody>();
        _AlphaBodies = new Dictionary<string, PlayerBody>();
        _Gestures = new List<Gesture>();
        

        // WHITEBEARD CRACK
        Gesture.SubGesture crackFirst =
            new Gesture.SubGesture(Position.RightArmCrossOverHeart, JointType.HandRight, true, Effect.Instance.Shirohige, false, 0.30f, 1.5f);
        Gesture.SubGesture crackSecond =
            new Gesture.SubGesture(Position.RightArmOutwardMid, JointType.HandRight, false, Effect.Instance.CrackParent, true, 0.45f, 2.5f);

        List<Gesture.SubGesture> crackGesture = new List<Gesture.SubGesture>(new Gesture.SubGesture[] {
           crackFirst, crackSecond});
        Gesture crack = new Gesture(crackGesture, true);
        _Gestures.Add(crack);


        // FIRE SHIELD
        Gesture.SubGesture fireshieldFirst =
            new Gesture.SubGesture(Position.HandsOnHead, JointType.SpineMid, true, Effect.Instance.FireShield, false, 0.60f, 4f);
        Gesture.SubGesture watershieldFirst =
            new Gesture.SubGesture(Position.TPose, JointType.SpineMid, true, Effect.Instance.WaterShield, true, 0.35f, 4f);

        List<Gesture.SubGesture> fireshieldGesture = new List<Gesture.SubGesture>(new Gesture.SubGesture[] {
            fireshieldFirst, watershieldFirst });
        Gesture fireshield = new Gesture(fireshieldGesture, false);
        _Gestures.Add(fireshield);


        // KAMEHAMEHA
        Gesture.SubGesture kamehamehaFirst =
            new Gesture.SubGesture(Position.ChargingKamehamehaMid, JointType.HandLeft, true, Effect.Instance.Kame, false, 0.60f, 3f);
        
        Gesture.SubGesture kamehamehaSecondRight =
            new Gesture.SubGesture(Position.KamehamehaRight, JointType.HandRight, JointType.SpineMid, true, Effect.Instance.Kamehameha, true, 0.50f, 3f);
        
        Gesture.SubGesture kamehamehaSecondLeft =
            new Gesture.SubGesture(Position.KamehamehaLeft, JointType.HandLeft, JointType.SpineMid, true, Effect.Instance.Kamehameha, true, 0.50f, 3f);
        
        List<Gesture.SubGesture> kamehamehaGesture = new List<Gesture.SubGesture>(new Gesture.SubGesture[] {
            kamehamehaFirst, kamehamehaSecondRight, kamehamehaSecondLeft });
        Gesture kamehameha = new Gesture(kamehamehaGesture, true, true);
        _Gestures.Add(kamehameha);


        // CHARIZARD WINGS
        Gesture.SubGesture Wings =
            new Gesture.SubGesture(Position.TPose, JointType.Neck, true, Effect.Instance.Wings, true, 0.35f, 5f);

        List<Gesture.SubGesture> wingGesture = new List<Gesture.SubGesture>(new Gesture.SubGesture[] {
           Wings, kamehamehaSecondLeft});
        Gesture wings = new Gesture(wingGesture, true);
        _Gestures.Add(wings);


        // LIGHTNING STORM
        Gesture.SubGesture Lightning =
            new Gesture.SubGesture(Position.YPose, JointType.Neck, false, Effect.Instance.Lightning, true, 0.60f, 5f);

        List<Gesture.SubGesture> lightningGesture = new List<Gesture.SubGesture>(new Gesture.SubGesture[] {
            Lightning, kamehamehaSecondLeft });
        Gesture lightning = new Gesture(lightningGesture, false);
        _Gestures.Add(lightning);
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
        return new PlayerBody(id, greenMaterial, redMaterial);
    }

    private PlayerBody CreateBodyObject(string name)
    {
        return new PlayerBody(name, greenMaterial, redMaterial);
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
                player.inGesture = true;
                player.isAmbidextrous = _Gestures[i].ambidexterity;
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
