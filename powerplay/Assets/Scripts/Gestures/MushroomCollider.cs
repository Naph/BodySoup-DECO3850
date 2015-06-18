using UnityEngine;
using System.Collections;

public class MushroomCollider : MonoBehaviour {

    ParticleSystem mushroom;

	// Use this for initialization
	void Start () {
        mushroom = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnParticleCollision(GameObject other)
    {
        if (other.transform.parent != null && other.transform.parent.name.StartsWith("Body"))
        {
            Debug.Log("MUSHROOM COLLISION: " + other.name);
            Debug.Log(other.transform.parent.name);
            string id = other.transform.parent.name.Remove(0, 6);
            Debug.Log(id);
            ulong bodyId;
            BodySourceView bsv = FindObjectOfType<BodySourceView>();
            PlayerBody body;
            if (ulong.TryParse(id, out bodyId))
            {
                if ((body = bsv.getBody(bodyId)) != null)
                {
                    body.Mushroom();
                }
            }
            else if ((body = bsv.getBody(id)) != null)
            {
                body.Mushroom();
            }
        }
    }
}
