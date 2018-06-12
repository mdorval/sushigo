using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerEvent
{
    public delegate void CardChosen(Player myPlayer); //used to speed up AI if taking too long
    public delegate void StateChanged(Player myPlayer, Player.PlayerState stateToCheck);
    public delegate void CardPlayed(Player myPlayer, CardType cardPlayed);
}

public abstract class Player : MonoBehaviour //, IListensPlayedCard
{
    public enum PlayerState
    {
        WaitingToDraw, //The player has no cards or current action
        //This transition handled by Deck
        Drawing, //The deck is moving but hasn't reached the player's hand
        ChoosingCard, //The player is choosing a card
        Passing, //The deck has left the player's hand but hasn't reached the table
        WaitingToPlay, //The player's deck is on the table and they are holding a card ready to be played
        //This transition handled by Deck
        PlayingCard, //The playing card has left the hand but has not reached the table
        //This transition only happens if there's no cards left, handled by Deck
        WaitingForNextRound,//The player has no cards and did not pass a deck
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
    public ParticleSystem cardPlayParticleSystem = null;
    public PlayerEvent.StateChanged evtStateChanged = null;
    public PlayerEvent.CardChosen evtCardChosen = null;
    public PlayerEvent.CardPlayed evtCardPlayed = null;

    //public EventStateChanged evStateChanged = null;
    protected CardType cardToPlay = CardType.Null;

    protected Vector3 distanceBetweenScoreGroup = new Vector3(0.7f, 0, 0);

    private PlayerState _state = PlayerState.WaitingToDraw;
    /// <summary>
    /// The state this player is in
    /// </summary>
    /// <returns>Playerstate</returns>
    public PlayerState State()
    {
        return _state;
    }
    /// <summary>
    /// Sets the State for this player
    /// </summary>
    /// <param name="newState">state to set</param>
    protected void SetNewState(PlayerState newState)
    {
        _state = newState;
        if (evtStateChanged != null)
        {
            evtStateChanged(this, newState);
        }
    }

    public void Start()
    {
        scoreCard.SetUserName(playerName);
        scoreCard.SetColor(textColor);
        Deck.Instance().AddScoreCard(scoreCard);
        Init();
    }

    public virtual void Init() { }

    /// <summary>
    /// Deals cards to hand
    /// </summary>
    /// <param name="dealthand">cards in hand</param>
    public void DealHand(List<CardType> dealthand)
    {
        handCards.AddRange(dealthand);
        OnHandDealt();
    }

    protected abstract void OnHandDealt();

    /// <summary>
    /// Starts the pass of the pack
    /// </summary>
    /// <returns>If there was cards left to pass</returns>
    public bool PassCardPack()
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

    /// <summary>
    /// Called when a card pack is finished passing (when it hits the table)
    /// </summary>
    /// <param name="gameObject">cardPack</param>
    public void OnCardPackPassed(GameObject gameObject)
    {
        cardPack = null;
        SetNewState(PlayerState.WaitingToPlay);
    }

    /// <summary>
    /// Starts drawing the next turn's card pack from the table
    /// </summary>
    public void DrawCardPack()
    {
        SetNewState(PlayerState.Drawing);
        MoveRequest request = new MoveRequest(handPosition.transform.position, this.gameObject, this.OnCardPackDrawn,24);
        cardPack = this.GetComponentInChildren<CardPack>();
        cardPack.SetMoveRequest(request);
    }

    /// <summary>
    /// Called when the card pack has reached the player's hand
    /// </summary>
    /// <param name="gameobject">cardPack</param>
    public void OnCardPackDrawn(GameObject gameobject)
    {
        cardPack = gameobject.GetComponent<CardPack>();
        gameobject.SetActive(false);
        SetNewState(PlayerState.ChoosingCard);
        DealHand(cardPack.Cards());
    }

    /// <summary>
    /// Resets the game
    /// </summary>
    public virtual void Reset()
    {
        ClearCards(false);
        scoreCard.Clear();
        handCards.Clear();
    }

    /// <summary>
    /// Clears the Cards on the table readying for the next round
    /// </summary>
    /// <param name="savePuddings">whether to save the puddings (new round or new game?)</param>
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

    /// <summary>
    /// Called when a card is picked by the player
    /// </summary>
    /// <param name="card">The Card picked</param>
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
        PassCardPack();
    }

    /// <summary>
    /// Called by Deck when everyone is ready to Play cards
    /// </summary>
    public void PlayCard()
    {
        SetNewState(PlayerState.PlayingCard);
        playingCard.Resume();
        playingCard = null;
    }

    /// <summary>
    /// Creates a new Scoregroup for the given card
    /// </summary>
    /// <param name="card">The Card for the new scoregroup</param>
    /// <param name="localPosition">The position for the new scoreGroup</param>
    /// <returns>The new Scoregroup</returns>
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
        //Add our scorecard to the group
        group.SetScoreCard(scoreCard);
        //Add particle system if exists
        if (cardPlayParticleSystem != null)
        {
            group.SetParticleSystem(cardPlayParticleSystem);
        }
        //Connect our eventListener to the scoregroup
        group.evtCardPlayed += this.OnCardPlayed;
        return group;
    }
    /// <summary>
    /// Called when the card hits the scoregroup
    /// </summary>
    /// <param name="card"></param>
    public void OnCardPlayed(CardType card)
    {
        if (evtCardPlayed != null)
        {
            evtCardPlayed(this, card);
        }
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
