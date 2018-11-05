using UnityEngine;
using System.Collections;
using System.Linq;

public class HungryBehaviour : BehaviourState
{
    private Gras m_target;

    public HungryBehaviour(CowController _cow) 
        : base(_cow)
    {
        
    }

    public override void StartBehaviour()
    {
        base.StartBehaviour();
        foreach (Renderer r in m_cow.m_renderers)
        {
            r.material.color = Color.yellow;
        }

        FindNextFoodSource();
    }

    protected override void Behave()
    {
        if (m_target == null)
        {
            FindNextFoodSource();
        }

        if (m_target != null && (m_target.transform.position - m_cow.transform.position).magnitude < 1)
        {
            m_cow.m_hunger -= m_target.Eat(m_cow.m_hunger);
            m_target = null;
        }
    }

    void FindNextFoodSource()
    {
        m_target = NavGrid.Instance.m_allGras.Where(o => o.Food > 25).
                            Where(o => NavGrid.Instance.m_allCows.All(p => (p.transform.position - o.transform.position).magnitude > 2)).
                            Aggregate((o1, o2) => (o1.transform.position - m_cow.transform.position).magnitude <
                                                (o2.transform.position - m_cow.transform.position).magnitude ? o1 : o2);

        m_cow.SetTarget(m_target.transform.position);
    }
}
