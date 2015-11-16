using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections.Generic;

public class GameManagerScript : NetworkBehaviour {
	
	public GameObject robotPrefab;
	public GameObject clawPrefab;

	public int[] robotsToSpawn;
	public int[] scoreToWin;
	private bool _isSpawning = false;
	private int _robotsRemaining = 0;
	private int _robotsSpawned = 0;
	private int _score;							//number of robots that reached goal in the current level

	private int _savedRobots;					//number of robots that reached goal in all levels combined
	private int _retries;						//Amount of retries to finish the game

	private bool _showSettings = false;

	const int SPAWN_RATE = 1;					//robots spawned per second
	const int DELAY_BEFORE_START_SPAWN = 1;
	const float CAMERA_LERP_TIME = 3;			//The time it takes for camera to move to the next level location

	private int _currentLevel = 1;				//Starts at 1

	public Text scoreText;
	public Text remainingText;
	public Text levelText;

	public GameObject cam;
	public GameObject settingsPanel;
	public GameObject gameMenuPanel;
	public GameObject levelCompletePanel;
	public GameObject retryLevelPanel;
	public GameObject finalScorePanel;			//The final info screen showing the amount of robots saved throughout the game
	public Text levelTextMenu;
	public Image savedToolFill;					//the tool saving progress bar
	public WidgetControlScript[] players;			

	public AudioClip _robotExplosionSound;		//Sound when robot explodes
	public AudioClip _robotScoreSound;			//Sound when robot arrives at goal

	public WidgetDetectionAlgorithm algorithm;

	public Transform[] levelLocations;
	private Transform _currentLevelLocation;

	private AudioSource _audio;

	public Button _levelDisc, _toolboxDisc;
	public Goal _firstGoal;

	void Awake()
	{
		_audio = GetComponent<AudioSource>();
		if(savedToolFill == null) savedToolFill = GameObject.Find("Saved_Fill").GetComponent<Image>();

		GameVariables.LevelDisconnect = _levelDisc;
		GameVariables.ToolboxDisconnect = _toolboxDisc;
		GameVariables.CurrentGoal = _firstGoal;
		GameVariables.GameManager = this;
		GameVariables.SavedToolFill = savedToolFill;

		if(players == null)
		{
			GameVariables.Players = new List<WidgetControlScript>();
			for(int i = 0; i < GameObject.FindObjectsOfType<WidgetControlScript>().Length; i++)
			{
				GameVariables.Players.Add(GameObject.Find("Player " + (i + 1).ToString()).GetComponent<WidgetControlScript>());
			}
		}
		else
		{
			for(int i = 0; i < players.Length; i++)
			{
				GameVariables.Players.Add(players[i]);
			}
		}
		

		GameVariables.ToolboxCanvas = GameObject.Find("ToolboxPanel").GetComponent<Toolbox_Move>();
		GameVariables.WidgetAlgorithm = WidgetDetectionAlgorithm.instance;

		//Set up variables in case they were not set in the inspector
		if(scoreText == null) scoreText = GameObject.Find("ScoredRobots").GetComponent<Text>();
		if(remainingText == null) remainingText = GameObject.Find("RemainingRobots").GetComponent<Text>();
		if(levelText == null) levelText = GameObject.Find("Level").GetComponent<Text>();
		if(levelTextMenu == null) levelTextMenu = GameObject.Find("LevelText").GetComponent<Text>();
		

		if(levelLocations.Length == 0)
			Debug.LogError("No level locations are assigned in the GameManager object!");
		else
		{
			_currentLevelLocation = levelLocations[0];
			GameVariables.CurrentSpawn = _currentLevelLocation.GetComponentInChildren<Spawner>().transform;
		}
	}



	void Start()
	{
		UpdateUI();
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
		ShowStartPanel(false);

		//Enable widget detection algorithm 
		//algorithm.enabled = true;

		//Maybe start countdown before robots spawn
		//....
		
		//Start spawning robots
		StartSpawn();
	}


	



	void StartSpawn()
	{
		if(!_isSpawning && _robotsRemaining == 0)
		{
			InvokeRepeating("Spawn", DELAY_BEFORE_START_SPAWN, SPAWN_RATE);
		}
	}



	void Spawn()
	{
		GameObject go = (GameObject)Instantiate(robotPrefab, GameVariables.CurrentSpawn.position, Quaternion.identity);
		_robotsSpawned++;
		_robotsRemaining++;
		UpdateUI();
		if(_robotsSpawned >= robotsToSpawn[_currentLevel - 1])
		{
			CancelInvoke("Spawn");
			_isSpawning = false;
		}
	}



	void SpawnClaw(Transform robot)
	{
		GameObject newClaw = (GameObject)Instantiate(clawPrefab, _currentLevelLocation.FindChild("Portal").position + (Vector3.up*2), Quaternion.identity);
		newClaw.GetComponent<Claw>().Target = robot;
		newClaw.GetComponent<Claw>().NextSpawn = levelLocations[_currentLevel].GetComponentInChildren<Spawner>().transform;
	}




	public void RobotScored(Transform robot)
	{
		_robotsRemaining--;
		_score++;
		
		UpdateUI();
		
		if(_score >= scoreToWin[_currentLevel - 1] && _robotsRemaining == 0)
		{
			_savedRobots += _score;
			LevelComplete();
		}
		if(_robotsRemaining == 0 && _score < scoreToWin[_currentLevel - 1])
		{
			LoseGame();
		}
	}



	public void RobotDied()
	{
		//Show explosion where robot died
		//...
		//Play explosion sound
		PlaySound(_robotExplosionSound);

		_robotsRemaining--;
		UpdateUI();

		//if(!_isSpawning && _score + _robotsRemaining < scoreToWin[_currentLevel - 1])
		//{
		//	LoseGame();
		//}

		if(_robotsSpawned == robotsToSpawn[_currentLevel - 1] && _robotsRemaining == 0 && _score < scoreToWin[_currentLevel - 1])
		{
			LoseGame();
		}
	}



	public void ShowStartPanel(bool b)
	{
		gameMenuPanel.SetActive(b);
	}



	//Reset variables related to the robots, as new level will start
	void ResetVariables()
	{
		_robotsRemaining = 0;
		_robotsSpawned = 0;
		_score = 0;
		UpdateUI();
	}



	void UpdateUI()
	{
		//The menu title
		levelTextMenu.text = "Level " + _currentLevel;

		//The values in the info bar in the top of the screen
		scoreText.text = "Score: " + _score;
		remainingText.text = "Robots: " + _robotsRemaining;
		levelText.text = "Level: " + _currentLevel;
	}



	public void PrepareNewLevel()
	{
		ResetVariables();
		_currentLevel++;
		_currentLevelLocation = levelLocations[_currentLevel - 1];

		//Enable new goal object
		//GameVariables.CurrentGoal = GameVariables.GameManager.CurrentLevelLocation.GetComponentInChildren<Goal>();
		GameVariables.CurrentGoal = GameVariables.GameManager.CurrentLevelLocation.FindChild("GoalLift").gameObject.AddComponent<Goal>();
		GameVariables.CurrentGoal.level = GameVariables.GameManager.CurrentLevel;
		//GameVariables.CurrentGoal.enabled = true;

		UpdateUI();
	}



	public void ToggleSettings()
	{
		_showSettings = !_showSettings;
		settingsPanel.SetActive(_showSettings);
	}



	public void LevelComplete()
	{
		//TODO: Show a panel saying "Congratulations!" and have a button for moving to the next level
		//Application.LoadLevel("Level " + (currentLevel + 1).ToString());
		//StartCoroutine("MoveCameraToNextLevel");
		if(CurrentLevel < levelLocations.Length)
		{
			ResetWidgetTools();
			levelCompletePanel.SetActive(true);
		}
		else
		{
			GameOver();
		}
		

		
	}



	//This is called when game is lost
	public void LoseGame()
	{
		//TODO: Show canvas with "You lost!" box, one button: Retry.
		//TODO: Retry reloads scene
		ResetWidgetTools();
		retryLevelPanel.SetActive(true);
		//Application.LoadLevel(Application.loadedLevel);
	}



	public void NextLevel(GameObject curPanel)
	{
		curPanel.SetActive(false);

		//if(CurrentLevel <= levelLocations.Length)
		//{
			StartCoroutine("MoveCameraToNextLevel");
			StartCoroutine(GameVariables.CurrentGoal.MoveToNextLocation());
		//}
		//else
		//{
		//	GameOver();
		//}
		
	}



	public void RetryLevel(GameObject curPanel)
	{
		//Increment the amount of times the player has retried a level. Will be shown in the end
		_retries++;

		ResetVariables();
		StartSpawn();

		curPanel.SetActive(false);
	}



	public void GameOver()
	{
		Debug.Log("GameOver");
		Text finalScore = finalScorePanel.transform.FindChild("FinalScore").GetComponent<Text>();
		finalScore.text = "Robots: " + _savedRobots.ToString() + " / " + CalculateMaxRobots() + "\n" + "Retries: " + _retries;
		finalScorePanel.SetActive(true); 
	}





	IEnumerator MoveCameraToNextLevel()
	{
		float elapsedTime = 0;
		Vector3 startingPos = cam.transform.position;
		Vector3 endPos = levelLocations[_currentLevel].position + new Vector3(0, 0, startingPos.z);
		 
		while (elapsedTime < CAMERA_LERP_TIME)
		{
			cam.transform.position = Vector3.Lerp(startingPos, endPos, (elapsedTime / CAMERA_LERP_TIME));
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}

		//Show panel to start new level
		ShowStartPanel(true);
	}



	public void ResetWidgetTools()
	{
		for(int i = 0; i < GameVariables.Players.Count; i++)
		{
			GameVariables.Players[i].RemoveTool();
		}
	}



	public int CalculateMaxRobots()
	{
		int amount = 0;
		for(int i = 0; i < robotsToSpawn.Length; i++)
		{
			amount += robotsToSpawn[i];
		}
		return amount;
	}



	public void PlaySound(AudioClip clip)
	{
		if(clip != null)
		{
			_audio.clip = clip;
			_audio.Play();
		}
		else
		{
			Debug.LogWarning("No audio clip was found.");
		}
	}



	//****************Getters and Setters******************

	public Transform LevelLocation(int index)
	{
		return levelLocations[index];
	}



	public Transform CurrentLevelLocation
	{
		get { return _currentLevelLocation; }
		set { _currentLevelLocation = value; }

	}



	public int CurrentLevel
	{
		get { return _currentLevel; }
		set { _currentLevel = value; }
	}



	public int RobotsSpawned
	{
		get { return _robotsSpawned; }
		set { _robotsSpawned = value; }
	}



	public int RobotsRemaining
	{
		get { return _robotsRemaining; }
		set { _robotsRemaining = value; }
	}



	public int Score
	{
		get { return _score; }
		set { _score = value; }
	}
}