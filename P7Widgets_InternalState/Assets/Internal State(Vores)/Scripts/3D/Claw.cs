using UnityEngine;
using System.Collections;

public class Claw : MonoBehaviour 
{
	
	private Transform _target;
	private Transform _nextSpawn;
	const float LERP_TIME_TO_TARGET = 1;
	const float LERP_TIME_TO_SPAWN = 4;
	private float elapsedTime = 0;
	private Vector3 startingPos;
	private Vector3 endPos;
	private bool _carryingRobot = false;

	// Use this for initialization
	void Start () 
	{
		startingPos = transform.position;
	}
	


	// Update is called once per frame
	void Update () 
	{
		UpdateMovement();
	}



	void UpdateMovement()
	{
		if(_target != null && _carryingRobot == false)
		{
			endPos = _target.position + Vector3.up;

			if(elapsedTime < LERP_TIME_TO_TARGET)
			{
				transform.position = Vector3.Lerp(startingPos, endPos, (elapsedTime / LERP_TIME_TO_TARGET));
				elapsedTime += Time.deltaTime;
			}
		}

		else if(_carryingRobot == true && _nextSpawn != null)
		{
			endPos = _nextSpawn.position;

			if(elapsedTime < LERP_TIME_TO_SPAWN)
			{
				transform.position = Vector3.Lerp(startingPos, endPos, (elapsedTime / LERP_TIME_TO_SPAWN));
				elapsedTime += Time.deltaTime;
			}
		}
	}



	void OnTriggerEnter(Collider col)
	{
		if(col.transform == _target)
		{
			_carryingRobot = true;
			_target.parent = transform;
		}
	}



	public Transform Target
	{
		get { return _target; }
		set { _target = value; }
	}



	public Transform NextSpawn
	{
		get { return _nextSpawn; }
		set { _nextSpawn = value; }
	}
}