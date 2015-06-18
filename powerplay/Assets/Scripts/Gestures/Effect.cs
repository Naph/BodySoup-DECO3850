using UnityEngine;
using System.Collections;

public class Effect : MonoBehaviour {

    private static Effect _Instance = null;
    public static Effect Instance
    {
        get
        {
            if (_Instance == null)
                _Instance = (Effect)FindObjectOfType(typeof(Effect));
            return _Instance;
        }
    }

    public GameObject[] CastingShield;
    public GameObject[] Shield;
    public GameObject[] Crack;
    public GameObject[] CastingCrack;
    public GameObject[] CastingKame;
    public GameObject[] Kame;
    public GameObject[] IronMan;
    public GameObject[] Wings;
    public GameObject[] Lightning;
    public GameObject[] Feathers;
    public GameObject[] MarioBlock;
    public GameObject[] Wave;
    public GameObject[] None;
    public GameObject[] RightBigBang;
    public GameObject[] LeftExplosion;

    /*
    public GameObject[] Stomp;
    public GameObject[] FireAura;
    public GameObject[] Cyclone;
    public GameObject[] MeteorStorm; //we need to increase the Y of this effect *so it starts higher*

    public GameObject[] FireBurst;
    public GameObject[] LeftHolyShine;
    public GameObject[] RightSnowStorm; //we need to increase the Y of this effect *so it starts higher*
    */
}
