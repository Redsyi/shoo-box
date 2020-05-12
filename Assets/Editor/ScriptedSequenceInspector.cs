using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ScriptedSequence))]
public class ScriptedSequenceInspector : Editor
{
    public override void OnInspectorGUI()
    {
        ScriptedSequence sequence = (ScriptedSequence)target;

        base.DrawDefaultInspector();
        if (GUILayout.Button("Set sequence locations to current position"))
        {
            foreach (ScriptedSequence.ScriptedStep step in sequence.sequence)
            {
                step.position = sequence.transform.position;
            }
            EditorUtility.SetDirty(target);
        }
    }
}
