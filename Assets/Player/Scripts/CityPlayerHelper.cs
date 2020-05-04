using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityPlayerHelper : MonoBehaviour
{
    public float maxHP;
    public Image healthBar;
    public Image arrestFill;
    public GameObject arrestBar;
    public float arrestRate;
    public float arrestDecayRate;

    float HP;
    float arrestProgress;
    public static int numArresters;

    private void Start()
    {
        HP = maxHP;
        numArresters = 0;
    }
    
    /// <summary>
    /// Take damage, and reload the level with an appropriate message if dead
    /// </summary>
    public void TakeDamage(float amount, DamageSource source)
    {
        if (HP > 0)
        {
            HP -= amount;
            healthBar.fillAmount = Mathf.Clamp01(HP / maxHP);
            if (HP <= 0)
            {
                LevelBridge.Reload((source == DamageSource.HELICOPTER ? "Tip: A well-flung sandal can take down a helicopter" : "Tip: Boots are especially effective versus tanks"));
            }
        }
    }

    /// <summary>
    /// call police when changing forms
    /// </summary>
    public void OnChangeForm()
    {
        CityDirector.current.SetIntensity(1);
    }

    private void Update()
    {
        if (numArresters > 0)
        {
            arrestProgress = Mathf.Clamp01(arrestProgress + arrestRate * Time.deltaTime);
            if (arrestProgress >= 1f)
            {
                LevelBridge.Reload("Busted!");
            }
        }
        else
        {
            arrestProgress = Mathf.Clamp01(arrestProgress - arrestRate * Time.deltaTime);
        }

        if (arrestProgress <= 0f)
        {
            arrestBar.SetActive(false);
        } else
        {
            arrestBar.SetActive(true);
            arrestFill.fillAmount = arrestProgress;
        }
    }
}
