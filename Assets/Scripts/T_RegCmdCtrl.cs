using UnityEngine;
using System.Collections;

public class T_RegCmdCtrl : MonoBehaviour {
	private GameObject active_Kinase_P2;
	private float delay;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (tag == "T_Reg") {
			Roam.Roaming (this.gameObject);
		} else {
			Roam.Roaming (this.gameObject);
		}
	}
}
