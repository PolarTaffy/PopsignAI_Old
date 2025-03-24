using UnityEngine;
using System.Collections;
using InitScriptName;

public enum GameState
{
    Playing,
    Highscore,
    GameOver,
    Pause,
    Win,
    WaitForPopup,
    WaitAfterClose,
    BlockedGame,
    Tutorial,
    PreTutorial,
    WaitForStar
}


public class GamePlay : MonoBehaviour
{
    public static GamePlay Instance;
    public GameState gameStatus;
    public bool InGamePauseTriggered = false;
    public bool isPopSignAI = false;
    bool winStarted;
    private VideoManager sharedVideoManager;

    public GameState GameStatus
    {
        get { return GamePlay.Instance.gameStatus; }
        set
        {
            if (GamePlay.Instance.gameStatus != value)
            {
                if (value == GameState.Win)
                {
                    //POPSign once the level is clear, the current playing video should be cleared too.
                    sharedVideoManager = VideoManager.getVideoManager();
                    sharedVideoManager.curtVideo = null;
                    sharedVideoManager.shouldChangeVideo = true;
                    Debug.Log("GameState set!");
                    if (!winStarted)
                        StartCoroutine(WinAction());
                }
                else if (value == GameState.GameOver)
                {
                    StartCoroutine(LoseAction());
                }
                else if (value == GameState.Tutorial && gameStatus != GameState.Playing)
                {
                    value = GameState.Playing;
                    gameStatus = value;
                }
                else if (value == GameState.PreTutorial && gameStatus != GameState.Playing)
                {
                    ShowTypeExplanation();
                }

            }
            if (value == GameState.WaitAfterClose)
                StartCoroutine(WaitAfterClose());
            if (value == GameState.Tutorial)
            {
                if (gameStatus != GameState.Playing)
                    GamePlay.Instance.gameStatus = value;
            }
            GamePlay.Instance.gameStatus = value;
        }
    }

    // Use this for initialization
    private void Awake()
    {
         if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        // end of new code

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            if (Input.GetKey(KeyCode.W))
            {
                Debug.Log("Before: " + GamePlay.Instance.GameStatus);
                GamePlay.Instance.GameStatus = GameState.Win;
                Debug.Log("After: " + GamePlay.Instance.GameStatus);
            }
            if (Input.GetKey(KeyCode.L)) { LevelData.LimitAmount = 0; GamePlay.Instance.GameStatus = GameState.GameOver; }
            if (Input.GetKey(KeyCode.D)) mainscript.Instance.destroyAllballs();
            if (Input.GetKey(KeyCode.M)) LevelData.LimitAmount = 1;
        }
    }

    IEnumerator WinAction()
    {
        Debug.Log("Dooing WinAction");
        winStarted = true;
        GameObject.Find("Canvas").transform.Find("LevelCleared").gameObject.SetActive(true);
        sharedVideoManager = VideoManager.getVideoManager();
        sharedVideoManager.shouldChangeVideo = false;

        if (PlayerPrefs.GetInt("MaxLevel") == PlayerPrefs.GetInt("OpenLevel"))
        {
            if(!isPopSignAI)
            {
                PlayerPrefs.SetInt("MaxLevel", PlayerPrefs.GetInt("MaxLevel") + 1);
                PlayerPrefs.Save();
                PlayerPrefs.SetInt("MaxLevel", PlayerPrefs.GetInt("MaxLevel") + 1);
                PlayerPrefs.Save();
            }
        }

        if (PlayerPrefs.GetInt("MaxLevel") >= PlayerPrefs.GetInt("NumLevels"))
        {
            PlayerPrefs.SetInt("AllLevelsCleared", 1);
            PlayerPrefs.SetInt("CongratsModalShown", 0);
            PlayerPrefs.Save();
        }

        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.winSound);
        yield return new WaitForSeconds(1f);

        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.aplauds);
        if (PlayerPrefs.GetInt(string.Format("Level.{0:000}.StarsCount", mainscript.Instance.currentLevel), 0) < mainscript.Instance.stars)
        {
            PlayerPrefs.SetInt(string.Format("Level.{0:000}.StarsCount", mainscript.Instance.currentLevel), mainscript.Instance.stars);
            PlayerPrefs.Save();
        }
        /*
        if( PlayerPrefs.GetInt( string.Format( "Level.{0:000}.Score", mainscript.Instance.currentLevel ), 0) < mainscript.Score )
        {
            PlayerPrefs.SetInt( string.Format( "Level.{0:000}.Score", mainscript.Instance.currentLevel ), mainscript.Score );
            // PlayerPrefs.SetInt( "Score" + mainscript.Instance.currentLevel, mainscript.Score );
            PlayerPrefs.Save();
        }
        */
        GameObject.Find("Canvas").transform.Find("LevelCleared").gameObject.SetActive(false);
        GameObject.Find("MenuCanvas").transform.Find("MenuComplete").gameObject.SetActive(true);
        // if (mainscript.Instance.stars < 3)
        //     GameObject.Find("MenuComplete").transform.Find("Image/LevelCompleteStars/CompleteStar3").gameObject.SetActive(true);
        // if (mainscript.Instance.stars < 2)
        //     GameObject.Find("MenuComplete").transform.Find("Image/LevelCompleteStars/CompleteStar2").gameObject.SetActive(true);
        // if (mainscript.Instance.stars < 1)
        //     GameObject.Find("MenuComplete").transform.Find("Image/LevelCompleteStars/CompleteStar1").gameObject.SetActive(true);
        winStarted = false;
    }

    void ShowTypeExplanation()
    {
        GameObject.Find("Canvas").transform.Find("LevelTypeExplanation").gameObject.SetActive(true);
    }

    IEnumerator LoseAction()
    {
        SoundBase.Instance.GetComponent<AudioSource>().PlayOneShot(SoundBase.Instance.OutOfMoves);
        GameObject.Find("Canvas").transform.Find("OutOfMoves").gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        GameObject.Find("Canvas").transform.Find("OutOfMoves").gameObject.SetActive(false);
        // if(LevelData.LimitAmount <= 0)
        GameObject.Find("MenuCanvas").transform.Find("MenuGameOver").gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
    }

    IEnumerator WaitAfterClose()
    {
        yield return new WaitForSeconds(1);
        GameStatus = GameState.Playing;
    }
}
