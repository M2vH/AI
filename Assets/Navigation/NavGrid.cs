using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class NavGrid : MonoBehaviour 
{
    public static NavGrid Instance;
    public bool m_Debug = false;
    public LayerMask m_groundAndObstaclesMask;
    NavGridPoint[,] m_gridPoints;
    [HideInInspector]
    public List<Gras> m_allGras = new List<Gras>();
    [HideInInspector]
    public List<CowController> m_allCows = new List<CowController>();

	// Use this for initialization
	void Awake () 
	{
        Instance = this;
        GenerateGrid();
        m_allGras.AddRange(FindObjectsOfType<Gras>());
        m_allCows.AddRange(FindObjectsOfType<CowController>());
    }
	
	// Update is called once per frame
	void Update () 
	{
	
	}

    void GenerateGrid()
    {
        m_gridPoints = new NavGridPoint[101,101];

        RaycastHit hit;
        for (int x = -50; x <= 50; x++)
        {
            for (int z = -50; z <= 50; z++)
            {
                if (Physics.Raycast(new Vector3(x, 50, z), new Vector3(0, -1, 0), out hit, 100, m_groundAndObstaclesMask))
                {
                    m_gridPoints[x + 50, z + 50] = new NavGridPoint(hit.point, hit.collider.name == "Plane");
                }
            }
        }

        for (int x = -50; x <= 50; x++)
        {
            for (int z = -50; z <= 50; z++)
            {
                if (x + 49 > 0 && m_gridPoints[x + 49, z + 50].Active)
                    m_gridPoints[x + 50, z + 50].m_Neighbours.Add(m_gridPoints[x+ 49, z + 50]);
                if (z + 49 > 0 && m_gridPoints[x + 50, z + 49].Active)
                    m_gridPoints[x + 50, z + 50].m_Neighbours.Add(m_gridPoints[x + 50, z + 49]);
                if (x + 51 < 100 && m_gridPoints[x + 51, z + 50].Active)
                    m_gridPoints[x + 50, z + 50].m_Neighbours.Add(m_gridPoints[x + 51, z + 50]);
                if (z + 51 < 100 && m_gridPoints[x + 50, z + 51].Active)
                    m_gridPoints[x + 50, z + 50].m_Neighbours.Add(m_gridPoints[x + 50, z + 51]);

                if (x + 49 > 0 && z + 49 > 0 && m_gridPoints[x + 49, z + 49].Active)
                    m_gridPoints[x + 50, z + 50].m_Neighbours.Add(m_gridPoints[x + 49, z + 49]);
                if (x + 51 < 100 && z + 51 < 100 && m_gridPoints[x + 51, z + 51].Active)
                    m_gridPoints[x + 50, z + 50].m_Neighbours.Add(m_gridPoints[x + 51, z + 51]);
                if (x + 49 > 0 && z + 51 < 100 && m_gridPoints[x + 49, z + 51].Active)
                    m_gridPoints[x + 50, z + 50].m_Neighbours.Add(m_gridPoints[x + 49, z + 51]);
                if (x + 51 < 100 && z + 49 > 0 && m_gridPoints[x + 51, z + 49].Active)
                    m_gridPoints[x + 50, z + 50].m_Neighbours.Add(m_gridPoints[x + 51, z + 49]);
            }
        }
    }

    public List<Vector3> GetPath(Vector3 _start, Vector3 _end)
    {
        NavGridPoint startP = GetNearestGridPoint(_start);
        NavGridPoint endP = GetNearestGridPoint(_end);

        startP.m_GroundCost = 0;
        // Hierin befinden sich alle bereits komplett durchsuchten Punkte
        LinkedList<NavGridPoint> closedList = new LinkedList<NavGridPoint>();
        // Hierin befinden sich alle noch zu untersuchenden Punkte
        LinkedList<NavGridPoint> openList = new LinkedList<NavGridPoint>();
        openList.AddLast(startP);

        NavGridPoint tmp = null;
        float maxCosts;
        float calc;
        while (openList.Count > 0)
        {
            maxCosts = float.MaxValue;

            // Findet den billgsten Node
            foreach (NavGridPoint op in openList)
            {
                calc = op.m_GroundCost + op.GetHeuristicCosts(_end);
                if (calc < maxCosts)
                {
                    maxCosts = calc;
                    tmp = op;
                }
            }

            if (tmp == endP)
            {
                return FollowBreadCrumbs(endP, _start, _end);
            }
            // findet das kleinste Element in der Liste
            //tmp = openList.Aggregate((o1, o2) => o1.m_GroundCost + o1.GetHeuristicCosts(_end) 
            //                                    < o2.m_GroundCost + o2.GetHeuristicCosts(_end) 
            //                ? o1 : o2);


            // umsortieren des aktuellen Punktes
            openList.Remove(tmp);
            closedList.AddLast(tmp);


            foreach (NavGridPoint neighbour in tmp.m_Neighbours)
            {
                if (closedList.Contains(neighbour))
                {
                    continue;
                }

                calc = tmp.m_GroundCost + (neighbour.Position - tmp.Position).magnitude;
                if (calc < neighbour.m_GroundCost)
                {
                    neighbour.m_GroundCost = calc;
                    neighbour.m_Previous = tmp;
                }

                if (!openList.Contains(neighbour))
                {
                    openList.AddLast(neighbour);
                }
            }
        }

        ResetGrid();
        return null;
    }

    private List<Vector3> FollowBreadCrumbs(NavGridPoint _endP, Vector3 _start, Vector3 _end)
    {
        Stack<Vector3> path = new Stack<Vector3>();
        path.Push(_end);

        NavGridPoint tmp = _endP;
        while (tmp.m_Previous != null)
        {
            path.Push(tmp.Position);
            tmp = tmp.m_Previous;
        }


        ResetGrid();
        path.Push(tmp.Position);
        path.Push(_start);
        return path.ToList();
    }

    public void ResetGrid()
    {
        foreach (NavGridPoint ngp in m_gridPoints)
        {
            ngp.Reset();
        }
    }

    public NavGridPoint GetNearestGridPoint(Vector3 _pos)
    {
        _pos += new Vector3(50,0,50);
        int x = Mathf.Clamp(Mathf.RoundToInt(_pos.x), 0, 100);
        int z = Mathf.Clamp(Mathf.RoundToInt(_pos.z), 0, 100);

        return m_gridPoints[x, z];
    }

    void OnDrawGizmos()
    {
        if (m_gridPoints == null || !m_Debug)
        {
            return;
        }

        foreach (NavGridPoint ngp in m_gridPoints)
        {
            if (!ngp.Active)
                continue;

            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(ngp.Position, 0.1f);
            Gizmos.color = Color.magenta;
            foreach (NavGridPoint neighbour in ngp.m_Neighbours)
            {
                //if (neighbour.Active)
                Gizmos.DrawLine(ngp.Position, neighbour.Position);
            }
        }
    }
}
