using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CowController : MonoBehaviour 
{
    public Renderer[] m_renderers;

    private BehaviourState m_currentState;
    public float m_Speed = 10;
    public float m_hunger = 20;
    private List<Vector3> m_path;
    private int m_currentPathIndex;

    // Use this for initialization
    void Awake () 
	{
        m_renderers = GetComponentsInChildren<Renderer>();

        BehaviourState idle = new IdleBehaviour(this);
        BehaviourState hungry = new HungryBehaviour(this);

        idle.m_transitions.Add(IdleToHungry, hungry);
        // Lambda-schreibweise für anonyme Funktionen
        hungry.m_transitions.Add(() => m_hunger < 25, idle);

        m_currentState = idle;
        m_currentState.StartBehaviour();
	}

    bool IdleToHungry()
    {
        return m_hunger > 50; 
    }
	
	// Update is called once per frame
	void Update () 
	{
        m_hunger += Time.deltaTime * 5;
        if (m_currentState != null)
        {
            m_currentState = m_currentState.Update();
        }

        if (m_hunger > 100)
        {
            Destroy(gameObject);
        }

        if (m_path != null)
        {
            if (m_currentPathIndex >= m_path.Count)
            {
                m_path = null;
                return;
            }

            transform.Translate((m_path[m_currentPathIndex] - transform.position).normalized * Time.deltaTime * m_Speed, Space.World);
            if ((m_path[m_currentPathIndex] - transform.position).sqrMagnitude < 0.5f)
            {
                m_currentPathIndex++;
            }
        }
    }

    public void SetTarget(Vector3 _pos)
    {
        m_path = NavGrid.Instance.GetPath(transform.position, _pos);
        m_currentPathIndex = 0;
    }

    void OnDestroy()
    {
        if (NavGrid.Instance != null)
        {
            NavGrid.Instance.m_allCows.Remove(this);
        }
    }
}
