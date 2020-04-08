using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerDebugger : MonoBehaviour
{
    private VideoPlayer player;
    private void Start()
    {
        player = GetComponent<VideoPlayer>();
        
    }

    private void Update()
    {
        print(player.isPlaying);
    }
}
