using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;

public class Gesture {

    public List<SubGesture> subGestures;
    public bool repeatable;
    public bool ambidexterity;
    int curStep;

    public Gesture(List<SubGesture> subGestures, bool repeatable)
    {
        this.subGestures = subGestures;
        this.repeatable = repeatable;
        curStep = 0;
    }


    public Gesture(List<SubGesture> subGestures, bool repeatable, bool ambidexterity)
    {
        this.subGestures = subGestures;
        this.repeatable = repeatable;
        this.ambidexterity = ambidexterity;
        curStep = 0;
    }

    public int count
    {
        get { return this.subGestures.Count; }
    }

    public SubGesture current
    {
        get
        {
            SubGesture result;
            try
            {
                result = this.subGestures[curStep];
            }
            catch (ArgumentOutOfRangeException)
            {
                result = null;
            }

            return result;
        }
    }

    public SubGesture first
    {
        get { return this.subGestures[0]; }
    }


    public class SubGesture
    {
        public Dictionary<LigDir, Vector3> position;
        public String jointTracked;
        public String directionOrigin;
        public GameObject effect;
        public bool pairedToJoint;
        public float fudgeFactor;
        public float timeout;

        public SubGesture(Dictionary<LigDir, Vector3> position, JointType jointTracked, bool pairedToJoint, GameObject effect, float fudgeFactor, float timeout)
        {
            this.position = position;
            this.jointTracked = jointTracked.ToString();
            this.directionOrigin = "Unset";
            this.effect = effect;
            this.pairedToJoint = pairedToJoint;
            this.fudgeFactor = fudgeFactor;
            this.timeout = timeout;
        }

        public SubGesture(Dictionary<LigDir, Vector3> position, JointType jointTracked, JointType directionOrigin, bool pairedToJoint, GameObject effect, float fudgeFactor, float timeout)
        {
            this.position = position;
            this.jointTracked = jointTracked.ToString();
            this.directionOrigin = directionOrigin.ToString();
            this.effect = effect;
            this.pairedToJoint = pairedToJoint;
            this.fudgeFactor = fudgeFactor;
            this.timeout = timeout;
        }
    }
}
