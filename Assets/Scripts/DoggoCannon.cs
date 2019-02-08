using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoggoCannon : MonoBehaviour {

	public GameObject doggo;

	float cooldown = 0.05f;
	float coolTimer = 0f;
	float deviation = 1000f;

	void Update () {
		if (Input.GetMouseButton(0)){
			coolTimer += Time.deltaTime;
			if (coolTimer > cooldown) {
				coolTimer = 0f;
				GameObject newDog = Instantiate (doggo, transform.position + transform.forward * 0.3f, transform.rotation) as GameObject;
				newDog.GetComponent<Rigidbody> ().AddForce (transform.forward * 30000f + transform.up * (Random.value * 2 - 1f)* deviation + transform.right * (Random.value * 2 - 1f)* deviation);
				Doggo.bodies.Add (newDog.GetComponent<Rigidbody> ());
				AudioSource.PlayClipAtPoint (newDog.GetComponent<Doggo>().pop, transform.position);
			}
		}
		if (Input.GetMouseButtonDown (1)) {
			GameObject newDog = Instantiate (doggo, transform.position + transform.forward * 0.3f, transform.rotation) as GameObject;
			Doggo.bodies.Add (newDog.GetComponent<Rigidbody> ());
			AudioSource.PlayClipAtPoint (newDog.GetComponent<Doggo>().pop, transform.position);
		}
		if (Input.GetKeyDown (KeyCode.R)) {
			Doggo.bodies = null;
			SceneManager.LoadScene (0);
		}
	}
}
