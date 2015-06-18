using UnityEngine;
using System;
using System.Collections.Generic;
using Windows.Kinect;
using KinectJoint = Windows.Kinect.Joint;
using SubGesture = Gesture.SubGesture;

public class PlayerBody {

    private static float kinectDisplacement;
    public Material redMaterial;
    public Material greenMaterial;
    public Material whiteMaterial;

    public GameObject bodyObject;
    private Dictionary<Gesture, ActiveEffect> activeEffects;

    private Gesture currentGesture;
    private SubGesture currentSubGesture;
    private int currentGestureStep = 0;
    private int repeat = 0;

    private List<GameObject> renderedJoints;
    private List<string> nonRender = new List<string>(new string[] {
        "ElbowLeft", "ElbowRight", "WristLeft", "WristRight",
        "KneeLeft", "KneeRight", "FootLeft", "FootRight", 
        "AnkleLeft", "AnkleRight", "Head", "SpineShoulder", 
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

    private float mushroomWidth = 0.05f;

    private Dictionary<LigDir, Vector3> jointPositions;


    public PlayerBody(ulong id, Material green, Material red, Material white, float kinectDisplacement)
    {
        this.bodyObject = new GameObject("Body: " + id);
        this.renderedJoints = new List<GameObject>();
        this.activeEffects = new Dictionary<Gesture, ActiveEffect>();
        PlayerBody.kinectDisplacement = kinectDisplacement;

        redMaterial = red;
        greenMaterial = green;
        whiteMaterial = white;

        AddJoints();
    }

    public PlayerBody(string name, Material green, Material red, Material white, float kinectDisplacement)
    {
        this.bodyObject = new GameObject("Body: " + name);
        this.renderedJoints = new List<GameObject>();
        this.activeEffects = new Dictionary<Gesture, ActiveEffect>();
        PlayerBody.kinectDisplacement = kinectDisplacement;

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
            jointRender.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
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
            lr.SetColors(Color.white, Color.white);
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
            Transform sourceTransform = this.bodyObject.transform.FindChild(jt.ToString());
            
            if (renderBoneMap.ContainsKey(jt))
            {
                List<JointType> targetJoints = renderBoneMap[jt];

                foreach (JointType target in targetJoints)
                {

                    if (nonRender.Contains(target.ToString()))
                    {
                        continue;
                    }

                    Transform targetTransform = this.bodyObject.transform.FindChild(target.ToString());

                    Renderer rnd = sourceTransform.GetComponent<Renderer>();
                    rnd.material = redMaterial;

                    if (currentGesture != null && currentGestureStep > 0)
                    {
                        if (ComparePosition(currentGesture.subGestures[currentGestureStep - 1]))
                        {
                            rnd.material = greenMaterial;
                        }
                        else
                        {
                            rnd.material = redMaterial;
                        }
                    }

                    targetTransform.gameObject.SetActive(true);

                    Renderer rnd2 = targetTransform.GetComponent<Renderer>();
                    rnd2.material = redMaterial;

                    LineRenderer lr = sourceTransform.GetComponent<LineRenderer>();
                    lr.SetPosition(0, sourceTransform.localPosition);
                    lr.SetPosition(1, targetTransform.localPosition);

                    //lr.SetColors(Color.cyan, Color.cyan);
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

                    if (currentGesture != null && currentGestureStep != 0)
                    {
                        if (ComparePosition(currentGesture.subGestures[currentGestureStep - 1]))
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
                    
                    //lr.SetColors(Color.green, Color.green);
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

        UpdateEffect();

        InstantiateDict();

        RefreshJoints(floats);

        if (currentGesture != null && currentGestureStep > 0)
            SubGestureListener();

        //SweepActiveEffects();
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

        //SweepActiveEffects();

        UpdateEffect();

        InstantiateDict();

        RefreshJoints(body);

        if (currentGesture != null && currentGestureStep > 0)
            SubGestureListener();

        Debug.Log(activeEffects.Count);
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
            if (currentGesture != gesture)
            {
                if (currentSubGesture.timeout > 1)
                {
                    ExecuteGesture(gesture, 0);
                }
            }
        }
        else
        {
            ExecuteGesture(gesture, 0);
        }
    }


    private void ExecuteGesture(Gesture gesture, int index)
    {
        SubGesture newGesture = gesture.subGestures[index];
        ActiveEffect newEffect = ActiveEffectBuilder(newGesture);
        
        if (!ReferenceEquals(null, currentSubGesture))
        {
            if ((newGesture.isFinisher
                && !currentSubGesture.isFinisher
                && gesture.IndexOf(currentSubGesture) < 0) || 
                (activeEffects.ContainsKey(gesture)
                && !gesture.hasMultipleSteps))
            {
                return;
            }
        }

        currentGesture = gesture;
        currentGestureStep = index;

            // New
        if (index == 0 && !activeEffects.ContainsKey(gesture))
        {
            if (!newGesture.isFinisher)
            {
                currentGestureStep++;
            }

            newEffect.Activate();
            activeEffects.Add(gesture, newEffect);
        }
            // Existing
        else if (index > 0 && !newGesture.isFinisher
                 && activeEffects.ContainsKey(gesture))
        {
            currentGestureStep++;

            RemoveActiveEffects(gesture);
            newEffect.Activate();
            activeEffects.Add(gesture, newEffect);
        }
            // Last
        else if (newGesture.isFinisher
                 && activeEffects.ContainsKey(gesture))
        {
            currentGestureStep++;

            RemoveActiveEffects(gesture);
            newEffect.Activate();
            activeEffects.Add(gesture, newEffect);
        }

        currentSubGesture = gesture.subGestures[index];
    }


    private void SubGestureListener()
    {
        if (!ComparePosition(currentSubGesture))
        {
            if (currentGesture.count > currentGestureStep)
            {
                SubGesture nextPosition = currentGesture.subGestures[currentGestureStep];
                
                if (nextPosition.isFinisher)
                {
                    SubGesture finisher = CompareAndGetPosition(currentGesture.getFinishers);
                    
                    if (finisher != null)
                    {
                        ExecuteGesture(currentGesture, currentGesture.IndexOf(finisher));
                    }
                }
                else
                {
                    if (ComparePosition(nextPosition))
                    {
                        ExecuteGesture(currentGesture, currentGestureStep);
                    }
                }
            }
        }
    }


    private void UpdateEffect()
    {
        foreach (KeyValuePair<Gesture, ActiveEffect> pair in activeEffects) {

            if (pair.Value.isActive)
            {
                pair.Value.UpdateEffect(bodyObject);
            }
            else
            {
                RemoveActiveGesture(pair.Key);
                break;
            }
        }
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

    
    private void RemoveActiveEffects(Gesture gesture)
    {
        if (activeEffects.ContainsKey(gesture))
        {
            activeEffects[gesture].Destroy();
            activeEffects.Remove(gesture);
        }
    }
    

    private void RemoveActiveGesture(Gesture gesture)
    {
        if (gesture == currentGesture)
        {
            currentGesture = null;
            currentGestureStep = 0;
            currentSubGesture = null;
        }
        activeEffects.Remove(gesture);
    }


    public bool ComparePosition(SubGesture gesture)
    {
        if (ReferenceEquals(null, gesture))
        {
            return false;
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


    private static Vector3 GetJointVector3(KinectJoint joint)
    {
        return new Vector3(joint.Position.X * 5 - kinectDisplacement, joint.Position.Y * 5, 5);
    }


    private static Vector3 GetJointVector3(float x, float y, float z)
    {
        return new Vector3(x * 5 + kinectDisplacement, y * 5, 5);
    }


    private float dist(Vector3 v1, Vector3 v2)
    {
        if (v2 == Vector3.zero)
        {
            return 0;
        }

        return Vector3.Distance(v1, v2);
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

    public void Mushroom()
    {
        mushroomWidth += 0.1f;
        foreach (LineRenderer lr in this.bodyObject.GetComponentsInChildren<LineRenderer>())
        {

            lr.SetWidth(mushroomWidth, mushroomWidth);
            lr.SetColors(new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value), new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value));
        }
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
