using UnityEngine;
using System;
using System.Collections.Generic;
using Windows.Kinect;
using KinectJoint = Windows.Kinect.Joint;

public class PlayerBody {

    public bool inGesture = false;
    private Gesture.SubGesture prevGesture;
    public Material redMaterial;
    public Material greenMaterial;
    
    public GameObject bodyObject;
    private GameObject activeEffect;
    private Transform activeJoint;
    private List<string> effectRotation;
    private bool pairedToJoint;

    private float chargeTimer = 1f;
    private float chargeMagnitude = 0.05f;

    private List<GameObject> renderedJoints;
    private List<string> nonRender = new List<string>(new string[] { 
        "ElbowLeft", "ElbowRight", "WristLeft", "WristRight" });

    private Transform ShoulderLeft;
    private Transform ElbowLeft;
    private Transform WristLeft;
    private Transform HandLeft;

    private Transform ShoulderRight;
    private Transform ElbowRight;
    private Transform WristRight;
    private Transform HandRight;

    private Vector3 RightUpperArm;
    private Vector3 RightLowerArm;
    private Vector3 RightHand;

    private Vector3 LeftUpperArm;
    private Vector3 LeftLowerArm;
    private Vector3 LeftHand;

    private Gesture currentGesture;
    private int currentSubGesture;

    private Dictionary<LigDir, Vector3> jointPositions;

    private int layer;

    public PlayerBody(ulong id, Material green, Material red)
    {
        this.bodyObject = new GameObject("Body: " + id);
        this.renderedJoints = new List<GameObject>();
        this.effectRotation = new List<string>();

        redMaterial = red;
        greenMaterial = green;

        AddJoints();
    }
    

    private void AddJoints()
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

            jointRender.transform.parent = bodyObject.transform;

            renderedJoints.Add(jointRender);
        }
    }


    public void RefreshBody(Body body)
    {
        // Refresh visual representation of Kinect body
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

                    Renderer rnd = jointObj.gameObject.GetComponent<Renderer>();
                    rnd.material = redMaterial;

                    if(currentGesture != null)
                    {
                        if (ComparePosition(currentGesture.subGestures[currentSubGesture]))
                        {
                            rnd.material = greenMaterial;
                        }
                        else
                        {
                            rnd.material = redMaterial;
                        }
                    }
                    else
                    {
                    }

                    LineRenderer lr = jointObj.GetComponent<LineRenderer>();
                    lr.SetPosition(0, jointObj.localPosition);
                    lr.SetPosition(1, GetJointVector3(body.Joints[target]));
                    lr.SetColors(Color.magenta, Color.magenta);
                    lr.enabled = true;
                }
            }
        }

        UpdateGesture();

        if (activeEffect != null && activeJoint != null && pairedToJoint)
        {
            UpdateGesturePosition();
        }

        // Local positions of left joints
        ShoulderLeft  = bodyObject.transform.FindChild("ShoulderLeft");
        ElbowLeft     = bodyObject.transform.FindChild("ElbowLeft");
        WristLeft     = bodyObject.transform.FindChild("WristLeft");
        HandLeft      = bodyObject.transform.FindChild("HandLeft");

        // Local positions of right joints
        ShoulderRight = bodyObject.transform.FindChild("ShoulderRight");
        ElbowRight    = bodyObject.transform.FindChild("ElbowRight");
        WristRight    = bodyObject.transform.FindChild("WristRight");
        HandRight     = bodyObject.transform.FindChild("HandRight");

        // Update Right Orientations
        RightUpperArm = (ElbowRight.localPosition - ShoulderRight.localPosition).normalized;
        RightLowerArm = (WristRight.localPosition - ElbowRight.localPosition).normalized;
        RightHand     = (HandRight.localPosition - WristRight.localPosition).normalized;

        // Update Left Orientations
        LeftUpperArm  = (ElbowLeft.localPosition - ShoulderLeft.localPosition).normalized;
        LeftLowerArm  = (WristLeft.localPosition - ElbowLeft.localPosition).normalized;
        LeftHand      = (HandLeft.localPosition - WristLeft.localPosition).normalized;

        InstantiateDict();
    }

    private void InstantiateDict()
    {
        jointPositions = new Dictionary<LigDir, Vector3>()
        {
            {LigDir.LeftUpperArm, LeftUpperArm},
            {LigDir.LeftLowerArm, LeftLowerArm},
            {LigDir.LeftHand, LeftHand},
            {LigDir.RightUpperArm, RightUpperArm},
            {LigDir.RightLowerArm, RightLowerArm},
            {LigDir.RightHand, RightHand}
        };
    }

    public void StartGesture(Gesture gesture)
    {
        if (currentGesture != null)
        {
            if (activeEffect != null)
            {
                if (currentGesture != gesture)
                {
                    activeJoint = null;
                    MonoBehaviour.Destroy(activeEffect);
                    ExecuteGesture(gesture, 0);
                }
                else
                {
                    if (currentSubGesture != 0 && gesture.repeatable)
                    {
                        ExecuteGesture(gesture, 0);
                    }
                }
            }
        }
        
        if (currentGesture == null)
        {
            ExecuteGesture(gesture, 0);
        }
    }


    private void ExecuteGesture(Gesture gesture, int index)
    {
        currentGesture = gesture;
        currentSubGesture = index;

        SetEffect(gesture.subGestures[index].effect,
                  gesture.subGestures[index].jointTracked,
                  gesture.subGestures[index].directionOrigin,
                  gesture.subGestures[index].pairedToJoint,
                  gesture.subGestures[index].timeout);
    }


    public void SetEffect(GameObject effect, String joint, String origin, bool paired, float timeout)
    {
        pairedToJoint = paired;
        activeJoint = this.bodyObject.transform.FindChild(joint);
        effectRotation.Clear();

        activeEffect = (GameObject)MonoBehaviour.Instantiate(effect, new Vector3(0f, 999f, 0f), Quaternion.identity);
        MonoBehaviour.Destroy(activeEffect, timeout);

        if (currentSubGesture + 1 == currentGesture.count)
        {
            if (activeEffect.GetComponent<ParticleSystem>() != null)
            {
                activeEffect.GetComponent<ParticleSystem>().startSize *= chargeTimer;
                activeEffect.GetComponent<ParticleSystem>().Play();
            }
        }

        chargeTimer = 1f;

        if (!pairedToJoint)
        {
            UpdateGesturePosition();
        }

        if (!origin.Equals("Unset"))
        {
            effectRotation.Insert(0, origin);
            effectRotation.Insert(1, joint);

            EffectRotation();
        }
    }

    // ambidexterity means currentGesture.count - 1 == currentGesture.count - 2 
    public void UpdateGesture()
    {
        if (currentGesture != null)
        {
            if (currentSubGesture + 1 < currentGesture.count)
            {
                if (ComparePosition(currentGesture.subGestures[currentSubGesture + 1]))
                {
                    // Entered next SubGesture in current Gesture
                    MonoBehaviour.Destroy(activeEffect);
                    currentSubGesture++;
                    ExecuteGesture(currentGesture, currentSubGesture);
                }

                if (ComparePosition(currentGesture.subGestures[currentSubGesture]))
                {
                    // Holding SubGesture position
                    chargeTimer += chargeMagnitude;
                }
            }
            else
            {
                if (ComparePosition(currentGesture.subGestures[currentSubGesture]))
                {
                    currentGesture = null;
                }
            }
        }

        if (activeEffect == null)
        {
            currentGesture = null;
            currentSubGesture = 0;
            chargeTimer = 1f;
        }
    }


    private void UpdateGesturePosition()
    {
        Vector3 pos = activeJoint.position;
        activeEffect.transform.position = pos;

        if (effectRotation.Count > 1)
        {
            EffectRotation();
        }
    }
    

    public bool ComparePosition(Gesture.SubGesture gesture)
    {
        if ( currentGesture != null &&
            gesture == currentGesture.subGestures[currentSubGesture] &&
            currentGesture.ambidexterity &&
            currentSubGesture == currentGesture.count - 2) 
        {
            foreach (KeyValuePair<LigDir, Vector3> pair in this.jointPositions)
            {
                if (dist(pair.Value, gesture.position[pair.Key]) > gesture.fudgeFactor ||
                    dist(pair.Value, currentGesture.subGestures[currentSubGesture + 1].position[pair.Key]) > gesture.fudgeFactor)
                {
                    return false;
                }
            }
        }


        foreach (KeyValuePair<LigDir, Vector3> pair in this.jointPositions)
        {
            if (dist(pair.Value, gesture.position[pair.Key]) > gesture.fudgeFactor)
            {
                return false;
            }
        }

        return true;
    }


    public void EffectRotation()
    {
        Vector3 from = bodyObject.transform.FindChild(effectRotation[0]).localPosition;
        Vector3 to = bodyObject.transform.FindChild(effectRotation[1]).localPosition;

        Vector3 pos = (from - to).normalized;
        Vector3 rotation = pos * 90;

        Vector3 effect = activeEffect.transform.localPosition;
        effect.y += pos.y * -1.05f;
        effect.x += pos.x * -1.05f;

        if (from.y > to.y)
        {
            rotation.y = 90f;
            rotation.x = rotation.x + 90f;
        }
        else
        {
            rotation.y = -90f;
            rotation.x = rotation.x - 90f;
        }

        activeEffect.transform.rotation = Quaternion.Euler(rotation);
        activeEffect.transform.localPosition = effect;
    }


    public void DestroyBody()
    {
        MonoBehaviour.Destroy(this.bodyObject);
        MonoBehaviour.Destroy(this.activeEffect);
    }


    private static Vector3 GetJointVector3(KinectJoint joint)
    {
        return new Vector3(joint.Position.X * 5, joint.Position.Y * 5, 5);
    }


    private float dist(Vector3 v1, Vector3 v2)
    {
        if (v2 == Vector3.zero)
        {
            return 0;
        }

        return Vector3.Distance(v1, v2);
    }


    public string ToFileString()
    {
        string output = "New Pose Recorded: " + System.DateTime.Now + Environment.NewLine;

        foreach (KeyValuePair<LigDir, Vector3> pair in jointPositions)
        {
            if (pair.Key != LigDir.RightHand)
            {
                output +=
                    "\t\t{ LigDir." + pair.Key +
                    ", new Vector3(" + pair.Value.x + "f, " + pair.Value.y + "f, " + pair.Value.z + "f) }," + Environment.NewLine;
            }
            else
            {
                output +=
                    "\t\t{ LigDir." + pair.Key +
                    ", new Vector3(" + pair.Value.x + "f, " + pair.Value.y + "f, " + pair.Value.z + "f) }" + Environment.NewLine;
            }
        }

        return output;
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
        
            //{ JointType.HandTipLeft, new List<JointType>(){JointType.HandLeft} },
            //{ JointType.ThumbLeft, new List<JointType>(){JointType.HandLeft} },
            { JointType.HandLeft, new List<JointType>(){JointType.WristLeft} },
            { JointType.WristLeft, new List<JointType>(){JointType.ElbowLeft} },
            { JointType.ElbowLeft, new List<JointType>(){JointType.ShoulderLeft} },
            { JointType.ShoulderLeft, new List<JointType>(){JointType.SpineShoulder, JointType.HipLeft } },
        
            //{ JointType.HandTipRight, new List<JointType>(){JointType.HandRight} },
            //{ JointType.ThumbRight, new List<JointType>(){JointType.HandRight} },
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
