using UnityEngine;
using System;
using System.Collections.Generic;
using Windows.Kinect;
using KinectJoint = Windows.Kinect.Joint;

public class PlayerBody {

    public Material redMaterial;
    public Material greenMaterial;
    public Material whiteMaterial;
    
    public GameObject bodyObject;
    private Dictionary<Gesture, ActiveEffect> activeEffects;
    //private Transform activeJoint;
    //private List<string> effectRotation;
    //private bool pairedToJoint;

    private Gesture currentGesture;
    private int IcurrentSubGesture;
    private Gesture.SubGesture currentSubGesture;
    public bool inGesture = false;
    public bool isAmbidextrous = false;

    //private float chargeTimer = 1f;
    //private float chargeMagnitude = 0.05f;

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

    private Dictionary<LigDir, Vector3> jointPositions;


    public PlayerBody(ulong id, Material green, Material red, Material white)
    {
        this.bodyObject = new GameObject("Body: " + id);
        this.renderedJoints = new List<GameObject>();
        this.activeEffects = new Dictionary<Gesture, ActiveEffect>();
        //this.effectRotation = new List<string>();

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
        //this.effectRotation = new List<string>();

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

                    if (currentGesture != null)
                    {
                        if (ComparePosition(currentGesture.current))
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

                    if (currentGesture != null)
                    {
                        if (ComparePosition(currentGesture.current))
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

        UpdateGesture();

        UpdateEffectPositions();
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

        UpdateGesture();

        UpdateEffectPositions();
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
            if (currentGesture != gesture && currentGesture.curStep == 0)
            {
                ExecuteGesture(gesture);
            }
            else
            {
                if (gesture.repeatable && currentGesture.curStep != 0)
                {
                    ExecuteGesture(gesture);
                }
            }
        }
        
        if (currentGesture == null)
        {
            ExecuteGesture(gesture);
        }
    }


    private void ExecuteGesture(Gesture gesture)
    {
        currentGesture = gesture;

        // gesture.current NEEDS TO BE CHECKED FOR NULL VALUES
        var effect = new ActiveEffect(
                gesture.current.effect,
                gesture.current.jointTracked,
                this.bodyObject.transform.FindChild(gesture.current.jointTracked).position,
                gesture.current.directionOrigin,
                gesture.current.pairedToJoint,
                gesture.current.timeout);

        if (activeEffects.ContainsKey(currentGesture))
        {
            activeEffects[currentGesture].Destroy();

            activeEffects[currentGesture] = effect;
        }
        else
        {
            activeEffects.Add(gesture, effect);
        }
        
        effect.UpdateEffect(this.bodyObject);
    }


    public void UpdateGesture()
    {
        if (currentGesture != null)
        {
            if (currentGesture.next.isFinisher)
            {
                if (ComparePosition(currentGesture.getFinishers))
                {
                    currentGesture.Step();
                    ExecuteGesture(currentGesture);
                }
                else
                {
                    currentGesture = null;
                }
            }
            else
            {
                if (ComparePosition(currentGesture.next))
                {
                    currentGesture.Step();
                    ExecuteGesture(currentGesture);
                }
            }
        }
    }


    private void UpdateEffectPositions()
    {
        foreach (KeyValuePair<Gesture, ActiveEffect> activeEffect in activeEffects) {

            if (activeEffect.Value.effect != null)
            {
                activeEffect.Value.UpdateEffect(bodyObject);
            }
            else
            {
                RemoveActiveEffect(activeEffect.Key);
                break;
            }
        }
    }
    

    public bool ComparePosition(Gesture.SubGesture gesture)
    {
        if (currentGesture == null && inGesture == true)
        {
            return false;
        }
        
        foreach (KeyValuePair<LigDir, Vector3> pair in this.jointPositions)
        {
            if (dist(pair.Value, gesture.position[pair.Key]) > gesture.fudgeFactor)
            {
                inGesture = false;
                return false;
            }
        }

        return true;
    }


    public bool ComparePosition(List<Gesture.SubGesture> gestures)
    {
        for (int i = 0; i < gestures.Count; i++)
        {
            if (ComparePosition(gestures[i]))
            {
                return true;
            }
        }

        return false;
    }


    public Gesture.SubGesture CompareAndGetPosition(List<Gesture.SubGesture> gestures)
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


    public void RemoveActiveEffect(Gesture gesture)
    {
        activeEffects.Remove(gesture);
        if (currentGesture == gesture)
        {
            currentGesture = null;
            IcurrentSubGesture = 0;
            isAmbidextrous = false;
        }
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
