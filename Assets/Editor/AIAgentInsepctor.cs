using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(AIAgent))]
public class AIAgentInsepctor : Editor
{
    //List<AIInterest> summonInterests;

    public override void OnInspectorGUI()
    {
        AIAgent agent = (AIAgent)target;

        base.DrawDefaultInspector();

        if (agent.chaseBehavior == ChaseBehaviors.SEQUENCE)
        {
            agent.chaseOverrideSequence = (ScriptedSequence)EditorGUILayout.ObjectField(agent.chaseOverrideSequence, typeof(ScriptedSequence), true);
        }
        /*else if (agent.chaseBehavior == ChaseBehaviors.SUMMON)
        {
            serializedObject.Update();
            if (summonInterests == null)
                summonInterests = new List<AIInterest>(agent.chaseBroadcastInterests);
            GUILayout.Label("Summon interests:");
            for (int i = 0; i < summonInterests.Count; ++i)
            {
                EditorGUILayout.BeginHorizontal();
                AIInterest interest = (AIInterest)EditorGUILayout.EnumPopup(summonInterests[i]);//EditorGUILayout.EnumFlagsField(summonInterests[i]);
                summonInterests[i] = interest;
                agent.chaseBroadcastInterests[i] = interest;
                if (GUILayout.Button("-"))
                {
                    summonInterests.RemoveAt(i);
                    RemakeInterestsArray();
                }
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("+"))
            {
                summonInterests.Add(AIInterest.AIRPORT_CROWD);
                RemakeInterestsArray();
            }
            serializedObject.ApplyModifiedProperties();
        }*/

        GUILayout.Space(20);

        if (GUILayout.Button("Create Patrol Point"))
        {
            GameObject patrolPoint = PrefabUtility.InstantiatePrefab(agent.targetPrefab) as GameObject;
            patrolPoint.transform.position = agent.transform.position;
            patrolPoint.name = $"{agent.gameObject.name} patrol point {agent.patrolPoints.Length + 1}";
            EditorUtility.SetDirty(target);
            //Undo.RecordObject(target, "Set Patrol Point");
            ArrayUtility.Insert(ref agent.patrolPoints, agent.patrolPoints.Length, patrolPoint.transform);
        }
    }
    /*
    void RemakeInterestsArray()
    {
        AIAgent agent = (AIAgent)target;

        agent.chaseBroadcastInterests = new AIInterest[summonInterests.Count];
        for (int i = 0; i < summonInterests.Count; ++i)
        {
            agent.chaseBroadcastInterests[i] = summonInterests[i];
        }
    }*/
}
