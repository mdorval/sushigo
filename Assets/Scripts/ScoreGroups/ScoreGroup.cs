using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScoreGroupEvent
{
    public delegate void CardPlayed(CardType cardType);
}

public abstract class ScoreGroup: MonoBehaviour {
    protected List<CardType> cards = new List<CardType>();
    protected ScoreCard scoreCard;
    protected ParticleSystem cardPlayParticleSystem = null;
    private ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
    public Vector3 positionOfNextCard = new Vector3(0f, 0f, 0f);
    private static Vector3 nextCardDelta = new Vector3(0, 0.01f, -0.2f);
    /// <summary>
    /// Whether the card can be played on this group. 
    /// Used by player to decide whether to use an existing score group or create a new scoregroup
    /// </summary>
    /// <param name="card">Card To Check</param>
    /// <returns></returns>
    public abstract bool CanPlayOnGroup(CardType card);
    /// <summary>
    /// Played when card is played on this group
    /// </summary>
    /// <param name="card">The type of card played</param>
    public abstract void OnCardPlayedOnGroup(CardType card);

    public ScoreGroupEvent.CardPlayed evtCardPlayed = null;

    /// <summary>
    /// Sets the particle system to use
    /// </summary>
    /// <param name="system">Particle system</param>
    public void SetParticleSystem(ParticleSystem system)
    {
        cardPlayParticleSystem = system;
    }

    /// <summary>
    /// Sets the ScoreCard for this player
    /// </summary>
    /// <param name="myScoreCard">The ScoreCard</param>
    public void SetScoreCard(ScoreCard myScoreCard)
    {
        scoreCard = myScoreCard;
    }

    void OnDestroy()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Gets a move request to place a card into place on this scoregroup
    /// </summary>
    /// <returns></returns>
    public MoveRequest GetNextCardMoveRequest()
    {
        MoveRequest request = new MoveRequest(this.transform.TransformPoint(positionOfNextCard), this.gameObject, this.OnCardInPlace);
        positionOfNextCard += nextCardDelta;
        return request;
    }

    protected void EmitParticles(int intensity,CardType card)
    {
        if (cardPlayParticleSystem != null)
        {
            ParticleSystem.MainModule settings = cardPlayParticleSystem.main;
            settings.startColor = Deck.Instance().deckInfo.byType(card).particleColor;
            cardPlayParticleSystem.Emit(intensity*3);
        }
    }

    /// <summary>
    /// Called when played
    /// </summary>
    /// <param name="obj">The card object that was played</param>
    private void OnCardInPlace(GameObject obj)
    {
        PlayedCard card = obj.GetComponent<PlayedCard>();
        cards.Add(card.card);
        if (cardPlayParticleSystem != null)
        {
            cardPlayParticleSystem.transform.position = obj.transform.position;
        }
        OnCardPlayedOnGroup(card.card);
        if (evtCardPlayed != null)
        {
            evtCardPlayed(card.card);
        }
    }
}
