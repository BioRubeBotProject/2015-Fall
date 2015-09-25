using UnityEngine;
using System.Collections;

public class KinaseCmdCtrl : MonoBehaviour
{
	private GameObject active_G_Protein;
	private float delay;

	// Use this for initialization
	void Start ()
	{
		//active_G_Protein = null;
		delay = 0.0f;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if (tag == "Kinase") {
			Roam.Roaming (this);
		}
		else if (tag == "Kinase_Phase_2") {
			if((delay += Time.deltaTime) >= 5.0f && active_G_Protein != null) {

			}
			else {
				Roam.Roaming (this) ;
			}
		}
	}

	public void Get_G_Protein (GameObject obj) {
		if (obj.tag == "FreeG_Protein") {
			active_G_Protein = obj;
		}
	}
}

