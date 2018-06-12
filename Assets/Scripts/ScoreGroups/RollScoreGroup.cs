using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollScoreGroup : ScoreGroup {

    public override bool CanPlayOnGroup(CardType card)
    {
        return ScoreCard.RollValues.ContainsKey(card);
    }

    public override void OnCardPlayedOnGroup(CardType card)
    {
        scoreCard.AddToRolls(card);
        EmitParticles(ScoreCard.RollValues[card], card);
    }
}
