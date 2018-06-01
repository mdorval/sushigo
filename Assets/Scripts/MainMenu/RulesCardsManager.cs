using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class RulesCardsManager : MonoBehaviour {
    public List<Text> displayText;
    public List<SpriteRenderer> displaySprites;
    public List<GameObject> displayPanels;
    public Button nextButton;
    public Button previousButton;
    public DeckInfo info;
    public MainMenuManager manager;
    private List<CardType> cardsWithDescription = new List<CardType>();
    int page = 0;

    void Start () {
        foreach(CardInfo info in info.cards)
        {
            if (info.rulesPageText != "")
            {
                cardsWithDescription.Add(info.type);
            }
        }
        previousButton.onClick.AddListener(PreviousPage);
        nextButton.onClick.AddListener(NextPage);
        Init();
	}

    public void Reset()
    {
        if (page != 0)
        {
            page = 0;
            Init();
        }
    }

    void Init()
    {
        var cardsToShow = page == 0 ? cardsWithDescription.Take(3).ToArray() : cardsWithDescription.Skip(page*3).Take(3).ToArray();
        for (int i = 0; i < 3; i++)
        {
            if (cardsToShow.Count() < (i+1))
            {
                //Hide the panel if we're not showing all the cards
                displayPanels[i].SetActive(false);
                displayText[i].text = "";
                displaySprites[i].gameObject.SetActive(false);
            }
            else
            {
                displayPanels[i].SetActive(true);
                displayText[i].text = info.byType(cardsToShow[i]).rulesPageText;
                displaySprites[i].gameObject.SetActive(true);
                displaySprites[i].sprite = info.byType(cardsToShow[i]).cardSprite;
            }
        }
        //If there's another page after this one, enable the next button.
        if ( ((page+1) * 3) >= cardsWithDescription.Count())
        {
            nextButton.gameObject.SetActive(false);
        }
        else
        {
            nextButton.gameObject.SetActive(true);
        }
    }
    
    void PreviousPage()
    {
        if (page == 0)
        {
            this.gameObject.SetActive(false);
            manager.LoadRulesPanel();
        }
        else
        {
            page--;
            Init();
        }
    }

    void NextPage()
    {
        //Next button hides if the next page not valid, so no error-checking here
        page++;
        Init();
    }


    // Update is called once per frame
    void Update () {
		
	}
}
