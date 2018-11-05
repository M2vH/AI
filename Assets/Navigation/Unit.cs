using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour 
{
    public float m_Speed = 10;
    public LayerMask m_groundMask;

    private Camera m_cam;
    private List<Vector3> m_path;
    private int m_currentPathIndex;

	// Use this for initialization
	void Awake () 
	{
        m_cam = FindObjectOfType<Camera>();
	}
	
	// Update is called once per frame
	void Update () 
	{
        if (Input.GetMouseButtonDown(1))
        {
            Ray r = m_cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(r, out hit, float.MaxValue, m_groundMask))
            {
                m_path = NavGrid.Instance.GetPath(transform.position, hit.point);
                m_currentPathIndex = 0;
            }
        }

        if (m_path != null)
        {
            if (m_currentPathIndex >= m_path.Count)
            {
                m_path = null;
                return;
            }

            transform.Translate((m_path[m_currentPathIndex] - transform.position).normalized * Time.deltaTime * m_Speed , Space.World);
            if ((m_path[m_currentPathIndex] - transform.position).sqrMagnitude < 0.5f)
            {
                m_currentPathIndex++;
            }
        }
	}
}
