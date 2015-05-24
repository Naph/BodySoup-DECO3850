using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;

public class Gesture {

    public List<SubGesture> subGestures;
    public bool repeatable;
    public bool ambidexterity;
    public int curStep;


    public Gesture(List<SubGesture> subGestures, bool repeatable, bool ambidexterity = false)
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


    public SubGesture next
    {
        get { return this.subGestures[curStep + 1]; }
    }

    public List<SubGesture> getFinishers
    {
        get {

            List<SubGesture> result = new List<SubGesture>();

            foreach (SubGesture sg in subGestures)
            {
                if (sg.isFinisher) result.Add(sg);
            }

            return result;
        }
    }


    public void Step()
    {
        curStep++;
    }

    public class SubGesture
    {
        public Dictionary<LigDir, Vector3> position;
        public String jointTracked;
        public String directionOrigin;
        public GameObject effect;
        public bool pairedToJoint;
        public bool isFinisher;
        public float fudgeFactor;
        public float timeout;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position">Position of gesture</param>
        /// <param name="jointTracked">JointType to map effect to</param>
        /// <param name="pairedToJoint">Effect follows jointTracked</param>
        /// <param name="effect">Effect created from gesture execution</param>
        /// <param name="isfinisher">Subgesture finishes a Gesture</param>
        /// <param name="fudgeFactor">Percentage of allowed slack</param>
        /// <param name="timeout">Time for triggering next gesture</param>
        public SubGesture(Dictionary<LigDir, Vector3> position, JointType jointTracked, bool pairedToJoint, GameObject effect, bool isFinisher, float fudgeFactor, float timeout)
        {
            this.position = position;
            this.jointTracked = jointTracked.ToString();
            this.directionOrigin = "Unset";
            this.effect = effect;
            this.pairedToJoint = pairedToJoint;
            this.isFinisher = isFinisher;
            this.fudgeFactor = fudgeFactor;
            this.timeout = timeout;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position">Position of gesture</param>
        /// <param name="jointTracked">JointType to map effect to</param>
        /// <param name="directionOrigin">JointType of angle origin to jointTracked position</param>
        /// <param name="pairedToJoint">Effect follows jointTracked</param>
        /// <param name="effect">Effect created from gesture execution</param>
        /// <param name="isfinisher">Subgesture finishes a Gesture</param>
        /// <param name="fudgeFactor">Percentage of allowed slack</param>
        /// <param name="timeout">Time for triggering next gesture</param>
        public SubGesture(Dictionary<LigDir, Vector3> position, JointType jointTracked, JointType directionOrigin, bool pairedToJoint, GameObject effect, bool isFinisher, float fudgeFactor, float timeout)
        {
            this.position = position;
            this.jointTracked = jointTracked.ToString();
            this.directionOrigin = directionOrigin.ToString();
            this.effect = effect;
            this.pairedToJoint = pairedToJoint;
            this.isFinisher = isFinisher;
            this.fudgeFactor = fudgeFactor;
            this.timeout = timeout;
        }
    }
}
