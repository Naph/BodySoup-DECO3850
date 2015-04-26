using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Kinect = Windows.Kinect;

public class BodySourceView : MonoBehaviour 
{
    public GameObject BodySourceManager;
    public Material matter;

    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
    private BodySourceManager _BodyManager;

    private int interval = 1;
    private int maxInterval = 6;

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

        Kinect.Body[] bodies = _BodyManager.getBodies();
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
                Destroy(_Bodies[trackingId]);
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
            }
        }
	}

    //TODO: Add capsule collider to body torso
    private GameObject CreateBodyObject(ulong id)
    {
        GameObject body = new GameObject("Body: " + id);

        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            GameObject jointRender = GameObject.CreatePrimitive(PrimitiveType.Quad);
            LineRenderer lr = jointRender.AddComponent<LineRenderer>();
            lr.enabled = false;
            lr.SetWidth(0.05f, 0.05f);
            jointRender.transform.localPosition = new Vector3(99, 99, 99);
            Renderer rnd = jointRender.gameObject.GetComponent<Renderer>();

            rnd.material = matter;
            
            jointRender.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            jointRender.name = jt.ToString();
            jointRender.transform.parent = body.transform;
        }

        return body;
    }

    //TODO: Udpate capsule collider to body torso
    private void RefreshBody(Kinect.Body body, GameObject bodyObject)
    {
        Dictionary<Kinect.JointType, List<Kinect.JointType>> boneMap = UnityBody.boneMap;

        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            Kinect.Joint sourceJoint = body.Joints[jt];

            if (boneMap.ContainsKey(jt))
            {
                List<Kinect.JointType> targetJoints = boneMap[jt];

                foreach (Kinect.JointType target in targetJoints)
                {
                    Transform jointObj = bodyObject.transform.FindChild(jt.ToString());
                    jointObj.localPosition = GetJointPosition(sourceJoint);
                    

                    LineRenderer lr = jointObj.GetComponent<LineRenderer>();
                    lr.SetPosition(0, jointObj.localPosition);
                    lr.SetPosition(1, GetJointPosition(body.Joints[target]));
                    lr.SetColors(Color.magenta, Color.magenta);
                    lr.enabled = true;
                }
            }
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
        string s = new string('.', interval*5);

        if (interval < 5)
        {
            Debug.Log("Capturing" + s);
            interval++;
        }
        else
        {
            using (StreamWriter file =
                new StreamWriter(@"..\CapturedBodyFrames.txt", true))
            {
                List<string> lines = new List<string>{"Dictionary<Vector3, UnityJoint> JointPositions = ",
                    "\tnew Dictionary<Vector3, UnityJoint>{"};
                
                Kinect.Body[] bodies = _BodyManager.getBodies();
                for (int i = 0; i < bodies.Length; i++)
                {
                    var body = bodies[i];

                    if (body == null)
                    {
                        continue;
                    }

                    if (body.IsTracked)
                    {
                        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
                        {
                            if (jt != Kinect.JointType.ThumbRight)
                            {
                                lines.Add("\t{ Vector3" + GetJointPosition(body.Joints[jt]) + ", JointType." + jt + " },");
                            }
                            else
                            {
                                lines.Add("\t{ Vector3" + GetJointPosition(body.Joints[jt]) + ", JointType." + jt + " }");
                                lines.Add("};");
                            }
                        }
                    }
                }
                
                foreach (string line in lines)
                    file.WriteLine(line);
            }

            Debug.Log("Captured");
            interval = 1;
            CancelInvoke("CaptureBodyFrame");
        }
    }


    private static Vector3 GetJointPosition(Kinect.Joint joint)
    {
        return new Vector3(joint.Position.X * 5, joint.Position.Y * 5, joint.Position.Z);
    }
}
