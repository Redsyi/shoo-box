using UnityEngine;
using UnityEngine.AI;
using kTools.Decals;

public class ClickPlayerMovement : MonoBehaviour
{

    private NavMeshAgent agent;
    [SerializeField] private LayerMask movableLayer;
    [SerializeField] private DecalData onClickDecal;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(myRay, out hitInfo, 100, movableLayer))
            {
                agent.SetDestination(hitInfo.point);
                DecalSystem.GetDecal(onClickDecal, hitInfo.point, Vector3.down, Vector2.one);
            }
        }
    }
}
