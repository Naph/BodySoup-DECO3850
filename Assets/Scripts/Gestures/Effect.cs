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

    public GameObject FireShield;
    public GameObject WaterShield;
    public GameObject CrackParent;
    public GameObject WindWorks;
    public GameObject Kamehameha;
}
