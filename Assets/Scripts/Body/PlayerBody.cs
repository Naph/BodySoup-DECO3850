using UnityEngine;
using System;
using System.Collections.Generic;
using Windows.Kinect;
using KinectJoint = Windows.Kinect.Joint;
using SubGesture = Gesture.SubGesture;

public class PlayerBody {

    public Material redMaterial;
    public Material greenMaterial;
    public Material whiteMaterial;
    
    public GameObject bodyObject;
    private Dictionary<Gesture, ActiveEffect> activeEffects;

    private Gesture currentGesture;
    private int IcurrentSubGesture = 0;
    public Gesture.SubGesture theFinisher;

    private List<GameObject> renderedJoints;
    private List<string> nonRender = new List<string>(new string[] { 
        "ElbowLeft", "ElbowRight", "WristLeft", "WristRight",
        "KneeLeft", "KneeRight", "FootLeft", "FootRight", 
        "AnkleLeft", "AnkleRight", 
        "HandTipLeft", "HandTipRight", "ThumbLeft", "ThumbRight"});

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

    private Dictionary<LigDir, Vector3> jointPositions;


    public PlayerBody(ulong id, Material green, Material red, Material white)
    {
        this.bodyObject = new GameObject("Body: " + id);
        this.renderedJoints = new List<GameObject>();
        this.activeEffects = new Dictionary<Gesture, ActiveEffect>();

        redMaterial = red;
        greenMaterial = green;
        whiteMaterial = white;

        AddJoints();
    }

    public PlayerBody(string name, Material green, Material red, Material white)
    {
        this.bodyObject = new GameObject("Body: " + name);
        this.renderedJoints = new List<GameObject>();
        this.activeEffects = new Dictionary<Gesture, ActiveEffect>();

        redMaterial = red;
        greenMaterial = green;
        whiteMaterial = white;

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
            lr.material = whiteMaterial;
            jointRender.transform.parent = bodyObject.transform;

            renderedJoints.Add(jointRender);
        }
    }


    private void RefreshJoints(float[] floats)
    {
        int current = 0;

        for (JointType jt = JointType.SpineBase; jt <= JointType.ThumbRight; jt++)
        {
            Transform jointObj = this.bodyObject.transform.FindChild(jt.ToString());

            jointObj.localPosition = GetJointVector3(floats[current], floats[current + 1], floats[current + 2]);
            current = current + 3;
            jointObj.gameObject.SetActive(false);
        }

        for (JointType jt = JointType.SpineBase; jt <= JointType.ThumbRight; jt++)
        {
            Transform sourceTranform = this.bodyObject.transform.FindChild(jt.ToString());
            //sourceTranform.gameObject.SetActive(true);
            if (renderBoneMap.ContainsKey(jt))
            {
                List<JointType> targetJoints = renderBoneMap[jt];
                sourceTranform.gameObject.SetActive(true);
                foreach (JointType target in targetJoints)
                {
                    Transform targetTransform = this.bodyObject.transform.FindChild(target.ToString());
                    targetTransform.gameObject.SetActive(true);
                    Renderer rnd = sourceTranform.GetComponent<Renderer>();
                    rnd.material = redMaterial;

                    if (currentGesture != null && IcurrentSubGesture > 0)
                    {
                        if (ComparePosition(currentGesture.subGestures[IcurrentSubGesture - 1]))
                        {
                            rnd.material = greenMaterial;
                        }
                        else
                        {
                            rnd.material = redMaterial;
                        }
                    }

                    LineRenderer lr = sourceTranform.GetComponent<LineRenderer>();
                    lr.SetPosition(0, sourceTranform.localPosition);
                    lr.SetPosition(1, targetTransform.localPosition);
                    
                    lr.SetColors(Color.cyan, Color.cyan);
                    lr.enabled = true;
                }
            }
        }
    }


    private void RefreshJoints(Body body)
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

                    if (currentGesture != null && IcurrentSubGesture != 0)
                    {
                        if (ComparePosition(currentGesture.subGestures[IcurrentSubGesture - 1]))
                        {
                            rnd.material = greenMaterial;
                        }
                        else
                        {
                            rnd.material = redMaterial;
                        }
                    }

                    LineRenderer lr = jointObj.GetComponent<LineRenderer>();
                    lr.SetPosition(0, jointObj.localPosition);
                    lr.SetPosition(1, GetJointVector3(body.Joints[target]));
                    
                    lr.SetColors(Color.green, Color.green);
                    lr.enabled = true;
                }
            }
        }
    }


    public void RefreshBody(float[] floats)
    {
        // Local positions of left joints
        ShoulderLeft = bodyObject.transform.FindChild("ShoulderLeft");
        ElbowLeft = bodyObject.transform.FindChild("ElbowLeft");
        WristLeft = bodyObject.transform.FindChild("WristLeft");
        HandLeft = bodyObject.transform.FindChild("HandLeft");

        // Local positions of right joints
        ShoulderRight = bodyObject.transform.FindChild("ShoulderRight");
        ElbowRight = bodyObject.transform.FindChild("ElbowRight");
        WristRight = bodyObject.transform.FindChild("WristRight");
        HandRight = bodyObject.transform.FindChild("HandRight");

        // Update Right Orientations
        RightUpperArm = (ElbowRight.localPosition - ShoulderRight.localPosition).normalized;
        RightLowerArm = (WristRight.localPosition - ElbowRight.localPosition).normalized;
        RightHand = (HandRight.localPosition - WristRight.localPosition).normalized;

        // Update Left Orientations
        LeftUpperArm = (ElbowLeft.localPosition - ShoulderLeft.localPosition).normalized;
        LeftLowerArm = (WristLeft.localPosition - ElbowLeft.localPosition).normalized;
        LeftHand = (HandLeft.localPosition - WristLeft.localPosition).normalized;

        InstantiateDict();

        RefreshJoints(floats);

        if (currentGesture != null && IcurrentSubGesture > 0) UpdateGesture();

        UpdateEffect();

        SweepActiveEffects();
    }


    public void RefreshBody(Body body)
    {
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

        RefreshJoints(body);

        if (currentGesture != null && IcurrentSubGesture > 0) UpdateGesture();

        UpdateEffect();

        SweepActiveEffects();
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


    /// <summary>
    /// Start a new Gesture
    /// Called from BodySourceView when a body enters the first position
    /// of any gesture.
    /// </summary>
    /// <param name="gesture">Gesture this PlayerBody has entered</param>
    public void StartGesture(Gesture gesture)
    {
        if (currentGesture != null)
        {
            if (currentGesture != gesture && IcurrentSubGesture == 0)
            {
                ExecuteGesture(gesture, 0);
            }
        }
        else
        {
            ExecuteGesture(gesture, 0);
        }
    }


    private void ExecuteGesture(Gesture gesture, int index)
    {
        currentGesture = gesture;
        IcurrentSubGesture = index;
        
        ActiveEffect newEffect = ActiveEffectBuilder(gesture.subGestures[index]);
        
        if (IcurrentSubGesture == 0 && !activeEffects.ContainsKey(gesture))
        {
            // New
            Debug.Log("New");

            newEffect.Activate();
            activeEffects.Add(gesture, newEffect);
            IcurrentSubGesture++;
        }
        else if (0 < IcurrentSubGesture && IcurrentSubGesture < gesture.count - 1
                 && activeEffects.ContainsKey(gesture))
        {
            // Existing
            Debug.Log("Existing");

            RemoveInactiveEffects(gesture);
            newEffect.Activate();
            activeEffects[gesture] = newEffect;
            IcurrentSubGesture++;
        }
        else if (IcurrentSubGesture == gesture.count - 1
                 && activeEffects.ContainsKey(gesture))
        {
            // Last
            Debug.Log("Last");

            RemoveInactiveEffects(gesture);
            newEffect.Activate();
            activeEffects[gesture] = newEffect;
            IcurrentSubGesture++;
        }
    }


    public void UpdateGesture()
    {
        // IcurrentSubGesture > 0

        Debug.Log(IcurrentSubGesture);

        if (!ComparePosition(currentGesture.subGestures[IcurrentSubGesture - 1]))
        {
            // Not in previous recorded pose
            if (currentGesture.subGestures.Length > IcurrentSubGesture)
            {
                // Index is in bounds
                SubGesture nextPosition = currentGesture.subGestures[IcurrentSubGesture];

                if (nextPosition.isFinisher)
                {
                    SubGesture position = CompareAndGetPosition(currentGesture.getFinishers);
                    
                    if (position != null)
                    {
                        activeEffects[currentGesture] = ActiveEffectBuilder(position);
                        ExecuteGesture(currentGesture, currentGesture.IndexOf(position));
                    }
                }
                else
                {
                    Debug.Log("Fired");
                    if (ComparePosition(nextPosition))
                    {
                        GameObject effect = activeEffects[currentGesture].effect;
                        activeEffects[currentGesture] = ActiveEffectBuilder(nextPosition);
                        MonoBehaviour.Destroy(effect);

                        ExecuteGesture(currentGesture, IcurrentSubGesture);
                    }
                }
            }
        }
        else
        {
            if (!activeEffects.ContainsKey(currentGesture))
            {
                ExecuteGesture(currentGesture, IcurrentSubGesture);
            }
        }
    }


    private void UpdateEffect()
    {
        foreach (KeyValuePair<Gesture, ActiveEffect> pair in activeEffects) {

            if (pair.Value.effect != null)
            {
                pair.Value.UpdateEffect(bodyObject);
            }
            else
            {
                RemoveActiveEffect(pair.Key);
                break;
            }
        }
    }
    

    public bool ComparePosition(SubGesture gesture)
    {
        foreach (KeyValuePair<LigDir, Vector3> pair in this.jointPositions)
        {
            if (dist(pair.Value, gesture.position[pair.Key]) > gesture.fudgeFactor)
            {
                return false;
            }
        }

        return true;
    }


    public SubGesture CompareAndGetPosition(List<SubGesture> gestures)
    {
        for (int i = 0; i < gestures.Count; i++)
        {
            if (ComparePosition(gestures[i]))
            {
                return gestures[i];
            }
        }

        return null;
    }


    public void DestroyBody()
    {
        if (activeEffects.Count > 0)
        {
            foreach (KeyValuePair<Gesture, ActiveEffect> effect in activeEffects)
            {
                effect.Value.Destroy();
            }
            activeEffects.Clear();
        }

        MonoBehaviour.Destroy(this.bodyObject);
    }


    private ActiveEffect ActiveEffectBuilder(SubGesture subGesture)
    {
        return new ActiveEffect(
            subGesture.effect,
            subGesture.jointTracked,
            this.bodyObject.transform.FindChild(subGesture.jointTracked).position,
            subGesture.directionOrigin,
            subGesture.isFinisher,
            subGesture.pairedToJoint,
            subGesture.timeout);
    }


    private void SweepActiveEffects()
    {
        foreach (KeyValuePair<Gesture, ActiveEffect> pair in activeEffects)
        {
            if (pair.Value.effect == null)
            {
                RemoveActiveEffect(pair.Key);
            }
        }
    }

    
    private void RemoveInactiveEffects(Gesture gesture)
    {
        if (activeEffects.ContainsKey(gesture))
        {
            activeEffects.Remove(gesture);
        }
    }
    

    private void RemoveActiveEffect(Gesture gesture)
    {
        if (gesture == currentGesture)
        {
            currentGesture = null;
            IcurrentSubGesture = 0;
        }
        activeEffects.Remove(gesture);
    }


    private static Vector3 GetJointVector3(KinectJoint joint)
    {
        return new Vector3(joint.Position.X * 5, joint.Position.Y * 5, 5);
    }


    private static Vector3 GetJointVector3(float x, float y, float z)
     {
         return new Vector3(x * 5, y * 5, 5);
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
            { JointType.FootLeft,       new List<JointType>(){JointType.AnkleLeft} },
            { JointType.AnkleLeft,      new List<JointType>(){JointType.KneeLeft} },
            { JointType.KneeLeft,       new List<JointType>(){JointType.HipLeft} },
            { JointType.HipLeft,        new List<JointType>(){JointType.SpineBase} },
        
            { JointType.FootRight,      new List<JointType>(){JointType.AnkleRight} },
            { JointType.AnkleRight,     new List<JointType>(){JointType.KneeRight} },
            { JointType.KneeRight,      new List<JointType>(){JointType.HipRight} },
            { JointType.HipRight,       new List<JointType>(){JointType.SpineBase} },
        
            { JointType.HandTipLeft,    new List<JointType>(){JointType.HandLeft} },
            { JointType.ThumbLeft,      new List<JointType>(){JointType.HandLeft} },
            { JointType.HandLeft,       new List<JointType>(){JointType.WristLeft} },
            { JointType.WristLeft,      new List<JointType>(){JointType.ElbowLeft} },
            { JointType.ElbowLeft,      new List<JointType>(){JointType.ShoulderLeft} },
            { JointType.ShoulderLeft,   new List<JointType>(){JointType.SpineShoulder, JointType.HipLeft } },
        
            { JointType.HandTipRight,   new List<JointType>(){JointType.HandRight} },
            { JointType.ThumbRight,     new List<JointType>(){JointType.HandRight} },
            { JointType.HandRight,      new List<JointType>(){JointType.WristRight} },
            { JointType.WristRight,     new List<JointType>(){JointType.ElbowRight} },
            { JointType.ElbowRight,     new List<JointType>(){JointType.ShoulderRight} },
            { JointType.ShoulderRight,  new List<JointType>(){JointType.SpineShoulder, JointType.HipRight} },
        
            { JointType.SpineBase,      new List<JointType>(){JointType.SpineMid} },
            { JointType.SpineMid,       new List<JointType>(){JointType.Neck} },
            { JointType.SpineShoulder,  new List<JointType>(){JointType.Neck} },
            { JointType.Neck,           new List<JointType>(){JointType.Neck} }
        };
}
