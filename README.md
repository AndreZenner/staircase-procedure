# Unity Staircase Procedure Toolkit

A toolkit that makes implementing psychophysical detection threshold experiments based on the adaptive staircase procedure, also known as weighted up/down method, in Unity super easy.  

<img src="/Resources/StaircasePlot.png"  width="700">

The toolkit has been developed by [André Zenner](https://umtl.cs.uni-saarland.de/people/andre-zenner.html), Kristin Ullmann, and Chiara Karr at the [Ubiquitous Media Technology Lab (UMTL)](https://umtl.cs.uni-saarland.de/), a Human-Computer Interaction (HCI) research group at Saarland University.  
It's made for realizing perception experiments with Unity and under the hood, it's powered by Python.
The toolkit has been repeatedly used to implement Virtual Reality experiments published in peer-reviewed journals and conferences (see [below](#References) for pointers to those papers).

Key features:  
- live plotting functionality  
- saves results to CSV files  
- super easy to interface with  
- support for the 1 up/1 down method
- support for the weighted up/down method  
- supports multiple staircases in parallel  
- supports a "quick start", where larger steps are taken until X (can be set freely) reversals have occurred (to speed up finding the relevant stimulus range early in the experiment)  

We open-source the toolkit here so that other researchers can make use of it for their perception experiments (see the [license](LICENSE.md) for details).  
Feel free to use the toolkit for your own experiments and projects (and if you find the time, let us know about it by sending an (informal) [email to André](mailto:andre.zenner@dfki.de)).


## Reference
Please reference the toolkit as follows (e.g. with the BibTex below).

> Zenner, A., & Ullmann, K.. (2021). AndreZenner/staircase-procedure: The Unity Staircase Procedure Toolkit. https://github.com/AndreZenner/staircase-procedure

```
@misc{Zenner:2021:UnityStaircase, 
 title={AndreZenner/staircase-procedure: The Unity Staircase Procedure Toolkit}, 
 url={https://github.com/AndreZenner/staircase-procedure}, 
 journal={GitHub}, 
 author={Zenner, André and Ullmann, Kristin}, 
 year={2021}, 
 month={Aug}
} 
```


## Overview

A toolkit for Unity implementing an interleaved staircase procedure to conduct psychophysical threshold experiments.
The results of the procedure are saved in csv files.

**Three additional tools are included (For more information see [Python Tools](#PythonTools)):**
- **Live Plotter:** The staircase procedure is visualized by a graph that is plotted in realtime with matplotlib. 
- **File Plotter:** The plot can be generated afterwards from the csv file.
- **Data Analysis (beta):** A Python application can be run to analyse the data and perform statistical tests.

**What is an interleaved staircase procedure?**
In experiments that investigate whether a participant is able to detect a stimulus, multiple stimuli are tested one after the other to determine the detection threshold. The interleaved staircase procedure provides an algorithm to estimate the threshold based on a set of such trials. For each trial, the algorithm randomly selects one of two sequences: Sequence 1 (which initially starts with a high stimulus intensity) and Sequence 2 (which initally starts with a low stimulus intensity). If the participant notices the stimulus, the intensity will be decreased in the next trial of that sequence, whereas if the participants doesn't notice the stimulus, the intensity will be increased. The point at which the answer is changing from 'noticed' to 'not noticed' or vice versa is called 'reversal point'. The threshold is then calculated by averaging the last reversal points.  

**What is a _weighted_ interleaved staircase procedure (aka. weighted up/down method)?**
When using the default staircase procedure (the 1 up/1 down method), the threshold that the sequence will converge to is the stimulus at which participants have a 50% chance of correctly detecting it (aka. the 50% threshold, $P_{50}$). To target other thresholds, the _weighted_ up/down method can be used, which means that the stimulus is increased more than it is decreased (i.e., the steps up are larger than the steps down). The following formula can be used in order to determine the required relative step sizes (i.e., suitable values for `stepsUp` and `stepsDown`) in order to approximate a target threshold $P_{target}$:  
$$P_{target} = {Steps_{up} \over Steps_{up} + Steps_{down}}$$
The formula above results in:
$${Steps_{down} \over Steps_{up}} = {1 - P_{target} \over P_{target}}$$

***Example: Targeting the 50%-correct threshold*** $P_{50}$  
$${Steps_{down} \over Steps_{up}} = {1 - P_{50} \over P_{50}} = {1 - 0.50 \over 0.50} = {1 \over 1}$$
This is the default setting of `stepsUp` and `stepsDown`.

***Example: Targeting the 75%-correct threshold*** $P_{75}$  
$${Steps_{down} \over Steps_{up}} = {1 - P_{75} \over P_{75}} = {1 - 0.75 \over 0.75} = {0.25 \over 0.75} = {1 \over 3}$$
Targeting this threshold (instead of the 50%-correct threshold) can be necessary, for example, when a symmetric question (e.g., "Was the stimulus left or right?") or a 2AFC question (e.g., "Which presentation showed the stimulus? Presentation 1 or 2?") is used and participants have a 50% guessing chance (i.e., the psychometric function goes from 50% to 100%, not from 0% to 100%).  

If you want to read more about this topic, we recommend the chapter on Adaptive Methods in this excellent book by Kingdom & Prins: 
*Psychophysics: A Practical Introduction (2016).Frederick A.A. Kingdom, Nicolaas Prins.*

<img src="/Resources/staircase_plot.svg"  height="420">


### Table of Contents
1. [Getting Started](#GettingStarted)
2. [How It Works](#HowItWorks)
3. [How Results are Saved](#HowResultsAreSaved)
4. [Python Tools](#PythonTools)

# <a name="GettingStarted"></a> 1. Getting started

## <a name="Requirements"></a> Requirements:

- `Unity (>= 2019.3.x)`
- `Python (>= 3.7)` - if prompted during Python installation on Windows: install `tkinter`!
- `numpy (>= 1.19.0)`
- `matplotlib (>= 3.3.0)`
- `scipy (>= 1.6.3)`
- `pandas (>= 1.1.4)`

## Installation & Setup:

* Drag the UnityPackage (UnityPackages > StaircaseProcedureV{number}.unitypackage) into your Unity project and import the files
* For each staircase that should run in parallel, one ```StaircaseProcedure``` script must be in the scene. Either drag the ```Staircase Procedure``` Prefab from the Prefab Folder into the scene (see example in ```StaircaseScene```) or add the script programatically to an existing GameObject (see ```MultipleStaircaseScene``` for an example).  
* Set your preferences in the inspector:
  * **Results Path (string):** The path to a directory, where the csv files are saved (e.g C:\\Users\\...\\Results). If the specified directory doesn't exist it will be automatically created. (See [Folder Structure](#FolderStructure))
  * **Python Path (string):** Set the path to your python executable (e.g. C:\\Users\\...\\Python37\\python.exe on Windows (cmd on Windows: ```where.exe python```))
  * **Delimiter (string):** The specified character is used to separate values in csv files (default: ";")    
  \
  Optional Settings for the Staircase LivePlotter (see [LivePlotter](#LivePlotter) for more information):
  * **Live Plotter (bool):** If set to true, the procedure is plotted with python and the graph is shown in a separate window (default: true) (see [LivePlotter](#LivePlotter))
  * **Show Subplots (bool):** If set to true, Sequence 1 and Sequence 2 will be shown as subplots (see image above) (default: false) 
  * **Start Python Automatically (bool):** By default, the python client script is called by the staircase script. Set to false if you want to run the client from your console (>> python client.py 127.0.0.1 65000 ) (default: true)
  * **IP Address (string):** The socket is bound to the specified IP Address (default: 127.0.0.1)
  * **Port (int):** The Python script (i.e., the live plotter) and the Unity program communicate via a socket, which is bound to the specified port (default: 65000). If you have multiple staircases in parallel, each must have its individual (and free) socket.  
  * If you want to make sure that everything is correctly set up you can add the script ```TestManager``` to your scene. See [Testing](#Testing) and the example scenes.
* In case anything doesn't work as expected (e.g. Python Window doesn't open) see section [Troubleshooting](#Troubleshooting)
<img src="/Resources/StaircaseProcedureInspector.png"  height="220">


# <a name="HowItWorks"></a> 2. How it works

## Steps of the procedure:

1. Call [`Init(...)`](#InitParams) to initialize a new procedure. (A separate python window with the graph should open.)
2. Repeat the following steps:
   (a) Call `GetNextStimulus()` to receive the next stimulus intensity (float). Present this stimulus to the participant. The stimulus value will also be logged to the console.
   (b) Call `TrialFinished(bool stimulusNoticed)` with the answer of the participant (true: stimulus was noticed, false: stimulus was not noticed). A [TrialData](#TrialData) class with the trial information will be returned for your information.
   (c) Call `isLastTrial()` to check if the last trial has just been completed. If `isLastTrial()` returns `true`: continue with step 3 (in this case, the staircase is completed). If `isLastTrial()` returns `false`: repeat again starting with (a).
3. Once `isLastTrial()` is `true` call `GetThreshold()` to receive the threshold value (float) and plot the threshold in the graph. This will also save the threshold to the csv file.
4. For a new procedure just start at step 1 again...

*Hint:* Convenient access to the instance (that was created first) of the ```StaircaseProcedure``` class is provided by: ```StaircaseProcedure.SP.{Function}```.  
*Hint 2:* Convenient access to all staircases in parallel is provided via the list ```StaircaseProcedure.AllSPs```.  
*Hint 3:* The static method ```StaircaseProcedure.AreAllSPsFinished()``` tells you when all parallel staircases are completed.  

## <a name="InitParams"></a> Initialization

### Required Parameters
- `float minimumValue` the *value corresponding to the minimum stimulus* as defined by your experiment. This is the value that is expected to be *not noticeable*, i.e. to result in *"I did not notice the stimulus"* responses from participants.  
- `float maximumValue` the *value corresponding to the maximum stimulus* as defined by your experiment. This is the value that is expected to be *noticeable*, i.e. to result in *"I did notice the stimulus"* responses from participants.
- `int numberOfSteps` total number of steps (sized 1) from `minimum` to `maximum` (`minimum` will correspond to step `0`, `maximum` will correspond to step `numberOfSteps`) -> [How It Works](#HowItWorks)   
- `int startStepSequ1` start position/step of Sequence 1 (integer between 0 and numberOfSteps)  
- `int startStepSequ2` start position/step  of Sequence 2 (integer between 0 and numberOfSteps)  
-  `int stopAmount` defines when the staircase procedure ends.   
If `stopCriterionReversals` is set to `true`: *each individual sequence* will continue until `stopAmount` many *reversals* have occurred *in that specific sequence* (i.e. both sequences will perform `stopAmount` many reversals *each*).   
If `stopCriterionReversals` is set to `false`: *each individual sequence* will continue until `stopAmount/2` many *trials* have occurred *in that specific sequence* (i.e. both sequences *together* will perform `stopAmount` many trials).
- `int numberThresholdPoints` when the staircase procedure is completed, a threshold will be computed for each sequence. The thresholds of sequence 1 and sequence 2 will be averaged and the result will be the final threshold value shown in the plot and saved to file. When computing the threshold for a sequence, the last `numberThresholdPoints` reversals of that sequence will be averaged. 
- `string experimentName` name of the experiment
- `string conditionName`  name of the condition
- `string numberParticipant` number of the participant 

### Optional Parameters   
- `string plotTitle` title that is displayed above the chart (default: 'Staircase Results - {CONDITION} - Participant {X}')
- `bool stopCriterionReversals` (default: `true`)   
If `stopCriterionReversals` is set to `true`: *each individual sequence* will continue until `stopAmount` many *reversals* have occurred *in that specific sequence* (i.e. both sequences will perform `stopAmount` many reversals *each*).   
If `stopCriterionReversals` is set to `false`: *each individual sequence* will continue until `stopAmount/2` many *trials* have occurred *in that specific sequence* (i.e. both sequences *together* will perform `stopAmount` many trials).  
- `bool strictLimits` (default: `false`) This bool affects how cases are handled in which the user reports to notice a stimulus in a trial that presented the minimum stimulus (or reports not to notice a stimulus in a trial that presented the maximum stimulus).   
If `strictLimits` is set to `false` the staircase procedure will *virtually* continue beyond the minimum (maximum) but the minimum (maximum) stimulus will be returned as long as the *virtual staircase* is below the minimum (above the maximum) value.   
If `strictLimits` is set to `true` the staircase procedure will strictly stop to decrease (increase) at the minimum (maximum). Also in this case the minimum (maximum) stimulus will be returned. If in such a case the minimum (maximum) stimulus is not noticed (noticed) at some point, the stimulus will immediately increase again.
- `int stepsUp` (default: `1`) (increase it to realize a weighted up/down method) how many steps to increase the stimulus when answer was wrong/not detected (min. 1)
- `int stepsDown` (default: `1`) (use it to realize a weighted up/down method) how many steps to decrease the stimulus when answer was correct/detected (min. 1; usually kept at 1)
- `int stepsUpStartEarly` (default: `1`) (increase it to realize a "quick start") how many steps to increase the stimulus when answer was wrong/not detected (min. 1) before the first `quickStartEarlyUntilReversals` reversals have occurred
-  `int stepsDownStartEarly` (default: `1`) (increase it to realize a "quick start") how many steps to decrease the stimulus when answer was correct/detected (min. 1) before the first `quickStartEarlyUntilReversals` reversals have occurred
-  `int quickStartEarlyUntilReversals` (default: `0` = "quick start" is off) (increase it to realize a "quick start") specifies the number of reversals that have to occur until the step size that is applied upwards switches from `stepsUpStartEarly` to `stepsUpStartLate` and the step size that is applied downwards changes from `stepsDownStartEarly` to `stepsDownStartLate`
-  `int stepsUpStartLate` (default: `1`) (increase it to realize a "quick start" with an additional, intermediate step size) how many steps to increase the stimulus when answer was wrong/not detected (min. 1) after `quickStartEarlyUntilReversals` and before `quickStartLateUntilReversals` reversals have occurred
-  `int stepsDownStartLate` (default: `1`) (increase it to realize a "quick start" with an additional, intermediate step size) how many steps to decrease the stimulus when answer was correct/detected (min. 1) after `quickStartEarlyUntilReversals` and before `quickStartLateUntilReversals` reversals have occurred
-  `int quickStartLateUntilReversals` (default: `0` = "quick start" is off) (increase it to realize a "quick start" with an additional, intermediate step size) specifies the number of reversals that have to occur until the step size that is applied upwards switches from `stepsUpStartLate` to `stepsUp` and the step size that is applied downwards changes from `stepsDownStartLate` to `stepsDown`

### Example 1:
You can call the function in any of your Unity C# scripts:

```csharp
StaircaseProcedure.SP.Init(minimumValue: 0.0f, maximumValue: 1.0f, numberOfSteps: 5,
    startStepSequ1: 1, startStepSequ2: 5,
    stopAmount: 4, numberThresholdPoints: 3, experimentName: "VR_Experiment",
    conditionName: "baseline", numberParticipant: 3
    );
```

<img src="/Resources/example_staircase.png"  width="700">

### Example 2:
An example for a staircase with 3 different step sizes, which are configured through the "quick start" parameters:

<img src="/Resources/3StepSizesExampleAnnotated.png"  width="700">

## <a name="TrialData"></a> TrialData class:

When a trial finishes, `TrialFinished()` will return the following information about the trial that just finished:

`int sequence` the sequence number which is either 1 or 2  
`int indexTrial` the index of the trial (total index)   
`int indexSequence` the index of the sequence (sequence index)     
`float stimulus` the stimulus intensity  
`bool stimulusNoticed` the answer of the participant whether they noticed the stimulus  
`bool reversal` whether it was a reversal point 

(import the `Staircase` namespace to use the `TrialData` type by writing `using Staircase;` in your scripts)

## <a name="Testing"></a> Testing:
If you want to test the StaircaseProcedure you can add the script "TestManager.cs" to your scene.
Just select the keys you want to use for testing and fill out the init parameter fields.   
For an example of using multiple staircase procedures in parallel, see the second example scene.  

<img src="/Resources/TestManager.png"  height="320">


# <a name="HowResultsAreSaved"></a> 3. How Results are Saved

## CSV File with Results:
For each procedure, a csv file is generated to save the experiment data. The csv file looks like this:

<img src="/Resources/CSV_Excel.png"  height="280">

## <a name="FolderStructure"></a> Folder Structure:

The StaircaseProcedure automatically generates the following folder structure to save the csv files:   

  `{Results}` **Main Directory.** The name of the directory. 'ResultsPath' is the path to this directory.   
  `{experimentName}` **Experiment Directory.** The name of the experiment that you passed as parameter for "experimentName" in the Init method.   
  `P_{experimentName}_0` **Participant Directory.** The title is generated automatically for each participant: P_{experimentName}_ {numberOfParticipant}.   
  `P_{experimentName}_0_{conditionName}.csv` **CSV File.** The title is generated automatically for each condition:   P_{experimentName}_ {numberOfParticipant}_ {conditionName}.csv.   

  ```
  {Results}
  │
  └───{experimentName}
  │   │
  │   └───P_{experimentName}_0
  │   │   │   P_{experimentName}_0_{conditionName}.csv
  │   │   │   P_{experimentName}_0_{conditionName}.csv
  │   │   │   ...
  │   │   
  │   └───P_{experimentName}_1
  │       │   P_{experimentName}_1_{conditionName}.csv
  │       │   P_{experimentName}_1_{conditionName}.csv
  │       │   ...
  │   
  └───{experimentName}
  │   │
  │   └───P_{experimentName}_0
  │   │   │   P_{experimentName}_0_{conditionName}.csv
  │   │   │   P_{experimentName}_0_{conditionName}.csv
  │   │   │   ...
  │   │ ...
  │ ...
  ```


# <a name="PythonTools"></a> 4. Python Tools:

## <a name="LivePlotter"></a> Live Plotter:
The Live Plotter is the main feature of the StaircaseProcedure. It creates a plot in realtime based on the participants answers.
The data is send via sockets from the server (Unity) to the client (Python). Matplotlib is used to create the GUI for the plot.

<img src="/Resources/StaircasePlot.png"  height="220">

## File Plotter:
In case you want to plot the data afterwards there is an additional script to create the plot from the csv file.
In your console go to the `PythonTools/FilePlotter` folder in your Unity project and run (with Python 3):
```
python plot_from_file.py {delimiter} {csv-filepath}  (optional: {save-svg-path}) (optional: {plot-width-in-inches}) (optional: {plot-height-in-inches})
```

For example, to create a 8x4 inches plot, you would call:
```
python plot_from_file.py ";" "C:\Users\...\P_ExperimentName_0_ConditionName.csv" "C:\Users\...\P_ExperimentName_0_ConditionName.svg" 8 4
```

## Data Analysis Tool (beta):
Once you finished your experiments, you can open the data analysis tool to analyse the results and perform statistical tests. You can use the button `Open Data Analysis Application` in the Unity Inspector, or run the script from your console.

To open it in the console go to `PythonTools/dataanalysis` and execute:
> $ python3 dataanalysis.py

Once the Data Analysis window is open, click on "Open Experiment Directory" and select the `{Results Path}/{experimentName}` folder.
After that, the results of all participants will be read and plotted.
The average thresholds of the different conditions will be checked for normality (Shapiro-Wilk test).
Moreover, a non-parametric analysis will be performed to check if the conditions resulted in significantly different thresholds.
Enjoy! ;)

<img src="/Resources/DataAnalysisTool.png"  height="320">

# <a name="Troubleshooting"></a> Troubleshooting

**Common Python Issues:**
- Check the paths and avoid any spaces in the folder and file names!
- Make sure the "Python Path" variable in the `StaircaseProcedure` script points to the `python.exe` file.
- Make sure the paths in the "Python Path" variable and the "Results Path" variable both have escaped `\` characters (i.e., use `\\`, for example: `C:\\folder\\subfolder`).
- Make sure that all additional libraries are installed correctly (e.g. `pip3 install ...`) (see [Requirements](#Requirements)).
- `Matplotlib` requires a module called `tkinter`, which is sometimes missing (especially on Windows).  If it is missing, try to reinstall python and pay attention to select "install tkinter" during the install process. You can test whether it is installed by running python and try the following:
 ```
>>> import tkinter
>>> tkinter.TkVersion
 ```

- If you want to read error messages in order to find errors that occur when the live plotter is used, you can start the live plotter manually from the console (so that error prints can be read in the console). To do so, go to folder `PythonTools/liveplotter` and run the python client:
  > $ python3 client.py 127.0.0.1 65000

**Socket Errors:**
- If you get a blank (white) window when calling ```Init()``` (instead of a plot of the coordinate system) or a ```System.Net.Sockets.SocketException (0x80004005)``` => Probably a Python process is still running. Go to the TaskManager and terminate the corresponding (or best: all existing) Python processes. Normally each socket address (protocol, network address or port) may only be used once at a time and a old process is still blocking it. Alternatively, also another (non-Python) process might be blocking the port. In this case, try another port number.  


# Releases
To use the toolkit, just download the `.unitypackage` file with the highest version number in the folder [`UnityPackages`](UnityPackages/) and import it into your Unity project.

# <a name="References"></a> Experiments that used the toolkit
The experiments reported in the following papers have used the toolkit for implementing the staircase method:
- Myung Jin Kim, Eyal Ofek, Michel Pahud, Mike J Sinclair, and Andrea Bianchi. 2024. Big or Small, It’s All in Your Head: Visuo-Haptic Illusion of Size-Change Using Finger-Repositioning. In Proceedings of the CHI Conference on Human Factors in Computing Systems (CHI '24). Association for Computing Machinery, New York, NY, USA, Article 751, 1–15. https://doi.org/10.1145/3613904.3642254
- André Zenner, Chiara Karr, Martin Feick, Oscar Ariza, and Antonio Krüger. 2024. Beyond the Blink: Investigating Combined Saccadic &amp; Blink-Suppressed Hand Redirection in Virtual Reality. In Proceedings of the CHI Conference on Human Factors in Computing Systems (CHI '24). Association for Computing Machinery, New York, NY, USA, Article 750, 1–14. https://doi.org/10.1145/3613904.3642073
- Martin Feick, André Zenner, Simon Seibert, Anthony Tang, and Antonio Krüger. 2024. The Impact of Avatar Completeness on Embodiment and the Detectability of Hand Redirection in Virtual Reality. In Proceedings of the CHI Conference on Human Factors in Computing Systems (CHI '24). Association for Computing Machinery, New York, NY, USA, Article 548, 1–9. https://doi.org/10.1145/3613904.3641933
- André Zenner, Chiara Karr, Martin Feick, Oscar Ariza, and Antonio Krüger. 2023. The Detectability of Saccadic Hand Offset in Virtual Reality. In Proceedings of the 29th ACM Symposium on Virtual Reality Software and Technology (VRST '23). Association for Computing Machinery, New York, NY, USA, Article 82, 1–2. https://doi.org/10.1145/3611659.3617223
- A. Zenner, K. P. Regitz and A. Krüger, "Blink-Suppressed Hand Redirection," 2021 IEEE Virtual Reality and 3D User Interfaces (VR), 2021, pp. 75-84, doi: 10.1109/VR50410.2021.00028.
- A. Zenner, K. Ullmann and A. Krüger, "Combining Dynamic Passive Haptics and Haptic Retargeting for Enhanced Haptic Feedback in Virtual Reality," in IEEE Transactions on Visualization and Computer Graphics, vol. 27, no. 5, pp. 2627-2637, May 2021, doi: 10.1109/TVCG.2021.3067777.
- Martin Feick, Niko Kleer, André Zenner, Anthony Tang, and Antonio Krüger. 2021. Visuo-haptic Illusions for Linear Translation and Stretching using Physical Proxies in Virtual Reality. In <i>Proceedings of the 2021 CHI Conference on Human Factors in Computing Systems</i> (<i>CHI '21</i>). Association for Computing Machinery, New York, NY, USA, Article 220, 1–13. DOI: https://doi.org/10.1145/3411764.3445456

# <a name="Licenese"></a> License
Before use, please see the [LICENSE](LICENSE.md) for copyright and license details.

This work was supported by the [Deutsches Forschungszentrum für Künstliche Intelligenz GmbH](https://www.dfki.de/) (DFKI; German Research Center for Artificial Intelligence) and [Saarland University](https://www.uni-saarland.de/).
<p><img src="/Resources/dfki-logo.jpg" alt="DFKI Logo" width="250"></p>
<p><img src="/Resources/uds-logo.png" alt="Saarland University Logo" width="250"></p>
