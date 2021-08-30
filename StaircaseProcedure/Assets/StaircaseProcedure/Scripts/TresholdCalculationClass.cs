using UnityEngine;
using System.Collections.Generic;

public class ThresholdCalculationClass
{

    /// <summary> Calculates the Threshold </summary>
    public static float ThresholdCalculation(List<float> reversals, int numberThresholdPoints, bool plot)
    {
        float sum = 0;
        int size = reversals.Count;
        int count = numberThresholdPoints;
        if (size == 0 || count == 0)
        {
            Debug.LogWarning("No Reversal Points");
            return 0;
        }
        if (numberThresholdPoints > size)
        {
            count = size;
            Debug.LogWarning("GetThreshold: int numberThresholdPoints was bigger than the size of the list with reversal points.");
        }

        for (int i = size - count; i < size; ++i)
        {
            sum += reversals[i];
        }

        float threshold = sum / count;
        return threshold;
    }

    /// <summary> Calls the threshold calculation for each sequence first, then calculates the overall threshold </summary>
    public static float GetThresholdReversals(Sequence sequenceOne, Sequence sequenceTwo, int numberThresholdPoints)
    {
        List<float> rerversalsSequ1 = sequenceOne.reversals;
        List<float> rerversalsSequ2 = sequenceTwo.reversals;
        float t1 = ThresholdCalculation(rerversalsSequ1, numberThresholdPoints, false);
        float t2 = ThresholdCalculation(rerversalsSequ2, numberThresholdPoints, false);
        float t = (t1 + t2) / 2;
        return t;
    }
}