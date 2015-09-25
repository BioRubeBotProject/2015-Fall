using UnityEngine;
using System.Collections;


public class Roam : MonoBehaviour {
	public static float _max = 150f;
	public static float _min = -150f;
	public static float _speed = 5.0f;

	public static void Roaming(GameObject Obj) {
		if (Time.timeScale > 0)// if simulation is running
		{
			float randomX, randomY;		//random number between minX/maxX and minY/maxY
			randomX = Random.Range (_min,_max); //get random x vector coordinate
			randomY = Random.Range (_min, _max); //get random y vector coordinate
			//apply a force to the object in direction (x,y):
			Obj.GetComponent<Rigidbody2D> ().AddForce (new Vector2(randomX, randomY), ForceMode2D.Force);
		}
	}

	public static Vector3 CalcMidPoint ( GameObject obj_1, GameObject obj_2 ) {
		float[] temp = new float[2];
		temp [0] = (obj_1.transform.position.x + obj_2.transform.position.x)/2.0f;
		temp [1] = (obj_1.transform.position.y + obj_2.transform.position.y)/2.0f;
		Vector3 meetingPoint = new Vector3 (temp [0], temp [1], obj_1.transform.position.z);

		return meetingPoint;
	}

	public static GameObject FindClosest(Transform my, string objTag) {
		float distance = Mathf.Infinity; //initialize distance to 'infinity'
		
		GameObject[] gos; //array of gameObjects to evaluate
		GameObject closestObject = null;
		//populate the array with the objects you are looking for
		gos = GameObject.FindGameObjectsWithTag(objTag);
		
		//find the nearest object ('objectTag') to me:
		foreach (GameObject go in gos)
		{	
			//calculate square magnitude between objects
			Vector3 diff = my.position - go.transform.position;
			float curDistance = diff.sqrMagnitude;
			if (curDistance < distance)
			{
				closestObject = go; //update closest object
				distance = curDistance;//update closest distance
			}
		}
		return closestObject;
	}/* end FindClosest */

	public static bool ProceedToVector (GameObject obj, Vector3 approachVector) {
		float step = _speed * Time.deltaTime;
		Vector2 originalVector = obj.transform.position;
		obj.transform.position = Vector3.MoveTowards (obj.transform.position, approachVector, step);


		return (approachVector == obj.transform.position);
	}
}