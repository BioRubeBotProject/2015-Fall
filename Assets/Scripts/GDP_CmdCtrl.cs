using UnityEngine;
using System.Collections;

public class GDP_CmdCtrl : MonoBehaviour {

	public ParticleSystem destructionEffect;	// 'poof' special effect for 'expended' GDP

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (tag == "ReleasedGDP") {
			tag = "DyingGDP";
			StartCoroutine (ReleasingGDP ());
			StartCoroutine (DestroyGDP ()); //Destroy GDP
		}

		Roam.Roaming ( this.gameObject );
	}

	//	ReleasingGDP waits for 3 seconds after docking before actually releasing the GDP  */
	public IEnumerator ReleasingGDP ()
	{
		yield return new WaitForSeconds (3f);
		transform.parent = null;
		transform.GetComponent<Rigidbody2D> ().isKinematic = false;
		transform.GetComponent<CircleCollider2D> ().enabled = true;
	} 
	//	6 seconds after the GDP is released it will be destroyed in a puff of smoke (of sorts)
	public IEnumerator DestroyGDP()
	{
		yield return new WaitForSeconds (6f);
		ParticleSystem explosionEffect = Instantiate(destructionEffect) as ParticleSystem;
		explosionEffect.transform.position = transform.position;
		explosionEffect.loop = false;
		explosionEffect.Play();
		Destroy(explosionEffect.gameObject, explosionEffect.duration);
		Destroy(gameObject);
	}
}
