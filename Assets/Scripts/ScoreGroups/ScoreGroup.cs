using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScoreGroup: MonoBehaviour {
    protected List<CardType> cards = new List<CardType>();
    protected int score = 0;
    protected Vector3 positionOfNextCard = new Vector3(0f, 0f, 0f);
    private static Vector3 nextCardDelta = new Vector3(0, 0.01f, -0.2f);
    public abstract bool CanPlayOnGroup(CardType card);
    public abstract void CardPlayedOnGroup(CardType card,ScoreCard scoreCard);
    public void AddCard(CardType card,ScoreCard scoreCard)
    {
        //Note that a card can be added here that fails CanPlayOnGroup
        //The first card is vetted by Player.createNewScoreGroupForCard
        //Any subsequent cards are vetted by CanPlayOnGroup
        GameObject mygameobject = Instantiate(GetComponentInParent<Deck>().playedCardPrefab,transform);
        
        mygameobject.transform.localPosition = positionOfNextCard;
        positionOfNextCard += nextCardDelta;
        PlayedCard mycard = mygameobject.GetComponent<PlayedCard>();
        mycard.ApplyCard(card, GetComponentInParent<Deck>().textureForCard(card));
        cards.Add(card);
        CardPlayedOnGroup(card, scoreCard);
    }

    void OnDestroy()
    {
        Destroy(gameObject);
    }
}
