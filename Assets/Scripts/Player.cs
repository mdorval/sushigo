using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerEvent
{
    public delegate void CardChosen(Player myPlayer); //used to speed up AI if taking too long
    public delegate void StateChanged(Player myPlayer, Player.PlayerState stateToCheck);
}

public abstract class Player : MonoBehaviour //, IListensPlayedCard
{
    public enum PlayerState
    {
        WaitingToDraw,
        //This transition handled by Deck
        Drawing,
        ChoosingCard,
        Passing,
        WaitingToPlay,
        //This transition handled by Deck
        PlayingCard,
        //This transition only happens if there's no cards left, handled by Deck
        WaitingForNextRound,
        //This transition handled by Deck
    }
    protected List<CardType> handCards = new List<CardType>();
    public ScoreCard scoreCard = new ScoreCard();
    public Color textColor;
    public string playerName;
    public GameObject handPosition;
    private CardPack cardPack = null;
    private bool endOfRound = false;
    private PlayedCard playingCard = null;
    public PlayerEvent.StateChanged evtStateChanged = null;
    public PlayerEvent.CardChosen evtCardChosen = null;

    //public EventStateChanged evStateChanged = null;
    protected CardType cardToPlay = CardType.Null;

    protected Vector3 distanceBetweenScoreGroup = new Vector3(0.7f, 0, 0);

    private PlayerState _state = PlayerState.WaitingToDraw;
    public PlayerState State()
    {
        return _state;
    }
    protected void SetNewState(PlayerState newState)
    {
        _state = newState;
        if (evtStateChanged != null)
        {
            evtStateChanged(this, newState);
        }
    }

    public int playerIndex = 0;

    public void Start()
    {
        scoreCard.SetUserName(playerName);
        scoreCard.SetHtmlColor(textColor);
        Deck.Instance().AddScoreCard(scoreCard);
        Init();
    }

    public virtual void Init() { }

    public void DealHand(List<CardType> dealthand)
    {
        handCards.AddRange(dealthand);
        OnHandDealt();
    }

    protected abstract void OnHandDealt();

    public bool PassPack()
    {
        SetNewState(PlayerState.Passing);
        if (handCards.Count == 0)
        {
            SetNewState(PlayerState.WaitingToPlay);
            endOfRound = true;
            return false;
        }
        endOfRound = false;
        if (cardPack == null)
        {
            GameObject gameObj = Instantiate(Deck.Instance().cardPackPrefab, handPosition.transform);
            cardPack = gameObj.GetComponent<CardPack>();
        }
        cardPack.SetCards(handCards);
        Player neighbor = Deck.Instance().GetNeighbor(this);
        Vector3 targetPosition;
        if (Deck.Instance().PassingLeft())
        {
            cardPack.gameObject.transform.rotation = transform.rotation;
            targetPosition = transform.position;
        }
        else
        {
            cardPack.gameObject.transform.rotation = neighbor.gameObject.transform.rotation;
            targetPosition = neighbor.transform.position;
        }
        cardPack.gameObject.SetActive(true);
        MoveRequest request = new MoveRequest(targetPosition, neighbor.gameObject, this.OnCardPackPassed,24);
        cardPack.SetMoveRequest(request);
        return true;
    }

    public void OnCardPackPassed(GameObject gameObject)
    {
        cardPack = null;
        SetNewState(PlayerState.WaitingToPlay);
    }

    public void DrawCardPack()
    {
        SetNewState(PlayerState.Drawing);
        MoveRequest request = new MoveRequest(handPosition.transform.position, this.gameObject, this.OnCardPackDrawn,24);
        cardPack = this.GetComponentInChildren<CardPack>();
        cardPack.SetMoveRequest(request);
    }

    public void OnCardPackDrawn(GameObject gameobject)
    {
        cardPack = gameobject.GetComponent<CardPack>();
        gameobject.SetActive(false);
        SetNewState(PlayerState.ChoosingCard);
        DealHand(cardPack.Cards());
    }

    public void Reset()
    {
        ClearCards(false);
        scoreCard.clear();
        handCards.Clear();
    }

    public void ClearCards(bool savePuddings)
    {
        foreach (ScoreGroup group in GetComponentsInChildren<ScoreGroup>())
        {
            if (savePuddings && group.GetType() == typeof(PuddingScoreGroup))
            {
                group.transform.localPosition = distanceBetweenScoreGroup;
            }
            else
            {
                Destroy(group);
            }
        }
    }

    public void OnCardPicked(CardType card)
    {
        handCards.Remove(card);
        Vector3 position = distanceBetweenScoreGroup;
        //Create the PlayingCard prefab to bring the card into 3D space
        playingCard = Instantiate(Deck.Instance().playingCardPrefab, handPosition.transform).GetComponent<PlayedCard>();
        playingCard.ApplyCard(Deck.Instance().deckInfo.byType(card));
        //pause the playing card, we will hold it in midair until everyone has picked a card
        playingCard.Pause();
        ScoreGroup foundGroup = null;
        foreach (ScoreGroup group in GetComponentsInChildren<ScoreGroup>())
        {
            if (group.CanPlayOnGroup(card))
            {
                foundGroup = group;
                break;
            }
            else
            {
                position = (new Vector3(group.transform.localPosition.x, 0, 0)) + distanceBetweenScoreGroup;
            }
        }
        if (!foundGroup)
        {
            foundGroup = CreateNewScoreGroupForCard(card, position);
        }
        playingCard.SetMoveRequest(foundGroup.GetNextCardMoveRequest());
        if (evtCardChosen != null)
            evtCardChosen(this);
        PassPack();
    }

    public void PlayCard()
    {
        SetNewState(PlayerState.PlayingCard);
        playingCard.Resume();
        playingCard = null;
    }

    protected ScoreGroup CreateNewScoreGroupForCard(CardType card,Vector3 localPosition)
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
        group.evtCardPlayed += this.OnCardPlayed;
        return group;
    }

    public void OnCardPlayed(CardType card)
    {
        if (!endOfRound)
        {
            SetNewState(PlayerState.WaitingToDraw);
        }
        else
        {
            SetNewState(PlayerState.WaitingForNextRound);
        }
    }
}
