using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AIAgent))]
public class AIAgentInsepctor : Editor
{
    public override void OnInspectorGUI()
    {
        AIAgent agent = (AIAgent)target;

        base.DrawDefaultInspector();
        if (GUILayout.Button("Create Patrol Point"))
        {
            GameObject patrolPoint = PrefabUtility.InstantiatePrefab(agent.targetPrefab) as GameObject;
            patrolPoint.transform.position = agent.transform.position;
            patrolPoint.name = $"{agent.gameObject.name} patrol point";
            EditorUtility.SetDirty(target);
            //Undo.RecordObject(target, "Set Patrol Point");
            agent.patrolPoint = patrolPoint.transform;
        }
    }
}
