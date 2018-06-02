using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SashimiScoreGroup : ScoreGroup {
    public override bool CanPlayOnGroup(CardType card)
    {
        return card == CardType.Sashimi && cards.Count < 3;
    }

    public override void OnCardPlayedOnGroup(CardType card)
    {
        if (cards.Count == 3)
        {
            scoreCard.AddToScore(5);
        }
    }
}
