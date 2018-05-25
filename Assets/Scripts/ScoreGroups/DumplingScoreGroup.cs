using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumplingScoreGroup : ScoreGroup {

    public override bool CanPlayOnGroup(CardType card)
    {
        return card == CardType.Dumpling;
    }

    public override void CardPlayedOnGroup(CardType card, ScoreCard scoreCard)
    {
        if (cards.Count <= 5)
        {
            scoreCard.addToScore(cards.Count);
        }
    }
}
