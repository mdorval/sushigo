using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerEvent
{
    public delegate void CardChosen(Player myPlayer); //used to speed up AI if taking too long
    public delegate void CardPlayed(Player myPlayer, CardType card); //used for AI logic?
    public delegate void StateChanged(Player myPlayer, Player.PlayerState stateToCheck);
}

public abstract class Player : MonoBehaviour //, IListensPlayedCard
{
    public enum PlayerState
    {
        WaitingToDraw,
        //TODO: Drawing
        ChoosingCard,
        WaitingToPlay,
        PlayingCard,
        //TODO: Passing
    }
    protected List<CardType> handCards;
    public List<CardType> passedCards;
    public ScoreCard scoreCard;
    public Color textColor;
    public string playerName;
    public GameObject handPosition;

    public PlayerEvent.StateChanged evtStateChanged = null;
    public PlayerEvent.CardChosen evtCardChosen = null;
    public PlayerEvent.CardPlayed evtCardPlayed = null;

    //public EventStateChanged evStateChanged = null;

    private PlayerState _state = PlayerState.WaitingToDraw;
    public PlayerState State()
    {
        return _state;
    }
    private void setNewState(PlayerState newState)
    {
        _state = newState;
        if (evtStateChanged != null)
        {
            evtStateChanged(this, newState);
        }

        //GetComponentInParent<Deck>().OnPlayerStateChanged(newState);
    }
    protected CardType cardToPlay = CardType.Null;

    protected Vector3 distanceBetweenScoreGroup;

    private Deck mydeck;

    public int playerIndex = 0;

    public void Start()
    {
        scoreCard = new ScoreCard(playerName, textColor);
        mydeck = GetComponentInParent<Deck>();
        mydeck.AddScoreCard(scoreCard);
        distanceBetweenScoreGroup = new Vector3(mydeck.playedCardPrefab.transform.localScale.x + 0.04f, 0, 0);
        Init();
    }

    public virtual void Init()
    {

    }

    public abstract void dealHand(List<CardType> dealthand);

    public bool PassHand()
    {
        passedCards = handCards;
        return passedCards.Count > 0;
    }

    public void reset()
    {
        cardToPlay = CardType.Null;
        clearCards(false);
        scoreCard.clear();
        handCards.Clear();
        passedCards.Clear();
    }
    
    public void clearCards(bool savePuddings)
    {
        foreach (ScoreGroup group in GetComponentsInChildren<ScoreGroup>())
        {
            if (savePuddings && group.GetType() == typeof(PuddingScoreGroup))
            {
                group.transform.localPosition = new Vector3(0, 0, 0);
            }
            else
            {
                Destroy(group);
            }
        }
    }

    public void pickCardToPlay(CardType card)
    {
        handCards.Remove(card);
        cardToPlay = card;

        setNewState(PlayerState.WaitingToPlay);
    }

    public void playCard()
    {
        AddCardToPlayArea(cardToPlay);
        cardToPlay = CardType.Null;
    }

    public virtual void AddCardToPlayArea(CardType card)
    {
        Vector3 position = new Vector3(0, 0, 0);
        PlayedCard playingcard = Instantiate(GetComponentInParent<Deck>().playingCardPrefab, handPosition.transform).GetComponent<PlayedCard>();
        playingcard.ApplyCard(card, GetComponentInParent<Deck>().textureForCard(card));
        //Get all the played cards first
        foreach (ScoreGroup group in GetComponentsInChildren<ScoreGroup>())
        {
            if (group.CanPlayOnGroup(card))
            {
                playingcard.setTarget(group);
                return;
            }
            else
            {
                position = (new Vector3(group.transform.localPosition.x, 0, 0)) + distanceBetweenScoreGroup;
            }
        }
        ScoreGroup ngroup = createNewScoreGroupForCard(card, position);
        playingcard.setTarget(ngroup);

    }

    protected ScoreGroup createNewScoreGroupForCard(CardType card,Vector3 localPosition)
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
        ScoreGroup group = gameobj.GetComponent<ScoreGroup>();
        group.SetScoreCard(scoreCard);
        group.evtCardPlayed += CardPlayed;
        return group;
    }

    public void CardPlayed(CardType card)
    {
        setNewState(PlayerState.WaitingToDraw);
    }
}
