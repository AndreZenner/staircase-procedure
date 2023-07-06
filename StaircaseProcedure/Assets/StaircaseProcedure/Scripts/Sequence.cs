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
    private int stepsUp, stepsDown;
    private int stepsUpStartEarly, stepsDownStartEarly;
    private int quickStartEarlyUntilReversals;
    private int stepsUpStartLate, stepsDownStartLate;
    private int quickStartLateUntilReversals;

    private float counter;

    public Sequence(
        float startStimulus, 
        int numSequence, 
        Dir direction, 
        bool strictLimits, 
        int numberOfSteps,
        int stepsUp,
        int stepsDown,
        int stepsUpStartEarly,
        int stepsDownStartEarly,
        int quickStartEarlyUntilReversals,
        int stepsUpStartLate,
        int stepsDownStartLate,
        int quickStartLateUntilReversals,
        float minimumValue, 
        float maximumValue, 
        int startStep)
    {
        reversals = new List<float>();
        stimulus = startStimulus;
        this.numSequence = numSequence;
        indexSequence = 1;
        this.direction = direction;
        this.strictLimits = strictLimits;
        this.numberOfSteps = numberOfSteps;
        this.stepsUp = stepsUp;
        this.stepsDown = stepsDown;
        this.stepsUpStartEarly = stepsUpStartEarly;
        this.stepsDownStartEarly = stepsDownStartEarly;
        this.quickStartEarlyUntilReversals = quickStartEarlyUntilReversals;
        this.stepsUpStartLate = stepsUpStartLate;
        this.stepsDownStartLate = stepsDownStartLate;
        this.quickStartLateUntilReversals = quickStartLateUntilReversals;
        this.minimumValue = minimumValue;
        this.maximumValue = maximumValue;
        this.currentStep = startStep;
        this.counter = startStep;
    }

    /// <summary> Decrease StepCounter and set stimulus </summary>
    public void DecCounter()
    {
        //if still in "quick start" mode -> use the (weighted) start steps size(s)
        if (reversals.Count < quickStartEarlyUntilReversals)    // use "Early" step sizes until quickStartEarlyUntilReversals-many reversals have occurred
            counter = counter - stepsDownStartEarly;
        else if (reversals.Count < quickStartLateUntilReversals)// use "Late" step sizes if more then quickStartEarlyUntilReversals-many reversals, but less than quickStartLateUntilReversals-many reversals have occurred
            counter = counter - stepsDownStartLate;
        else //else: use the (weighted) step size
            counter = counter - stepsDown;

        if (strictLimits && counter < 0)
        {
            counter = 0;
        }

        stimulus = Mathf.Lerp(minimumValue, maximumValue, (float)Mathf.Clamp((float)counter, (float)0, (float)numberOfSteps) / (float)numberOfSteps);

    }

    /// <summary> Increase StepCounter and set stimulus </summary>
    public void IncCounter()
    {
        //if still in "quick start" mode -> use the (weighted) start steps size(s)
        if (reversals.Count < quickStartEarlyUntilReversals)    // use "Early" step sizes until quickStartEarlyUntilReversals-many reversals have occurred
            counter = counter + stepsUpStartEarly;
        else if (reversals.Count < quickStartLateUntilReversals)// use "Late" step sizes if more then quickStartEarlyUntilReversals-many reversals, but less than quickStartLateUntilReversals-many reversals have occurred
            counter = counter + stepsUpStartLate;
        else //else: use the (weighted) step size
        counter = counter + stepsUp;

        if (strictLimits && counter > numberOfSteps)
        {
            counter = numberOfSteps;
        }

        stimulus = Mathf.Lerp(minimumValue, maximumValue, (float)Mathf.Clamp((float)counter, (float)0, (float)numberOfSteps) / (float)numberOfSteps);
    }
}