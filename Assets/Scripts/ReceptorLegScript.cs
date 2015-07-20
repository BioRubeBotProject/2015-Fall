using UnityEngine;
using System.Collections;

//Updated 6/27/2015 - E. Rogers
//Lines 20-21:  Disable ATP collider while dropping off a phosphate
//Lines 29-30:  Enable the ATP collider once phosphate dropped off
//Lines 27-32:  Change receptor leg tags (referenced in moveG_Protein_Alt.cs)

public class ReceptorLegScript : MonoBehaviour {

	public ParticleSystem DestructionEffect;

	private IEnumerator OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "ATP")
		{

			ReceptorLegProperties objProps = (ReceptorLegProperties)this.GetComponent("ReceptorLegProperties");
			objProps.isActive = false;
			//turn off collider while dropping off phosphate
			other.GetComponent<CircleCollider2D>().enabled = false;
			other.GetComponent<ATPproperties>().changeState(false);
			other.GetComponent<ATPproperties>().dropOff();

			yield return new WaitForSeconds(3);
			Transform tail = other.transform.FindChild ("Tail");
			tail.transform.SetParent (transform);
			objProps.GetComponent<CircleCollider2D>().enabled = false;			
			other.GetComponent<ATPproperties>().changeState(true);
			//turn collider back on
			other.GetComponent<CircleCollider2D>().enabled = true;
			other.gameObject.tag = "Untagged";

			//code added to identify a 'left' receptor phosphate for G-protein docking
			//if it is a left phosphate, G-protein must rotate to dock
			//NOTE: EACH PHOSPHATE ATTACHED TO A RECEPTOR IS NOW TAGGED AS "receptorPhosphate"
			tail.transform.tag = "receptorPhosphate";
			if (transform.name == "_InnerReceptorFinalLeft")
				tail.transform.GetChild(0).tag = "left";
			StartCoroutine(Explode (other.gameObject));
		}
	}
	private IEnumerator Explode(GameObject other)
	{
		yield return new WaitForSeconds (3f);
		//Instantiate our one-off particle system
		ParticleSystem explosionEffect = Instantiate(DestructionEffect) as ParticleSystem;
		explosionEffect.transform.position = other.transform.position;
		
		//play it
		explosionEffect.loop = false;
		explosionEffect.Play();
		
		//destroy the particle system when its duration is up, right
		//it would play a second time.
		Destroy(explosionEffect.gameObject, explosionEffect.duration);
		
		//destroy our game object
		Destroy(other.gameObject);
		
	}

}
