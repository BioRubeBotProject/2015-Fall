using UnityEngine;
using System.Collections;

//This code is heavly commented for future students.

public class moveG_Protein_Alt : MonoBehaviour
{
	static float SPEED = 5f;	
	static float MAX_X = 100f;
	static float MAX_Y = 100f;
	static float MIN_X = -100f;
	static float MIN_Y = -100f;

	public GameObject GDP, childGDP;	// for use creating a child of this object

	private bool docked = false;		// g-protein position = receptor phosphate position?
	private bool knownOffset = false;	// is the target phosphate left or right of receptor?
	private bool spotted = false;		// found receptor phosphate?
	private bool stillHaveGDP = true;	// is the phosphate still attached?
	private bool targeting = false;		// g-protein targeting phosphate?


	private float deltaDistance;		// distance between the receptor phosphate and the g-protein moving towards it
	private float randomX, randomY;		// random number between MIN/MAX_X and MIN/MAX_Y

	private GameObject closestPhosphate;// the closest receptor phosphate in relation to this g-protein
	private GameObject closestProtein;	// the closest g-protein to the closest receptor phosphate

	private Vector2 randomDirection;	// new direction vector
	private Vector3 dockingPosition;	// where to station the g-protein at docking
	private Vector3 lastPosition;		// previous position while moving to phosphate

	public void Start()
	{
	/*	For the purpose of demonstrating g-protein functionality, g-protein will parent
		a GDP, and the two will move together to an activated receptor (a receptor where
		ATP has dropped off a phosphate at the receptor leg). Once there, the g-protein
		will "dock" with the phosphate at the receptor and release the GDP.*/

		// create an instance of GDP located to the right side of the G-protein ( that's the '+ new Vector3'):
		childGDP = (GameObject)Instantiate (GDP, transform.position + new Vector3(0.86f, 0.13f, 0), Quaternion.identity);

	/*	Disable the GDP's collider and its rigidbody by setting it to isKinematic (object will not respond to
	 	physics) and parent the G-protein to the _childGDP. The two objects will now have
	 	free synchronous motion while still responding to collisions  */
		childGDP.GetComponent<CircleCollider2D> ().enabled = false;
		childGDP.GetComponent<Rigidbody2D> ().isKinematic = true;
		childGDP.transform.parent = transform;

		//initialize the closestPhosphate and closestProtein for no reason
		//other than to assign them something (can't pass a 'null' reference parameter (?))
		closestPhosphate = gameObject;
		closestProtein = gameObject;

		// keep track of where this object was
		lastPosition = transform.position;
	} //END START

/*  FixedUpdate runs at a set frequency providing for more 'fluid' physics claculations.
 *  Algorithm Summary:
 *  If running and I'm not currently docked
 * 		If I'm NOT currently targeting a phosphate
 * 			look for an open phosphate
 * 			if found
 * 				find out how close I am
 * 				find out if somebody is closer
 * 				if I'm closest
 * 					call dibs 
 * 				//END IF FOUND//
 * 			else (no phosphate found) move on 
 * 			//END NOT FOUND//
 * 		else I'm targeting a phosphate
 * 			move toward the phosphate
 * 			check to see if stuck and if so give a push
 * 			if reached phosphate, remove this G-protein and phosphate from
 * 			find closest (change tag) and release the phosphate*/

	private void FixedUpdate()
	{
		if (Time.timeScale > 0 && !docked) { 

			if (!targeting) {/*look for an open phosphate*/
				spotted = GameObject.FindGameObjectWithTag ("receptorPhosphate");//look for a receptor phosphate

				if (spotted) {//there's a phosphate open
					FindClosest (ref closestPhosphate, "receptorPhosphate"); //find my closest phosphate
					FindClosest (ref closestProtein, "G_Protein");//find the closest g-protein to my closest phosphate
					if (gameObject == closestProtein) lockOn(closestPhosphate.transform);//if I'm closest, 'call dibs'
				}//end if spotted
				else roam ();//no phosphate spotted
			}//end if !targeting

			else {/*head towards target*/
				if (!knownOffset){/*is it a left or right phosphate?*/
					dockingPosition = getOffset (closestPhosphate.transform);
					knownOffset = true;}//end if unk
				docked = moveToReceptor (closestPhosphate.transform);//head towards target
			}//end targeting
		}//end running and not docked
		if (docked && stillHaveGDP){
			hide(closestPhosphate.transform);//don't look for these when finding closest
			//throw in another reset position to compensate for any late hits
			transform.position = getOffset(closestPhosphate.transform);
			StartCoroutine(releaseGDP ());
			stillHaveGDP = false;
		}//end docked and released phosphate
	}//END FIXED UPDATE

	//Random movement
	private void roam()
	{
		randomX = Random.Range (MIN_X, MAX_X); //get random x vector coordinate
		randomY = Random.Range (MIN_Y, MAX_Y); //get random y vector coordinate
		//apply a force to the object in direction (x,y):
		GetComponent<Rigidbody2D> ().AddForce (new Vector2(randomX, randomY), ForceMode2D.Force);
	}//END ROAM

/*	FindClosest: finds closest object to calling object by traversing an array
 *  This approach is probably not the most efficient method to find the closest
 *  object, but  given the current timeline it will do.
 *  If time permits, further research should be done and code modified to improve performance*/

	private void FindClosest(ref GameObject closestObject, string objectTag)
	{
		//declare an array of GameObjects and populate it with the objects you are looking for
		GameObject[] gos;
		gos = GameObject.FindGameObjectsWithTag(objectTag);
		
		float distance = Mathf.Infinity;
		//find the nearest object ('objectTag') to me
		foreach (GameObject go in gos){	
			//calculate square magnitude between objects
			Vector3 diff = transform.position - go.transform.position;
			float curDistance = diff.sqrMagnitude;
			if (curDistance < distance){
				closestObject = go; //update closest object
				distance = curDistance;//update closest distance
			}}
	}//END FIND CLOSEST

	//Once it is determined this g-protein has found a phosphate, change the tags so
	//these objects are overlooked in future searches for open phosphates and closest
	//g-proteins
	private void lockOn(Transform phosphate)
	{
		targeting = true;
		transform.tag = "targeting";
		phosphate.transform.tag = "target";
	}

	//Determines if the phosphate is left or right of a receptor leg and sets the offset
	private Vector3 getOffset(Transform phosphate)
	{
		//target receptor left or right (direct G-protein direction) 
		if (phosphate.GetChild (0).tag == "leftPhosphate") 
			return phosphate.position + new Vector3 (-0.9f, -0.15f, 0);
		else
			return phosphate.position + new Vector3(0.9f,-0.16f,0);
	}

	//Moves g-protein toward phosphate while making sure the protein is not stuck
	//behind another object
	private bool moveToReceptor(Transform phosphate)
	{
		//Unity manual says if the distance between the two objects is < SPEED * Time.deltaTime,
		//protein position will equal docking...doesn't seem to work, so it's hard coded below
		transform.position = Vector2.MoveTowards (transform.position, dockingPosition, SPEED *Time.deltaTime);

		if (!docked && Vector3.Distance (transform.position, lastPosition) < SPEED * Time.deltaTime)
			roam ();//if I didn't move...I'm stuck.  Give me a push (roam())
		lastPosition = transform.position;//breadcrumb trail
		//check to see how close to the phosphate and disable collider when close
		deltaDistance = Vector3.Distance (transform.position, dockingPosition);
		//once in range, station object at docking position
		if (deltaDistance < SPEED*Time.deltaTime) {
			transform.GetComponent<BoxCollider2D> ().enabled = false;
			transform.GetComponent<Rigidbody2D>().isKinematic = true;
			transform.position = dockingPosition;
			if (phosphate.GetChild(0).tag == "leftPhosphate")
				transform.Rotate(180f,0,180f); //orientate protein for docking
		}//end if close enough
		return (transform.position==dockingPosition);
	}//END MOVE TO RECEPTOR


	//Mission Accomplished.  
	private void hide(Transform phosphate)
	{
		phosphate.tag = "OccupiedReceptor";
		transform.tag = "DockedG_Protein";
	}

	private IEnumerator releaseGDP ()
	{
		yield return new WaitForSeconds (3f);
		Debug.Log ("Enter Release GDP");
		childGDP.transform.parent = null;
		childGDP.transform.GetComponent<Rigidbody2D> ().isKinematic = false;
		childGDP.transform.GetComponent<CircleCollider2D> ().enabled = true;

	}

}