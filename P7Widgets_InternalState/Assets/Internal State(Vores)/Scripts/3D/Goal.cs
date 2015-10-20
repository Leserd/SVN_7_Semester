using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour {

	float _timeBeforeDestroy = 0.1f;
	GameManagerScript _gameManager;


	void Awake()
	{
		_gameManager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
	}



	void OnTriggerEnter(Collider col)
	{
		if(col.tag == "Robot" && col.GetComponent<Rigidbody>().isKinematic == false)
		{
			print("Robot reached goal!");
			_gameManager.RobotScored(col.transform);
			col.GetComponent<RobotMovement>().enabled = false;
			col.GetComponent<Rigidbody>().isKinematic = true;
			//Destroy(col.gameObject, _timeBeforeDestroy);		//TODO: no longer destroy, instead fly robot out of screen.
		}
	}
}