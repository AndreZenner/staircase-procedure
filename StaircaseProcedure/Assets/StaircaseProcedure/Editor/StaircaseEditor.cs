using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Diagnostics;

[CustomEditor(typeof(StaircaseProcedure))]
[CanEditMultipleObjects]
public class StaircaseEditor: Editor{

    SerializedProperty pathProperty;


    public override void OnInspectorGUI(){
        serializedObject.Update();

        base.OnInspectorGUI();
        if (GUILayout.Button("Open Data Analysis Application")){
            UnityEngine.Debug.Log("Open Application..");

            pathProperty = serializedObject.FindProperty("pythonPath");

            Process p = new System.Diagnostics.Process ();
            p.StartInfo.FileName = pathProperty.stringValue;
            p.StartInfo.Arguments = Application.dataPath + "/StaircaseProcedure/PythonTools/dataanalysis/dataanalysis.py ";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = false;
            p.Start ();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
