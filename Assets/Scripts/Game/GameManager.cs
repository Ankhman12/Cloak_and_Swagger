using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


// Game States
// for now we are only using these two
public enum GameState { MENU, LEVEL_SELECT, GAME }

public delegate void OnStateChangeHandler();

public class GameManager : MonoBehaviour
{
    protected GameManager() { }
    private static GameManager instance = null;
    public event OnStateChangeHandler OnStateChange;
    public GameState gameState { get; private set; }

    bool paused = false;

    //public Panel startPanel;
    //public Panel pausePanel;
    //public Panel gameOverPanel;

    public Scene currentScene;

    public List<string> sceneNames;

    public AudioSource mainMusic;

    public static GameManager Instance
    {
        get
        {
            if (GameManager.instance == null)
            {
                //DontDestroyOnLoad(GameManager.instance);
                GameManager.instance = new GameManager();
            }
            return GameManager.instance;
        }

    }

    void Awake() {
        
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += PlayMusic;
    }

    public void SetGameState(GameState state)
    {
        this.gameState = state;
        OnStateChange();
    }

    public void OnApplicationQuit()
    {
        GameManager.instance = null;
    }

    //public void Pause()
    //{
    //    if (paused)
    //    {
    //        paused = false;
    //       Time.timeScale = 1;
    //        pausePanel.SetActive(false);
    //    }
    //    else
    //    {
    //        pausePanel.SetActive(true);
    //    }
    //}

    public void PlayTut() 
    {
        //Load tutorial/level select scene
        SceneManager.LoadScene("TutorialLevel");
    }

    public void Play()
    {
        //Load tutorial/level select scene
        SceneManager.LoadScene("Level_1");
    }

    public void GameOver()
    {
        //Load Game Over Scene
        SceneManager.LoadScene("GameOver");
    }

    public void QuitGame() 
    {
        Application.Quit();
    }

    public void PlayMusic(Scene scene, LoadSceneMode mode) {
        mainMusic.Play();
    }
    
}