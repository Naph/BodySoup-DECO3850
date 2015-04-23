using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;

public class BodySourceView : MonoBehaviour 
{
    public GameObject BodySourceManager;

    private UnityBody _UnityBody;

    private Dictionary<ulong, UnityBody> _Bodies = new Dictionary<ulong, UnityBody>();
    private BodySourceManager _BodyManager;
	
	
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
            }
        }

        List<ulong> knownIds = new List<ulong>(_Bodies.Keys);
        
        for (int i = 0; i < bodies.Length; i++)
        {
            var body = bodies[i];

            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                if (!_Bodies.ContainsKey(body.TrackingId))
                {
                    _Bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                }

                RefreshBody(_Bodies[body.TrackingId]);
            }
        }
	}


    private UnityBody CreateBodyObject(ulong id)
    {
        UnityBody body = new UnityBody();
        body.trackingId = id;
        
        return body;
    }

    
    private void RefreshBody(UnityBody body)
    {
        /* Delete untracked bodies
        for (int i = 0; i < knownIds.Count; i++)
        {
            var trackingId = knownIds[i];

            if (!trackedIds.Contains(trackingId))
            {
                Destroy(_Bodies[trackingId]);
                _Bodies.Remove(trackingId);
            }
        }*/


    }
}
