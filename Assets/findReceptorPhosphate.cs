using UnityEngine;
using System.Collections;

public class findReceptorPhosphate : MonoBehaviour {

	public bool displayPath = true;
	public float maxHeadingChange = 60;
	public Transform SightEnd;
	public Transform sightStart;
	public int speed = 100;
	public bool spotted = false;
	
	private float heading;
	private GameObject[] myFoundObjs;
	private GameObject myTarget;
	

	
	private void Raycasting()
	{
		int x = 0;
		myFoundObjs = GameObject.FindGameObjectsWithTag("autoPhosphate");
		while (x < myFoundObjs.Length && myFoundObjs[x].GetComponent<Rigidbody2D>().isKinematic == true)
		{
			x++;
		}
		
		int count = myFoundObjs.GetUpperBound(0);
		
		if (x <= count) {
			if (myFoundObjs [x].GetComponent<Rigidbody2D> ().isKinematic == true) {
				//Debug.Log("We found a receptor!");
				sightStart = myFoundObjs [x].transform;
				transform.position += transform.up * Time.deltaTime * speed;
				if (displayPath == true) {
					Debug.DrawLine (sightStart.position, SightEnd.position, Color.green);
				}
				spotted = Physics2D.Linecast (sightStart.position, SightEnd.position);
				
				
				Quaternion rotation = Quaternion.LookRotation (SightEnd.position - sightStart.position, sightStart.TransformDirection (Vector3.up));
				transform.rotation = new Quaternion (0, 0, rotation.z, rotation.w);
			}
		} 
		else {
			sightStart = null;
			spotted = false;
		}
	}

}
