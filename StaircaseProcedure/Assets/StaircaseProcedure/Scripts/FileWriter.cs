using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;
using System.Diagnostics;


namespace Staircase{
    
public class FileWriter{

    private List<string> experimentList = new List<string>();
    private List<int> participantList = new List<int>();
    private string resultsPath;
    private string logPath;

    private string csvpath, plotTitle, delimiter;
    private List<string> headerInitList = new List<string> {"","minimumValue","maximumValue","numberOfSteps","startStepSequ1","startStepSequ2","stopAmount","numberThresholdPoints","stopCriterionReversals","strictLimits","showSubPlots"};
    private List<string> headerTrialList = new List<string> {"","indexTrial","sequence","stimulus","stimulusNoticed","indexSequence","reversal"};
    
    public FileWriter(string resultsPath, string delimiter){
        this.resultsPath = resultsPath;
        this.delimiter = delimiter;
    }

    public void InitWriter(string experimentName, string conditionName, int numberParticipant,string plotTitle){
        this.plotTitle = plotTitle;

        if(!experimentList.Contains(experimentName)) {
            experimentList.Add(experimentName);
            //create experiment directory
            string experimentDirectory = Path.Combine(resultsPath, experimentName);
            CreateDirectory(experimentDirectory);
            participantList = new List<int>();
        }
       
        string participantDirName = "P_" + experimentName + "_" + numberParticipant;

        if(!participantList.Contains(numberParticipant)){
            participantList.Add(numberParticipant);
            //create participant directory
            string participantDirectory = Path.Combine(resultsPath,experimentName, participantDirName);
            CreateDirectory(participantDirectory);
        }

        string filename = "P_" + experimentName + "_" + numberParticipant + "_" + conditionName + ".csv";
        csvpath = Path.Combine(resultsPath,experimentName,participantDirName, filename);

        WriteCharacters(csvpath, "Staircase Results",false);
        WriteCharacters(csvpath, "");
        WriteCharacters(csvpath, "" + delimiter +  "experimentName" + delimiter +  "conditionName" + delimiter +  "numberParticipant");
        WriteCharacters(csvpath, "name" + delimiter + experimentName + delimiter +  conditionName + delimiter +  numberParticipant.ToString());

    }

    public void CreateDirectory(string path){
        try {
            // Determine whether the directory exists.
            if (Directory.Exists(path)){ return; }
            // Try to create the directory.
            DirectoryInfo di = Directory.CreateDirectory(path);

        } catch (Exception e) {
            UnityEngine.Debug.Log("The process failed: " +  e);
        }
    }

    // Method for Async FileWrite
    public async void WriteCharacters (string path, string text, bool append = true) {
        using (FileStream stream = new FileStream (path, append ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
        using (StreamWriter sw = new StreamWriter (stream)) {
            await sw.WriteLineAsync (text);
        }
    }

    public void WriteInitCSV(params object[] list){
        WriteCharacters(csvpath, "");
        WriteCharacters(csvpath, string.Join(delimiter, headerInitList));
        WriteCharacters(csvpath, "init" + delimiter + string.Join(delimiter, list));
        WriteCharacters(csvpath, "");
        WriteCharacters(csvpath, string.Join(delimiter, headerTrialList));
    }

    public void LogTrialData(TrialData data) {
        WriteCharacters(csvpath, "trial" + delimiter + data.indexTrial + delimiter + data.sequence + delimiter + data.stimulus + delimiter+ data.stimulusNoticed + delimiter+ data.indexSequence + delimiter+ data.reversal);
    }

    public void LogThreshold (float threshold) {
        WriteCharacters(csvpath, "");
        WriteCharacters(csvpath, "threshold" + delimiter + threshold );
    }
}
}
