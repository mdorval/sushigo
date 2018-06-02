using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollScoreGroup : ScoreGroup {

    public override bool CanPlayOnGroup(CardType card)
    {
        return card == CardType.Roll_Double || card == CardType.Roll_Single || card == CardType.Roll_Triple;
    }

    public override void OnCardPlayedOnGroup(CardType card)
    {
        switch (card)
        {
            case CardType.Roll_Single:
                scoreCard.AddToRolls(1);
                EmitParticles(1, card);
                break;
            case CardType.Roll_Double:
                scoreCard.AddToRolls(2);
                EmitParticles(2, card);
                break;
            case CardType.Roll_Triple:
                scoreCard.AddToRolls(3);
                EmitParticles(3, card);
                break;
        }
    }
}
