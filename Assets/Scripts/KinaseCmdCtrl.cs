using UnityEngine;
using System.Collections;

public class KinaseCmdCtrl : MonoBehaviour
{
	private GameObject active_G_Protein;
	public GameObject Kinase_P2;
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
		} else if (tag == "Kinase_Prep_A") {
			if ((delay += Time.deltaTime) >= 5.0f && active_G_Protein != null) {
				if (!midpointSet) {
					midpoint = Roam.CalcMidPoint (active_G_Protein, this.gameObject);
					midpointSet = true;
				} else if (midpointSet) {
					if (!midpointAchieved [0] || !midpointAchieved [1]) {
						if (!midpointAchieved [0]) {
							midpointAchieved [0] = ApproachVector (active_G_Protein, midpoint, new Vector3 (2.0f, 0.0f, 0.0f), 2.5f);
						}

						if (!midpointAchieved [1]) {
							midpointAchieved [1] = ApproachVector (this.gameObject, midpoint, new Vector3 (-2.0f, 0.0f, 0.0f), 2.5f);
						}
					} else if (midpointAchieved [0] && midpointAchieved [1]) {
						delay = 0.0f;
						midpointAchieved [0] = midpointAchieved [1] = false;
						tag = "Kinase_Prep_B";
					}
				}

			} else {
				Roam.Roaming (this.gameObject);
			}
		} else if (tag == "Kinase_Prep_B") {
			if (!midpointAchieved [0] || !midpointAchieved [1]) {
				if (!midpointAchieved [0]) {
					midpointAchieved [0] = ApproachVector (active_G_Protein, midpoint, new Vector3 (0.0f, 1.0f, 0.0f), 1.75f);
				}
				
				if (!midpointAchieved [1]) {
					midpointAchieved [1] = ApproachVector (this.gameObject, midpoint, new Vector3 (0.0f, -1.0f, 0.0f), 1.75f);
				}
			} else {
				midpointAchieved [0] = midpointAchieved [1] = false;
				this.GetComponent<PolygonCollider2D>().enabled = false;
				active_G_Protein.GetComponent<BoxCollider2D>().enabled = false;
				tag = "Kinase_Prep_C";
			}
		} else if (tag == "Kinase_Prep_C") {
			midpointAchieved[0] = Roam.ProceedToVector(active_G_Protein,midpoint + new Vector3(0.0f,0.35f,0.0f));
			midpointAchieved[1] = Roam.ProceedToVector(this.gameObject,midpoint + new Vector3(0.0f,-0.35f,0.0f));

			if(midpointAchieved[0] && midpointAchieved[1]) {
				if((delay += Time.deltaTime) >= 3) {
					Instantiate(Kinase_P2,gameObject.transform.position, Quaternion.identity);
					active_G_Protein.GetComponent<BoxCollider2D>().enabled = true;
					Destroy (gameObject);
				}
			}
		}
		else if ( tag == "Kinase_Phase_2" ) {
			Roam.Roaming (this.gameObject);
		}
		else {
			Roam.Roaming (this.gameObject) ;
		}
	}

	private bool ApproachVector(GameObject obj, Vector3 destination, Vector3 offset, float restraint) {
		if (Vector3.Distance (obj.transform.position,midpoint) > restraint ) {
			Roam.Roaming (obj);
		}
		return Roam.ProceedToVector( obj, destination + offset);
	}

	public void Get_G_Protein (GameObject obj) {
		if (obj.tag == "FreeG_Protein") {
			active_G_Protein = obj;
		}
	}
}

