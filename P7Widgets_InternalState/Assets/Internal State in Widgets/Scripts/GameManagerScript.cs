using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections.Generic;

public class GameManagerScript : NetworkBehaviour {

    public bool useTwoDevices;

	public GameObject robotPrefab;
	public GameObject clawPrefab;

    public Canvas toolboxCanvas;
    public Canvas gameCanvas;
    public Camera toolboxCamera;
    public Camera gameCamera;

    public Button toolboxButton;
    public Image levelStar;
    public Image levelStarMenu;
    public Image levelStarComplete;
    public Image levelStarFailed;

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
	const int DELAY_BEFORE_START_SPAWN = 1;     //The extra delay after counting down to spawn
    const float SPAWN_COUNT_DOWN_TIME = 3;      //The time to count down before spawning
	const float CAMERA_LERP_TIME = 3;			//The time it takes for camera to move to the next level location

	private int _currentLevel = 1;				//Starts at 1

    public bool menuShown = false;              //is a menu currently shown on the screen

	//public Text scoreText;
	//public Text remainingText;
	//public Text levelText;

	public GameObject cam;
	public GameObject settingsPanel;
	public GameObject gameMenuPanel;
	public GameObject levelCompletePanel;
	public GameObject retryLevelPanel;
	public GameObject finalScorePanel;			//The final info screen showing the amount of robots saved throughout the game
	
	public Image savedToolFill;					//the tool saving progress bar
	public WidgetControlScript[] players;			

	public AudioClip _robotExplosionSound;		//Sound when robot explodes
	public AudioClip _robotScoreSound;			//Sound when robot arrives at goal
    public AudioClip levelCompleteSound;
    public WidgetDetectionAlgorithm algorithm;

	public Transform[] levelLocations;
	private Transform _currentLevelLocation;

	private AudioSource _audio;

	public Button _levelDisc, _toolboxDisc;
	public Goal _firstGoal;

    public GameObject explosion;

    public Text spawnCountdownText;
    private float timeBeforeSpawn = 0;


    void Awake()
	{
        GameStateManager.TwoDevices = useTwoDevices;

        robotGUI = new List<GUI_Robot>();

		_audio = GetComponent<AudioSource>();
		if(savedToolFill == null) savedToolFill = GameObject.Find("Saved_Fill").GetComponent<Image>();

		GameVariables.LevelDisconnect = _levelDisc;
		GameVariables.ToolboxDisconnect = _toolboxDisc;
		GameVariables.CurrentGoal = _firstGoal;
		GameVariables.GameManager = GetComponent<GameManagerScript>();
		GameVariables.SavedToolFill = savedToolFill;
        GameVariables.ToolboxPanel = GameObject.Find("ToolboxPanel").GetComponent<Toolbox_Move>();
        //if(players == null)
        //{
        //	GameVariables.Players = new List<WidgetControlScript>();
        //	for(int i = 0; i < FindObjectsOfType<WidgetControlScript>().Length; i++)
        //	{
        //		GameVariables.Players.Add(GameObject.Find("Player " + (i + 1).ToString()).GetComponent<WidgetControlScript>());
        //	}
        //}
        //else
        //{
        GameVariables.Players = new List<WidgetControlScript>();
        for (int i = 0; i < players.Length; i++)
		{
			GameVariables.Players.Add(players[i]);
		}
		//}
		

		GameVariables.ToolboxCanvas = GameObject.Find("ToolboxPanel").GetComponent<Toolbox_Move>();
		GameVariables.WidgetAlgorithm = WidgetDetectionAlgorithm.instance;

		//Set up variables in case they were not set in the inspector
		//if(scoreText == null) scoreText = GameObject.Find("ScoredRobots").GetComponent<Text>();
		//if(remainingText == null) remainingText = GameObject.Find("RemainingRobots").GetComponent<Text>();
		//if(levelText == null) levelText = GameObject.Find("Level").GetComponent<Text>();
		if(levelStarMenu == null) levelStarMenu = GameObject.Find("LevelStarMenu").GetComponent<Image>();
        if (levelStarComplete == null) levelStarComplete = GameObject.Find("LevelStarComplete").GetComponent<Image>();
        if (levelStarFailed == null) levelStarFailed = GameObject.Find("LevelStarFailed").GetComponent<Image>();
        if (toolboxButton == null) toolboxButton = GameObject.Find("ToolboxButtonOpener").GetComponent<Button>();
        if (levelStar == null) levelStar = GameObject.Find("LevelStar").GetComponent<Image>();


        if (levelLocations.Length == 0)
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
        SetupRobotGUI();
    }



	void Update () {
		if(Input.GetKeyDown(KeyCode.Space))
		{
            ToggleToolbox();
		}
        if(toolboxButton != null)
        {
            if (Network.connections.Length > 1 && toolboxButton.enabled == true){
                toolboxButton.enabled = false;
            }
        }
        
	}



    public void ToggleToolbox()
    {
        if(toolboxCanvas.enabled == true)
        {
            toolboxCanvas.enabled = false;
            toolboxCamera.enabled = false;
            gameCanvas.enabled = true;
            gameCamera.enabled = true;

            toolboxButton.image.sprite = Resources.Load<Sprite>("GUI/ToolboxButton/ToolboxOpenButton");

            GameStateManager.State = GameState.GAME;
        }
        else
        {
            gameCanvas.enabled = false;
            gameCamera.enabled = false;
            toolboxCanvas.enabled = true;
            toolboxCamera.enabled = true;

            toolboxButton.image.sprite = Resources.Load<Sprite>("GUI/ToolboxButton/ToolboxCloseButton");

            GameStateManager.State = GameState.TOOLBOX;
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
            //InvokeRepeating("Spawn", DELAY_BEFORE_START_SPAWN, SPAWN_RATE);
            StartCoroutine(CountDownSpawn());
		}
	}



	void Spawn()
	{
		GameObject go = (GameObject)Instantiate(robotPrefab, GameVariables.CurrentSpawn.position, Quaternion.identity);
		_robotsSpawned++;
		_robotsRemaining++;
		//UpdateUI();
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

        UpdateRobotGUI(GUIRobotState.Scored);

        //Play score sound
        PlaySound(_robotScoreSound);

       // UpdateUI();
		
        CheckLevelConditions();
    }



	public void RobotDied(Transform robot)
	{
        //Show explosion where robot died
        Instantiate(explosion, robot.position, Quaternion.identity);
        //Play explosion sound
        UpdateRobotGUI(GUIRobotState.Dead);

        PlaySound(_robotExplosionSound);

		_robotsRemaining--;
		//UpdateUI();

        CheckLevelConditions();
	}



    //Check if the game matches win or loss conditions
    public void CheckLevelConditions()
    {
        if(_robotsRemaining == 0 && _robotsSpawned == robotsToSpawn[_currentLevel - 1])
        {
            if (_score >= scoreToWin[_currentLevel - 1])
            {
                _savedRobots += _score;
                LevelComplete();
            }
            else if ( _score < scoreToWin[_currentLevel - 1])
            {
                LoseGame();
            }
        }
        else
        {
            return;
        }
    }



	public void ShowStartPanel(bool b)
	{
		gameMenuPanel.SetActive(b);
	}



	//Reset variables related to the robots, as new level will start
	void ResetVariables()
	{
        //Destroy all robots as a safety measure
        GameObject[] robots = GameObject.FindGameObjectsWithTag("Robot");
        foreach (GameObject go in robots)
            Destroy(go);

		_robotsRemaining = 0;
		_robotsSpawned = 0;
		_score = 0;

       // SetupRobotGUI();
        //UpdateUI();
	}



	void UpdateUI()
	{
		//The menu title
        Sprite levelStarSprite = Resources.Load<Sprite>("GUI/LevelStars/ToolboxStarButton_" + _currentLevel);

        //Set level star sprite for the start game panel
        levelStarMenu.sprite = levelStarSprite;

        //Set level star sprite for the completed game panel
        levelStarComplete.sprite = levelStarSprite;

        //Set level star sprite for the failed game panel
        levelStarFailed.sprite = levelStarSprite;

        //Set level star for top bar
        levelStar.sprite = levelStarSprite;

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

        SetupRobotGUI();

        UpdateUI();
	}



	public void ToggleSettings()
	{
		_showSettings = !_showSettings;
		settingsPanel.SetActive(_showSettings);
	}



	public void LevelComplete()
	{
		if(CurrentLevel < levelLocations.Length)
		{
			//ResetWidgetTools();
            //DebugConsole.Log("Reset Tools, level:  " + CurrentLevel);
			levelCompletePanel.SetActive(true);
            PlaySound(levelCompleteSound);
		}
		else
		{
			GameOver();
		}
	}



	//This is called when game is lost
	public void LoseGame()
	{
		retryLevelPanel.SetActive(true);
        GameVariables.CurrentGoal.Close();
        GameVariables.CurrentSpawn.GetComponent<Spawner>().Close();
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

        SetupRobotGUI();
        ResetVariables();
		StartSpawn();

		curPanel.SetActive(false);
	}



    public void RetryLevelAfterCompletion(GameObject curPanel)
    {
        //Increment the amount of times the player has retried a level. Will be shown in the end
        _retries++;

        _savedRobots -= _score;

        SetupRobotGUI();
        ResetVariables();
        StartSpawn();

        curPanel.SetActive(false);
    }



    public void GameOver()
	{
		Debug.Log("GameOver");
        PlaySound(levelCompleteSound);
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



    IEnumerator CountDownSpawn()
    {
        timeBeforeSpawn = SPAWN_COUNT_DOWN_TIME;
        spawnCountdownText.enabled = true;

        while(timeBeforeSpawn > 0)
        {
            timeBeforeSpawn -= Time.deltaTime;
            spawnCountdownText.text = timeBeforeSpawn.ToString("F0");
            yield return new WaitForEndOfFrame();
        }
        spawnCountdownText.text = "GO";
        GameVariables.CurrentSpawn.GetComponent<Spawner>().Open();
        GameVariables.CurrentGoal.Open();
        InvokeRepeating("Spawn", DELAY_BEFORE_START_SPAWN, SPAWN_RATE);

        yield return new WaitForSeconds(DELAY_BEFORE_START_SPAWN);

        spawnCountdownText.enabled = false;
        StopCoroutine(CountDownSpawn());
    }


    //*************Robot progress GUI functions here**************************
    private RectTransform guiRobotSpawn;
    private float widthPadding = -5;
    public Image guiRobotPrefab;
    List<GUI_Robot> robotGUI;

    public void SetupRobotGUI()
    {
        //Clear the list of gui robots before making new
        if (robotGUI.Count > 0)
        {
            for (int i = 0; i < robotGUI.Count; i++)
            {
                Destroy(robotGUI[i].RobotImage.gameObject);
            }
        }

        robotGUI.Clear();
        robotGUI = new List<GUI_Robot>();

        guiRobotSpawn = GameObject.Find("RobotGUILocation").GetComponent<RectTransform>();

        //Create robots and add to list
        for (int i = 0; i < robotsToSpawn[_currentLevel - 1]; i++)
        {
            Image newImg = Instantiate(guiRobotPrefab);
            newImg.transform.parent = guiRobotSpawn.parent;
            newImg.GetComponent<RectTransform>().localPosition = guiRobotSpawn.localPosition + new Vector3((newImg.rectTransform.sizeDelta.x * i) + widthPadding * i, 0,0);

            newImg.rectTransform.localScale = new Vector3(1,1,1);

            GUI_Robot newRobot = new GUI_Robot(newImg);

            robotGUI.Add(newRobot);

        }
    }



    public void UpdateRobotGUI(GUIRobotState rs)
    {
        foreach(GUI_Robot rob in robotGUI)
        {
            if(rob.State == GUIRobotState.Alive)
            {
                rob.State = rs;
                break;
            }
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