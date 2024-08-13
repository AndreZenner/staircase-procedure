using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    public bool enableTesting = true;

    [Header("Keyboard Settings")]
    public KeyCode keycodeInitializeProcedure = KeyCode.I;
    public KeyCode keycodeStimulusNoticed = KeyCode.Y;
    public KeyCode keycodeStimulusNotNoticed = KeyCode.N;

    [Header("Init Procedure Parameters")]
    public string experimentName = "ExperimentName";
    public string conditionName = "ConditionName";

    public int numberParticipant = 0;
    public float minimumValue = 0.0f;
    public float maximumValue = 1.0f;
    public int numberOfSteps = 10;
    public int stepsUp = 1;
    public int stepsDown = 1;
    public int stepsUpStartEarly = 1;
    public int stepsDownStartEarly = 1;
    public int quickStartEarlyUntilReversals = 0;
    public int stepsUpStartLate = 1;
    public int stepsDownStartLate = 1;
    public int quickStartLateUntilReversals = 0;
    public int startStepSequ1 = 0;
    public int startStepSequ2 = 8;
    public int stopAmount = 5;
    public int numberThresholdPoints = 3;
    public bool stopCriterionReversals = true;
    public bool strictLimits = false;
    public bool singleSequence = false;
    public bool singleSequenceUp = false;
    public string plotTitle = "";

    // Update is called once per frame
    void Update()
    {
        // at the beginning of your experiment: init the staircase once with your settings:
        // HERE: PRESS I
        if(enableTesting){
            if (Input.GetKeyDown(keycodeInitializeProcedure)) {
                StaircaseProcedure.SP.Init(
                    minimumValue:minimumValue, 
                    maximumValue: maximumValue,
                    numberOfSteps: numberOfSteps,
                    startStepSequ1: startStepSequ1,
                    startStepSequ2: startStepSequ2,
                    stepsUp: stepsUp,
                    stepsDown: stepsDown,
                    stepsUpStartEarly: stepsUpStartEarly,
                    stepsDownStartEarly: stepsDownStartEarly,
                    quickStartEarlyUntilReversals: quickStartEarlyUntilReversals,
                    stepsUpStartLate: stepsUpStartLate,
                    stepsDownStartLate: stepsDownStartLate,
                    quickStartLateUntilReversals: quickStartLateUntilReversals,
                    stopAmount: stopAmount,
                    numberThresholdPoints: numberThresholdPoints, 
                    experimentName: experimentName,
                    conditionName: conditionName,
                    numberParticipant: numberParticipant,
                    stopCriterionReversals: stopCriterionReversals, 
                    strictLimits: strictLimits, 
                    singleSequence: singleSequence,
                    singleSequenceUp: singleSequenceUp,
                    plotTitle: plotTitle
                    );
                StaircaseProcedure.SP.GetNextStimulus();
            }

            // during your experiment: tell the staircase about the participant's response and get the next stimulus
            // HERE: Press Y or N
            if (Input.GetKeyDown(keycodeStimulusNoticed)) {
                if (StaircaseProcedure.SP.IsFinished())
                {
                    Debug.Log("Staircase is finished. Nothing to do anymore.");
                } else {
                    StaircaseProcedure.SP.TrialFinished(true);
                    if (StaircaseProcedure.SP.IsFinished())
                        StaircaseProcedure.SP.GetThreshold();
                    else
                        StaircaseProcedure.SP.GetNextStimulus();
                }
            }
            else if (Input.GetKeyDown(keycodeStimulusNotNoticed)){
                if (StaircaseProcedure.SP.IsFinished())
                {
                    Debug.Log("Staircase is finished. Nothing to do anymore.");
                }
                else
                {
                    StaircaseProcedure.SP.TrialFinished(false);
                    if(StaircaseProcedure.SP.IsFinished())
                        StaircaseProcedure.SP.GetThreshold();
                    else
                        StaircaseProcedure.SP.GetNextStimulus();
                }
            }
        }
    }
}
