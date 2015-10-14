// **************************************************************
// **** Updated on 10/08/15 by Kevin Means
// **** 1.) removed excessive "phases" of receptor
// **** 2.) rotates opposite direction for left receptor leg
// **************************************************************

using System.Collections;
using UnityEngine;

public class receptorScript : MonoBehaviour
{
	#region Public Fields + Properties + Events + Delegates + Enums
	
	public GameObject _ActiveReceptor;
	
	#endregion Public Fields + Properties + Events + Delegates + Enums
	
	#region Private Methods
	
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "ECP") {
			ExternalReceptorProperties objProps = (ExternalReceptorProperties)this.GetComponent("ExternalReceptorProperties");
			objProps.isActive = false;
			other.GetComponent<ExtraCellularProperties>().changeState(false);
			other.GetComponent<Rigidbody2D>().isKinematic = true;
			StartCoroutine(transformReceptor(other));
		}
	}
	
	private IEnumerator transformReceptor(Collider2D other)
	{
		yield return new WaitForSeconds(2);
		GameObject NewReceptor = (GameObject)Instantiate(_ActiveReceptor, transform.position, transform.rotation);
    GameObject.Find("EventSystem").GetComponent<ObjectCollection>().Add (NewReceptor);
		this.gameObject.SetActive(false);
	}
	
	#endregion Private Methods
}