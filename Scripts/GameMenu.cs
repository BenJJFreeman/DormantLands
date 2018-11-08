using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour {

    public int level;
    public GameObject gameplayCanvas;
    public GameObject gameMenu;
    public GameObject confimationCanvas;
    public GameObject restartConfirm;   
    public GameObject menuConfirm;
    public GameObject quitConfirm;
    public GameObject optionsCanvas;
    public GameObject endGameCanvas;
    public OptionsMenu optionsMenu;
    public AudioControl audioControl;
    void Start()
    {
        optionsMenu.SetUp();
        gameplayCanvas.SetActive(false);
        StartCoroutine(WaitToShowUI(2));
    }
    IEnumerator WaitToShowUI(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        gameplayCanvas.SetActive(true);
    }
    public void LoadNextLevel()
    {

        if ((level + 1) > SaveLoadSystem.Load().level)
        {
            SaveLoadSystem.Save(new GameData(level + 1));
        }
        SceneManager.LoadScene(level+1);
    }
    public void RestartLevelInit(bool _active)
    {
        confimationCanvas.SetActive(_active);
        restartConfirm.SetActive(_active);
    }
    public void RestartLevel()
    {
        SceneManager.LoadScene(level);
    }
    public void LoadMenuInit(bool _active)
    {
        confimationCanvas.SetActive(_active);
        menuConfirm.SetActive(_active);
    }
    public void LoadMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void QuitGameInit(bool _active)
    {
        confimationCanvas.SetActive(_active);
        quitConfirm.SetActive(_active);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void OpenGameMenu(bool _active)
    {
        gameMenu.SetActive(_active);
        gameplayCanvas.SetActive(!_active);

    }
    public void OpenOptionsMenu(bool _active)
    {
        optionsCanvas.SetActive(_active);
        gameMenu.SetActive(!_active);

    }
    
}
