using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;

public class BodySourceView : MonoBehaviour 
{
    public GameObject BodySourceManager;
    public Material BoneMaterial;

    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();

    public List<ulong> ulongs;

    private BodySourceManager _BodyManager;

    public Material matter;

    void Start()
    {
        ulongs = new List<ulong>();
        ulongs.Add(0);
        ulongs.Add(0);
        ulongs.Add(0);
        ulongs.Add(0);
        ulongs.Add(0);
        ulongs.Add(0);
    }

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
                //Debug.Log("body - " + i);
                ulongs[i] = body.TrackingId;
            }
        }

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
        // Delete BodyObject for untracked ID's
        // or 1000ms grace period to reattach to Kinect.Body
        int y = 0;

        for (int i = 0; i < bodies.Length; i++)
        {
            y++;
            
            var body = bodies[i];

            if (body == null)
            {
                continue;
            }
            //Debug.Log("bodies take two - " + i);
            if (body.IsTracked)
            {
                if (!_Bodies.ContainsKey(body.TrackingId))
                {
                    _Bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                }
                //Debug.Log(body.TrackingId);
                RefreshBody(body, _Bodies[body.TrackingId]);
            }
        }
       // Debug.Log(y);
	}

    //TODO: Add capsule collider to body torso
    private GameObject CreateBodyObject(ulong id)
    {
        Debug.Log(id);
        GameObject body = new GameObject("Body: " + id);

        //Dictionary<Kinect.JointType, Kinect.JointType> boneMap = UnityBody.boneMap;// unityBody.boneMap;

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
        //UnityBody unityBody = new UnityBody();
        Dictionary<Kinect.JointType, List<Kinect.JointType>> boneMap = UnityBody.boneMap;// unityBody.boneMap;

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

            //Kinect.Joint sourceJoint = body.Joints[jt];
            //List<Kinect.Joint> targetJoint = null;
            
            //if(boneMap.ContainsKey(jt))
            //{
                //targetJoint = body.Joints[boneMap[jt]];
            //}
            
           /* Transform jointObj = bodyObject.transform.FindChild(jt.ToString());
            jointObj.localPosition = GetJointPosition(sourceJoint);
            
            LineRenderer lr = jointObj.GetComponent<LineRenderer>();
            if(targetJoint.HasValue)
            {
                lr.SetPosition(0, jointObj.localPosition);
                lr.SetPosition(1, GetJointPosition(targetJoint.Value));
                lr.SetColors(Color.magenta, Color.magenta);
                lr.enabled = true;
            }
            else
            {
                lr.enabled = false;
            }*/
        }
    }
    

    private static Vector3 GetJointPosition(Kinect.Joint joint)
    {
        return new Vector3(joint.Position.X * 5, joint.Position.Y * 5, joint.Position.Z);
    }
}
