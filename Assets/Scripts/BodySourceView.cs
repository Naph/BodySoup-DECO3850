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

    private int interval = 1;
    private int maxInterval = 6;

    public Material redMaterial;
    public Material greenMaterial;


    void Start()
    {
        _Bodies = new Dictionary<ulong, PlayerBody>();
        _Gestures = new List<Gesture>();
        
        // WHITEBEARD CRACK
        Gesture.SubGesture crackFirst =
            new Gesture.SubGesture(Position.RightArmCrossOverHeart, JointType.HandRight, true, Effect.Instance.WindWorks, 0.30f, 1.5f);
        Gesture.SubGesture crackSecond =
            new Gesture.SubGesture(Position.RightArmOutwardMid, JointType.HandRight, false, Effect.Instance.CrackParent, 0.45f, 2.5f);

        List<Gesture.SubGesture> crackGesture = new List<Gesture.SubGesture>(new Gesture.SubGesture[] {
           crackFirst, crackSecond});
        Gesture crack = new Gesture(crackGesture, true);
        _Gestures.Add(crack);

        // FIRE SHIELD
        Gesture.SubGesture fireshieldFirst =
            new Gesture.SubGesture(Position.HandsOnHead, JointType.SpineMid, true, Effect.Instance.FireShield, 0.60f, 4f);
        Gesture.SubGesture watershieldFirst =
            new Gesture.SubGesture(Position.TPose, JointType.SpineMid, true, Effect.Instance.WaterShield, 0.35f, 4f);

        List<Gesture.SubGesture> fireshieldGesture = new List<Gesture.SubGesture>(new Gesture.SubGesture[] {
            fireshieldFirst, watershieldFirst});
        Gesture fireshield = new Gesture(fireshieldGesture, false);
        _Gestures.Add(fireshield);

        // KAMEHAMEHA RIGHT
        Gesture.SubGesture kamehamehaFirstRight =
            new Gesture.SubGesture(Position.ChargingKamehamehaRight, JointType.HandRight, true, Effect.Instance.WindWorks, 0.60f, 4f);
        Gesture.SubGesture kamehamehaSecondRight =
            new Gesture.SubGesture(Position.ShootingKamehamehaRight, JointType.HandRight, JointType.SpineMid, false, Effect.Instance.Kamehameha, 0.80f, 2f);

        List<Gesture.SubGesture> kamehamehaGestureRight = new List<Gesture.SubGesture>(new Gesture.SubGesture[] {
            kamehamehaFirstRight, kamehamehaSecondRight});
        Gesture kamehamehaRight = new Gesture(kamehamehaGestureRight, false);
        _Gestures.Add(kamehamehaRight);

        // KAMEHAMEHA LEFT
        Gesture.SubGesture kamehamehaFirstLeft =
            new Gesture.SubGesture(Position.ChargingKamehamehaLeft, JointType.HandLeft, true, Effect.Instance.WindWorks, 0.60f, 4f);
        Gesture.SubGesture kamehamehaSecondLeft =
            new Gesture.SubGesture(Position.ShootingKamehamehaLeft, JointType.HandLeft, JointType.SpineMid, false, Effect.Instance.Kamehameha, 0.80f, 2f);

        List<Gesture.SubGesture> kamehamehaGestureLeft = new List<Gesture.SubGesture>(new Gesture.SubGesture[] {
            kamehamehaFirstLeft, kamehamehaSecondLeft});
        Gesture kamehamehaLeft = new Gesture(kamehamehaGestureLeft, false);
        _Gestures.Add(kamehamehaLeft);
    }


	void Update()
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


    /**
     * Refreshes PlayerBody to evaluate Kinect.Body positions
     */
    private void RefreshBody(Body body, PlayerBody player)
    {
        player.RefreshBody(body);
    }


    /**
     * Informs PlayerBody of player if they have started a new gesture.
     */
    private void GestureListener(PlayerBody player)
    {
        for (int i = 0; i < _Gestures.Count; i++)
        {
            if (player.ComparePosition(_Gestures[i].First))
            {
                player.inGesture = true;
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
