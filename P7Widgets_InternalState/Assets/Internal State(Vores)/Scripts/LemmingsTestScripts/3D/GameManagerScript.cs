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

	private bool _showSettings = false;

	const int SPAWN_RATE = 1;				//Lemmings spawned per second
	const int DELAY_BEFORE_START_SPAWN = 1;

	public int currentLevel;

	public Text scoreText;
	public Text remainingText;
	public Text deadText;

	public GameObject settingsPanel;
	public GameObject gameMenuPanel;
	public Text levelText;

	public WidgetDetectionAlgorithm algorithm;

	void Awake()
	{
		_spawnLoc = GameObject.Find("LemmingSpawn").transform;
		scoreText = GameObject.Find("ScoredLemmings").GetComponent<Text>();
		remainingText = GameObject.Find("RemainingLemmings").GetComponent<Text>();
		deadText = GameObject.Find("DeadLemmings").GetComponent<Text>();
		levelText = GameObject.Find("LevelText").GetComponent<Text>();
	}



	void Start()
	{
		levelText.text = Application.loadedLevelName;
	}



	void Update () {
		if(Input.GetKeyDown(KeyCode.Space))
		{
			StartSpawn();
		}
	}



	public void StartGame()
	{
		//Called from Start Game button

		//Disable game menu panel
		gameMenuPanel.SetActive(false);

		//Enable widget detection algorithm 
		//algorithm.enabled = true;

		//Maybe start countdown before lemmings spawn
		//....

		//Start spawning lemmings
		StartSpawn();


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
		if(_score == scoreToWin)
		{
			LevelComplete();
		}
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



	public void LevelComplete()
	{
		//Show a panel saying "Congratulations!" and have a button for moving to the next level
		Application.LoadLevel("Level " + (currentLevel + 1).ToString());
	}



	//This is called when game is lost
	public void LoseGame()
	{
		//Show canvas with "You lost!" box, one button: Retry.
		//Retry reloads scene
		//Application.LoadLevel(Application.loadedLevel);
	}



	void UpdateUI()
	{
		scoreText.text = "Score: " + _score;
		remainingText.text = "Lemmings: " + _lemmingsRemaining;
		deadText.text = "Deaths: " + (_lemmingsSpawned - _lemmingsRemaining - _score).ToString();
	}



	public void ToggleSettings()
	{
		_showSettings = !_showSettings;
		settingsPanel.SetActive(_showSettings);
	}
}