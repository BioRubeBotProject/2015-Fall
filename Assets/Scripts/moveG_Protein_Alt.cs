using UnityEngine;
using System.Collections;

public class moveG_Protein_Alt : MonoBehaviour
{
	private GameObject[] _foundAutoPhosphates;
	public Transform _autoPhosphate;
	private Vector2 _randomDirection;		//new direction vector
	public float _maxX, _maxY, _minX, _minY;//min/max vector values
	private float _randomX, _randomY;		//random number between minX maxX and minY and maxY
	public bool _spotted = false;	

	private void FixedUpdate()
	{
		if (Time.timeScale > 0)
		{
			_spotted = GameObject.FindGameObjectWithTag("autoPhosphate");
			if (_spotted) {moveToAutoPhosphate();}
			else roam(); 
		}
	}


	private void roam()
	{
		_randomX = Random.Range (_minX, _maxX); //get random x vector coordinate
		_randomY = Random.Range (_minY, _maxY); //get random y vector coordinate
		//apply a force to the object in direction (x,y):
		GetComponent<Rigidbody2D> ().AddForce (new Vector2 (_randomX, _randomY), ForceMode2D.Force);
	}
	
	private void moveToAutoPhosphate()
	{

		_foundAutoPhosphates = GameObject.FindGameObjectsWithTag("autoPhosphate");
		int _numOfAutoPhosphates = _foundAutoPhosphates.GetUpperBound(0);
		if (_numOfAutoPhosphates >= 0)
		{
			_autoPhosphate = _foundAutoPhosphates [_numOfAutoPhosphates].transform;
			//while (transform.position != _autoPhosphate.position)
			transform.position = Vector2.MoveTowards(transform.position, _autoPhosphate.position, maxDistanceDelta: 0.2f);
		} 
	}
}
