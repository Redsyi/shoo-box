using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuggageJostle : MonoBehaviour, IKickable
{
    public BoxCollider fromLeftJostle;
    public BoxCollider fromRightJostle;
    public Vector3 move;
    public float speed;
    public float jostleDist;
    private bool isJostling;
    private float distMoved;
    private Vector3 currMove;


    public void OnKick(GameObject Kicker)
    {
        currMove = (fromLeftJostle.enabled ? move * -1 : move);
        isJostling = true;
        StartCoroutine(Jostle());
    }

    private IEnumerator Jostle()
    {
        while (distMoved < jostleDist) // Move forward
        {
            distMoved += speed * Time.fixedDeltaTime;
            yield return transform.parent.position += (speed * Time.fixedDeltaTime * currMove); // Assumes you want to move its parent
        }
        while (distMoved > 0) // Move back
        {
            distMoved -= speed * Time.fixedDeltaTime;
            yield return transform.parent.position -= (speed * Time.fixedDeltaTime * currMove); // Assumes you want to move its parent
        }
        isJostling = false;
    }

    public bool GetIsJostling()
    {
        return isJostling;
    }

    public void SwitchJostles()
    {
        fromLeftJostle.enabled = !fromLeftJostle.enabled;
        fromRightJostle.enabled = !fromRightJostle.enabled;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + move * jostleDist);
    }
}
