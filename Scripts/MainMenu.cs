using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public GameObject menuObject;
    public GameObject levelSelectorObject;
    public GameObject optionsObject;
    public GameObject extraMenu;
    public GameObject confirmationCanvas;
    public GameObject clearSaveDatConfimationBox;

    public Button[] levelButtons;


    public OptionsMenu optionsMenu;
	void Start () {


        optionsMenu.SetUp();

    }
    void SetUpLevelButtons()
    {
        GameData data = SaveLoadSystem.Load();

        for (int i = 0; i < levelButtons.Length; i++)
        {
            levelButtons[i].interactable = false;
        }
        for (int i = 0; i < data.level; i++)
        {
            if (levelButtons[i] != null)
            {
                levelButtons[i].interactable = true;
            }
        }
        levelButtons[0].interactable = true;
        levelButtons[1].interactable = true;
    }
    public void OpenLevelSelector(bool active)
    {
        menuObject.SetActive(!active);
        levelSelectorObject.SetActive(active);
        SetUpLevelButtons();
    }
    public void LoadLevel (int level){


        SceneManager.LoadScene(level);

    }
    public void OpenOptionsMenu(bool _active)
    {

        menuObject.SetActive(!_active);
        optionsObject.SetActive(_active);

    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void OpenExtraMenu(bool _active)
    {
        extraMenu.SetActive(_active);
        menuObject.SetActive(!_active);
    }
    public void OpenLinkToItchIO()
    {
        Application.OpenURL("https://benfreeman.itch.io/");
    }
    public void ClearSaveDatConfirmationBox(bool _active)
    {
        confirmationCanvas.SetActive(_active);
        clearSaveDatConfimationBox.SetActive(_active);

    }
    public void ClearSaveData()
    {
        SaveLoadSystem.Save(new GameData(2));        
    }

}
