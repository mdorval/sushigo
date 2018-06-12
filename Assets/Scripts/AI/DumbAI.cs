using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumbAI : BaseAI
{
    public DumbAI(ComputerPlayer ourPlayer): base(ourPlayer)
    {
    }
    public override CardType ChooseCard(List<CardType> pack)
    {
        int r = (int)UnityEngine.Random.Range(0.0f, pack.Count);
        return pack[r];
    }

    public override void Dispose()
    {
    }
}
