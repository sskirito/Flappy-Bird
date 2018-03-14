using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class GameController : MonoBehaviour {

    GameController controller;
    Vector3 playerStartPosition;
    UICon UIManager;

    [HideInInspector]
    public bool gameOver;
    [HideInInspector]
    public bool gameStarted;
    [HideInInspector]
    public string playerID;
    [HideInInspector]
    public string playerPassword;
    [HideInInspector]
    public bool canGameStart;
    [HideInInspector]
    public int score;
    [HideInInspector]
    public int surviveTime;

    public GameObject[] tubePrefabs;
    public GameObject[] Props;
    public GameObject gameStartHints;
    public GameObject gameOverHints;
    public GameObject player;
    public Image fadeImage;
    public Text scoreText;

    public GameObject OnlineOrOffline;
    public GameObject LoginOrRegist;
    public InputField playerIDInput;
    public InputField playerPasswordInput;
    public GameObject SureToRegist;
    public GameObject ErrorInfo;
    public Text ErrorText;
    public GameObject PlayGame;
    public GameObject LBButton;
    public GameObject LocalBoardButton;
    public GameObject Settings;
    public Text CurrentScoreText;
    public Text BestScoreText;
    public GameObject Loading;
    public GameObject Land;
    public GameObject NEW;
    public GameObject ScoreButton;
    public GameObject ScoreButtonGray;
    public Text ShowDifficultyText;
    public GameObject LogoutButton;

    float easyInstantiateDelay = 2f;
    float instantiateDelay;
    float hardestInstantiateDelay = 1f;
    float currentTimeInstantiate;
    float currentTimeProp = -1f;
    int passedTubeCount;
    int difficulty;

    bool isOnlineClicked;
    bool isLoginClicked;
    bool isRegistClicked;
    bool isLeadBoardClicked;

    int BestScore;

    const string xmlModel = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<VC>\n  <Group>\n  </Group>\n</VC>";

    private void Awake() {
        Screen.SetResolution(288 * 2, 512 * 2, false);
    }

    // Use this for initialization
    void Start () {
        gameOver = false;
        gameStarted = false;
        difficulty = 0;
        Land.SetActive(false);
        NEW.SetActive(false);
        ShowDifficultyText.text = "EASY!";
        ShowDifficultyText.color = new Color(100, 255, 0);

        playerID = "Default Player";

        controller = FindObjectOfType<GameController>();
        player.GetComponent<PlayerController>().OnGameStartHints += SetGameStartHints;
        player.GetComponent<PlayerController>().OnGameOverHints += SetGameOverHints;
        UIManager = FindObjectOfType<UICon>();

        playerStartPosition = player.transform.position;

        PlayGame.SetActive(true);

        gameStartHints.SetActive(true);
        SetDifficultyTextAnimator();
        if (!Directory.Exists(Application.dataPath + @"/XML")) {
            Directory.CreateDirectory(Application.dataPath + @"/XML");
        }

        PlayClicked();

        LogoutButton.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        
        if (controller.gameStarted && !controller.gameOver) {
            scoreText.text = "Score: " + score;
            instantiateDelay = Mathf.Lerp(easyInstantiateDelay, hardestInstantiateDelay, Difficulty.GetDifficulty());

            float targetStartY = Random.Range(-difficulty * 0.5f, 3f);
            Vector3 targetPosition = new Vector3(0, targetStartY, 0);
            Vector3 targetRotation = new Vector3();
            if (currentTimeInstantiate > instantiateDelay) {
                Instantiate(tubePrefabs[difficulty], targetPosition, Quaternion.Euler(targetRotation));
                passedTubeCount++;
                currentTimeInstantiate = 0f;
            }

            float targetStartYProp = Random.Range(-3f, 4f);
            Vector3 targetPositionProp = new Vector3(4, targetStartYProp, 0);
            Vector3 targetRotationProp = Vector3.zero;
            if(currentTimeProp > instantiateDelay) {
                if (passedTubeCount >= 3 + 2 * difficulty) {
                    int index = Random.Range(0, Props.Length);
                    Instantiate(Props[index], targetPositionProp, Quaternion.Euler(targetRotationProp));
                    passedTubeCount = 0;
                }
                currentTimeProp = 0f;
            }
            currentTimeProp += Time.deltaTime;
            currentTimeInstantiate += Time.deltaTime;
        }

        if (!ClientSocket.isLoading) {
            Loading.SetActive(false);
            if (isOnlineClicked) {
                isOnlineClicked = false;
                if (ClientSocket.receivedMessage == "客户端连接数已达上限") {
                    ErrorText.text = "Too Many Logins.";
                    ErrorInfo.SetActive(true);
                    canGameStart = true;
                } else if ( ClientSocket.connnetionSuccess) {
                    PlayGame.SetActive(false);
                    LoginOrRegist.SetActive(true);
                    gameStartHints.SetActive(false);

                    player.SetActive(false);
                } else if (!ClientSocket.connnetionSuccess) {
                    ErrorText.text = "Cannot Connect to the Server.";
                    ErrorInfo.SetActive(true);
                    canGameStart = true;
                }
            } else if (isLoginClicked) {
                isLoginClicked = false;
                if (ClientSocket.receivedMessage == "Login Failure") {
                    ErrorText.text = "Wrong ID or Password!\nPlease Check Again.";
                    ErrorInfo.SetActive(true);
                } else if (ClientSocket.receivedMessage == "Refused by the server") {
                    ErrorText.text = "Refused By The Server";
                    ErrorInfo.SetActive(true);
                } else {
                    string[] ss = ClientSocket.receivedMessage.Split('|');
                    if (!File.Exists(Application.dataPath + @"/XML/" + playerID + ".xml")) {
                        File.WriteAllText(Application.dataPath + @"/XML/" + playerID + ".xml", xmlModel);
                    }
                    UIManager.filePath = Application.dataPath + @"/XML/" + playerID + ".xml";

                    List<GameInfoModel> list = new List<GameInfoModel>();

                    for (int i = 1; i < ss.Length - 1; i += 3) {
                        list.Add(new GameInfoModel {
                            Id = playerID,
                            Score = System.Convert.ToInt32(ss[i]),
                            Time = System.Convert.ToInt32(ss[i + 1]) / 1000
                        });
                    }
                    while (list.Count > 5) {
                        list.RemoveAt(list.Count - 1);
                    }
                    UIManager.VCRemoveXmlElement();
                    UIManager.VCAddXml(list);

                    PlayGame.SetActive(true);
                    gameStartHints.SetActive(true);
                    SetDifficultyTextAnimator();
                    player.SetActive(true);
                    LoginOrRegist.SetActive(false);
                    LogoutButton.SetActive(true);
                    canGameStart = true;
                }
            } else if (isRegistClicked) {
                isRegistClicked = false;
                if (ClientSocket.receivedMessage == "Sign In Failure") {
                    ErrorText.text = "This ID has benn Registered.\nPlease Change Another One.";
                    ErrorInfo.SetActive(true);
                } else if (ClientSocket.receivedMessage == "Sign In Success") {
                    LoginOrRegist.SetActive(false);
                    SureToRegist.SetActive(false);
                    PlayGame.SetActive(true);
                    UIManager.filePath = Application.dataPath + @"/XML/" + playerID + ".xml";
                    File.WriteAllText(UIManager.filePath, xmlModel);
                    gameStartHints.SetActive(true);
                    SetDifficultyTextAnimator();
                    player.SetActive(true);
                    LogoutButton.SetActive(true);
                    canGameStart = true;
                }
            } else if (isLeadBoardClicked) {
                isLeadBoardClicked = false;
                string[] ss = ClientSocket.receivedMessage.Split('|');
                for (int i = 1; i < ss.Length - 1; i += 4) {
                    UIManager.countText[(i - 1) / 4].text = ss[i];
                    UIManager.countText[(i - 1) / 4 + 5].text = ss[i + 1];
                    UIManager.countText[(i - 1) / 4 + 10].text = ss[i + 2] + "″";
                    if ((i - 1) / 4 >= 4) {
                        break;
                    }
                }
                LBButton.SetActive(false);
                LocalBoardButton.SetActive(true);
            }
        }
	}

    public void RestartGame() {
        Land.SetActive(false);
        NEW.SetActive(false);

        Difficulty.gameStartTime = Time.time;
        score = 0;
        currentTimeProp = -1f;
        passedTubeCount = 0;
        scoreText.text = "Score: 0";
        player.transform.position = playerStartPosition;
        player.transform.eulerAngles = Vector3.zero;
        gameOver = false;
        gameStarted = false;

        gameOverHints.SetActive(false);

        foreach(var x in FindObjectsOfType<TubeController>()) {
            Destroy(x.gameObject);
        }
        foreach(var x in gameStartHints.GetComponentsInChildren<Animator>()) {
            x.SetTrigger("Restart");
        }

        currentTimeInstantiate = 0f;

        PlayGame.SetActive(true);
        scoreText.gameObject.SetActive(false);
        player.SetActive(false);

        ResetDifficulty();
        PlayClicked();
    }

    void ResetDifficulty() {
        Difficulty.offset = 0;
        TubeController.hardestMoveSpeedOffset = 0;
        LandScroller.maxSpeedOffset = 0;
    }

    void SetGameStartHints() {
        foreach(var x in gameStartHints.GetComponentsInChildren<Animator>()) {
            x.SetTrigger("GameStart");
        }
    }

    void SetGameOverHints() {
        fadeImage.GetComponent<Animator>().SetTrigger("GameOverFade");
        gameOverHints.SetActive(true);
        scoreText.gameObject.SetActive(false);
        canGameStart = false;
        surviveTime = (int)(Time.time - Difficulty.gameStartTime);

        UIManager.filePath = Application.dataPath + @"/XML/" + playerID + ".xml";
        if (difficulty == 2) {
            if (!File.Exists(UIManager.filePath)) {
                File.WriteAllText(UIManager.filePath, xmlModel);
                List<GameInfoModel> list = new List<GameInfoModel> {
                new GameInfoModel {
                    Id = playerID,
                    Score = score,
                    Time = surviveTime
                }
            };
                UIManager.VCAddXml(list);
            } else {
                UIManager.UpdateRankList();
            }
        }

        if (difficulty == 0) {
            SetScoreAndBestScore("Easy");
        }else if(difficulty == 1) {
            SetScoreAndBestScore("Normal");
        }else {
            SetScoreAndBestScore("Hard");
        }
        CurrentScoreText.text = score.ToString();
        BestScoreText.text = BestScore.ToString();

        if (ClientSocket.connnetionSuccess) {
            UploadClicked();
        }
    }

   void SetScoreAndBestScore(string info) {
        if (score > PlayerPrefs.GetInt(playerID + info, 0)) {
            NEW.SetActive(true);
        }
        BestScore = Mathf.Max(score, PlayerPrefs.GetInt(playerID + info, 0));
        PlayerPrefs.SetInt(playerID + info, BestScore);
    }

    public void OnlineClicked() {
        canGameStart = false;
        Loading.SetActive(true);
        isOnlineClicked = true;
        ClientSocket.Connect();
        //if (ClientSocket.connnetionSuccess) {
        //    OnlineOrOffline.SetActive(false);
        //    LoginOrRegist.SetActive(true);
        //}
    }

    public void OfflineClicked() {
        OnlineOrOffline.SetActive(false);
        PlayGame.SetActive(true);
        UIManager.filePath = Application.dataPath + "/XML/" + playerID + ".xml";
        if (!File.Exists(UIManager.filePath)) {
            File.WriteAllText(UIManager.filePath, xmlModel);
        }
    }

    public void LoginClicked() {
        GetPlayerIDPassword();
        if (playerID == "Default Player") {
            return;
        }
        Loading.SetActive(true);
        isLoginClicked = true;
        ClientSocket.EncryptSend("Login " + playerID + " " + playerPassword);
        ClientSocket.Receive();
    }
    public void RegistClicked() {
        SureToRegist.SetActive(true);
    }

    public void WaitClicked() {
        SureToRegist.SetActive(false);
    }

    public void OKClicked() {
        GetPlayerIDPassword();
        if (playerID == "Default Player") {
            return;
        }
        Loading.SetActive(true);
        isRegistClicked = true;
        ClientSocket.EncryptSend("Reg " + playerID + " " + playerPassword);
        ClientSocket.Receive();
    }
    public void ErrorInfoBackClicked() {
        ErrorInfo.SetActive(false);
        if (SureToRegist.activeSelf) {
            SureToRegist.SetActive(false);
        }
    }

    public void PlayClicked() {
        //PlayGame.SetActive(false);
        canGameStart = true;
        Land.SetActive(true);
        canGameStart = true;
        scoreText.gameObject.SetActive(true);
        player.SetActive(true);
        //foreach (var x in gameStartHints.GetComponentsInChildren<Animator>()) {
        //    x.SetTrigger("Restart");
        //}
        SetDifficultyTextAnimator();
    }
    public void ScoreClicked() {
        UIManager.filePath = Application.dataPath + @"\XML\" + playerID + ".xml";
        if (!File.Exists(UIManager.filePath)) {
            File.WriteAllText(UIManager.filePath, xmlModel);
        }
        UIManager.ShowRankList();
    }
    public void LeadBoardClicked() {
        if (!ClientSocket.connnetionSuccess) {
            ErrorInfo.SetActive(true);
            ErrorText.text = "You Are Not Playing Online!";
        } else {
            isLeadBoardClicked = true;
            Loading.SetActive(true);
            ClientSocket.EncryptSend("LB");
            ClientSocket.Receive();
        }
    }
    public void LocalBoardClicked() {
        LocalBoardButton.SetActive(false);
        LBButton.SetActive(true);
        UIManager.ClearRankList();
        UIManager.ShowRankList();
        canGameStart = false;
    }
    public void MenuClicked() {
        Settings.SetActive(true);
        canGameStart = false;
    }

    public void DifficultMoved() {
        float difficultyValue = FindObjectOfType<Scrollbar>().value;
        if(difficultyValue == 0f) {
            difficulty = 0;
            ShowDifficultyText.color = new Color(0.3921569f, 1f, 0f);
            ShowDifficultyText.text = "EASY!";
            ShowDifficultyText.GetComponent<Animator>().SetTrigger("Easy");

        } else if(difficultyValue == .5f) {
            difficulty = 1;
            ScoreButton.SetActive(false);
            ScoreButtonGray.SetActive(true);
            ShowDifficultyText.color = new Color(1f, 1f, 0f);
            ShowDifficultyText.text = "NORMAL!";
            ShowDifficultyText.GetComponent<Animator>().SetTrigger("Normal");
        } else {
            difficulty = 2;
            ScoreButton.SetActive(true);
            ScoreButtonGray.SetActive(false);
            ShowDifficultyText.color = new Color(1f, 0.7843137f, 0f);
            ShowDifficultyText.text = "HARD!";
            ShowDifficultyText.GetComponent<Animator>().SetTrigger("Hard");
        }
    }
    public void ScoreGrayClicked() {
        ErrorInfo.SetActive(true);
        ErrorText.text = "RankList Is Only Available in Hard Mode";
    }
    public void LogoutClicked() {
        if (!ClientSocket.connnetionSuccess) {
            ErrorText.text = "You Haven't Logged In Yet!";
            ErrorInfo.SetActive(true);
        }
        playerID = "";
        playerPassword = "";
        ClientSocket.Close();
        SceneManager.LoadScene(0);
    }
    public void BackToPlayClicked() {
        Settings.SetActive(false);
        canGameStart = true;
        if (LoginOrRegist.activeSelf) {
            LoginOrRegist.SetActive(false);
            PlayGame.SetActive(true);
            player.SetActive(true);
            gameStartHints.SetActive(true);
            SetDifficultyTextAnimator();
            ClientSocket.Close();
        }
    }

    public void UploadClicked() {
        //if (!ClientSocket.connnetionSuccess) {
        //    ErrorInfo.SetActive(true);
        //    ErrorText.text = "You Are Not Playing Online!";
        //} else {
            //List<GameInfoModel> RankList = UIManager.GetXMLInfo();
            string msg = "";
            if (difficulty == 0) {
                msg += "Easy " + score + " " + ((int)((Time.time - Difficulty.gameStartTime) * 1000)).ToString();
            } else if (difficulty == 1) {
                msg += "Normal " + score + " " + ((int)((Time.time - Difficulty.gameStartTime) * 1000)).ToString();
            } else if (difficulty == 2) {
                msg += "Scores " + score + " " + ((int)((Time.time - Difficulty.gameStartTime) * 1000)).ToString();
            }
            ClientSocket.EncryptSend(msg);
    }

    void GetPlayerIDPassword() {
        if (playerIDInput.text == string.Empty || playerPasswordInput.text == string.Empty) {
            ErrorText.text = "Input Cannot be Empty!";
            ErrorInfo.SetActive(true);
            return;
        } else {
            foreach (var x in playerIDInput.text) {
                if (!((x >= '0' && x <= '9') || (x >= 'a' && x <= 'z') || (x >= 'A' && x <= 'Z'))) {
                    ErrorText.text = "Illegal Input!";
                    ErrorInfo.SetActive(true);
                    return;
                }
            }
            foreach (var x in playerPasswordInput.text) {
                if (!((x >= '0' && x <= '9') || (x >= 'a' && x <= 'z') || (x >= 'A' && x <= 'Z'))) {
                    ErrorText.text = "Illegal Input!";
                    ErrorInfo.SetActive(true);
                    return;
                }
            }
            playerID = playerIDInput.text;
            playerPassword = playerPasswordInput.text;
        }
    }

    void SetDifficultyTextAnimator() {
        if (difficulty == 0) {
            ShowDifficultyText.GetComponent<Animator>().SetTrigger("Easy");
            ShowDifficultyText.color = new Color(0.3921569f, 1f, 0f);
        } else if (difficulty == 1) {
            ShowDifficultyText.GetComponent<Animator>().SetTrigger("Normal");
            ShowDifficultyText.color = new Color(1f, 1f, 0f);
        } else if (difficulty == 2) {
            ShowDifficultyText.GetComponent<Animator>().SetTrigger("Hard");
            ShowDifficultyText.color = new Color(1f, 0.7843137f, 0f);
        }
    }

    private void OnApplicationQuit() {
        ClientSocket.Close();
    }
}
