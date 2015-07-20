using UnityEngine;
using System.Collections;


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
		ReturnLocation = transform.position;
	}
	void OnMouseDrag()
	{
		transform.position = Camera.main.ScreenToWorldPoint (new Vector3 (x, y, 0.6f));
	}

	void OnMouseUp()
	{
		transform.position = ReturnLocation;
		spawnLocation.transform.position = Camera.main.ScreenToWorldPoint (new Vector3 (x, y, spawnedObject.transform.position.z + 1));
		Instantiate (spawnedObject, spawnLocation.transform.position, Quaternion.identity);
	}

}
