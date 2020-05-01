using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class that is attached to npcs and allows them to throw shit at the player
/// </summary>
public class ThrowItem : MonoBehaviour
{
    public ThrownItem objectToThrow;
    public float speed;
    public float spin;
   //private Vector3 target;
   
     
    public void Throw(Vector3 target)
    {

        //myObject = objectToThrow;
        //StartCoroutine(Throwing(target.position));
        ThrownItem item = Instantiate(objectToThrow, transform.parent.position, transform.parent.rotation);
        item.rigidbody.AddForce((target - transform.parent.position).normalized * speed);
        item.rigidbody.AddRelativeTorque(new Vector3(spin, 0));
        item.thrower = gameObject;
    }

    /*
    private IEnumerator Throwing(Vector3 position)
    {
        //yield return new WaitForSeconds(2.0f);
        Vector3 target = position + new Vector3(0, 2, 0);
        GameObject myObject = Instantiate(objectToThrow, transform.parent.position, Quaternion.identity).gameObject;
        while ((myObject.transform.position - target).sqrMagnitude > 0.1f)
        {
            //print("Target's position: " + target);
            //print("Object's position: " + myObject.transform.position);
            myObject.transform.position = Vector3.MoveTowards(myObject.transform.position, target, speed * Time.deltaTime);
            yield return null;
        }
        //Debug.Log("Hit player");
        if(myObject)
            Destroy(myObject);

    }*/
}
