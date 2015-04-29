using UnityEngine;
using System.Collections.Generic;
using Windows.Kinect;
using KinectJoint = Windows.Kinect.Joint;

public class PlayerBody : MonoBehaviour {

    public bool inGesture = false;
    private bool prev = true;
    public Material redMaterial;
    public Material greenMaterial;

    public GameObject bodyObject;
    private GameObject activeEffect;

    private List<GameObject> renderedJoints;
    private List<string> nonRender = new List<string>(new string[] { 
        "ElbowLeft", "ElbowRight", "WristLeft", "WristRight" });

    public PlayerBody(ulong id, Material green, Material red)
    {
        this.bodyObject = new GameObject("Body: " + id);
        
        this.renderedJoints = new List<GameObject>();

        redMaterial = red;
        greenMaterial = green;

        AddJoints();
    }

    public void SpawnEffect(GameObject effect)
    {;
    }


    public void AddJoints()
    {
        for (JointType jt = JointType.SpineBase; jt <= JointType.ThumbRight; jt++)
        {
            // Joint render
            GameObject jointRender = GameObject.CreatePrimitive(PrimitiveType.Cube);
            jointRender.transform.localPosition = new Vector3(99, 99, 99);
            jointRender.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            jointRender.name = jt.ToString();

            // Disabled joint renders
            if (nonRender.Contains(jt.ToString()))
            {
                jointRender.SetActive(false);
            }

            // Line renders
            LineRenderer lr = jointRender.AddComponent<LineRenderer>();
            lr.enabled = false;
            lr.SetWidth(0.05f, 0.05f);

            Renderer rnd = jointRender.gameObject.GetComponent<Renderer>();
            rnd.material = redMaterial;

            jointRender.transform.parent = bodyObject.transform;

            renderedJoints.Add(jointRender);
        }
    }


    public void RefreshBody(Body body)
    {
        for (JointType jt = JointType.SpineBase; jt <= JointType.ThumbRight; jt++)
        {
            KinectJoint sourceJoint = body.Joints[jt];

            if (renderBoneMap.ContainsKey(jt))
            {
                List<JointType> targetJoints = renderBoneMap[jt];

                foreach (JointType target in targetJoints)
                {
                    Transform jointObj = this.bodyObject.transform.FindChild(jt.ToString());
                    jointObj.localPosition = GetJointVector3(sourceJoint);

                    LineRenderer lr = jointObj.GetComponent<LineRenderer>();
                    lr.SetPosition(0, jointObj.localPosition);
                    lr.SetPosition(1, GetJointVector3(body.Joints[target]));
                    lr.SetColors(Color.magenta, Color.magenta);
                    lr.enabled = true;
                }
            }
        }

        if (activeEffect != null)
        {
            Transform jointObj = this.bodyObject.transform.FindChild("SpineMid");
            Vector3 pos = jointObj.position;
            activeEffect.transform.position = pos;
        }
    }


    public void SetMaterial(GameObject effect)
    {
        if (inGesture == prev)
        {
            if (inGesture)
            {
                activeEffect = (GameObject)Instantiate(effect, new Vector3(0f, 0f, 0f), Quaternion.identity);

                foreach (GameObject cuberj in renderedJoints)
                {
                    Renderer renderer = cuberj.GetComponent<Renderer>();
                    renderer.material = greenMaterial;
                }
                prev = false;
            }
            else
            {
                foreach (GameObject cuberj in renderedJoints)
                {
                    Renderer renderer = cuberj.GetComponent<Renderer>();
                    renderer.material = redMaterial;

                }

                Destroy(activeEffect);
                activeEffect = null;
                prev = true;
            }
        }
    }


    public void DestroyBody()
    {
        Destroy(this.bodyObject);
    }


    private static Vector3 GetJointVector3(KinectJoint joint)
    {
        return new Vector3(joint.Position.X * 5, joint.Position.Y * 5, 5);
    }


    private static Dictionary<JointType, List<JointType>> renderBoneMap =
        new Dictionary<JointType, List<JointType>>
        {
            //{ JointType.FootLeft, new List<JointType>(){JointType.AnkleLeft} },
            //{ JointType.AnkleLeft, JointType.KneeLeft },
            //{ JointType.KneeLeft, JointType.HipLeft },
            { JointType.HipLeft, new List<JointType>(){JointType.SpineBase} },
        
            //{ JointType.FootRight, new List<JointType>(){JointType.AnkleRight} },
            //{ JointType.AnkleRight, JointType.KneeRight },
            //{ JointType.KneeRight, JointType.HipRight },
            { JointType.HipRight, new List<JointType>(){JointType.SpineBase} },
        
            { JointType.HandTipLeft, new List<JointType>(){JointType.HandLeft} },
            { JointType.ThumbLeft, new List<JointType>(){JointType.HandLeft} },
            { JointType.HandLeft, new List<JointType>(){JointType.WristLeft} },
            { JointType.WristLeft, new List<JointType>(){JointType.ElbowLeft} },
            { JointType.ElbowLeft, new List<JointType>(){JointType.ShoulderLeft} },
            { JointType.ShoulderLeft, new List<JointType>(){JointType.SpineShoulder, JointType.HipLeft } },
        
            { JointType.HandTipRight, new List<JointType>(){JointType.HandRight} },
            { JointType.ThumbRight, new List<JointType>(){JointType.HandRight} },
            { JointType.HandRight, new List<JointType>(){JointType.WristRight} },
            { JointType.WristRight, new List<JointType>(){JointType.ElbowRight} },
            { JointType.ElbowRight, new List<JointType>(){JointType.ShoulderRight} },
            { JointType.ShoulderRight, new List<JointType>(){JointType.SpineShoulder, JointType.HipRight} },
        
            { JointType.SpineBase, new List<JointType>(){JointType.SpineMid} },
            { JointType.SpineMid, new List<JointType>(){JointType.Neck} },
            //{ JointType.SpineShoulder, JointType.Neck },
            { JointType.Neck, new List<JointType>(){JointType.Neck} }
        };
}
