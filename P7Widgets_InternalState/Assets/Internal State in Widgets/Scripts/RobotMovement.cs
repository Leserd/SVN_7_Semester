using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotMovement : MonoBehaviour 
{

	Rigidbody _rigid;											//The rigidbody component
	float _speed = 2f;								
	float _decelerationSpeed = 0.96f;
	float _decelerationSpeedFalling = 0.98f;
	[SerializeField]bool _grounded = false;
	Transform _groundObj;										//The object this robot is currently standing on
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
		//Update to see if the robot has fallen too far and should be destroyed
		UpdateGravity();

		//Update the grounded state based on collisions
		UpdateGrounded();

		//Update movement of the robot
		UpdateMovement();
	}



	void UpdateGravity()
	{


		//outcommented as force is no longer used
		//if(_grounded)
		//	_rigid.velocity *= _decelerationSpeed;
		//else
		//	_rigid.velocity *= _decelerationSpeedFalling;

		if(transform.position.y < -5){
			_gameManager.RobotDied();
			Destroy(gameObject);
		}
	}



	public void UpdateGrounded()
	{
		//Check for which direction the collision happens, only ground if direction is down
		if(_collisions.Count == 0)
		{
			Grounded = false;
		}
		else
		{
			if(Grounded != true)
				Grounded = true;

			//_groundObj = null;
			for(int i = 0; i < _collisions.Count; i++)
			{
				Vector3 dir = (_collisions[i].transform.position - gameObject.transform.position).normalized;

				//Remove from collisions if it is disabled
				if(_collisions[i].gameObject.GetComponent<BoxCollider>())
					if(_collisions[i].gameObject.GetComponent<BoxCollider>().enabled == false)
						_collisions.Remove(_collisions[i].gameObject);

				if(dir.y > 0)
				{
					//If the collision is up, remove it from list
					_collisions.Remove(_collisions[i].gameObject);
				}
			}
		}

	}



	void UpdateMovement()
	{
		Vector3 dir = transform.TransformDirection(transform.right);
		if(_grounded)
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



	void OnCollisionExit(Collision col)
	{
		
		//if(col.gameObject.tag == "Ground")
		//{
		//	//_collisions.Remove(col.gameObject);

		//	//Grounded = UpdateGrounded();

		//	Grounded = false;
		//}


		if(_collisions.Contains(col.gameObject))
		{
			_collisions.Remove(col.gameObject);
		}
	}



	void OnCollisionEnter(Collision col)
	{
		if(col.gameObject.tag == "Ground")
		{
			//Check in which direction the collision occured
			Vector3 dir = (col.gameObject.transform.position - gameObject.transform.position).normalized;

			//If the collision happened downwards, set grounded = true;
			if(dir.y < 0)
			{
				//print("DOWN");
				//_collisions.Add(col.gameObject);

				//Grounded = UpdateGrounded();
				//_groundObj = col.transform;
				//Grounded = true;
				_collisions.Add(col.gameObject);
			}
		}
	}
}