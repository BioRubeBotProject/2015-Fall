using UnityEngine;
using System.Collections;
using System;

public class T_RegCmdCtrl : MonoBehaviour, Roam.CollectObject {
	private GameObject active_Kinase_P2;
	public ParticleSystem destructionEffect;

	public bool isActive;
	private float delay;
	private Vector3 midpoint;
	private bool[] midpointAchieved = new bool[2];
	private bool midpointSet;
	private float timeoutForInteraction;
	public float timeoutMaxInterval;

	// Use this for initialization
	void Start () {
    this.gameObject.GetComponent<CircleCollider2D> ().enabled = false;
		isActive = true;
		midpointSet = false;
		midpointAchieved [0] = false;
		midpointAchieved [1] = false;
		active_Kinase_P2 = null;
		delay = 0.0f;
		timeoutForInteraction = 0.0f;
	}
	
	// Update is called once per frame
  // Each outer if block is to setup a state to tell the object which state
  // To perform actions for
	void FixedUpdate () {

    // Check if the time if the interaction did not complete reset back to before
    // The interaction was setup to occur
		if (timeoutForInteraction > timeoutMaxInterval) {
			// If the interaction is in the state of looking for a Kinase
      if(tag == "T_Reg_Prep_A" || tag == "T_Reg_Prep_B") { 
        tag = "T_Reg"; // Reset the tag to when no interaction was setup to occur
				reset (); // Reset Components of both objects
			} else if ( tag == "ATP_tracking" ) { // If ATP is Tracking this Transcription Regulator
        // Reset Components of the Transcription Regulator
        this.gameObject.GetComponent<CircleCollider2D> ().enabled = true;
        this.gameObject.GetComponent<BoxCollider2D>().enabled = true;
        this.gameObject.GetComponent<TrackingProperties>().isFound = false;

        // Reset the Timeout for Interaction
        timeoutForInteraction = 0.0f;

        // Set the T_Reg back to is Active
        isActive = true;
        this.tag = "ATP_tracking";
      }
		}


    // Default State when nothing is happening, T_Reg will just roam
		if (tag == "T_Reg") {
      Roam.Roaming (this.gameObject);
    } 
    // Else enter the state of approaching a Kinase
    else if (tag == "T_Reg_Prep_A") {
      if ((delay += Time.deltaTime) >= 5.0f) { // If Time delay, is less than 5 seconds keep Roaming
        if (!midpointSet) { // If midpoint not set, setup the midpoint between 
                            // the paired Kinase and this T_Reg
          midpoint = Roam.CalcMidPoint (active_Kinase_P2, this.gameObject);

          // Say the has now been set
          midpointSet = true; 
        } 
        // Else Approach the midpoint, if this point has been achieved setup the next phase T_Reg_Prep_B
        else if (Roam.ApproachMidpoint (active_Kinase_P2, this.gameObject, midpointAchieved, midpoint, new Vector3 (0.0f, 1.75f, 0.0f), 2.5f)) {
          delay = 0;

          // Disable Collider Components
          active_Kinase_P2.GetComponent<PolygonCollider2D> ().enabled = false;
          this.GetComponent<BoxCollider2D> ().enabled = false;

          // Set MidpointAchieved back to false
          midpointAchieved [0] = midpointAchieved [1] = false;
          tag = "T_Reg_Prep_B"; // Enter the next state by changing the tag to T_Reg_Prep_B
        }
      } else { 
        // Continue Roaming for 5 seconds after entering this state
        Roam.Roaming (this.gameObject);
      }
      // Increment the timeout variable by delta time
      timeoutForInteraction += Time.deltaTime; 
    } 
    // Else if tag is T_Reg_Prep_B, enter the state next phase of approaching the Kinase
    else if (tag == "T_Reg_Prep_B") {
      // If Midpoint has not been achieved, approach the midpoint
      if (!midpointAchieved [0] || !midpointAchieved [1]) { 
        // Proceed to the Kinase
        midpointAchieved [0] = Roam.ProceedToVector (active_Kinase_P2, midpoint + new Vector3 (0.0f, 0.52f, 0.0f));
        midpointAchieved [1] = Roam.ProceedToVector (this.gameObject, midpoint + new Vector3 (0.0f, -0.52f, 0.0f));
      }
      // Check if the midpoint has been achieved
      if (midpointAchieved [0] && midpointAchieved [1]) {
        // Check if 3 seconds have passed since midpoint was Achieved
        if ((delay += Time.deltaTime) >= 3) {
          // Setup state for an ATP to come and dock with T_Regulator
          timeoutForInteraction = 0;
          delay = 0;
          tag = "ATP_tracking";
        } 
        // Roam with the Kinase
        else {
          // Check if the kinase has a parent
          if (active_Kinase_P2.gameObject.transform.parent == null) {
            // Set the kinase's parent to be this T_Reg
            active_Kinase_P2.transform.parent = this.gameObject.transform;

            // Switch kinase to move with the its parent
            active_Kinase_P2.GetComponent<Rigidbody2D> ().isKinematic = true;
            active_Kinase_P2.GetComponent<PolygonCollider2D> ().enabled = false;

            // Enable the Box Collider for this T_Reg
            this.gameObject.GetComponent<BoxCollider2D> ().enabled = true;
            // Enable the Circle Collider for the ATP to approach and "Dock"
            this.gameObject.GetComponent<CircleCollider2D> ().enabled = true;
          }
          //Roam together until 3 seconds have passed
          Roam.Roaming (this.gameObject);
        }
      }
      //Increment the timeout variable by delta time
      timeoutForInteraction += Time.deltaTime;
    } 
    // Else if tag is ATP_Tracking, wait for an ATP to Collide with the CircleCollider
    else if (tag == "ATP_tracking") {
      // Check if the T_Reg is active
      if (isActive == true) {
        // Find the Closest ATP
        GameObject ATP = Roam.FindClosest (transform, "ATP");
        // Check if the Closest ATP is not null, therefore one exists
        if (ATP != null) {
          // Set the z position for the T_Regulator to be off the 0.0f
          transform.position = new Vector3 (transform.position.x, transform.position.y, 2.0f);

          // Setup a Vector in 2D because we only care about distance in the x and y
          Vector2[] pos = new Vector2[2];

          // Collect the x and y values for this T_Reg and the ATP in separate Vector2 variables
          pos [0] = new Vector2 (transform.position.x, transform.position.y);
          pos [1] = new Vector2 (ATP.transform.position.x, ATP.transform.position.y);

          // Check if the Distance between the the ATP and the T_Reg is less than 6.0f
          if (Vector2.Distance (pos [0], pos [1]) < 6.0f) {
            // Set the T_Reg to be inactive because an ATP is close enough to dock
            isActive = false;
            // Disable the box Collider on this T_Reg
            this.GetComponent<BoxCollider2D> ().enabled = false;
          } 
        } 
        // Else turn the Collider back on because the ATP has disappeared
        else { 
          this.GetComponent<BoxCollider2D> ().enabled = true;
          isActive = true;
        }

        // Roam while the T_Reg is still active
        Roam.Roaming (this.gameObject);
      }

      //Increment the timeout variable by delta time
      timeoutForInteraction += Time.deltaTime;
    } 
    // Else if tag is T_Reg_With_Phosphate, Enter this block
    else if (tag == "T_Reg_With_Phosphate") {
      // Check if T_Reg is active
      if (isActive == true) {
        // Check if Kinase is still active
        if (active_Kinase_P2 != null) {
          //Enter the state of looking for the nearest NPC
          this.tag = "T_Reg_To_NPC";

          // Disable the Circle Collider the ATP was using
          this.gameObject.GetComponent<CircleCollider2D> ().enabled = false;

          // Reset the Kinase back to Kinase_Phase_2, when it was looking for a T_Reg
          active_Kinase_P2.GetComponent<Rigidbody2D> ().isKinematic = false;
          active_Kinase_P2.transform.parent = null;
          active_Kinase_P2.GetComponent<KinaseCmdCtrl> ().reset ();
          active_Kinase_P2.tag = "Kinase_Phase_2";
          active_Kinase_P2 = null;
        }

        // Roam while T_Reg is Active but not looking for a NPC
        Roam.Roaming (this.gameObject);
      } 
      // Wait 3.5 Seconds after entering the stage where we have a phosphate
      else if ((delay += Time.deltaTime) > 3.5f && isActive == false) {
        // Time to release the Kinase and start looking for an NPC
        isActive = true;
        this.GetComponent<BoxCollider2D> ().enabled = true;
      }
    } 
    // If tag is T_Reg_To_NPC, then start moving toward the nearest NPC;
    else if (tag == "T_Reg_To_NPC") {
      // Find the Closest NPC and the Nucleus of the Cell
      GameObject NPC = Roam.FindClosest (this.transform, "NPC");
      GameObject Nucleus = GameObject.FindGameObjectWithTag ("CellMembrane").transform.GetChild (0).gameObject;

      // Check if both the Nucleus and the NPC exist
      if (NPC != null && Nucleus != null) {
        // Get the Nucleus and NPC positions in the 2D (x,y)
        Vector2[] temp = new Vector2[2];
        temp [0].x = NPC.transform.position.x;
        temp [0].y = NPC.transform.position.y;
        temp [1].x = Nucleus.transform.position.x;
        temp [1].y = Nucleus.transform.position.y;

        // Get the Distance and the Angle between the nucleus and the NPC
        float Distance = Vector2.Distance (temp [1], temp [0]) + 5;
        float Angle = Vector2.Angle (temp [0], temp [1]);

        // Perform the Degrees to Radians Conversion
        float rads = (float)((Math.PI / 180.0f) * (Angle + 90));

        // Create a temporary variable for the vector to approach
        Vector3 tempPosition = new Vector3 ();
        tempPosition.x = Distance * (float)Math.Cos (rads) + temp [1].x;
        tempPosition.y = Distance * (float)Math.Sin (rads) + temp [1].y;
        tempPosition.z = 2;

        // Check and move to the tempPosition, if we have then change state to T_Reg_To_Nucleus
        if (Roam.ApproachVector (this.gameObject, tempPosition, new Vector3 (0, 0, 0), 0)) {
          this.tag = "T_Reg_To_Nucleus";
        }
      }
    } 
    // Check if Tag is T_Reg_To_Nucleus, proceed to the Nucleus
    else if (tag == "T_Reg_To_Nucleus") {
      // Get the Position of the Nucleus
      Vector3 vector = GameObject.FindGameObjectWithTag ("CellMembrane").transform.GetChild(0).position;
      // Turn off the Collider on the T_Reg so it can pass through the nucleus
      this.GetComponent<BoxCollider2D>().enabled = false;

      // Approach the Nucleus's midpoint
      if (Roam.ApproachVector (this.gameObject,vector,new Vector3(0.0f,0.0f,0.0f),0.0f)) {
        // T_Reg is in the Nucleus, Game is won
        this.GetComponent<BoxCollider2D>().enabled = true;
        this.tag = "T_Reg_Complete";
      }

    } 
    // Check if Tag is T_Reg_Complete,
    else if (tag == "T_Reg_Complete") {
      // Congratulations, the game is won
      Roam.Roaming(this.gameObject);
    }
	}

  // Enumerator for when the Circle Collider has been hit
	private IEnumerator OnTriggerEnter2D(Collider2D other)
	{
    // Check if Other is not null and its Tag is ATP and if it has the ATPpathfinding component script
		if (other != null && other.tag == "ATP" && other.GetComponent<ATPpathfinding> ().found == true) 
    {
      // Check if 
      if( this.tag == "ATP_tracking" ) {
      	//Get the T_RegCmdCtrl script for this T_Reg object
        T_RegCmdCtrl objProps = this.GetComponent<T_RegCmdCtrl> ();

        //Set the T_Reg to inactive and the tag to T_Reg_With_Phosphate
        objProps.gameObject.tag = "T_Reg_With_Phosphate";
      	objProps.isActive = false;

        // Disable All Colliders and set the state of ATP Properties to false
      	objProps.GetComponent<BoxCollider2D>().enabled = false;
      	other.GetComponent<CircleCollider2D> ().enabled = false; //turn off collider while dropping off phosphate
      	other.GetComponent<ATPproperties> ().changeState (false);
      	other.GetComponent<ATPproperties> ().dropOff (transform.name);

        // Wait 3 seconds before ATP hands over the phosphate to the T_Reg
      	yield return new WaitForSeconds (3);
      	Transform tail = other.transform.FindChild ("Tail");
      	tail.transform.SetParent (this.transform);
      	objProps.GetComponent<CircleCollider2D> ().enabled = false;			
      	other.GetComponent<ATPproperties> ().changeState (true);
      	other.GetComponent<CircleCollider2D> ().enabled = true;

      	//code added to identify a 'left' receptor phosphate for G-protein docking
        //if it is a left phosphate, G-protein must rotate to dock
      	//NOTE: EACH PHOSPHATE ATTACHED TO A RECEPTOR IS NOW TAGGED AS "receptorPhosphate"
      	tail.transform.tag = "T_RegPhosphate";
      	tail.transform.position = tail.parent.transform.position + new Vector3 (0.0f,-0.75f,0.0f);

      	StartCoroutine (Explode (other.gameObject)); //self-destruct after 3 seconds
      }
		}
	}

  //Enumerator for Exploding the ATP 
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

  // Method to reset the basic to before looking for a Kinase
	private void reset() {
  		active_Kinase_P2.GetComponent<KinaseCmdCtrl>().resetTarget();
  		this.GetComponent<BoxCollider2D>().enabled = true;
  		active_Kinase_P2.GetComponent<PolygonCollider2D>().enabled = true;
  		active_Kinase_P2 = null;
  		midpointSet = false;
  		midpointAchieved [0] = midpointAchieved [1] = false;
  		delay = 0;
  		timeoutForInteraction = 0.0f;
	}

  // Allows a Kinase to pass itself to this T_Reg by calling this method
	public void GetObject (GameObject obj, string newTag) {
		if (obj.tag == "Kinase_Phase_2") {
			this.gameObject.tag = newTag;
			active_Kinase_P2 = obj;
		}
	}
}
