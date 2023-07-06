using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleTestManager : MonoBehaviour
{
    public bool enableTesting = true;
    public int numberOfStaircases = 3;
    public StaircaseProcedure currStaircase = null;

    [Header("Keyboard Settings")]
    public KeyCode keycodeInitializeProcedure = KeyCode.I;
    public KeyCode keycodeStimulusNoticed = KeyCode.Y;
    public KeyCode keycodeStimulusNotNoticed = KeyCode.N;
    public KeyCode keycodeSelectRandomStaircase = KeyCode.R;


    [Header("Staircase Script Settings")]
    public string ResultsPath;
    public string PythonPath;
    public bool LivePlotter = true;
    public bool ShowSubPlots = true;
    public bool StartPythonAutomatically = true;
    public string Delimiter = ";";
    public string IPAddress = "127.0.0.1";
    public int PortOfFirstStaircase = 65000;


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
    public int stopAmount = 3;
    public int numberThresholdPoints = 2;
    public bool stopCriterionReversals = true;
    public bool strictLimits = false;
    public string plotTitle = "";

    // Update is called once per frame
    void Update()
    {
        if (enableTesting)
        {
            // press I to create 3 parallel staircases
            if (Input.GetKeyDown(keycodeInitializeProcedure))
            {
                for(int i = 0; i < numberOfStaircases; i++)
                {
                    StaircaseProcedure newSP = this.gameObject.AddComponent<StaircaseProcedure>() as StaircaseProcedure;
                    newSP.Create(
                        ResultsPath,
                        PythonPath,
                        LivePlotter,
                        ShowSubPlots,
                        StartPythonAutomatically,
                        Delimiter,
                        IPAddress,
                        PortOfFirstStaircase + i);
                    newSP.Awake();

                    newSP.Init(
                        minimumValue: minimumValue,
                        maximumValue: maximumValue,
                        numberOfSteps: numberOfSteps,
                        stepsUp: stepsUp,
                        stepsDown: stepsDown,
                        stepsUpStartEarly: stepsUpStartEarly,
                        stepsDownStartEarly: stepsDownStartEarly,
                        quickStartEarlyUntilReversals: quickStartEarlyUntilReversals,
                        stepsUpStartLate: stepsUpStartLate,
                        stepsDownStartLate: stepsDownStartLate,
                        quickStartLateUntilReversals: quickStartLateUntilReversals,
                        startStepSequ1: startStepSequ1,
                        startStepSequ2: startStepSequ2,
                        stopAmount: stopAmount,
                        numberThresholdPoints: numberThresholdPoints,
                        experimentName: experimentName,
                        conditionName: conditionName + "#" + i.ToString(),
                        numberParticipant: numberParticipant,
                        stopCriterionReversals: stopCriterionReversals,
                        strictLimits: strictLimits,
                        plotTitle: plotTitle
                    );
                }
            }

            // ALTERNATE R and {Y/N}
            // press R to select a random (not yet finished) staircase
            if (Input.GetKeyDown(keycodeSelectRandomStaircase))
            {
                if (StaircaseProcedure.AreAllSPsFinished())
                {
                    Debug.Log("All staircases are finished. Nothing to do anymore.");
                    return;
                }

                bool found = false;
                while (!found) {
                    int i = Random.Range(0, numberOfStaircases);
                    if (!StaircaseProcedure.AllSPs[i].IsFinished())
                    {
                        currStaircase = StaircaseProcedure.AllSPs[i];
                        found = true;
                    }
                }
                currStaircase.GetNextStimulus();
                Debug.Log("Selected staircase with condition " + currStaircase.GetConditionName());
            }

            // press Y or N to provide the answer
            if (Input.GetKeyDown(keycodeStimulusNoticed))
            {
                currStaircase.TrialFinished(true);
                if (currStaircase.IsFinished())
                    currStaircase.GetThreshold();
            }
            else if (Input.GetKeyDown(keycodeStimulusNotNoticed))
            {
                currStaircase.TrialFinished(false);
                if (currStaircase.IsFinished())
                    currStaircase.GetThreshold();
            }
        }
    }
}
