using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempuraScoreGroup : ScoreGroup {

    public override bool CanPlayOnGroup(CardType card)
    {
        return card == CardType.Tempura && cards.Count < 2;
    }

    public override void CardPlayedOnGroup(CardType card, ScoreCard scoreCard)
    {
        if (cards.Count == 2)
        {
            scoreCard.addToScore(5);
        }
    }
}
