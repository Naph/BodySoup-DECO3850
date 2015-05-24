using UnityEngine;
using System;
using System.Collections.Generic;
using Windows.Kinect;
using KinectJoint = Windows.Kinect.Joint;

public class PlayerBody {

    public Material redMaterial;
    public Material greenMaterial;
    
    public GameObject bodyObject;
    private Dictionary<Gesture, ActiveEffect> activeEffects;
    //private Transform activeJoint;
    //private List<string> effectRotation;
    //private bool pairedToJoint;

    private Gesture currentGesture;
    private int currentSubGesture;
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

    public PlayerBody(ulong id, Material green, Material red)
    {
        this.bodyObject = new GameObject("Body: " + id);
        this.renderedJoints = new List<GameObject>();
        this.activeEffects = new Dictionary<Gesture, ActiveEffect>();
        //this.effectRotation = new List<string>();

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
            if (currentGesture != gesture && currentSubGesture == 0)
            {
                ExecuteGesture(gesture, 0);
            }
            else
            {
                if (gesture.repeatable && currentSubGesture != 0)
                {
                    ExecuteGesture(gesture, 0);
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

        var subGesture = gesture.subGestures[index];
        var effect = new ActiveEffect(
                subGesture.effect,
                subGesture.jointTracked,
                this.bodyObject.transform.FindChild(subGesture.jointTracked).position,
                subGesture.directionOrigin,
                subGesture.pairedToJoint,
                subGesture.timeout);

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
        
        if (!subGesture.directionOrigin.Equals("Unset"))
        {
            effect.EffectRotation(this.bodyObject);
        }
    }


    public void UpdateGesture()
    {
        if (currentGesture != null)
        {
            if (currentSubGesture + 1 < currentGesture.count) // if not last
            {
                if (ComparePosition(currentGesture.subGestures[currentSubGesture + 1]))
                {
                    currentSubGesture++;
                    ExecuteGesture(currentGesture, currentSubGesture);
                }
                /*
                if (ComparePosition(currentGesture.subGestures[currentSubGesture]))
                {
                    // Holding SubGesture position
                    chargeTimer += chargeMagnitude;
                }
                */
            }
            else // if last
            {
                if (!ComparePosition(currentGesture.subGestures[currentSubGesture])) // if left last
                {
                    currentGesture = null;
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

            if (isAmbidextrous && 
                currentGesture.subGestures.Contains(gesture) &&
                gesture == currentGesture.subGestures[currentGesture.subGestures.Count - 2])
            {
                if (dist(pair.Value, currentGesture.subGestures[currentSubGesture + 2].position[pair.Key]) < gesture.fudgeFactor ||
                    dist(pair.Value, gesture.position[pair.Key]) < gesture.fudgeFactor)
                {
                    isAmbidextrous = false;
                    return true;
                }
            }

            if (dist(pair.Value, gesture.position[pair.Key]) > gesture.fudgeFactor)
            {
                inGesture = false;
                return false;
            }
        }

        return true;
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
            currentSubGesture = 0;
        }
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
