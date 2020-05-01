using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// activates a spriterenderer when the player is in legform
/// </summary>
public class TutorialBootHint : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Player player;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = Player.current;
    }
    
    void Update()
    {
        spriteRenderer.enabled = player.legForm;
    }
}
