using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumplingScoreGroup : ScoreGroup {
    public override bool CanPlayOnGroup(CardType card)
    {
        return card == CardType.Dumpling;
    }

    public override void OnCardPlayedOnGroup(CardType card)
    {
        if (cards.Count <= 5)
        {
            scoreCard.AddToScore(cards.Count);
            EmitParticles(cards.Count,card);
        }
    }
}
