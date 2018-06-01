using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ContinueDialog : MonoBehaviour {

    public Button buttonMain;
    public Button buttonAlt;
    public Text text;

	// Use this for initialization
	void Start () {
        //this.gameObject.SetActive(false);
    }

    public void ShowDialog(string dialogText, string buttonText, UnityAction buttonMainAction)
    {
        buttonAlt.gameObject.SetActive(false);
        text.text = dialogText;
        setupButton(buttonMain, buttonText, buttonMainAction);
        this.gameObject.SetActive(true);
    }

    public void ShowDialog(string dialogText, string buttonText, UnityAction buttonMainAction, string buttonAltText, UnityAction buttonAltAction)
    {
        buttonAlt.gameObject.SetActive(true);
        text.text = dialogText;
        setupButton(buttonMain, buttonText, buttonMainAction);
        setupButton(buttonAlt, buttonAltText, buttonAltAction);

        this.gameObject.SetActive(true);
    }

    private void setupButton(Button buttonToSetup, string buttonText, UnityAction buttonAction)
    {
        buttonToSetup.onClick.RemoveAllListeners();
        buttonToSetup.onClick.AddListener(buttonAction);
        buttonToSetup.onClick.AddListener(ClosePanel);
        foreach (Text iButtonText in buttonToSetup.GetComponentsInChildren<Text>())
        {
            iButtonText.text = buttonText;
        }

    }

    void ClosePanel()
    {
        this.gameObject.SetActive(false);
    }
}
