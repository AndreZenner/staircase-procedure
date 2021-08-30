using UnityEngine;
using System.Collections.Generic;

using Staircase;


/// <summary> Class to represent a sequence</summary>
public class Sequence
{
    public float stimulus { get; set; }
    public int numSequence { get; }
    public int indexSequence { get; set; }
    public Dir direction { get; set; }
    public List<float> reversals { get; }

    private bool strictLimits;
    private int numberOfSteps, currentStep;
    private float minimumValue, maximumValue;

    private float counter;
    private float stepSize = 1.0f;

    public Sequence(float startStimulus, int numSequence, Dir direction, bool strictLimits, int numberOfSteps, float minimumValue, float maximumValue, int startStep)
    {
        reversals = new List<float>();
        stimulus = startStimulus;
        this.numSequence = numSequence;
        indexSequence = 1;
        this.direction = direction;
        this.strictLimits = strictLimits;
        this.numberOfSteps = numberOfSteps;
        this.minimumValue = minimumValue;
        this.maximumValue = maximumValue;
        this.currentStep = startStep;
        this.counter = startStep;
    }

    /// <summary> Decrease StepCounter and set stimulus </summary>
    public void DecCounter()
    {

        counter = counter - stepSize;

        if (strictLimits && counter < 0)
        {
            counter = 0;
        }

        stimulus = Mathf.Lerp(minimumValue, maximumValue, (float)Mathf.Clamp((float)counter, (float)0, (float)numberOfSteps) / (float)numberOfSteps);

    }

    /// <summary> Increase StepCounter and set stimulus </summary>
    public void IncCounter()
    {

        counter = counter + stepSize;

        if (strictLimits && counter > numberOfSteps)
        {
            counter = numberOfSteps;
        }

        stimulus = Mathf.Lerp(minimumValue, maximumValue, (float)Mathf.Clamp((float)counter, (float)0, (float)numberOfSteps) / (float)numberOfSteps);
    }

}