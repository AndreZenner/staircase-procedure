
using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour
{

    System.Random rng = new System.Random();

    int counter = 0;
    int number_participant = 0;
    string[] conditions = { "conditionA", "conditionB", "conditionC", "conditionD" };

    // Start is called before the first frame update
    void Start()
    {
        StaircaseProcedure.SP.Init(minimumValue: 0.0f, maximumValue: 1.0f, numberOfSteps: 5, startStepSequ1: 1, startStepSequ2: 5, stopAmount: 4, numberThresholdPoints: 3, strictLimits: true, experimentName: "VR_Experiment", conditionName: "conditionA", numberParticipant: 0);
        StartCoroutine(ExampleCoroutine());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator ExampleCoroutine()
    {
        counter += 1;

        while (!StaircaseProcedure.SP.IsLastTrial())
        {
            float stim = StaircaseProcedure.SP.GetNextStimulus();

            bool randomBool = rng.Next(0, 2) > 0;
            StaircaseProcedure.SP.TrialFinished(randomBool);
            yield return new WaitForSeconds(0.5f);
        }
        float t = StaircaseProcedure.SP.GetThreshold();

        int pos = counter % 4;
        Debug.Log(counter);

        if (pos == 0)
        {
            number_participant += 1;
        }

        if (number_participant == 5) yield return 0;

        StaircaseProcedure.SP.Init(minimumValue: 0.0f, maximumValue: 1.0f, numberOfSteps: 5, startStepSequ1: 1, startStepSequ2: 5, stopAmount: 4, numberThresholdPoints: 3, strictLimits: true, experimentName: "VR_Experiment", conditionName: conditions[pos], numberParticipant: number_participant);
        StartCoroutine(ExampleCoroutine());

    }

}
