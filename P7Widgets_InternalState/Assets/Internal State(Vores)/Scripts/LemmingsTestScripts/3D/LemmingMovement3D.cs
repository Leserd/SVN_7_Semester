using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LemmingMovement3D : MonoBehaviour 
{

	Rigidbody _rigid;								//The rigidbody component
	float _speed = 2f;							//300
	float _decelerationSpeed = 0.96f;
	float _decelerationSpeedFalling = 0.98f;
	[SerializeField]bool _grounded = false;
	Transform _groundObj;							//The object this lemming is currently standing on
	float _groundedFadeTime = 0.4f;
	List<GameObject> _collisions = new List<GameObject>();
	GameManagerScript _gameManager;

	//Fall time
	//Smid alle OnCollision herind og så bare kald en generisk funktion på tool


	void Awake () 
	{
		_rigid = this.GetComponent<Rigidbody>();
		_gameManager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
	}
	


	void Update ()
	{
		UpdateGravity();
		UpdateMovement();

		//print(_collisions.Count);
	}



	void UpdateGravity()
	{


		//outcommented as force is no longer used
		//if(_grounded)
		//	_rigid.velocity *= _decelerationSpeed;
		//else
		//	_rigid.velocity *= _decelerationSpeedFalling;

		if(transform.position.y < -5){
			_gameManager.LemmingDied();
			Destroy(gameObject);
		}
	}



	void UpdateMovement()
	{
		Vector3 dir = transform.TransformDirection(transform.right);
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
		GetComponent<Rigidbody>().useGravity = !value;
		//print(GetComponent<Rigidbody2D>().gravityScale + ": " + !value + " : " + Time.time);
		}
	}



	bool UpdateGrounded()
	{
		//Check for which direction the collision happens, only ground if direction is down
		if(_collisions.Count > 0)
			return true;
		else
		{
			_groundObj = null;
			return false;
		}
			
	}



	void OnCollisionExit(Collision col)
	{
		
		if(col.gameObject.tag == "Ground")
		{
			_collisions.Remove(col.gameObject);

			Grounded = UpdateGrounded();
		}
	}



	void OnCollisionEnter(Collision col)
	{
		if(col.gameObject.tag == "Ground")
		{
			
			_collisions.Add(col.gameObject);

			Grounded = UpdateGrounded();
			_groundObj = col.transform;
		}
	}
}