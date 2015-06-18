using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;

public class Gesture {

    public SubGesture[] subGestures;
    //public bool repeatable;


    public Gesture(SubGesture[] subGestures)
    {
        this.subGestures = subGestures;
        //this.repeatable = repeatable;
    }

    public int count
    {
        get { return this.subGestures.Length; }
    }

    public int IndexOf(SubGesture index)
    {
        return Array.IndexOf(this.subGestures, index);
    }


    public SubGesture first
    {
        get { return this.subGestures[0]; }
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


    public bool hasMultipleSteps
    {
        get
        {
            return subGestures.Length > 1 ? true : false;
        }
    }

    public class SubGesture
    {
        public Dictionary<LigDir, Vector3> position;
        public String jointTracked;
        public String directionOrigin;
        public GameObject[] effect;
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
        public SubGesture(Dictionary<LigDir, Vector3> position, JointType jointTracked, bool pairedToJoint, GameObject[] effect, bool isFinisher, float fudgeFactor, float timeout)
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
        public SubGesture(Dictionary<LigDir, Vector3> position, JointType jointTracked, JointType directionOrigin, bool pairedToJoint, GameObject[] effect, bool isFinisher, float fudgeFactor, float timeout)
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
