// **************************************************************
// **** Updated on 10/02/15 by Kevin Means
// **** 1.) Added commentary
// **** 2.) Refactored code
// **** 3.) ATP now tracks any object appropriately tagged
// **** 4.) ATP roaming is now smooth and more random
// **** 5.) Added collision contingency for Inner Cell Wall
// **************************************************************

using UnityEngine;
using System.Collections;
using System;                                 // for math

public class ATPpathfinding : MonoBehaviour 
{	
	//------------------------------------------------------------------------------------------------
	#region Public Fields + Properties + Events + Delegates + Enums
	public bool droppedOff = false;             // is phospate gone?
  public bool found = false;                  // did this ATP find a dock?
  public float maxHeadingChange;              // max possible rotation angle at a time
  public int maxRoamChangeTime;               // how long before changing heading/speed
  public int minSpeed;                        // slowest the ATP will move
  public int maxSpeed;                        // fastest the ATP will move
  public string trackingTag;                  // objects of this tag are searched for and tracked
  public GameObject trackThis;                // the object with which to dock
  public Transform origin;                    // origin location/rotation is the physical ATP
	#endregion Public Fields + Properties + Events + Delegates + Enums
  //------------------------------------------------------------------------------------------------

	//------------------------------------------------------------------------------------------------
	#region Private Fields + Properties + Events + Delegates + Enums
  private int objIndex = 0;                   // the index containing the above "trackThis" object
  private float heading;                      // roaming direction
  private float headingOffset;                // used for smooth rotation while roaming
  private int movementSpeed;                  // roaming velocity
  private int roamInterval = 0;               // how long until heading/speed change while roaming
  private int roamCounter = 0;                // time since last heading speed change while roaming
  private int curveCounter = 90;              // used for smooth transition when tracking
	private GameObject[] foundObjs;             // all the current objects of the tracking tag
  private Quaternion rotate;                  // rotation while tracking
	#endregion Private Fields + Properties + Events + Delegates + Enums
  //------------------------------------------------------------------------------------------------
	
	#region Private Methods
	//------------------------------------------------------------------------------------------------
  // Directs the ATP to the proper, active receptor leg. Takes as a parameter the index of the 
  // leg in the receptor leg object array "foundObjs" that is supposed to seek. The ATP seeks after
  // the circle collider of the respective leg, which is projected to the side of each leg. This 
  // method will detect whether or not the "Inner Cell Wall" is in the ATP's line of sight with the
  // leg collider. If it is, a path must be plotted around it.  Once the ATP is within 10 units
  // (scaled with the scale of the Cell Membrane) the ATP will assume a standard approach vector in
  // order to be properly rotated once the collision occurs.
	private void Raycasting(int index)
	{	
		if(index < foundObjs.Length) 
    {
      trackThis = foundObjs[index];

      float cmScale = GameObject.FindGameObjectWithTag ("CellMembrane").transform.localScale.x;
      Vector3 trackCollider = trackThis.GetComponent<CircleCollider2D>().bounds.center;
      RaycastHit2D collision = Physics2D.Linecast(origin.position, trackCollider);

			if(collision.collider.name == "Inner Cell Wall")
			{
        curveCounter = 90;                                // refuel the curve (used in below elseif)
        rotate = Quaternion.LookRotation(origin.position-trackCollider, trackThis.transform.right);
			}                                                   
      else if(Vector3.Distance(trackCollider, origin.position) > 10 * cmScale)
			{                                                   // ^^ no collision and still too far away^
        float diffX = origin.position.x - trackCollider.x;
        float diffY = origin.position.y - trackCollider.y;
				float degrees = ((float)Math.Atan2(diffY, diffX) * (180 / (float)Math.PI) + 90);
				transform.eulerAngles = new Vector3 (0, 0, degrees - curveCounter);
        rotate = transform.localRotation;
        if(curveCounter >= 0) { curveCounter -= 1; }      // slowly rotate until counter empty
      }
			else                                                // ATP within 10 units of collider
      { 
        rotate = Quaternion.LookRotation(origin.position - trackCollider, -trackThis.transform.up);
			}
      transform.localRotation = new Quaternion(0,0,rotate.z, rotate.w);
      transform.position += transform.up * Time.deltaTime * maxSpeed;
		} 
	}

  //------------------------------------------------------------------------------------------------
  // ATP wanders when not actively seeking a receptor leg. This method causes the ATP to randomly
  // change direction and speed at random intervals.  The tendency for purely random motion objects
  // to generally gravitate toward the edges of a circular container has been artificially remedied
  // by Raycasting and turning the ATP onto a 180 degree course (directing them toward the center).  
  private void Roam()
  {
    if(Time.timeScale != 0)                               // if game not paused
    {
      roamCounter++;                                      
      if(roamCounter > roamInterval)                         
      {                                                   
        roamCounter = 0;
        var floor = Mathf.Clamp(heading - maxHeadingChange, 0, 360);  
        var ceiling = Mathf.Clamp(heading + maxHeadingChange, 0, 360);
        roamInterval = UnityEngine.Random.Range(5, maxRoamChangeTime);   
        movementSpeed = UnityEngine.Random.Range(minSpeed, maxSpeed);
        RaycastHit2D collision = Physics2D.Raycast(origin.position, origin.up);
        if(collision.collider != null &&                  // must check for instance first
           collision.collider.name == "Cell Membrane(Clone)" &&
           collision.distance < 2)
        {
          if(heading <= 180) { heading = heading + 180; }
          else { heading = heading - 180; }
          movementSpeed = maxSpeed;
          roamInterval = maxRoamChangeTime;
        }
        else { heading = UnityEngine.Random.Range(floor, ceiling); }
        headingOffset = (transform.eulerAngles.z - heading) / (float)roamInterval;
      }
      transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z - headingOffset);
      transform.position += transform.up * Time.deltaTime * movementSpeed;
    }
  }
	
	//------------------------------------------------------------------------------------------------
	private void Start()
	{
	}
	
  //------------------------------------------------------------------------------------------------
	// Update is called once per frame. Gets an array of potential GameObjects to track and tries to 
  // find one that is not "found" yet. If it finds one then it calls "Raycasting" and passes the 
  // index of the object to track in the array so that the ATP can seek it out.  Else, ATP wanders.
	private void Update()
	{
    if(droppedOff) { found = false; }         // prevents a spent ATP from attaching again
		else
    {
      foundObjs = GameObject.FindGameObjectsWithTag(trackingTag);
      if(found == false)
      {
        objIndex = 0;
        while(objIndex < foundObjs.Length && 
              foundObjs[objIndex].GetComponent<TrackingProperties>().isFound == true)
        {
          ++objIndex;
        }
        if(objIndex < foundObjs.Length) 
        {
          if(foundObjs[objIndex].GetComponent<TrackingProperties>().Find() == true)
          { 
            found = true; 
          }
        }
        else 
        {
          trackThis = null;
          found = false;
        }
      }
      if(found == true) { Raycasting(objIndex); }
		}
    if(found == false) { Roam(); }
	}
	#endregion Private Methods
}