using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SashimiScoreGroup : ScoreGroup {
    public override bool CanPlayOnGroup(CardType card)
    {
        return card == CardType.Sashimi && cards.Count < 3;
    }

    public override void CardPlayedOnGroup(CardType card, ScoreCard scoreCard)
    {
        if (cards.Count == 3)
        {
            scoreCard.addToScore(5);
        }
    }
}
