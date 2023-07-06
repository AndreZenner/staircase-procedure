using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


namespace Staircase{

    /// <summary> Class to store information about the init data </summary>
    public class InitData{

        public float minimumValue{ get; }
        public float maximumValue{ get; }
        public int numberOfSteps{ get; }
        public int startStepSequ1{ get; }
        public int startStepSequ2{ get; }
        public int stepsUp { get; }
        public int stepsDown { get; }
        public int stepsUpStartEarly { get; }
        public int stepsDownStartEarly { get; }
        public int quickStartEarlyUntilReversals { get; }
        public int stepsUpStartLate { get; }
        public int stepsDownStartLate { get; }
        public int quickStartLateUntilReversals { get; }
        public int stopAmount{ get; }
        public int numberThresholdPoints{ get; }
        public string resultsPath{ get; }
        public string experimentName{ get; }
        public string conditionName{ get; }
        public int numberParticipant{ get; }
        public bool stopCriterionReversals { get; }
        public bool strictLimits { get; }
        public string plotTitle { get; }

        public InitData(float minimumValue, 
            float maximumValue,
            int numberOfSteps,
            int startStepSequ1,
            int startStepSequ2, 
            int stepsUp, 
            int stepsDown,
            int stepsUpStartEarly,
            int stepsDownStartEarly,
            int quickStartEarlyUntilReversals,
            int stepsUpStartLate,
            int stepsDownStartLate,
            int quickStartLateUntilReversals,
            int stopAmount,
            int numberThresholdPoints,
            string resultsPath,
            string experimentName,
            string conditionName,
            int numberParticipant,
            bool stopCriterionReversals,
            bool strictLimits,
            string plotTitle
            )
        {
            this.minimumValue = minimumValue;
            this.maximumValue = maximumValue;
            this.numberOfSteps = numberOfSteps;
            this.startStepSequ1 = startStepSequ1;
            this.startStepSequ2 = startStepSequ2;
            this.stepsUp = stepsUp;
            this.stepsDown = stepsDown;
            this.stepsUpStartEarly = stepsUpStartEarly;
            this.stepsDownStartEarly = stepsDownStartEarly;
            this.quickStartEarlyUntilReversals = quickStartEarlyUntilReversals;
            this.stepsUpStartLate = stepsUpStartLate;
            this.stepsDownStartLate = stepsDownStartLate;
            this.quickStartLateUntilReversals = quickStartLateUntilReversals;
            this.stopAmount = stopAmount;
            this.numberThresholdPoints = numberThresholdPoints;
            this.numberThresholdPoints = numberThresholdPoints;
            this.resultsPath = resultsPath;
            this.experimentName = experimentName;
            this.conditionName = conditionName;
            this.numberParticipant = numberParticipant;
            this.stopCriterionReversals = stopCriterionReversals;
            this.strictLimits = strictLimits;
            this.plotTitle = plotTitle;
        }    
    }
}