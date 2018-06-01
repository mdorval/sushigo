﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScoreGroupEvent
{
    public delegate void CardPlayed(CardType cardType);
}

public abstract class ScoreGroup: MonoBehaviour {
    protected List<CardType> cards = new List<CardType>();
    protected ScoreCard scoreCard;
    public Vector3 positionOfNextCard = new Vector3(0f, 0f, 0f);
    private static Vector3 nextCardDelta = new Vector3(0, 0.01f, -0.2f);
    public abstract bool CanPlayOnGroup(CardType card);
    public abstract void CardPlayedOnGroup(CardType card);

    public ScoreGroupEvent.CardPlayed evtCardPlayed = null;

    public void SetScoreCard(ScoreCard myScoreCard)
    {
        scoreCard = myScoreCard;
    }

    void OnDestroy()
    {
        Destroy(gameObject);
    }
    public MoveRequest GetNextCardMoveRequest()
    {
        MoveRequest request = new MoveRequest(this.transform.TransformPoint(positionOfNextCard), this.gameObject, CardInPlace);
        positionOfNextCard += nextCardDelta;
        return request;
    }
        
    public void CardInPlace(GameObject obj)
    {
        PlayedCard card = obj.GetComponent<PlayedCard>();
        cards.Add(card.card);
        CardPlayedOnGroup(card.card);
        if (evtCardPlayed != null)
        {
            evtCardPlayed(card.card);
        }
    }
}
