using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//pretty much deprecated
public class AudioManager : MonoBehaviour
{
    public static NoiseIndicator noiseIndicatorPrefab;
    public NoiseIndicator initial;
    public static AudioManager instance;
    static List<AIAgent> registeredNPCs;


    //WWise variables
    const string stressRTPCName = "";       //Miki: replace this with the rtpc name for the stress scale
    const string runRTPCName = "";          //replace this with the rtpc name for the "run" scale
    const int stressRTPCMax = 100;          //replace this with the maximum value of the stress RTPC
    const int runsRTPCMax = 100;            //replace this with the maximum value of the "run" RTPC
    const float runFadeInTime = 0.5f;       //replace this with how long you want the "run" fade in to be
    const float runFadeOutTime = 0.5f;      //replace this with how long you want the "run" fade out to be
    const float stressFadeOutTime = 0.5f;   //replace this with how long you want the "stress" fade out to be when an npc gives up the chase for a player

    //state variables
    static float currStress;
    static float currRun;
    public static bool playerWasCaught;
    static float currInvestigationStress;

    /// <summary>
    /// Updates the actual RTPC values in WWise
    /// </summary>
    static void UpdateRTPCValues()
    {
        //Miki: uncomment the next two lines when you have everything you need plugged in
        //AkSoundEngine.SetRTPCValue(stressRTPCName, currStress * stressRTPCMax);
        //AkSoundEngine.SetRTPCValue(runRTPCName, currRun * runsRTPCMax);
    }

    /// <summary>
    /// registers an npc to be tracked by the audio manager
    /// </summary>
    public static void RegisterAgent(AIAgent agent)
    {
        if (registeredNPCs != null)
            registeredNPCs.Add(agent);
    }

    /// <summary>
    /// returns true if any registered npc is chasing the player
    /// </summary>
    static bool AgentIsChasing()
    {
        if (registeredNPCs != null)
        {
            foreach (AIAgent agent in registeredNPCs)
            {
                if (agent && agent.state == AIState.CHASE)
                    return true;
            }
        }
        return false;
    }
    
    /// <summary>
    /// returns the maximum player stress value among all registered npcs
    /// </summary>
    static float MaxAgentStressValue()
    {
        float maxSpotVal = 0f;
        if (registeredNPCs != null)
        {
            foreach (AIAgent agent in registeredNPCs)
            {
                if (agent)
                {
                    maxSpotVal = Mathf.Max(maxSpotVal, agent.spotProgress);
                    if (agent.investigatingPlayer)
                        currInvestigationStress = 1;
                }
            }
        }
        return maxSpotVal;
    }

    private void Update()
    {
        //ramp current run value up or down depending on if anything is chasing the player
        if (AgentIsChasing() && !playerWasCaught)
        {
            currRun = Mathf.Clamp01(currRun + Time.unscaledDeltaTime / runFadeInTime);
        } else
        {
            currRun = Mathf.Clamp01(currRun - Time.unscaledDeltaTime / runFadeInTime);
        }

        //update current stress value
        currStress = Mathf.Max(MaxAgentStressValue(), currInvestigationStress);

        UpdateRTPCValues();

        currInvestigationStress = Mathf.Clamp01(currInvestigationStress - Time.unscaledDeltaTime / stressFadeOutTime);
    }

































    private void OnDestroy()
    {
        currStress = 0f;
        currRun = 0f;
        UpdateRTPCValues();
    }

    private void Awake()
    {
        instance = this;
        playerWasCaught = false;
        registeredNPCs = new List<AIAgent>();
    }

    void Start()
    {
        noiseIndicatorPrefab = initial;
    }
    
    /// <summary>
    /// Make an AI-audible noise in the world
    /// </summary>
    /// <param name="pos">World position of the noise</param>
    /// <param name="radius">How far away AI will notice the noise, 0 for "silent"</param>
    /// <param name="clip">deprecated</param>
    /// <param name="volume">deprecated</param>
    public static void MakeNoise(Vector3 pos, float radius, AudioClip clip = null, float volume = 0)
    {
        NoiseIndicator noise = Instantiate(noiseIndicatorPrefab, pos, Quaternion.identity);
        noise.MakeNoise(radius, clip, volume);
    }
}
