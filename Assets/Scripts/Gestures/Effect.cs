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

    public GameObject[] FireShield;
    public GameObject[] WaterShield;
    public GameObject[] CrackParent;
    public GameObject[] Shirohige;
    public GameObject[] Kame;
    public GameObject[] Kamehameha;
    public GameObject[] IronMan;
    public GameObject[] Wings;
    public GameObject[] Lightning;
}
