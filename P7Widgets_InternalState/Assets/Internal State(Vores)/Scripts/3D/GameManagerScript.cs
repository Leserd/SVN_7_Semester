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
	private Transform _spawnLoc;
	private bool _isSpawning = false;
	private int _robotsRemaining = 0;
	private int _robotsSpawned = 0;
	private int _score;							//number of robots that reached goal

	private bool _showSettings = false;

	const int SPAWN_RATE = 1;					//robots spawned per second
	const int DELAY_BEFORE_START_SPAWN = 1;
	const float CAMERA_LERP_TIME = 3;			//The time it takes for camera to move to the next level location

	private int currentLevel = 1;				//Starts at 1

	public Text scoreText;
	public Text remainingText;
	public Text levelText;

	public GameObject cam;
	public GameObject settingsPanel;
	public GameObject gameMenuPanel;
	public Text levelTextMenu;

	public WidgetDetectionAlgorithm algorithm;

	public Transform[] levelLocations;
	private Transform _currentLevelLocation;

	public Button _levelDisc, _toolboxDisc;

	void Awake()
	{
		GameVariables.LevelDisconnect = _levelDisc;
		GameVariables.ToolboxDisconnect = _toolboxDisc;

		GameVariables.Players = new List<WidgetControlScript>();
		for(int i = 0; i < GameObject.FindObjectsOfType<WidgetControlScript>().Length; i++)
		{
			GameVariables.Players.Add(GameObject.Find("Player " + (i + 1).ToString()).GetComponent<WidgetControlScript>());
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
			_spawnLoc = _currentLevelLocation.FindChild("RobotSpawn");
		}
	}



	void Start()
	{
		
	}



	void Update () {
		if(Input.GetKeyDown(KeyCode.Space))
		{
			StartSpawn();
		}

		if(settingsPanel == null)
		{
			
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
		GameObject go = (GameObject)Instantiate(robotPrefab, _spawnLoc.position, Quaternion.identity);
		_robotsSpawned++;
		_robotsRemaining++;
		UpdateUI();
		if(_robotsSpawned >= robotsToSpawn[currentLevel - 1])
		{
			CancelInvoke("Spawn");
			_isSpawning = false;
		}
	}



	void SpawnClaw(Transform robot)
	{
		GameObject newClaw = (GameObject)Instantiate(clawPrefab, _currentLevelLocation.FindChild("Portal").position + (Vector3.up*2), Quaternion.identity);
		newClaw.GetComponent<Claw>().Target = robot;
		newClaw.GetComponent<Claw>().NextSpawn = levelLocations[currentLevel].FindChild("RobotSpawn");
	}




	public void RobotScored(Transform robot)
	{
		_robotsRemaining--;
		_score++;
		
		UpdateUI();

		SpawnClaw(robot);
		
		if(_score == scoreToWin[currentLevel - 1])		//TODO: wait until all robots are dead or have gone through, before finishing level
		{
			LevelComplete();
		}
	}



	public void RobotDied()
	{
		_robotsRemaining--;
		UpdateUI();
		if(!_isSpawning && _score + _robotsRemaining < scoreToWin[currentLevel - 1])
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
		levelTextMenu.text = Application.loadedLevelName;

		//The values in the info bar in the top of the screen
		scoreText.text = "Score: " + _score;
		remainingText.text = "Robots: " + _robotsRemaining;
		levelText.text = "Level: " + currentLevel;
	}



	public void PrepareNewLevel()
	{
		ResetVariables();
		currentLevel++;
		_currentLevelLocation = levelLocations[currentLevel - 1];
		_spawnLoc = _currentLevelLocation.FindChild("RobotSpawn");

		UpdateUI();
		ShowStartPanel(true);

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
		StartCoroutine("MoveCameraToNextLevel");
	}



	//This is called when game is lost
	public void LoseGame()
	{
		//TODO: Show canvas with "You lost!" box, one button: Retry.
		//TODO: Retry reloads scene
		//Application.LoadLevel(Application.loadedLevel);
	}



	IEnumerator MoveCameraToNextLevel()
	{
		float elapsedTime = 0;
		Vector3 startingPos = cam.transform.position;
		Vector3 endPos = levelLocations[currentLevel].position + new Vector3(0, 0, startingPos.z);
		 
		while (elapsedTime < CAMERA_LERP_TIME)
		{
			cam.transform.position = Vector3.Lerp(startingPos, endPos, (elapsedTime / CAMERA_LERP_TIME));
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
	}
}