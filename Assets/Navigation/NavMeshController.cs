using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class NavMeshController : MonoBehaviour 
{
    public LayerMask m_groundMask;

    private Camera m_cam;
    private NavMeshAgent m_agent;

    // Use this for initialization
    void Awake () 
	{
        m_agent = GetComponent<NavMeshAgent>();
        m_cam = FindObjectOfType<Camera>();
	}
	
	// Update is called once per frame
	void Update () 
	{
        if (Input.GetMouseButtonDown(0))
        {
            Ray r = m_cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(r, out hit, float.MaxValue, m_groundMask))
            {
                m_agent.SetDestination(hit.point);
            }
        }
    }
}
