using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseDoorsBehindMe : MonoBehaviour
{

    public Animator animator;
    public string trigger;
    public string startTrigger;
    // Start is called before the first frame update
    void Start()
    {
        animator.SetTrigger(startTrigger);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        animator.SetTrigger(trigger);
        Destroy(gameObject);
    }
}
