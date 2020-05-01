using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButtonSound : MonoBehaviour
{
    // Start is called before the first frame update
    public void onClick()
    {
        AkSoundEngine.PostEvent("UI_Click", gameObject);
    }

   
}
