using UnityEngine;
using System.Collections;

public class KinaseCmdCtrl : MonoBehaviour
{
	private GameObject active_G_Protein;
	private Vector3 midpoint;
	private bool[] midpointAchieved = new bool[2];
	private bool midpointSet;
	private float delay;

	// Use this for initialization
	void Start () {
		midpointSet = false;
		midpointAchieved [0] = false;
		midpointAchieved [1] = false;
		active_G_Protein = null;
		delay = 0.0f;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if (tag == "Kinase") {
			Roam.Roaming (this.gameObject);
		}
		else if (tag == "Kinase_Phase_2") {
			if((delay += Time.deltaTime) >= 5.0f && active_G_Protein != null) {
				if( !midpointSet) {
					midpoint = Roam.CalcMidPoint(active_G_Protein,this.gameObject);
					midpointSet = true;
				} else if( midpointSet && ( !midpointAchieved[0] || !midpointAchieved[1] ) ) {
					if(!midpointAchieved[0]) {
						Vector3 offset = new Vector3(2.0f,0.0f,0.0f); //later to change to more dynamic offset
						if (Vector3.Distance (active_G_Protein.transform.position,midpoint) > 2.5f ) {
							Roam.Roaming (active_G_Protein);
						}
						midpointAchieved[0] = Roam.ProceedToVector( active_G_Protein, midpoint + offset );
					}

					if(!midpointAchieved[1]) {
						Vector3 offset = new Vector3(2.0f,0.0f,0.0f); //later to change to more dynamic offset
						if (Vector3.Distance (this.gameObject.transform.position,midpoint) > 2.5f) {
							Roam.Roaming (this.gameObject);
						}
						midpointAchieved[1] = Roam.ProceedToVector( this.gameObject , midpoint - offset );
					}
				}
				else {
					//Circle Each other until G_protein is above Kinase
				} 
			} else {
				Roam.Roaming (this.gameObject);
			}
		}
		else {
			Roam.Roaming (this.gameObject) ;
		}
	}

	public void Get_G_Protein (GameObject obj) {
		if (obj.tag == "FreeG_Protein") {
			active_G_Protein = obj;
		}
	}
}

