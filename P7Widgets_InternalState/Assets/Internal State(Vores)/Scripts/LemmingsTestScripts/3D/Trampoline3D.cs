using UnityEngine;
using System.Collections;

public class Trampoline3D : WidgetTool3D {

	float _forcePower = 450;
	Vector3 _forceDirection = new Vector3();



	public override void OnCollisionEnter(Collision col)
	{
		if(col.gameObject.tag == "Lemming")
		{
			Vector3 lemmingSpeed = col.gameObject.GetComponent<Rigidbody>().velocity;
			//print(lemmingSpeed);
			_forceDirection = transform.TransformDirection(Vector3.up);
			
			col.gameObject.GetComponent<Rigidbody>().AddForce(_forceDirection * _forcePower);
			col.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(col.gameObject.GetComponent<Rigidbody>().velocity.x + lemmingSpeed.x, col.gameObject.GetComponent<Rigidbody>().velocity.y, 0);
		}
	}



	public override void Test()
	{
		//base.Test();
		print("Trampoline not base");
	}
}