using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
/// <summary>
/// Manager for all MainMenuPanels, but attached to the first MainMenu Panel
/// </summary>
public class MainMenuManager : MonoBehaviour {
    public Button PlayButton;
    public Button RulesButton;
    public Button AboutButton;
    public Button QuitButton;
    public Button RulesNextButton;
    public List<Button> BackToMenuButtons;

    private GameObject CurrentPanel;
    private GameObject MainPanel;
    public GameObject AboutPanel;
    public GameObject RulesPanel;
    public RulesCardsManager RulesCardsPanel;

    void Start()
    {
        MainPanel = this.gameObject;
        CurrentPanel = MainPanel;
        QuitButton.onClick.AddListener(Application.Quit);
        PlayButton.onClick.AddListener(LoadGame);
        AboutButton.onClick.AddListener(LoadAboutPanel);
        RulesButton.onClick.AddListener(LoadRulesPanel);
        RulesNextButton.onClick.AddListener(LoadRulesCardsPanel);
        foreach (Button backButton in BackToMenuButtons)
        {
            backButton.onClick.AddListener(LoadMainPanel);
        }

	}

    /// <summary>
    /// Loads The given panel and hides the current panel
    /// </summary>
    /// <param name="panelToLoad">The Panel to load</param>
    private void LoadPanel(GameObject panelToLoad)
    {
        CurrentPanel.SetActive(false);
        panelToLoad.SetActive(true);
        CurrentPanel = panelToLoad;
    }
    /// <summary>
    /// Loads the Game
    /// </summary>
    void LoadGame()
    {
        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// Loads the main Panel
    /// </summary>
    public void LoadMainPanel() { LoadPanel(MainPanel); }
    /// <summary>
    /// Loads the about Panel
    /// </summary>
    public void LoadAboutPanel() { LoadPanel(AboutPanel); }
    /// <summary>
    /// Loads the Rules Panel
    /// </summary>
    public void LoadRulesPanel() { LoadPanel(RulesPanel); }
    /// <summary>
    /// Loads the Rules Cards Panel and sets it to the first page
    /// </summary>
    public void LoadRulesCardsPanel()
    {
        RulesCardsPanel.Reset();
        LoadPanel(RulesCardsPanel.gameObject);
    }

}
