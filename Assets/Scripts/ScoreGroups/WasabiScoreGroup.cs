using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WasabiScoreGroup : ScoreGroup {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override bool CanPlayOnGroup(CardType card)
    {
        return (card == CardType.Nigiri_Egg || card == CardType.Nigiri_Salmon || card == CardType.Nigiri_Squid)
            && cards.Count < 2;
    }

}
