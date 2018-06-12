using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Used for Predicting how much a roll is worth for AI
/// </summary>
public class PredictRollScoreCard : IRollScorer
{
    private readonly int _rollsCount = 0;
    private int _rollsPotentialScore = 0;
    public PredictRollScoreCard(int rollsCount)
    {
        _rollsCount = rollsCount;
    }
    public int Rolls()
    {
        return _rollsCount;
    }

    public void SetRollsPotentialScore(int rollScore)
    {
        _rollsPotentialScore = rollScore;
    }

    public int GetRollsPotentialScore()
    {
        return _rollsPotentialScore;
    }
}
