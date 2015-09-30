using UnityEngine;
using System.Collections;

public class Goal3D : MonoBehaviour {

	float _timeBeforeDestroy = 0.8f;
	GameManagerScript _gameManager;


	void Awake()
	{
		_gameManager = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
	}



	void OnTriggerEnter(Collider col)
	{
		if(col.tag == "Lemming")
		{
			print("Lemming reached goal!");
			_gameManager.LemmingScored();
			Destroy(col.gameObject, _timeBeforeDestroy);
		}
	}
}