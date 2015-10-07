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
	private float timeoutForInteraction;
	public float timeoutMaxInterval;

	// Use this for initialization
	void Start () {
		midpointSet = false;
		midpointAchieved [0] = false;
		midpointAchieved [1] = false;
		active_G_Protein = null;
		delay = 0.0f;
		timeoutForInteraction = 0.0f;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if (timeoutForInteraction > timeoutMaxInterval) {
			if(tag == "Kinase_Prep_A" || tag == "Kinase_Prep_B" || tag == "Kinase_Prep_C") {
				active_G_Protein.GetComponent<G_ProteinCmdCtrl>().resetTarget();
				this.GetComponent<PolygonCollider2D>().enabled = true;
				active_G_Protein.GetComponent<BoxCollider2D>().enabled = true;
				active_G_Protein = null;
				midpointSet = false;
				midpointAchieved [0] = midpointAchieved [1] = false;
				delay = 0;
				timeoutForInteraction = 0.0f;
				tag = "Kinase";
			}
		}

		if (tag == "Kinase") {
			Roam.Roaming (this.gameObject);
		} else if (tag == "Kinase_Prep_A" || tag == "Kinase_Prep_B") {
			if ((delay += Time.deltaTime) >= 5.0f && active_G_Protein != null) {
				if (!midpointSet && tag == "Kinase_Prep_A") {
					midpoint = Roam.CalcMidPoint (active_G_Protein, this.gameObject);
					midpointSet = true;
				} else if (ApproachMidpoint(setupVector(), setupRestraint())) {
					setupNextPhase ();
				}
			} else {
				Roam.Roaming (this.gameObject);
			}
			timeoutForInteraction += Time.deltaTime;
		} else if (tag == "Kinase_Prep_C") {
			if(!midpointAchieved [0] || !midpointAchieved [1]){
				midpointAchieved[0] = Roam.ProceedToVector(active_G_Protein,midpoint + new Vector3(0.0f,0.35f,0.0f));
				midpointAchieved[1] = Roam.ProceedToVector(this.gameObject,midpoint + new Vector3(0.0f,-0.35f,0.0f));
			}
			if(midpointAchieved[0] && midpointAchieved[1]) {
				if((delay += Time.deltaTime) >= 3) {
					Instantiate(Kinase_P2,gameObject.transform.position, Quaternion.identity);
					active_G_Protein.GetComponent<G_ProteinCmdCtrl>().resetTarget();
					Destroy (gameObject);
				}
				else {
					active_G_Protein.GetComponent<BoxCollider2D>().enabled = true;
					Roam.RoamingTandem(active_G_Protein,this.gameObject,new Vector3 (0.0f, -0.70f, 0.0f));
				}
			}
			timeoutForInteraction += Time.deltaTime;
		}
		else if ( tag == "Kinase_Phase_2" ) {
			Roam.Roaming (this.gameObject);
		}
		else {
			Roam.Roaming (this.gameObject) ;
		}
	}

	private Vector3 setupVector(){
		if (tag == "Kinase_Prep_A") {
			return new Vector3 (-2.0f, 0.0f, 0.0f);
		} else if (tag == "Kinase_Prep_B") {
			return new Vector3 (0.0f, 1.0f, 0.0f);
		} else {
			return new Vector3 (0.0f, 0.0f, 0.0f);
		}
	}

	private float setupRestraint () {
		if (tag == "Kinase_Prep_A") {
			return 3.25f;
		} else if (tag == "Kinase_Prep_B") {
			return 1.75f;
		} else {
			return 0.0f;
		}
	}

	private void setupNextPhase() {
		if (tag == "Kinase_Prep_A") {
			midpointAchieved [0] = midpointAchieved [1] = false;
			tag = "Kinase_Prep_B";
		} else if (tag == "Kinase_Prep_B") {
			midpointAchieved [0] = midpointAchieved [1] = false;
			this.GetComponent<PolygonCollider2D>().enabled = false;
			active_G_Protein.GetComponent<BoxCollider2D>().enabled = false;
			tag = "Kinase_Prep_C";
			delay = 0.0f;
		} else if (tag == "Kinase_Prep_C") {
			
		}
	}

	private bool ApproachMidpoint (Vector3 Offset, float Restraint) {
		if (!midpointAchieved [0]) {
			midpointAchieved [0] = Roam.ApproachVector (active_G_Protein, midpoint, Offset, Restraint);
		}
	
		if (!midpointAchieved [1]) {
			midpointAchieved [1] = Roam.ApproachVector (this.gameObject, midpoint, -1 * Offset, Restraint);
		}
		return (midpointAchieved [0] && midpointAchieved [1]);
	}

	public void Get_G_Protein (GameObject obj) {
		if (obj.tag == "FreeG_Protein") {
			active_G_Protein = obj;
		}
	}
}

