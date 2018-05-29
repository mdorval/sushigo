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
    public Button RulesNextButton;
    public List<Button> BackToMenuButtons;

    private GameObject CurrentPanel;
    private GameObject MainPanel;
    public GameObject AboutPanel;
    public GameObject RulesPanel;
    public RulesCardsManager RulesCardsPanel;

    // Use this for initialization
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

    public void LoadMainPanel() { LoadPanel(MainPanel); }
    public void LoadAboutPanel() { LoadPanel(AboutPanel); }
    public void LoadRulesPanel() { LoadPanel(RulesPanel); }
    public void LoadRulesCardsPanel()
    {
        RulesCardsPanel.Reset();
        LoadPanel(RulesCardsPanel.gameObject);
    }

}
