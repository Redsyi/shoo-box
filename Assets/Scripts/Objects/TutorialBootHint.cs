﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBootHint : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Player player;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = FindObjectOfType<Player>();
    }
    
    void Update()
    {
        spriteRenderer.enabled = player.legForm;
    }
}