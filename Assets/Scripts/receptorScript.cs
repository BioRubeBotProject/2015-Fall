using UnityEngine;
using System.Collections;

public class receptorScript : MonoBehaviour
{
	public GameObject _ActiveReceptorA, _ActiveReceptorB, _ActiveReceptorC;

	/* Added 6/18/2015 - E. Rogers
	 * Once ECP has bound with receptor, autoselfphosphorylating occurs.
	 * The following GameObjects will be used in the function autophosphorylate()
	 * found below */
	public GameObject _phosphate, _spawnedPhosphate1, _spawnedPhosphate2;

	private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "ECP")
        {
			ExternalReceptorProperties objProps = (ExternalReceptorProperties)this.GetComponent("ExternalReceptorProperties");
			objProps.isActive = false;
			other.GetComponent<ExtraCellularProperties>().changeState(false);
            StartCoroutine(transformReceptor(other));
		}
    }

	/* Added 6/18/2015 -E.Rogers
	 * autophosphorylate instantiates (creates) two phosphate objects
	 * located at a vector position relative to the receptor so as to
	 * position them on the legs of the receptor.  Their respective RigidBody2D
	 * kinematic states are enabled to allow the phosphates to remain stationary
	 * until such time they are required interact with other objects (later,
	 * g-proteins will bind with these phosphates.*/

	public void autophosphorylate()
	{
		_spawnedPhosphate1 = (GameObject)Instantiate (_phosphate, transform.position + new Vector3 (0.77f, -2.7f, 0), Quaternion.identity);
		_spawnedPhosphate1.GetComponent<Rigidbody2D>().isKinematic = true;
		_spawnedPhosphate2 = (GameObject)Instantiate (_phosphate, transform.position + new Vector3 (-0.8f, -2.7f, 0), Quaternion.identity);
		_spawnedPhosphate2.GetComponent<Rigidbody2D>().isKinematic = true;
	}
	 

    private IEnumerator transformReceptor(Collider2D other)
    {
		// Destroy(other.gameObject);
		// if the player entered the trigger... create the object and get a reference to it: 

		GameObject NewAReceptorA = (GameObject)Instantiate (_ActiveReceptorA, transform.position, transform.rotation);
		// play the sound in the trigger AudioSource: 

		//Debug.Log("starting To waitThreeSeconds2");
		yield return new WaitForSeconds (1);
		GameObject NewAReceptorB = (GameObject)Instantiate (_ActiveReceptorB, NewAReceptorA.transform.position, NewAReceptorA.transform.rotation);
		//Debug.Log("Did we wait?2");
		NewAReceptorA.gameObject.SetActive (false);

		//Debug.Log("starting To waitThreeSeconds3");
		yield return new WaitForSeconds (1);
		GameObject NewAReceptorC = (GameObject)Instantiate (_ActiveReceptorC, NewAReceptorB.transform.position, NewAReceptorB.transform.rotation);

		NewAReceptorB.gameObject.SetActive (false);
		//Debug.Log("Did we wait3?");
		this.gameObject.SetActive (false);

		//Added 6/18/2015 by E. Rogers - add phosphates to receptor legs:
		autophosphorylate();
	}
}