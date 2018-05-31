using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuddingScoreGroup: ScoreGroup {

    public override bool CanPlayOnGroup(CardType card)
    {
        return card == CardType.Pudding;
    }

    public override void CardPlayedOnGroup(CardType card)
    {
        scoreCard.addToPuddings(1);
    }
}
