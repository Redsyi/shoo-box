using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// building that can be destroyed in the city
/// </summary>
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
    private float limit = -11f;
   
    public void OnKick(GameObject kicker)
    {
        if (numKicks < positions.Length)
        {
            
            newY = positions[numKicks++];
            StartCoroutine(Destroy());
        }
            
    }

    /// <summary>
    /// destroys one segment of the building
    /// </summary>
    private IEnumerator Destroy()
    {
        CityDirector.current.SetIntensity(2);
        CityDirector.current.IncreaseIntensity(0.1f);
        numDestroys++;
        CameraScript.current.ShakeScreen(ShakeStrength.INTENSE, ShakeLength.MEDIUM);
        Player.ControllerRumble(RumbleStrength.INTENSE, 0.1f);
        dust.gameObject.SetActive(true);
        while(transform.position.y > newY)
        {
            transform.position -= new Vector3(0, speed * Time.deltaTime / numDestroys, 0);
            Player.ControllerRumble(RumbleStrength.MEDIUM, 0.1f);
            yield return null;
        }
        if (transform.position.y < limit)
            gameObject.SetActive(false);
        yield return new WaitForSeconds(1.5f);
        numDestroys--;
        if (numDestroys == 0)
        {
            dust.gameObject.SetActive(false);
        }

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
                Vector3 scale = new Vector3(filter.transform.localScale.x * buildingModel.transform.localScale.x, filter.transform.localScale.y * buildingModel.transform.localScale.y, filter.transform.localScale.z * buildingModel.transform.localScale.z);
                Gizmos.DrawWireMesh(filter.sharedMesh, filter.transform.position + new Vector3(0, positions[viewPosition]), buildingModel.transform.rotation, scale);
            }
        }
    }
}
