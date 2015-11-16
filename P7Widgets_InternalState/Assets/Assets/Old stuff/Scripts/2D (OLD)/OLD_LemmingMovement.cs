using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OLD_LemmingMovement : MonoBehaviour 
{

	Rigidbody2D _rigid;								//The rigidbody component
	float _speed = 3f;							//300
	float _decelerationSpeed = 0.96f;
	float _decelerationSpeedFalling = 0.98f;
	[SerializeField]bool _grounded = false;
	Transform _groundObj;							//The object this lemming is currently standing on
	float _groundedFadeTime = 0.4f;
	List<GameObject> _collisions = new List<GameObject>();

	//Fall time
	//Smid alle OnCollision herind og så bare kald en generisk funktion på tool


	void Awake () 
	{
		_rigid = this.GetComponent<Rigidbody2D>();
	}
	


	void Update ()
	{
		UpdateGravity();
		UpdateMovement();

		//print(_collisions.Count);
	}



	void UpdateGravity()
	{



		//if(_grounded)
		//	_rigid.velocity *= _decelerationSpeed;
		//else
		//	_rigid.velocity *= _decelerationSpeedFalling;
	}



	void UpdateMovement()
	{
		Vector2 dir = transform.TransformDirection(transform.right);
		if(_grounded && _groundObj != null)
		{
			
			//_rigid.AddForce(dir * _speed * Time.deltaTime);
			transform.Translate(new Vector3(dir.x, dir.y, 0) * _speed * Time.deltaTime);
			//print(dir);
		} else
		{
			transform.Translate(new Vector3(dir.x, dir.y - 1, 0) * _speed/3 * Time.deltaTime);
		}
			
	}



	public bool Grounded
	{
		get { return _grounded; }
		set { _grounded = value;
		GetComponent<Rigidbody2D>().gravityScale = System.Convert.ToInt32(!value);
		//print(GetComponent<Rigidbody2D>().gravityScale + ": " + !value + " : " + Time.time);
		}
	}



	bool UpdateGrounded()
	{
		if(_collisions.Count > 0)
			return true;
		else
		{
			_groundObj = null;
			return false;
		}
			
	}



	void OnCollisionExit2D(Collision2D col)
	{
		if(col.gameObject.tag == "Ground")
		{
			_collisions.Remove(col.gameObject);

			Grounded = UpdateGrounded();
		}
	}



	void OnCollisionEnter2D(Collision2D col)
	{
		if(col.gameObject.tag == "Ground")
		{
			_collisions.Add(col.gameObject);

			Grounded = UpdateGrounded();
			_groundObj = col.transform;
		}
	}
}