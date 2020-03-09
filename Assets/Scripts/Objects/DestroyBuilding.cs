using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBuilding : MonoBehaviour, IKickable
{
    public int maxKicks;
    public float toGround;
    public ParticleSystem dust;
    public float speed;

    private float originalY;
    private float newY;
    private int numKicks = 0;
    private int numDestroys = 0;
   
    public void OnKick(GameObject kicker)
    {
        numKicks++;
        if (numKicks < maxKicks)
        {
         
            newY -= (originalY / maxKicks);
            StartCoroutine(Destroy());
        }
        else if (numKicks == maxKicks)
        {
            newY = toGround;
            StartCoroutine(Destroy());
        }
    }

    private IEnumerator Destroy()
    {
        numDestroys++;
        CameraScript.current.ShakeScreen(ShakeStrength.INTENSE, ShakeLength.MEDIUM);
        dust.gameObject.SetActive(true);
        while(transform.position.y > newY)
        {
            transform.position -= new Vector3(0, speed * Time.deltaTime, 0);
            yield return null;
        }
        yield return new WaitForSeconds(1.5f);
        numDestroys--;
        if(numDestroys == 0)
            dust.gameObject.SetActive(false);

    }

    private void Start()
    {
        originalY = transform.position.y;
        newY = originalY;
    }

}
