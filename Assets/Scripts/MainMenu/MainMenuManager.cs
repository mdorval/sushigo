using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainMenuManager : MonoBehaviour {
    public Button PlayButton;
    public Button RulesButton;
    public Button AboutButton;
    public Button QuitButton;
    public List<Button> BackToMenuButtons;

    private GameObject CurrentPanel;
    private GameObject MainPanel;
    public GameObject AboutPanel;

    // Use this for initialization
    void Start()
    {
        MainPanel = this.gameObject;
        CurrentPanel = MainPanel;
        QuitButton.onClick.AddListener(Application.Quit);
        PlayButton.onClick.AddListener(LoadGame);
        AboutButton.onClick.AddListener(LoadAboutPanel);
        foreach (Button backButton in BackToMenuButtons)
        {
            backButton.onClick.AddListener(LoadMainPanel);
        }

	}

    private void LoadPanel(GameObject panelToLoad)
    {
        CurrentPanel.SetActive(false);
        panelToLoad.SetActive(true);
        CurrentPanel = panelToLoad;
    }
    void LoadGame()
    {
        SceneManager.LoadScene(1);
    }

    void LoadMainPanel() { LoadPanel(MainPanel); }
    void LoadAboutPanel() { LoadPanel(AboutPanel); }
}
