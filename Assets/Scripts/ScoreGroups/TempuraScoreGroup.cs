using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempuraScoreGroup : ScoreGroup {

    public override bool CanPlayOnGroup(CardType card)
    {
        return card == CardType.Tempura && cards.Count < 2;
    }

    public override void OnCardPlayedOnGroup(CardType card)
    {
        if (cards.Count == 2)
        {
            scoreCard.AddToScore(5);
            EmitParticles(5,card);
        }
    }
}
