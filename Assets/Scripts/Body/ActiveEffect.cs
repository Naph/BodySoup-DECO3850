using UnityEngine;
using System;
using System.Collections.Generic;
using Windows.Kinect;
using KinectJoint = Windows.Kinect.Joint;

public class ActiveEffect {
    
    public GameObject effect;
    public String joint;
    private String origin;
    private bool paired;
    private float lifetime;


    public ActiveEffect(GameObject effect, String joint, Vector3 initialPos, String origin, bool paired, float timeout)
    {
        this.effect = (GameObject)MonoBehaviour.Instantiate(effect, initialPos, Quaternion.identity);
        this.joint = joint;
        this.origin = origin;
        this.paired = paired;
        MonoBehaviour.Destroy(this.effect, timeout);
    }


    public void UpdateEffect(GameObject body)
    {
        lifetime = +0.01f;

        if (paired)
        {
            effect.transform.position = body.transform.FindChild(joint).position;
        }

        if (!origin.Equals("Unset"))
        {
            EffectRotation(body);
        }
    }


    public void EffectRotation(GameObject body)
    {
        Vector3 from = body.transform.FindChild(origin).localPosition;
        Vector3 to = body.transform.FindChild(joint).localPosition;

        Vector3 pos = (from - to).normalized;
        Vector3 rotation = pos * 90;

        Vector3 effectPos = effect.transform.localPosition;
        effectPos.y += pos.y * -1.10f;
        effectPos.x += pos.x * -1.10f;

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

        effect.transform.localPosition = effectPos;
        effect.transform.rotation = Quaternion.Euler(rotation);
    }


    public void Destroy() {
        MonoBehaviour.Destroy(this.effect);
    }

}
