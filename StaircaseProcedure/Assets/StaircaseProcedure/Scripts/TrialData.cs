using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


namespace Staircase{
    
/// <summary> Class to store information about the data of a trial </summary>
public class TrialData{
    public int sequence { get; }
    public int indexTrial { get; }
    public int indexSequence { get; }
    public float stimulus { get; }
    public bool stimulusNoticed { get; }
    public bool reversal { get; }

    public TrialData(int sequence, int indexTrial, int indexSequence, float stimulus, bool stimulusNoticed, bool reversal){
        this.sequence = sequence;
        this.indexTrial = indexTrial;
        this.indexSequence = indexSequence;
        this.stimulus = stimulus;
        this.stimulusNoticed = stimulusNoticed;
        this.reversal = reversal;
    }

}
}