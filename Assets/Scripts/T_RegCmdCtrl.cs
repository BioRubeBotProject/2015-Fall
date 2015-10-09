using UnityEngine;
using System.Collections;

public class T_RegCmdCtrl : MonoBehaviour, Roam.CollectObject {
	private GameObject active_Kinase_P2;
	public ParticleSystem destructionEffect;

	public bool isActive;
	public bool havePhosphate;
	private float delay;
	private Vector3 midpoint;
	private bool[] midpointAchieved = new bool[2];
	private bool midpointSet;
	private float timeoutForInteraction;
	public float timeoutMaxInterval;

	// Use this for initialization
	void Start () {
		//myTarget = null;
		havePhosphate = false;
		isActive = true;
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
				} else if (Roam.ApproachMidpoint (active_Kinase_P2, this.gameObject, midpointAchieved, midpoint, new Vector3 (0.0f, 1.75f, 0.0f), 2.5f)) {
					delay = 0;
					active_Kinase_P2.GetComponent<PolygonCollider2D> ().enabled = false;
					this.GetComponent<BoxCollider2D> ().enabled = false;
					midpointAchieved [0] = midpointAchieved [1] = false;
					tag = "T_Reg_Prep_B";
				}
			} else {
				Roam.Roaming (this.gameObject);
			}
			timeoutForInteraction += Time.deltaTime;
		} else if (tag == "T_Reg_Prep_B") {
			if (!midpointAchieved [0] || !midpointAchieved [1]) {
				midpointAchieved [0] = Roam.ProceedToVector (active_Kinase_P2, midpoint + new Vector3 (0.0f, 0.52f, 0.0f));
				midpointAchieved [1] = Roam.ProceedToVector (this.gameObject, midpoint + new Vector3 (0.0f, -0.52f, 0.0f));
			}
			if (midpointAchieved [0] && midpointAchieved [1]) {

				if ((delay += Time.deltaTime) >= 3) {
					timeoutForInteraction = 0;
					delay = 0;
					tag = "ATP_tracking";
				} else {
					if (this.gameObject.transform.parent == null) {
						active_Kinase_P2.GetComponent<Rigidbody2D> ().isKinematic = true;
						active_Kinase_P2.GetComponent<PolygonCollider2D> ().enabled = false;
						active_Kinase_P2.transform.parent = this.gameObject.transform;
						this.gameObject.GetComponent<BoxCollider2D> ().enabled = true;
					}
					Roam.Roaming (this.gameObject);
				}
			}
			timeoutForInteraction += Time.deltaTime;
		} else if (tag == "ATP_tracking") { 
			if (isActive == true) {
				GameObject ATP = Roam.FindClosest (transform, "ATP");
				transform.position = new Vector3 (transform.position.x, transform.position.y, 2.0f);
				Vector2[] pos = new Vector2[2];
				pos [0] = new Vector2 (transform.position.x, transform.position.y);
				pos [1] = new Vector2 (ATP.transform.position.x, ATP.transform.position.y);
				if (Vector2.Distance (pos [0], pos [1]) < 6.0f) {
					isActive = false;
				} else { 
					isActive = true;
				}

				Roam.Roaming (this.gameObject);
			}
		} else if (tag == "T_Reg_With_Phosphate") {
			if (isActive == true) {
				if(active_Kinase_P2 != null) {
					active_Kinase_P2.GetComponent<Rigidbody2D> ().isKinematic = false;
					active_Kinase_P2.GetComponent<PolygonCollider2D> ().enabled = true;
					active_Kinase_P2.transform.parent = null;
					active_Kinase_P2.GetComponent<KinaseCmdCtrl>().reset();
					active_Kinase_P2.tag = "Kinase_Phase_2";
					active_Kinase_P2 = null;
				}
				Roam.Roaming(this.gameObject);
			}
			else if((delay += Time.deltaTime) > 3.5f) {
				isActive = true;
			}

		}
	}

	private IEnumerator OnTriggerEnter2D(Collider2D other)
	{
		GameObject obj = other.gameObject;
		if (obj != null) {
			if (obj.tag == "ATP" ) { // helps prevent rogue ATP from hijacking leg
				if(obj.GetComponent<ATPpathfinding> ().found == true) {
					if(this.tag == "ATP_tracking" ) {
						T_RegCmdCtrl objProps = this.GetComponent<T_RegCmdCtrl> ();
						objProps.isActive = false;
						objProps.gameObject.tag = "T_Reg_With_Phosphate";
						obj.GetComponent<CircleCollider2D> ().enabled = false; //turn off collider while dropping off phosphate
						obj.GetComponent<ATPproperties> ().changeState (false);
						obj.GetComponent<ATPproperties> ().dropOff (transform.name);
				
						yield return new WaitForSeconds (3);
						Transform tail = obj.transform.FindChild ("Tail");
						tail.transform.SetParent (this.transform);
						objProps.GetComponent<CircleCollider2D> ().enabled = false;			
						obj.GetComponent<ATPproperties> ().changeState (true);
						obj.GetComponent<CircleCollider2D> ().enabled = true;
					
						//code added to identify a 'left' receptor phosphate for G-protein docking
						//if it is a left phosphate, G-protein must rotate to dock
						//NOTE: EACH PHOSPHATE ATTACHED TO A RECEPTOR IS NOW TAGGED AS "receptorPhosphate"
						tail.transform.tag = "T_RegPhosphate";

						//objProps.havePhosphate = true;
						StartCoroutine (Explode (obj)); //self-destruct after 3 seconds

					}
				}
			}
		}
	}

	private IEnumerator Explode(GameObject other)
	{
		yield return new WaitForSeconds (3f);
		//Instantiate our one-off particle system
		ParticleSystem explosionEffect = Instantiate(destructionEffect) as ParticleSystem;
		explosionEffect.transform.position = other.transform.position;
		
		//play it
		explosionEffect.loop = false;
		explosionEffect.Play();
		
		//destroy the particle system when its duration is up, right
		//it would play a second time.
		Destroy(explosionEffect.gameObject, explosionEffect.duration);
		
		//destroy our game object
		Destroy(other.gameObject);
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

	public void GetObject (GameObject obj, string newTag) {
		if (obj.tag == "Kinase_Phase_2") {
			this.gameObject.tag = newTag;
			active_Kinase_P2 = obj;
		}
	}
}
