// **************************************************************
// **** Updated on 9/25/15 by Kevin Means
// **** 1.) Added all commentary
// **** 2.) Refactored some code
// **** 3.) Receptor now points toward CellMembrane's center
// **** 4.) Receptor now snaps to CellMembrane
// **************************************************************
using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class Spawner : MonoBehaviour , Tutorial.SwitchOnOff
{
	public static bool panning = true;// unknown
	public GameObject spawnedObject;  // for final object instantiation (after user releases mouse)
	public Vector3 spawnLocation;     // for final object instantiation (after user releases mouse)
	public Vector3 guidePosition;     // cannot change "transform.position.x,y, or z" directly
	GameObject cellMembrane;          // the one and only cellMembrane object for this world
	float x;                          // mouse x coordinate
	float y;                          // mouse y coordinate
	float degrees;                    // calculated # of degrees for object instantiation
	Vector3 ReturnLocation;           // original location of the "button"
	Quaternion ReturnRotation;        // orginal rotaion of the "button"
	
	//------------------------------------------------------------------------------------------------
	// This is only called once during the life of the program
	void Start ()                     
	{
	}
	
	//------------------------------------------------------------------------------------------------
	// This is called once per frame. 
	void Update ()
	{
		x = Input.mousePosition.x;
		y = Input.mousePosition.y;
	}
	
	//------------------------------------------------------------------------------------------------
	// This is called each time a user clicks the mouse button while hovering over a screen button.
	// Whenever a user clicks on an object button to create it, the member variable "degrees" (of 
	// rotation) is initialized to zero so that normal objects (other than the receptor) will 
	// effectively have an instantiated identity rotation, while the receptor still has the ability
	// to be instantiated with a custom rotation.  This method also saves the original location and 
	// rotation of the object's button in order to place back in the menu. It is determined if the
	// "Cell Membrane" exists at this point as well.  If so, that object is saved as "cellMembrane"
	// for use in other methods, if not, then "null" is saved.  This ensures that it's not called 
	// repeatedly during the many "OnMouseDrag" method calls.
	void OnMouseDown()
	{
		panning = false;
		degrees = 0.0f;
		ReturnLocation = transform.position;
		ReturnRotation.eulerAngles = transform.eulerAngles;
		cellMembrane = GameObject.FindGameObjectWithTag ("CellMembrane");
	}
	
	//------------------------------------------------------------------------------------------------
	// Called repeatedly as the user drags the mouse with the mouse button held down while hovering
	// over an object button. "Rotate and Snap" is only called for the Receptor and only when Cell
	// Membrane is in the world. "guidePosition" is a temporary position variable used for keeping
	// track of where the mouse is. The function "Camera.main.ScreenToWorldPoint" gets the mouse 
	// coordinates and converts them to world coordinates. The "spawnedObject" position is there 
	// because each object has a certain "Z" height or depth associated with it. Also, since 
	// the objects will be spawned at "camera height" the "+1" is so the object will be in front of 
	// the camera instead of on the same level.
	void OnMouseDrag()
	{
		guidePosition = Camera.main.ScreenToWorldPoint 
			(new Vector3 (x, y, spawnedObject.transform.position.z + 1));
		
		if(spawnedObject.name == "_ReceptorInactive" && cellMembrane != null) { RotateAndSnapObject(); }
		else { transform.position = guidePosition; }    // move the object to the mouse position
	}
	
	//------------------------------------------------------------------------------------------------
	// Called when user releases mouse button. The "if" statement disallows object creation until the
	// Cell Membrane is in place or if the user is trying to create the Cell Membrane. 
	void OnMouseUp()
	{
		if(cellMembrane != null || spawnedObject.name == "Cell Membrane")
		{
			spawnLocation = transform.position;
			GameObject obj = Instantiate (spawnedObject, spawnLocation, Quaternion.Euler(0f, 0f, degrees)) as GameObject;
      GameObject.Find("EventSystem").GetComponent<ObjectCollection>().Add (obj);
      // eliminate CM button once it is placed into world button original position button original rotation
      //Destroy( GameObject.FindGameObjectWithTag("CellMembraneButton"));
      obj = GameObject.FindGameObjectWithTag("CellMembraneButton") as GameObject;
      if(obj != null) {
        obj.SetActive (false);
      }
		}                                  
		transform.position = ReturnLocation;
		transform.localRotation = ReturnRotation;
		panning = true;
	}
	
	//------------------------------------------------------------------------------------------------
	// This is called everytime the mouse drags while holding a Receptor object. It's purpose is to
	// artificially rotate and place the receptor correctly in relation to the Cell Membrane. It 
	// starts with the guidePosition (mouse position relative to the world). It finds the arc tangent
	// of the difference between the center points of the mouse and Cell Membrane. It converts the 
	// radians to degrees and subtracts 90 to make it perpendicular to the Wall of the Cell Membrane,  
	// then the receptor is rotated to the number of degrees specified.  It then uses the distance
	// formula to calculate how close the receptor is to the center of the Cell Membrane. If the mouse
	// is within 40 units (times the scale of the membrane) the position of the receptor is snapped to
	// the wall of the Cell Membrane.
	//
	// ISSUES:
	// It would be nice to not have any hard-coded literals like the size of the Cell Membrane. I
	// cannot find the size or radius anywhere.
	void RotateAndSnapObject()
	{ 
		// Rotate:
		float diffX = guidePosition.x - cellMembrane.transform.position.x;
		float diffY = guidePosition.y - cellMembrane.transform.position.y;
		float rads = (float)Math.Atan2(diffY, diffX);
		degrees = (rads * (180 / (float)Math.PI)) - 90;
		transform.localRotation = Quaternion.Euler(0f, 0f, degrees);
		
		// Snap:
		float distance = (float)Math.Sqrt((diffX * diffX) + (diffY * diffY));
		if(distance < 40 * cellMembrane.transform.localScale.x)
		{
			float radius = 31 * cellMembrane.transform.localScale.x;
			Vector3 tempPosition = guidePosition;
			tempPosition.x = radius * (float)Math.Cos(rads) + cellMembrane.transform.position.x;
			tempPosition.y = radius * (float)Math.Sin(rads) + cellMembrane.transform.position.y;
			transform.position = tempPosition;
		}
		else { transform.position = guidePosition; }
	}

  void Tutorial.SwitchOnOff.enable () {
    if (this.GetComponent<Button> () != null) {
      this.GetComponent<Button>().interactable = true;
    }
    this.enabled = true;
    this.GetComponent<Collider2D> ().enabled = true;
  }

  void Tutorial.SwitchOnOff.transparent(bool value) {
    if (this.GetComponent<Button> () == null ) {
      if (value == true) {
        Roam.setAlpha (this.gameObject, 0.25f);
      } else {
        Roam.setAlpha (this.gameObject, 1.00f);
      }
    } else {
      if (value == true) {
        this.GetComponent<Button>().transition = Selectable.Transition.ColorTint;
      }
      else this.GetComponent<Button>().transition = Selectable.Transition.None;
    }
  }

  void Tutorial.SwitchOnOff.disable() {
    if (this.GetComponent<Button> () != null) {
      this.GetComponent<Button>().interactable = false;
    }
    this.enabled = false;
    this.GetComponent<Collider2D> ().enabled = false;
  }
}
