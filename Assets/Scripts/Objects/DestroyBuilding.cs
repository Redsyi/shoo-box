using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBuilding : MonoBehaviour, IKickable
{
    public float[] positions;
    public float toGround;
    public ParticleSystem dust;
    public float speed;

    [Header("Debug")]
    public int viewPosition = -1;
    public GameObject buildingModel;

    private float newY;
    private int numKicks = 0;
    private int numDestroys = 0;
   
    public void OnKick(GameObject kicker)
    {
        if (numKicks < positions.Length)
        {
         
            newY = positions[numKicks++];
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
            transform.position -= new Vector3(0, speed * Time.deltaTime / numDestroys, 0);
            yield return null;
        }
        yield return new WaitForSeconds(1.5f);
        numDestroys--;
        if(numDestroys == 0)
            dust.gameObject.SetActive(false);

    }

    private void Start()
    {
        newY = transform.position.y;
    }

    private void OnDrawGizmosSelected()
    {
        if (viewPosition >= 0 && viewPosition < positions.Length)
        {
            Gizmos.color = Color.red;
            foreach (MeshFilter filter in buildingModel.GetComponentsInChildren<MeshFilter>())
            {
                Gizmos.DrawWireMesh(filter.sharedMesh, filter.transform.position + new Vector3(0, positions[viewPosition]), Quaternion.identity, buildingModel.transform.localScale);
            }
        }
    }
}
