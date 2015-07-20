using UnityEngine;
using System.Collections;

public class Transparent : MonoBehaviour {

	public float alpha;
	// Use this for initialization
	void Start () {
		Color a = this.gameObject.GetComponentInChildren<Renderer> ().material.color;
		// ().material.color;
		a.a = alpha;
		
		this.gameObject.GetComponentInChildren<Renderer> ().material.color = a;
	}
	
	// Update is called once per frame
	void Update () {

	}
}
