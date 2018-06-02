using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WasabiScoreGroup : ScoreGroup {

    public override bool CanPlayOnGroup(CardType card)
    {
        return (card == CardType.Nigiri_Egg || card == CardType.Nigiri_Salmon || card == CardType.Nigiri_Squid)
            && cards.Count < 2;
    }

    public override void OnCardPlayedOnGroup(CardType card)
    {
        //Ignore wasabi card points-wise and particle-wise
        switch (card)
        {
            case CardType.Nigiri_Egg:
                scoreCard.AddToScore(3);
                EmitParticles(3,card);
                break;
            case CardType.Nigiri_Salmon:
                scoreCard.AddToScore(6);
                EmitParticles(6,card);
                break;
            case CardType.Nigiri_Squid:
                scoreCard.AddToScore(9);
                EmitParticles(9,card);
                break;
        }
    }
}
