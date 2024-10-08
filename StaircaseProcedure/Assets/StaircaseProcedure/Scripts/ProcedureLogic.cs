using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Staircase
{

    public class ProcedureLogic
    {

        private bool isFinished;
        private int indexTrial;
        private Sequence currSequence, sequenceOne, sequenceTwo;
        private List<TrialData> trialDataList;
        private List<int> sequenceList;
        private InitData init;
        private static System.Random ran = new System.Random();

        public ProcedureLogic(InitData init)
        {

            this.init = init;

            isFinished = false;
            indexTrial = 0;
            float absoluteStartStimulusLeft = Mathf.Lerp(init.minimumValue, init.maximumValue, (float)init.startStepSequ1 / (float)init.numberOfSteps);
            float absoluteStartStimulusRight = Mathf.Lerp(init.minimumValue, init.maximumValue, (float)init.startStepSequ2 / (float)init.numberOfSteps);
            sequenceOne = new Sequence(absoluteStartStimulusLeft, 1, Dir.UP, init.strictLimits, init.numberOfSteps, init.stepsUp, init.stepsDown, init.stepsUpStartEarly, init.stepsDownStartEarly, init.quickStartEarlyUntilReversals, init.stepsUpStartLate, init.stepsDownStartLate, init.quickStartLateUntilReversals, init.minimumValue, init.maximumValue, init.startStepSequ1);
            sequenceTwo = new Sequence(absoluteStartStimulusRight, 2, Dir.DOWN, init.strictLimits, init.numberOfSteps, init.stepsUp, init.stepsDown, init.stepsUpStartEarly, init.stepsDownStartEarly, init.quickStartEarlyUntilReversals, init.stepsUpStartLate, init.stepsDownStartLate, init.quickStartLateUntilReversals, init.minimumValue, init.maximumValue, init.startStepSequ2);

            if (init.singleSequence && init.stopCriterionReversals)
            {
                //artifically "end" one sequence to only work with the other sequence from now on
                for (int i = 0; i < init.stopAmount; i++)
                    (init.singleSequenceUp ? sequenceTwo : sequenceOne).reversals.Add(float.PositiveInfinity);
            }
            
            trialDataList = new List<TrialData>();

            // if number of trials is your stop criterion, a list will be created with 1s and 2s (sequence 1 or 2) to ensure random sequence selection with equal partial amounts
            if (!init.stopCriterionReversals) InitSequenceList();

        }

        /// <summary>Initializes a List of size 'stopAmount' with 1 and 2 like [1,1,1,1,...,2,2,2,2...] </summary>
        public void InitSequenceList()
        {
            if (init.singleSequence)
            {
                List<int> list1 = init.singleSequenceUp ? Enumerable.Repeat(1, init.stopAmount).ToList() : new List<int>();
                List<int> list2 = init.singleSequenceUp ? new List<int>() : Enumerable.Repeat(2, (int)Math.Ceiling((float)init.stopAmount)).ToList();
                sequenceList = list1.Concat(list2).ToList();
            }
            else
            {
                List<int> list1 = Enumerable.Repeat(1, init.stopAmount / 2).ToList();
                List<int> list2 = Enumerable.Repeat(2, (int)Math.Ceiling((float)init.stopAmount / 2)).ToList();
                sequenceList = list1.Concat(list2).ToList();
            }
        }

        /// <summary>Returns the next stimulus intensity based on the stopCriterion </summary>
        public float GetNextStimulus()
        {
            if (init.stopCriterionReversals)
            {
                return GetNextStimulusReversals();
            }
            else
            {
                return GetNextStimulusTrials();
            }
        }

        /// <summary> Changes the stimulus intensity of the current sequence according the given answer 'stimulusNoticed', then
        /// saves the data to a class TrialData and returns it </summary>
        /// <param name="stimulusNoticed">the answer from the participant</param>
        public TrialData TrialFinished(bool stimulusNoticed)
        {
            bool reversal = false;
            float stimulus = currSequence.stimulus;

            if (stimulusNoticed)
            {
                if (currSequence.direction == Dir.UP)
                {
                    if (currSequence.indexSequence == 1)
                    {
                        Debug.LogWarning("The first stimulus of the sequence was 'noticed' but should be 'not noticed'");
                    }
                    else
                    {
                        reversal = true;
                        currSequence.reversals.Add(stimulus);
                    }
                    currSequence.direction = Dir.DOWN;
                }
                currSequence.DecCounter();
            }
            else
            {
                if (currSequence.direction == Dir.DOWN)
                {
                    if (currSequence.indexSequence == 1)
                    {
                        Debug.LogWarning("The first stimulus of the sequence was 'not noticed' but should be 'noticed'");
                    }
                    else
                    {
                        reversal = true;
                        currSequence.reversals.Add(stimulus);
                    }
                    currSequence.direction = Dir.UP;
                }
                currSequence.IncCounter();
            }
            TrialData data = new TrialData(currSequence.numSequence, indexTrial, currSequence.indexSequence, stimulus, stimulusNoticed, reversal);
            trialDataList.Add(data);
            currSequence.indexSequence += 1;

            if (IsLastTrial())
                isFinished = true;

            return data;
        }

        /// <summary> Calls the threshold calculation </summary>
        public float GetThreshold()
        {
            if(init.singleSequence)
                return ThresholdCalculationClass.GetThresholdReversals(init.singleSequenceUp ? sequenceOne : null, init.singleSequenceUp ? null : sequenceTwo, init.numberThresholdPoints);
            else
                return ThresholdCalculationClass.GetThresholdReversals(sequenceOne, sequenceTwo, init.numberThresholdPoints);
        }

        /// <summary> One of the two sequences is randomly selected with a probability of 0.5. If one of the sequences has reached the stop amount of reversal points
        /// only the other sequence will be chosen. </summary>
        public float GetNextStimulusReversals()
        {
            if (sequenceOne.reversals.Count == init.stopAmount && sequenceTwo.reversals.Count == init.stopAmount)
            {
                Debug.LogWarning("StopAmount for Reversals reached.");
            }
            indexTrial++;
            if (sequenceOne.reversals.Count == init.stopAmount)
            {
                currSequence = sequenceTwo;
            }
            else if (sequenceTwo.reversals.Count == init.stopAmount)
            {
                currSequence = sequenceOne;
            }
            else
            {
                if (ran.Next(0, 2) == 0)
                {
                    currSequence = sequenceOne;
                }
                else
                {
                    currSequence = sequenceTwo;
                }
            }
            return currSequence.stimulus;
        }

        /// <summary>One of the two sequences is randomly selected with probability of 0.5 from the sequenceList </summary>
        public float GetNextStimulusTrials()
        {
            if (sequenceList.Count == 0)
            {
                Debug.LogWarning("GetNextStimulus: Stimulus list is empty.");
                return 0;
            }
            indexTrial++;
            int index = ran.Next(0, sequenceList.Count);
            int numNextSequence = sequenceList[index];
            sequenceList.RemoveAt(index);

            if (numNextSequence == 1)
            {
                currSequence = sequenceOne;
            }
            else
            {
                currSequence = sequenceTwo;
            }
            return currSequence.stimulus;
        }

        /// <summary> Returns true if the current trial is the last one </summary>
        public bool IsLastTrial()
        {
            if (init.stopCriterionReversals)
            {
                return (sequenceOne.reversals.Count == init.stopAmount && sequenceTwo.reversals.Count == init.stopAmount);
            }
            else
            {
                return indexTrial == init.stopAmount;
            }
        }

        /// <summary>
        /// Returns true if the staircase is finished and no additional trials need to be completed.
        /// </summary>
        public bool IsFinished()
        {
            return isFinished;
        }
    }

    public enum Dir
    {
        DOWN,
        UP
    }
}

