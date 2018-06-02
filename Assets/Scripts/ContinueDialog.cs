using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Modal Dialog to show at end of rounds
/// </summary>
public class ContinueDialog : MonoBehaviour {

    public Button buttonMain;
    public Button buttonAlt;
    public Text text;

    /// <summary>
    /// Shows the dialog
    /// </summary>
    /// <param name="dialogText">The text for the dialog</param>
    /// <param name="buttonText">The text for the main button</param>
    /// <param name="buttonMainAction">The action for the main button</param>
    /// <param name="buttonAltText">Text for the alt buttton (optional)</param>
    /// <param name="buttonAltAction">Action for the alt button (optional) </param>
    public void ShowDialog(string dialogText, string buttonText, UnityAction buttonMainAction, string buttonAltText=null, UnityAction buttonAltAction=null)
    {
        buttonAlt.gameObject.SetActive(true);
        text.text = dialogText;
        SetupButton(buttonMain, buttonText, buttonMainAction);
        if (buttonAltText != null && buttonAltAction != null)
        {
            SetupButton(buttonAlt, buttonAltText, buttonAltAction);
        }
        else
        {
            buttonAlt.gameObject.SetActive(false);
        }
        this.gameObject.SetActive(true);
    }

    /// <summary>
    /// Hooks a button to an action
    /// </summary>
    /// <param name="buttonToSetup">The button to setup</param>
    /// <param name="buttonText">The text</param>
    /// <param name="buttonAction">The action</param>
    private void SetupButton(Button buttonToSetup, string buttonText, UnityAction buttonAction)
    {
        buttonToSetup.onClick.RemoveAllListeners();
        buttonToSetup.onClick.AddListener(buttonAction);
        buttonToSetup.onClick.AddListener(ClosePanel);
        foreach (Text iButtonText in buttonToSetup.GetComponentsInChildren<Text>())
        {
            iButtonText.text = buttonText;
        }

    }

    /// <summary>
    /// Closes the Dialog
    /// </summary>
    void ClosePanel()
    {
        this.gameObject.SetActive(false);
    }
}
