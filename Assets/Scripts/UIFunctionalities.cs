using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFunctionalities : MonoBehaviour
{
    public static UIFunctionalities Instance { get; private set; }


    [Header("State GameObjects")]
    public GameObject LoginScreen;
    public GameObject MainMenu;
    public GameObject HighScoreMenu;
    public GameObject GameScreen;
    public GameObject InGameGameOverMenu;
    public GameObject InGameSettingsMenu;

    [Header("Extra Assets")]
    public Animator animatorBraids;
    public Image imageSoundOnOff;
    public Sprite spriteVolumeOn;
    public Sprite spriteVolumeOff;
    public Text[] HighScoresUsernames;
    public Text[] HighScoresScores;
    public Animator Login;
    public Animator CreateUser;
    public Image[] healths;
    public Text ScoreTextGameOver;

    private bool soundOn = true;

    public string UserName { get; set; }
    public string Password { get; set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        LoginScreen.SetActive(true);
        MainMenu.SetActive(false);
        HighScoreMenu.SetActive(false);
        GameScreen.SetActive(false);
        CupcakeController.Instance.gameObject.SetActive(false);
    }


    ///////////////////////////////////////////////////////////////
    //  UI FUNCTIONALITIES STARTS HERE ////////////////////////////
    ///////////////////////////////////////////////////////////////
    public void PlayButton()
    {
        CupcakeController.Instance.ResetCupcakes();

        MainMenu.SetActive(false);
        HighScoreMenu.SetActive(false);
        GameScreen.SetActive(true);
        InGameGameOverMenu.SetActive(false);
        CupcakeController.Instance.gameObject.SetActive(true);

        Player.Instance.Health = 3;
    }

    public void OpenHighScoresMenu()
    {
        for (int i = 0; i < 6; i++)
        {
            HighScoresUsernames[i].text = "";
            HighScoresScores[i].text = "";
        }

        int temp = Player.Instance.playerSave.HighScores.Count < 6 ?
            Player.Instance.playerSave.HighScores.Count : 6;
        for (int i = 0; i < temp; i++)
        {
            PlayerData pData = Player.Instance.playerSave.HighScores[i];
            HighScoresUsernames[i].text = pData.Username;
            HighScoresScores[i].text = pData.Score.ToString();
        }

        MainMenu.SetActive(false);
        HighScoreMenu.SetActive(true);
        GameScreen.SetActive(false);
        CupcakeController.Instance.gameObject.SetActive(false);
    }

    public void SoundOnOff()
    {
        soundOn = !soundOn;

        imageSoundOnOff.sprite = soundOn ? spriteVolumeOn : spriteVolumeOff;
        imageSoundOnOff.GetComponent<RectTransform>().sizeDelta 
            = (soundOn ? spriteVolumeOn : spriteVolumeOff).textureRect.size * 2f;

        AudioListener.volume = soundOn ? 1f : 0f;
    }

    public void GameOver()
    {
        ScoreTextGameOver.text = Player.Instance.ScoreText.text;
        InGameGameOverMenu.SetActive(true);

        animatorBraids.StartPlayback();
    }

    public void PlayAgain()
    {
        Player.Instance.ResetScore();
        CupcakeController.Instance.ResetCupcakes();

        InGameGameOverMenu.SetActive(false);

        animatorBraids.StopPlayback();
    }

    public void OpenInGameSettingsMenu()
    {
        animatorBraids.StartPlayback();

        InGameSettingsMenu.SetActive(true);

        CupcakeController.Instance.gameObject.SetActive(false);
    }

    public void CloseInGameSettingsMenu()
    {
        animatorBraids.StopPlayback();

        InGameSettingsMenu.SetActive(false);

        CupcakeController.Instance.gameObject.SetActive(true);
    }

    public void BackToMainMenu()
    {
        MainMenu.SetActive(true);
        HighScoreMenu.SetActive(false);
        GameScreen.SetActive(false);
        CupcakeController.Instance.gameObject.SetActive(false);

        Player.Instance.ResetScore();
        CupcakeController.Instance.ResetCupcakes();
        InGameSettingsMenu.SetActive(false);
    }

    public void LoginButton()
    {
        if (Player.Instance.Login(UserName, Password))
        {
            LoginScreen.SetActive(false);
            MainMenu.SetActive(true);
            HighScoreMenu.SetActive(false);
            GameScreen.SetActive(false);
        }
        else
        {
            Login.Play("LoginFail");
        }
    }
    
    public void CreateAccountButton()
    {
        if (Player.Instance.CreateNewUser(UserName, Password))
        {
            CreateUser.Play("CreateSuccess");
        }
        else
        {
            CreateUser.Play("CreateFail");
        }
    }

}
