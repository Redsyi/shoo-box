using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowItem : MonoBehaviour
{
   public GameObject objectToThrow;
   public float speed;
   //private Vector3 target;
   
     
    public void Throw(Transform target)
    {
        
        //myObject = objectToThrow;
        StartCoroutine(Throwing(target.position));
    }

    private IEnumerator Throwing(Vector3 position)
    {
        //yield return new WaitForSeconds(2.0f);
        Vector3 target = position + new Vector3(0, 2, 0);
        GameObject myObject = Instantiate(objectToThrow, transform.parent.position, Quaternion.identity);
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

    }
}
