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

    public override void OnCardPlayedOnGroup(CardType card)
    {
        switch (card)
        {
            case CardType.Nigiri_Egg:
                scoreCard.AddToScore(1);
                EmitParticles(1,card);
                break;
            case CardType.Nigiri_Salmon:
                scoreCard.AddToScore(2);
                EmitParticles(2,card);
                break;
            case CardType.Nigiri_Squid:
                scoreCard.AddToScore(3);
                EmitParticles(3,card);
                break;
        }
    }
}
