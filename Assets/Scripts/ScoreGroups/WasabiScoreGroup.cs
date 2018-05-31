using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WasabiScoreGroup : ScoreGroup {

    public override bool CanPlayOnGroup(CardType card)
    {
        return (card == CardType.Nigiri_Egg || card == CardType.Nigiri_Salmon || card == CardType.Nigiri_Squid)
            && cards.Count < 2;
    }

    public override void CardPlayedOnGroup(CardType card)
    {
        //Ignore wasabi card points-wise
        switch (card)
        {
            case CardType.Nigiri_Egg:
                scoreCard.addToScore(3);
                break;
            case CardType.Nigiri_Salmon:
                scoreCard.addToScore(6);
                break;
            case CardType.Nigiri_Squid:
                scoreCard.addToScore(9);
                break;
        }
    }
}
