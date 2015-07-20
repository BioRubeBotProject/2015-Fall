using UnityEngine;
using System.Collections;

//Updated 7/10/2015
//Line 18:  Added a static bool varible to flag when player is spawning an object and is not panning
//Lines 37 & 51:  Set panning to false when spawning and true once object has spawned
//Lines 43 & 49: add a y axis offset to the spawning object to keep object in sight (above finger) when placing object
public class Spawner : MonoBehaviour 
{
	public GameObject spawnLocation;
	public GameObject spawnedObject;
	float x;
	float y;
	float returnX;
	float returnY;
	float returnZ;
	Vector3 ReturnLocation;
	public static bool panning = true;


	// Use this for initialization
	void Start () 
	{

		ReturnLocation = transform.position;
	}
	
	// Update is called once per frame
	void Update () 
	{
		x = Input.mousePosition.x;
		y = Input.mousePosition.y;

	}
	void OnMouseDown()
	{
		panning = false;
		ReturnLocation = transform.position;

	}
	void OnMouseDrag()
	{
		transform.position = Camera.main.ScreenToWorldPoint (new Vector3 (x, y+60, 0.6f));
	}

	void OnMouseUp()
	{
		transform.position = ReturnLocation;
		spawnLocation.transform.position = Camera.main.ScreenToWorldPoint (new Vector3 (x, y+60, spawnedObject.transform.position.z + 1));// + 1));
		Instantiate (spawnedObject, spawnLocation.transform.position, Quaternion.identity);
		panning = true;
	}

}
