﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SashimiScoreGroup : ScoreGroup {
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override bool CanPlayOnGroup(CardType card)
    {
        return card == CardType.Sashimi && cards.Count < 3;
    }

}
