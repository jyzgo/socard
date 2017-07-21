using UnityEngine;
using System.Collections;

public class DebugWin : MonoBehaviour {

    public GameObject Plat1;
    public GameObject Plat2;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    public void OnCloseDebugPress()
    {
        Plat1.SetActive(false);
        Plat2.SetActive(false);
        gameObject.SetActive(false);
    }

}
