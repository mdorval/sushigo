using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuddingScoreGroup: ScoreGroup {

    public override bool CanPlayOnGroup(CardType card)
    {
        return card == CardType.Pudding;
    }

    public override void OnCardPlayedOnGroup(CardType card)
    {
        scoreCard.AddToPuddings(1);
        EmitParticles(2,card);
    }
}
