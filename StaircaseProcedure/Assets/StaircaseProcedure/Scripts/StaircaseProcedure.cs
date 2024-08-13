using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.Collections.Concurrent;

//namespace
using Staircase;

public class StaircaseProcedure : MonoBehaviour
{
    public static string VERSION = "v17";
    public static StaircaseProcedure SP;                // one of the parallel SPs
    public static List<StaircaseProcedure> AllSPs;      // list of all parallel SPs
    public static NumberFormatInfo nfi = new NumberFormatInfo();

    /// <summary>
    /// Returns true if all the parallel staircases are finished and no trials remain in the experiment.
    /// </summary>
    public static bool AreAllSPsFinished()
    {
        bool allFinished = true;
        foreach(StaircaseProcedure sp in AllSPs)
        {
            allFinished &= sp.IsFinished();
        }
        return allFinished;
    }
    
    // Variables that can be modified in the inspector
    [Tooltip("The path to a directory, where the csv files are saved(e.g C:\\Users\\...\\Results). If the specified directory doesn't exist it will be automatically created.")]
    [SerializeField]
    private string resultsPath;
    [Tooltip("Set the path to your python executable (e.g. C:\\Users\\...\\Python37\\python.exe on Windows or /usr/local/bin/python3.7 on Mac")]
    [SerializeField]
    private string pythonPath = "";
    [Tooltip("If set to true, the procedure is plotted with python and the graph is shown in a separate window (default: true)")]
    [SerializeField]
    private bool livePlotter = true;
    [Tooltip("If set to true, Sequence 1 and Sequence 2 will be shown as subplots (default: false)")]
    [SerializeField]
    private bool showSubPlots = false;
    [Tooltip("By default, the python client script is called by the staircase script. Set to false if you want to run the client from your console (>> python client.py 127.0.0.1 65000 ) (default: true)")]
    [SerializeField]
    private bool startPythonAutomatically = true;
    [Tooltip("The specified character is used to separate values in csv files (default: ;)")]
    [SerializeField]
    private string delimiter = ";";
    [Tooltip("The socket is bound to the specified IP Address (default: 127.0.0.1)")]
    [SerializeField]
    private string IPAddress = "127.0.0.1";
    [Tooltip("The socket is bound to the specified port (default: 65000; use different port for each staircase if they run in parallel)")]
    [SerializeField]
    private int port = 65000;


    // Private variables
    private PythonProcess process;
    private ServerSocket server;
    private FileWriter writer;
    private ConcurrentQueue<TrialData> cq;
    private ProcedureLogic proc;
    private InitData init;
    private bool isConnected = false;


    /// <summary>
    /// Call this to setup a staircase that has been instantiated programatically.
    /// </summary>
    /// <param name="ResultsPath"></param>
    /// <param name="PythonPath"></param>
    /// <param name="LivePlotter"></param>
    /// <param name="ShowSubPlots"></param>
    /// <param name="StartPythonAutomatically"></param>
    /// <param name="Delimiter"></param>
    /// <param name="IPAddress"></param>
    /// <param name="PortOfFirstStaircase"></param>
    public void Create (
        string ResultsPath,
        string PythonPath,
        bool LivePlotter = true,
        bool ShowSubPlots = true,
        bool StartPythonAutomatically = true,
        string Delimiter = ";",
        string IPAddress = "127.0.0.1",
        int PortOfFirstStaircase = 65000
        )
    {
        this.resultsPath = ResultsPath;
        this.pythonPath = PythonPath;
        this.livePlotter = LivePlotter;
        this.showSubPlots = ShowSubPlots;
        this.startPythonAutomatically = StartPythonAutomatically;
        this.delimiter = Delimiter;
        this.IPAddress = IPAddress;
        this.port = PortOfFirstStaircase;
    }

    /// <summary>Initializes the Staircase Procedure and starts the python plotter</summary>
    /// <param name="minimumValue">  the *value corresponding to the minimum stimulus* as defined by your experiment   </param>
    /// <param name="maximumValue">  the *value corresponding to the maximum stimulus* as defined by your experiment  </param>
    /// <param name="numberOfSteps"> total number of steps from `minimum` to `maximum` at highest resolution (i.e., when steps of size 1 are used) (`minimum` will correspond to step `0`, `maximum` will correspond to step `numberOfSteps`) </param>
    /// <param name="startStepSequ1"> start position/step of Sequence 1 (integer between 0 and numberOfSteps)  </param>
    /// <param name="startStepSequ2"> start position/step  of Sequence 2 (integer between 0 and numberOfSteps) </param>
    /// <param name="stopAmount">  defines when the staircase procedure ends. If `stopCriterionReversals` is set to `true`: *each individual sequence* will continue until `stopAmount` many* reversals* have occurred *in that specific sequence* (i.e.both sequences will perform `stopAmount` many reversals *each*). If `stopCriterionReversals` is set to `false`: *each individual sequence* will continue until `stopAmount/2` many* trials* have occurred *in that specific sequence* (i.e.both sequences * together* will perform `stopAmount` many trials). </param>
    /// <param name="numberThresholdPoints"> when the staircase procedure is completed, a threshold will be computed for each sequence. The thresholds of sequence 1 and sequence 2 will be averaged and the result will be the final threshold value shown in the plot and saved to file. When computing the threshold for a sequence, the last `numberThresholdPoints` reversals of that sequence will be averaged.  </param>
    /// <param name="experimentName"> name of the experiment </param>
    /// <param name="conditionName"> name of the condition (use different names for each staircase if multiple staircases exist in parallel) </param>
    /// <param name="numberParticipant"> number of the participant </param>
    /// <param name="stepsUp">(default: 1) (increase it to realize a weighted up/down method) how many steps to increase the stimulus when answer was wrong/not detected (min. 1)</param>
    /// <param name="stepsDown">(default: 1) (use it to realize a weighted up/down method) how many steps to decrease the stimulus when answer was correct/detected (min. 1; usually kept at 1)</param>
    /// <param name="stepsUpStartEarly">(default: 1) (increase it to realize a "quick start") how many steps to increase the stimulus when answer was wrong/not detected (min. 1) before the first quickStartEarlyUntilReversals reversals have occurred</param>
    /// <param name="stepsDownStartEarly">(default: 1) (increase it to realize a "quick start") how many steps to decrease the stimulus when answer was correct/detected (min. 1) before the first quickStartEarlyUntilReversals reversals have occurred</param>
    /// <param name="quickStartEarlyUntilReversals">(default: 0 = "quick start" is off) (increase it to realize a "quick start") specifies the number of reversals that have to occur until the step size that is applied upwards switches from stepsUpStartEarly to stepsUpStartLate and the step size that is applied downwards changes from stepsDownStartEarly to stepsDownStartLate</param>
    /// <param name="stepsUpStartLate">(default: 1) (increase it to realize a "quick start" with two different step size patterns) how many steps to increase the stimulus when answer was wrong/not detected (min. 1) before the first quickStartLateUntilReversals reversals have occurred</param>
    /// <param name="stepsDownStartLate">(default: 1) (increase it to realize a "quick start" with two different step size patterns) how many steps to decrease the stimulus when answer was correct/detected (min. 1) before the first quickStartLateUntilReversals reversals have occurred</param>
    /// <param name="quickStartLateUntilReversals">(default: 0 = "quick start" is off) (increase it to realize a "quick start") specifies the number of reversals that have to occur until the step size that is applied upwards switches from stepsUpStartLate to stepsUp and the step size that is applied downwards changes from stepsDownStartLate to stepsDown</param>
    /// <param name="stopCriterionReversals"> (default: `true`) If `stopCriterionReversals` is set to `true`: *each individual sequence* will continue until `stopAmount` many* reversals* have occurred *in that specific sequence* (i.e.both sequences will perform `stopAmount` many reversals *each*). If `stopCriterionReversals` is set to `false`: *each individual sequence* will continue until `stopAmount/2` many* trials* have occurred *in that specific sequence* (i.e.both sequences * together* will perform `stopAmount` many trials). </param>
    /// <param name="strictLimits"> (default: `false`) This bool affects how cases are handled in which the user reports to notice a stimulus in a trial that presented the minimum stimulus (or reports not to notice a stimulus in a trial that presented the maximum stimulus). If `strictLimits` is set to `false` the staircase procedure will *virtually* continue beyond the minimum(maximum) but the minimum(maximum) stimulus will be returned as long as the* virtual staircase* is below the minimum(above the maximum) value. If `strictLimits` is set to `true` the staircase procedure will strictly stop to decrease(increase) at the minimum(maximum). Also in this case the minimum(maximum) stimulus will be returned.If in such a case the minimum (maximum) stimulus is not noticed (noticed) at some point, the stimulus will immediately increase again. </param>
    /// <param name="plotTitle"> title that is displayed above the chart (default: 'Staircase Results - {CONDITION} - Participant {X}') </param>
    public void Init(
        float minimumValue, 
        float maximumValue, 
        int numberOfSteps,
        int startStepSequ1, 
        int startStepSequ2,
        int stopAmount, 
        int numberThresholdPoints,
        string experimentName,
        string conditionName, 
        int numberParticipant,
        int stepsUp = 1,
        int stepsDown = 1,
        int stepsUpStartEarly = 1,
        int stepsDownStartEarly = 1,
        int quickStartEarlyUntilReversals = 0,
        int stepsUpStartLate = 1,
        int stepsDownStartLate = 1,
        int quickStartLateUntilReversals = 0,
        bool stopCriterionReversals = true, 
        bool strictLimits = false, 
        bool singleSequence = false,
        bool singleSequenceUp = false,
        string plotTitle = ""
        )
    {
        if (String.IsNullOrEmpty(plotTitle))
        {
            plotTitle = "Staircase Results - " + conditionName + " - Participant " + numberParticipant;
        }

        init = new InitData(
            minimumValue, 
            maximumValue, 
            numberOfSteps,
            startStepSequ1, 
            startStepSequ2, 
            stepsUp,
            stepsDown,
            stepsUpStartEarly, 
            stepsDownStartEarly,
            quickStartEarlyUntilReversals,
            stepsUpStartLate,
            stepsDownStartLate,
            quickStartLateUntilReversals,
            stopAmount, 
            numberThresholdPoints,
            resultsPath, 
            experimentName, 
            conditionName, 
            numberParticipant,
            stopCriterionReversals, 
            strictLimits,
            singleSequence,
            singleSequenceUp,
            plotTitle);

        proc = new ProcedureLogic(init);
        nfi.NumberDecimalSeparator = ".";

        // write init-data to csv file
        writer.InitWriter(experimentName, conditionName, numberParticipant, plotTitle);
        writer.WriteInitCSV(minimumValue, maximumValue, numberOfSteps, startStepSequ1, startStepSequ2, stopAmount, numberThresholdPoints, stopCriterionReversals, strictLimits, showSubPlots, stepsUp, stepsDown, stepsUpStartEarly, stepsDownStartEarly, quickStartEarlyUntilReversals, stepsUpStartLate, stepsDownStartLate, quickStartLateUntilReversals);

        isConnected = false;
        // start client as process and connect with server
        if (livePlotter)
        {
            if (startPythonAutomatically)
            {
                Debug.Log("Start Plot Process");
                StartProcess(Application.dataPath + "/StaircaseProcedure/PythonTools/liveplotter/client.py " + IPAddress + " " + port);
                Debug.Log("Started:" + Application.dataPath + "/StaircaseProcedure/PythonTools/liveplotter/client.py " + IPAddress + " " + port);
            }
            StartCoroutine("AcceptClientCoroutine");
        }
    }


    /// <summary>Returns the next stimulus intensity based on the stopCriterion </summary>
    public float GetNextStimulus()
    {
        float stimulus = proc.GetNextStimulus();
        Debug.Log("Stimulus: " + stimulus);
        return stimulus;
    }

    /// <summary> Changes the stimulus intensity of the current sequence according the given answer 'stimulusNoticed', then
    /// saves the data to a class TrialData and returns it </summary>
    /// <param name="stimulusNoticed">the answer from the participant</param>
    public TrialData TrialFinished(bool stimulusNoticed)
    {
        TrialData trialData = proc.TrialFinished(stimulusNoticed);
        writer.LogTrialData(trialData);
        if (livePlotter) cq.Enqueue(trialData);
        return trialData;
    }

    /// <summary> Calls the threshold calculation based on the stopCriterion </summary>
    public float GetThreshold()
    {
        float threshold = proc.GetThreshold();
        writer.LogThreshold(threshold);
        if (isConnected && livePlotter)
        {
            //finish plotting all remaining data points in the queue and then draw the final threshold
            TrialData data;
            while (cq.TryDequeue(out data))
            {
                server.SendTrialDataToClient(data);
            }
            server.SendThresholdToClient(threshold);
        }
        return threshold;
    }

    /// <summary> Returns true if the current trial is the last one </summary>
    public bool IsLastTrial()
    {
        return proc.IsLastTrial();
    }

    /// <summary>
    /// Returns true if the staircase is finished and no additional trials need to be completed.
    /// </summary>
    public bool IsFinished()
    {
        return proc.IsFinished();
    }

    /// <summary>
    /// Returns the name of this staircase's condition
    /// </summary>
    public string GetConditionName()
    {
        return init.conditionName;
    }

    /// <summary> Starts the python script automatically </summary>
    public void StartProcess(string args)
    {
        process.StartProcess(args);
    }

    public void Awake()
    {
        if(!(String.IsNullOrEmpty(pythonPath) 
            || String.IsNullOrEmpty(IPAddress)
            || port == 0
            || String.IsNullOrEmpty(delimiter)))
        {
            Debug.Log("Hello! This is Staircase Procedure Toolkit " + VERSION);
            if (SP == null)
            {
                SP = this;
                AllSPs = new List<StaircaseProcedure>();
                AllSPs.Add(this);
            }
            else
            {
                AllSPs.Add(this);
            }
            // DontDestroyOnLoad(this);

            if (String.IsNullOrEmpty(resultsPath))
            {
                resultsPath = Application.dataPath + "/Results";
            }

            if (livePlotter)
            {
                server = new ServerSocket();
                server.ExecuteServer(IPAddress, port);
                process = new PythonProcess(pythonPath);
                cq = new ConcurrentQueue<TrialData>();
            }
            writer = new FileWriter(resultsPath, delimiter);

            // Set DecimalSeparator to point
            CultureInfo customCulture = (CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
        } else
        {
            Debug.LogWarning("Attention! Awake() has not been executed as parameters were not yet initialized. Make sure to set the parameters with Create() and call Awake() after that.");
        }
    }

    void Start()
    {

    }


    void Update()
    {
        if (isConnected && livePlotter)
        {
            // check queue 
            TrialData data;
            if (cq.TryDequeue(out data))
            {
                server.SendTrialDataToClient(data);
            }
        }
    }

    void OnApplicationQuit()
    {
        if (livePlotter)
        {
            server.CloseConnection();
        }
    }



    /// <summary> Sends init parameters to client </summary>
    private void InitClient()
    {
        server.SendInitToClient(init.minimumValue.ToString(nfi), init.maximumValue.ToString(nfi), init.numberOfSteps, init.stopCriterionReversals, init.stopAmount, init.plotTitle, init.resultsPath, init.experimentName, init.conditionName, init.numberParticipant, showSubPlots);
    }

    public IEnumerator AcceptClientCoroutine()
    {
        while (!isConnected)
        {
            try
            {
                server.AcceptClient();
                InitClient();
                isConnected = true;
            }
            catch
            {
            }
            yield return null;
        }
    }
}
