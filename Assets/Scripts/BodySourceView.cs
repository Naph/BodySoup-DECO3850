using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Windows.Kinect;
using KinectJoint = Windows.Kinect.Joint;

public class BodySourceView : MonoBehaviour 
{
    public GameObject BodySourceManager;

    private Dictionary<ulong, PlayerBody> _Bodies = new Dictionary<ulong, PlayerBody>();
    private BodySourceManager _BodyManager;
    private Body[] bodies;

    private int interval = 1;
    private int maxInterval = 6;

    public Material redMaterial;
    public Material greenMaterial;
    public GameObject fireshield;
    
	void Update()
    {
        if (BodySourceManager == null)
        {
            return;
        }

        _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();
        if (_BodyManager == null)
        {
            return;
        }

        bodies = _BodyManager.getBodies();
        if (bodies == null)
        {
            return;
        }

        List<ulong> trackedIds = new List<ulong>();
        for (int i = 0; i < bodies.Length; i++)
        {
            var body = bodies[i];

            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                trackedIds.Add(body.TrackingId);
            }
        }

        // DELETE BODY BLOCK
        List<ulong> knownIds = new List<ulong>(_Bodies.Keys);

        // First delete untracked bodies
        foreach (ulong trackingId in knownIds)
        {
            if (!trackedIds.Contains(trackingId))
            {
                _Bodies[trackingId].DestroyBody();
                _Bodies.Remove(trackingId);
            }
        }
        // END

        // Delete BodyObject for untracked ID's
        // or 1000ms grace period to reattach to Kinect.Body

        for (int i = 0; i < bodies.Length; i++)
        {
            
            var body = bodies[i];

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
                TrackGesture(_Bodies[body.TrackingId]);
            }
        }
	}

    //TODO: Add capsule collider to body torso
    private PlayerBody CreateBodyObject(ulong id)
    {
        return new PlayerBody(id, greenMaterial, redMaterial);
    }

    //TODO: Update capsule collider to body torso
    private void RefreshBody(Body body, PlayerBody playerBody)
    {
        playerBody.RefreshBody(body);
    }

    private void TrackGesture(PlayerBody playerBody) {

        UnityBody unityBody= new UnityBody(playerBody.bodyObject);
        Dictionary<LigDir, Vector3> joints = unityBody.joints;

        Renderer rnd = gameObject.GetComponent<Renderer>();

        if (isTPose(joints, 0.50f))
        {
            playerBody.inGesture = true;
            playerBody.SetMaterial(fireshield);
        }
        else
        {
            playerBody.inGesture = false;
            playerBody.SetMaterial(fireshield);
        }
    }


    void OnGUI()
    {
        if (Event.current.Equals(Event.KeyboardEvent("#return")))
        {
            InvokeRepeating("CaptureBodyFrame", 1.0f, 1.0f);
        }
    }


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

            for (int i = 0; i < bodies.Length; i++)
            {
                var body = bodies[i];

                if (body == null)
                {
                    continue;
                }

                if (body.IsTracked)
                {
                    UnityBody unityBody = new UnityBody(_Bodies[body.TrackingId].bodyObject);
                    output = unityBody.ToFileString();
                }
            }
            File.AppendAllText(@"../CapturedBodyFrames.txt", output);

            Debug.Log("Captured");
            interval = 1;
            CancelInvoke("CaptureBodyFrame");
        }
    }

    public float dist(Vector3 v1, Vector3 v2)
    {
        return Vector3.Distance(v1, v2);
    }

    public bool isTPose(Dictionary<LigDir, Vector3> ligDirDict, float fudgeFactor)
    {
        foreach (KeyValuePair<LigDir, Vector3> pair in tPose)
        {
            if ( dist(pair.Value, ligDirDict[pair.Key]) > fudgeFactor )
            {
                return false;
            }
        }
        return true;
    }

    private Dictionary<LigDir, Vector3> tPose = new Dictionary<LigDir, Vector3>()
    {
        { LigDir.LeftUpperArm, new Vector3(-0.8246481f, 0.5656461f, 0) },
        { LigDir.LeftLowerArm, new Vector3(0f, 0f, 0) },
        { LigDir.LeftHand, new Vector3(0.8958837f, 0.4442886f, 0) },
        { LigDir.RightUpperArm, new Vector3(0.7573851f, 0.6529684f, 0) },
        { LigDir.RightLowerArm, new Vector3(0f, 0f, 0) },
        { LigDir.RightHand, new Vector3(-0.8720062f, 0.4894949f, 0) }
    };
}
