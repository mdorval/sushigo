using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NigiriScoreGroup : ScoreGroup {

    public override bool CanPlayOnGroup(CardType card)
    {
        //Don't stack nigiri - nigiri should be layed down side by side
        //Nigiri can go on wasabi but not on each other
        return false;
    }

    public override void CardPlayedOnGroup(CardType card)
    {
        switch (card)
        {
            case CardType.Nigiri_Egg:
                scoreCard.addToScore(1);
                break;
            case CardType.Nigiri_Salmon:
                scoreCard.addToScore(2);
                break;
            case CardType.Nigiri_Squid:
                scoreCard.addToScore(3);
                break;
        }
    }
}
