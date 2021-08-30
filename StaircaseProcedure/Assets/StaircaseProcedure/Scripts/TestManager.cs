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
    public KeyCode keycodeGetThreshold = KeyCode.T;


    [Header("Init Procedure Parameters")]
    public string experimentName = "ExperimentName";
    public string conditionName = "ConditionName";

    public int numberParticipant = 0;
    public float minimumValue = 0.0f;
    public float maximumValue = 1.0f;
    public int numberOfSteps = 10;
    public int startStepSequ1 = 0;
    public int startStepSequ2 = 8;
    public int stopAmount = 5;
    public int numberThresholdPoints = 3;
    public bool stopCriterionReversals = true;
    public bool strictLimits = false;
    public string plotTitle = "";

    // Update is called once per frame
    void Update()
    {
        if(enableTesting){
            if (Input.GetKeyDown(keycodeInitializeProcedure)) {
                StaircaseProcedure.SP.Init(minimumValue:minimumValue, maximumValue: maximumValue, numberOfSteps: numberOfSteps,
                    startStepSequ1: startStepSequ1, startStepSequ2: startStepSequ2,
                    stopAmount: stopAmount, numberThresholdPoints: numberThresholdPoints, experimentName: experimentName,
                    conditionName: conditionName, numberParticipant: numberParticipant,
                    stopCriterionReversals: stopCriterionReversals, strictLimits: strictLimits, plotTitle: plotTitle
                    );
                StaircaseProcedure.SP.GetNextStimulus();
            }
            if (Input.GetKeyDown(keycodeStimulusNoticed)) {
                StaircaseProcedure.SP.TrialFinished(true);
                StaircaseProcedure.SP.GetNextStimulus();
            }
            else if (Input.GetKeyDown(keycodeStimulusNotNoticed)){
                StaircaseProcedure.SP.TrialFinished(false);
                StaircaseProcedure.SP.GetNextStimulus();
            }
            else if (Input.GetKeyDown(keycodeGetThreshold)){        
                StaircaseProcedure.SP.GetThreshold();
            }
        }
    }
}
