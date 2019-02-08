using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doggo : MonoBehaviour
{

	public LayerMask ground;
	public GameObject neck;
	public GameObject tail;
	public GameObject tailHide;
	public GameObject boomParticle;

	bool wagRight = true;

	bool isSleeping = false;
	float sleepTimer = 0f;
	float sleepTime = 1000f;
	Vector3 lastpos;

	int isPet = 0;

	bool doesBoom = true;
	float boomTime = 5f;
	float boomTimer = 0f;
	public static List<Rigidbody> bodies;
	public AudioClip pop;
	public AudioClip boom;


	void Awake(){
		Debug.Log ("POTATO!");
		if (doesBoom && bodies == null) {
			bodies = new List<Rigidbody> ();
			Rigidbody[] presentBodies = GameObject.FindObjectsOfType<Rigidbody> ();
			for (int i = 0; i < presentBodies.Length; i++) {
				bodies.Add (presentBodies [i]);
			}
		}
	}
	// Update is called once per frame
	void FixedUpdate ()
	{

		//float logic
		RaycastHit hit;
		if (Physics.Raycast (transform.position, -Vector3.up, out hit, Mathf.Infinity, ground, QueryTriggerInteraction.Ignore)) {
			Vector3 upforce = Vector3.up * (hit.point.y + 0.5f - transform.position.y) * 450f;
			if (upforce.y < 0)
				upforce = Vector3.zero;
			if (sleepTimer < sleepTime)
				GetComponent<Rigidbody> ().AddForce (upforce);
		}
		Quaternion store = transform.rotation;
		transform.LookAt (Vector3.zero + Vector3.up * transform.position.y);
		Vector3 rot = transform.eulerAngles;
		transform.rotation = store;

		/*float yaxis = transform.eulerAngles.y;
		if (yaxis > 180)
			yaxis = yaxis - 360f;
		float torqueAmount = rot.y - yaxis;
		if (torqueAmount > 180)
			torqueAmount -= 360f;*/
		 float torqueAmount = 0f;
		//GetComponent<Rigidbody> ().AddRelativeTorque (new Vector3 (0f, torqueAmount, 0f).normalized * (((Mathf.Abs (torqueAmount)) / 40f)));


		float anglex = (transform.eulerAngles.y > 180) ? 70 : -70;
		float xaxis = (transform.eulerAngles.x < 180)?transform.eulerAngles.x:transform.eulerAngles.x-360f;
		float normalx = -Mathf.Lerp (anglex * -hit.normal.x, anglex * -hit.normal.z, Mathf.Abs (Vector3.Dot (Vector3.forward, transform.forward))) + xaxis;
		normalx = (normalx < 180) ? normalx : normalx - 360;
		float anglez = (transform.eulerAngles.y > 90 && transform.eulerAngles.y < 270) ? -70 : 70;
		float zaxis = (transform.eulerAngles.z < 180)?transform.eulerAngles.z:transform.eulerAngles.z-360f;

		float normalz = -Mathf.Lerp (anglez * -hit.normal.z, anglez * -hit.normal.x, Mathf.Abs (Vector3.Dot (Vector3.right, transform.right))) + zaxis;
		normalz = (normalz < 180) ? normalz : normalz - 360;
		float bank = ((-torqueAmount/3f) + normalz < 180)?(-torqueAmount/3f) + normalz:(-torqueAmount/3f) + normalz - 360f;
		GetComponent<Rigidbody> ().AddRelativeTorque (new Vector3(-normalx,0f,-bank).normalized * ((Mathf.Abs(normalx) + Mathf.Abs(bank))/50f));







		//keeps doggo from breaking neck
		Vector3 neckRot = new Vector3 ((neck.transform.localEulerAngles.x < 180) ? neck.transform.localEulerAngles.x : neck.transform.localEulerAngles.x - 360, (neck.transform.localEulerAngles.y < 180) ? neck.transform.localEulerAngles.y : neck.transform.localEulerAngles.y - 360, (neck.transform.localEulerAngles.z < 180) ? neck.transform.localEulerAngles.z : neck.transform.localEulerAngles.z - 360);
		neck.transform.localEulerAngles = new Vector3 (Mathf.Clamp (neckRot.x, -90, 90), Mathf.Clamp (neckRot.y, -90, 90), Mathf.Clamp (neckRot.z, -90, 90));

		if (isPet > 0) //pet timer
			isPet--;
		if (isPet == 0 && sleepTimer < sleepTime) { //if not being pet or sleeping, act normal
			neck.transform.localRotation = Quaternion.Lerp (neck.transform.localRotation, Quaternion.identity, Time.deltaTime);
			tail.GetComponent<HingeJoint> ().useMotor = false;
		}
		if (sleepTimer > sleepTime) //if its been long enough to sleep, start checking for movement
			lastpos = transform.position;
		if (lastpos != null && lastpos == transform.position && isPet == 0) {  //if not moving or being pet, start counting down to sleep
			sleepTimer += Time.fixedDeltaTime;
		} else { //otherwise reset sleep countdown
			sleepTimer = 0f;
		}
		//if (path != null && path.Count > 5f) { //wake up if doggo has places to go
		//	sleepTimer = 0f;
		//}
		lastpos = transform.position;

		if (sleepTimer > sleepTime) { //go to sleep
			neck.transform.localRotation = Quaternion.Lerp (neck.transform.localRotation, Quaternion.Euler (new Vector3 (60f, 0f, 0f)), Time.deltaTime);
			tailHide.GetComponent<MeshRenderer> ().enabled = true;
			tail.GetComponent<MeshRenderer> ().enabled = false;
			JointSpring temp = tailHide.GetComponent<HingeJoint> ().spring;
			temp.targetPosition = -90f;
			tailHide.GetComponent<HingeJoint> ().spring = temp;

		}
		//if (GameObject.FindObjectOfType<Pathfinding> ().startNode == null) { //if no path is open, teleport behind player
		//	transform.position = GameObject.FindObjectOfType<SteamVR_Camera> ().transform.position - transform.forward * 0.5f;
		//}



		if (doesBoom) {
			boomTimer += Time.fixedDeltaTime;
			if (boomTimer > boomTime) {
				Destroy (tailHide);
				for (int i = transform.childCount - 1; i > -1; i--) {
					Destroy (transform.GetChild (i).gameObject, 5f);
					transform.GetChild (i).gameObject.AddComponent<Rigidbody> ();
					transform.GetChild (i).GetComponent<Rigidbody> ().useGravity = true;
					if (transform.GetChild (i).GetComponent<HingeJoint> () != null)
						Destroy (transform.GetChild (i).GetComponent<HingeJoint> ());
					bodies.Add(transform.GetChild(i).GetComponent<Rigidbody>());
					transform.GetChild (i).parent = null;
				}
				for (int i = 0; i < bodies.Count; i++) {
					if (bodies [i] != null) {
						bodies [i].AddExplosionForce (1000f, transform.position, 10f);
					} else {
						bodies.RemoveAt (i);
					}
				}
				GameObject boomPart = Instantiate (boomParticle, transform.position, Quaternion.identity)as GameObject;
				Destroy (boomPart, 1f);
				AudioSource.PlayClipAtPoint (boom, transform.position);
				Destroy (gameObject);
			}
		}
	}
	/*void OnTriggerStay(Collider Col){ //petting logic
		if (Col.gameObject.GetComponentInParent<Hand>() != null) {
			isPet = 5;
			Quaternion start = neck.transform.rotation;
			neck.transform.LookAt (GameObject.FindObjectOfType<SteamVR_Camera> ().transform);
			neck.transform.rotation = Quaternion.Lerp (start, neck.transform.rotation, Time.deltaTime);
			JointSpring temp = tailHide.GetComponent<HingeJoint> ().spring;
			temp.targetPosition = 0f;
			tailHide.GetComponent<HingeJoint> ().spring = temp;
			if (tailHide.transform.eulerAngles.z > -38) {
				tailHide.GetComponent<MeshRenderer> ().enabled = false;
				tail.GetComponent<MeshRenderer> ().enabled = true;
				tail.GetComponent<HingeJoint> ().useMotor = true;
				JointMotor temp2 = new JointMotor ();
				if (tail.transform.localEulerAngles.x > 30f && tail.transform.localEulerAngles.x < 90f) {
					wagRight = true;
				}
				if (tail.transform.localEulerAngles.x > 270f && tail.transform.localEulerAngles.x < 330f) {
					wagRight = false;
				}
				if (wagRight){
					temp2.targetVelocity = -120f* (Col.GetComponentInParent<VelocityEstimator> ().GetVelocityEstimate ().magnitude+Col.GetComponentInParent<VelocityEstimator> ().GetAngularVelocityEstimate ().magnitude);
				} else {
					temp2.targetVelocity = 120f * (Col.GetComponentInParent<VelocityEstimator> ().GetVelocityEstimate ().magnitude+Col.GetComponentInParent<VelocityEstimator> ().GetAngularVelocityEstimate ().magnitude);
				}
				temp2.force = 100f;
				tail.GetComponent<HingeJoint> ().motor = temp2;
			}
		}
	}*/
}
