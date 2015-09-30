using UnityEngine;
using System.Collections;

public class Trampoline : WidgetTool {

	float _forcePower = 450;
	Vector3 _forceDirection = new Vector3();



	public override void OnCollisionEnter2D(Collision2D col)
	{
		if(col.gameObject.tag == "Lemming")
		{
			Vector2 lemmingSpeed = col.gameObject.GetComponent<Rigidbody2D>().velocity;
			print(lemmingSpeed);
			_forceDirection = transform.TransformDirection(Vector3.up);
			
			col.gameObject.GetComponent<Rigidbody2D>().AddForce(_forceDirection * _forcePower);
			col.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(col.gameObject.GetComponent<Rigidbody2D>().velocity.x + lemmingSpeed.x, col.gameObject.GetComponent<Rigidbody2D>().velocity.y);
		}
	}
}