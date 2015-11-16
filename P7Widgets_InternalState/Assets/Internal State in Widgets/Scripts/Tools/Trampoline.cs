using UnityEngine;
using System.Collections;

public class Trampoline : MonoBehaviour {

	float _forcePower = 400;
	Vector3 _forceDirection = new Vector3();



	public void OnCollisionEnter(Collision col)
	{
		if(col.gameObject.tag == "Robot")
		{
			Vector3 robotSpeed = col.gameObject.GetComponent<Rigidbody>().velocity;
			col.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(col.gameObject.GetComponent<Rigidbody>().velocity.x, 0, col.gameObject.GetComponent<Rigidbody>().velocity.z);
			//print(robotSpeed);
			_forceDirection = transform.TransformDirection(Vector3.up);
			
			col.gameObject.GetComponent<Rigidbody>().AddForce(_forceDirection * _forcePower);
			col.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(col.gameObject.GetComponent<Rigidbody>().velocity.x + robotSpeed.x, col.gameObject.GetComponent<Rigidbody>().velocity.y, 0);
		}
	}
}