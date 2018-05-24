using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour {

    protected List<CardType> handCards;
    public List<CardType> passedCards;

    public int score = 0;
    public int playerIndex = 0;

    public abstract void dealHand(List<CardType> dealthand);

    public virtual void passHand()
    {
        passedCards = handCards;
    }

    public void AddCardToPlayArea(CardType card)
    {
        Vector3 position = new Vector3(0, 0, 0);
        //Get all the played cards first
        foreach (ScoreGroup group in GetComponentsInChildren<ScoreGroup>())
        {
            if (group.CanPlayOnGroup(card))
            {
                group.AddCard(card);
                return;
            }
            else
            {
                position = (new Vector3(group.transform.localPosition.x, 0, 0)) + (new Vector3(1, 0, 0));
            }
        }
        createNewScoreGroupForCard(card, position);
        //new scoregroup = 
    }

    void createNewScoreGroupForCard(CardType card,Vector3 localPosition)
    {
        GameObject gameobj = new GameObject("ScoreGroup");
        gameobj.transform.SetParent(transform);
        gameobj.transform.localPosition = localPosition;
        gameobj.transform.localRotation = Quaternion.identity;
        switch (card)
        {
            case CardType.Roll_Single:
            case CardType.Roll_Double:
            case CardType.Roll_Triple:
                gameobj.AddComponent<RollScoreGroup>();
                break;
            case CardType.Dumpling:
                gameobj.AddComponent<DumplingScoreGroup>();
                break;
            case CardType.Nigiri_Squid:
            case CardType.Nigiri_Salmon:
            case CardType.Nigiri_Egg:
                gameobj.AddComponent<NigiriScoreGroup>();
                break;
            case CardType.Wasabi:
                gameobj.AddComponent<WasabiScoreGroup>();
                break;
            case CardType.Sashimi:
                gameobj.AddComponent<SashimiScoreGroup>();
                break;
            case CardType.Tempura:
                gameobj.AddComponent<TempuraScoreGroup>();
                break;
            case CardType.Pudding:
                gameobj.AddComponent<PuddingScoreGroup>();
                break;
            default:
                gameobj.AddComponent<GenericScoreGroup>();
                break;
        }
        gameobj.GetComponent<ScoreGroup>().AddCard(card);
    }

}
