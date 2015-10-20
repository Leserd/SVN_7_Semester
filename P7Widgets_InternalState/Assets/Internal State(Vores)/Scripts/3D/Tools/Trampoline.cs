using UnityEngine;
using System.Collections;

public class Trampoline : WidgetTool {

	float _forcePower = 450;
	Vector3 _forceDirection = new Vector3();



	public override void OnCollisionEnter(Collision col)
	{
		if(col.gameObject.tag == "Robot")
		{
			Vector3 robotSpeed = col.gameObject.GetComponent<Rigidbody>().velocity;
			//print(robotSpeed);
			_forceDirection = transform.TransformDirection(Vector3.up);
			
			col.gameObject.GetComponent<Rigidbody>().AddForce(_forceDirection * _forcePower);
			col.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(col.gameObject.GetComponent<Rigidbody>().velocity.x + robotSpeed.x, col.gameObject.GetComponent<Rigidbody>().velocity.y, 0);
		}
	}



	public override void Test()
	{
		//base.Test();
		print("Trampoline not base");
	}
}