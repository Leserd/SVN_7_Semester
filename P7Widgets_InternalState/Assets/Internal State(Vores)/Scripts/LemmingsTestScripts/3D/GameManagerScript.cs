using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour {

	public GameObject lemmingPrefab;
	public int lemmingsToSpawn;
	public int scoreToWin;
	private Transform _spawnLoc;
	private bool _isSpawning = false;
	private int _lemmingsRemaining = 0;
	private int _lemmingsSpawned = 0;
	private int _score;						//number of lemmings that reached goal

	const int SPAWN_RATE = 1;				//Lemmings spawned per second
	const int DELAY_BEFORE_START_SPAWN = 1;

	public Text scoreText;
	public Text remainingText;
	public Text deadText;

	void Awake()
	{
		_spawnLoc = GameObject.Find("LemmingSpawn").transform;
		scoreText = GameObject.Find("ScoredLemmings").GetComponent<Text>();
		remainingText = GameObject.Find("RemainingLemmings").GetComponent<Text>();
		deadText = GameObject.Find("DeadLemmings").GetComponent<Text>();
	}



	void Update () {
		if(Input.GetKeyDown(KeyCode.Space))
		{
			StartSpawn();
		}
	}



	void StartSpawn()
	{
		if(!_isSpawning && _lemmingsRemaining == 0)
		{
			InvokeRepeating("Spawn", DELAY_BEFORE_START_SPAWN, SPAWN_RATE);
		}
	}



	void Spawn()
	{
		GameObject go = (GameObject)Instantiate(lemmingPrefab, _spawnLoc.position, Quaternion.identity);
		_lemmingsSpawned++;
		_lemmingsRemaining++;
		UpdateUI();
		if(_lemmingsSpawned >= lemmingsToSpawn)
		{
			CancelInvoke("Spawn");
			_isSpawning = false;
		}

	}



	public void LemmingScored()
	{
		_lemmingsRemaining--;
		_score++;
		UpdateUI();
	}



	public void LemmingDied()
	{
		_lemmingsRemaining--;
		UpdateUI();
		if(!_isSpawning && _score + _lemmingsRemaining < scoreToWin)
		{
			LoseGame();
		}
	}



	//This is called when game is lost
	public void LoseGame()
	{
		
	}



	void UpdateUI()
	{
		scoreText.text = "Score: " + _score;
		remainingText.text = "Lemmings: " + _lemmingsRemaining;
		deadText.text = "Deaths: " + (_lemmingsSpawned - _lemmingsRemaining - _score).ToString();
	}
}