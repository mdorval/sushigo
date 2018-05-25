using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericScoreGroup : ScoreGroup {

    public override bool CanPlayOnGroup(CardType card)
    {
        //These are cards we don't stack
        return false;
    }

    public override void CardPlayedOnGroup(CardType card, ScoreCard scoreCard)
    {
        //Do nothing
    }
}
