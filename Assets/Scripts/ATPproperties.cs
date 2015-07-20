using UnityEngine;
using System.Collections;

public class ATPproperties : MonoBehaviour {

	#region Public Fields + Properties + Events + Delegates + Enums
	
	public Color ActiveColor = Color.white;
	public bool allowMovement = true;
	public bool isActive = true;
	public Color NonActiveColor = Color.gray;
	public Quaternion rotation;
	public bool spin = false;
	
	#endregion Public Fields + Properties + Events + Delegates + Enums
	
	#region Public Methods
	
	public void changeState(bool message)
	{
		this.isActive = message;
		if (this.isActive == false)
		{
			this.allowMovement = false;
			this.GetComponent<ATPpathfinding>().enabled = false;
			foreach (Transform child in this.transform)
			{
				if (child.name == "Phosphate Transport Body")
				{
					child.GetComponent<Renderer>().material.color = NonActiveColor;
					break;
				}
			}
		}
		else
		{
			this.allowMovement = true;
			foreach (Transform child in this.transform)
			{
				if (child.name == "Phosphate Transport Body")
				{
					child.GetComponent<Renderer>().material.color = ActiveColor;
					break;
				}
			}
			this.GetComponent<ATPpathfinding>().enabled = true;
		}
	}



	public void dropOff()
	{
		//Debug.Log("I made it here!!");
		spin = true;
		rotation = transform.rotation * Quaternion.AngleAxis( 180, Vector3.back ); 
		this.gameObject.GetComponent<ATPpathfinding> ().droppedOff = true;
	}
	#endregion Public Methods
	
	#region Private Methods
	
	private void Start()
	{
		ATPproperties objProps = (ATPproperties)this.GetComponent("ATPproperties");
		changeState(objProps.isActive);
	}

	private void Update()
	{
		if (this.isActive == false) {
			this.allowMovement = false;
		}
		if (this.allowMovement == false) {
			this.GetComponent<ATPpathfinding> ().enabled = false;
		}
		if (this.isActive == true) {
			this.allowMovement = true;
			this.GetComponent<ATPpathfinding> ().enabled = true;
		}
		if (spin) {
			transform.rotation = Quaternion.Slerp (transform.rotation, rotation, 2 * Time.deltaTime);
			if (Quaternion.Angle(transform.rotation,rotation)==0 ) {
				//Debug.Log("And I spun around for awhile!!");
				spin = false;

			}
		}
	}
	
	#endregion Private Methods
}
