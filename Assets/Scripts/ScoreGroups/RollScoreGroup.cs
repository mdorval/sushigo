using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollScoreGroup : ScoreGroup {

    public override bool CanPlayOnGroup(CardType card)
    {
        return card == CardType.Roll_Double || card == CardType.Roll_Single || card == CardType.Roll_Triple;
    }

    public override void CardPlayedOnGroup(CardType card, ScoreCard scoreCard)
    {
        switch (card)
        {
            case CardType.Roll_Single:
                scoreCard.addToRolls(1);
                break;
            case CardType.Roll_Double:
                scoreCard.addToRolls(2);
                break;
            case CardType.Roll_Triple:
                scoreCard.addToRolls(3);
                break;
        }
    }
}
