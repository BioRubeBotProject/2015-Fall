using UnityEngine;
using System.Collections;

public class T_RegCmdCtrl : MonoBehaviour {
	private GameObject active_Kinase_P2;
	private float delay;
	private Vector3 midpoint;
	private bool[] midpointAchieved = new bool[2];
	private bool midpointSet;
	private float timeoutForInteraction;
	public float timeoutMaxInterval;

	// Use this for initialization
	void Start () {
		//myTarget = null;
		midpointSet = false;
		midpointAchieved [0] = false;
		midpointAchieved [1] = false;
		active_Kinase_P2 = null;
		delay = 0.0f;
		timeoutForInteraction = 0.0f;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (timeoutForInteraction > timeoutMaxInterval) {
			if(tag == "T_Reg_Prep_A" ) {
				reset ();
			}
		}


		if (tag == "T_Reg") {
			Roam.Roaming (this.gameObject);
		} else if (tag == "T_Reg_Prep_A") {
			if ((delay += Time.deltaTime) >= 5.0f) {
				if (!midpointSet) {
					midpoint = Roam.CalcMidPoint (active_Kinase_P2, this.gameObject);
					midpointSet = true;
				} else if (Roam.ApproachMidpoint (active_Kinase_P2,this.gameObject,midpointAchieved,midpoint, new Vector3(0.0f,1.75f,0.0f), 2.5f )) {
					active_Kinase_P2.GetComponent<PolygonCollider2D>().enabled = false;
					this.GetComponent<BoxCollider2D>().enabled =false;
					midpointAchieved [0] = midpointAchieved [1] = false;
					tag = "T_Reg_Prep_B";
				}
			} else {
				Roam.Roaming (this.gameObject);
			}
			timeoutForInteraction += Time.deltaTime;
		} else if(tag == "T_Reg_Prep_B" ) {
			if(!midpointAchieved [0] || !midpointAchieved [1]){
				midpointAchieved[0] = Roam.ProceedToVector(active_Kinase_P2,midpoint + new Vector3(0.0f,0.50f,0.0f));
				midpointAchieved[1] = Roam.ProceedToVector(this.gameObject,midpoint + new Vector3(0.0f,-0.50f,0.0f));
			}
			if(midpointAchieved[0] && midpointAchieved[1]) {
				/*if((delay += Time.deltaTime) >= 3) {

				}
				else {
					active_Kinase_P2.GetComponent<BoxCollider2D>().enabled = true;
					Roam.RoamingTandem(active_Kinase_P2,this.gameObject,new Vector3 (0.0f, -0.70f, 0.0f));
				}*/
			}
			timeoutForInteraction += Time.deltaTime;
		}
	}

	private void reset() {
		active_Kinase_P2.GetComponent<KinaseCmdCtrl>().resetTarget();
		this.GetComponent<BoxCollider2D>().enabled = true;
		active_Kinase_P2.GetComponent<PolygonCollider2D>().enabled = true;
		active_Kinase_P2 = null;
		midpointSet = false;
		midpointAchieved [0] = midpointAchieved [1] = false;
		delay = 0;
		timeoutForInteraction = 0.0f;
		tag = "T_Reg";
	}

	public void Get_Kinase_P2(GameObject obj) {
		if( obj.tag == "Kinase_Phase_2") {
			active_Kinase_P2 = obj;
		}
	}
}
