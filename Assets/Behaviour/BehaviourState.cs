using UnityEngine;
using System.Collections.Generic;

public abstract class BehaviourState 
{
    public Dictionary<System.Func<bool>, BehaviourState> m_transitions = new Dictionary<System.Func<bool>, BehaviourState>();
    protected CowController m_cow;

    // Delegates sind Referenzen auf Funktionen, mit ihnen entsteht ein neuer Datentyp, der eine Funktion mit der selben Signatur aufnehmen kann
    // später kann diese aufgerufen werden
    //delegate int Test(int _a);
    //System.Action<int, int, bool> t = null;
    //System.Func<bool, int> b = null;

    public BehaviourState(CowController _controller)
    {
        m_cow = _controller;
    }

    public virtual void StartBehaviour()
    {
        Debug.Log(ToString() + " started!");
    }

    public virtual void EndBehaviour()
    {
        Debug.Log(ToString() + " stopped!");
    }

    public BehaviourState Update()
    {
        foreach (var item in m_transitions)
        {
            if (item.Key())
            {
                EndBehaviour();
                item.Value.StartBehaviour();
                return item.Value;
            }
        }

        Behave();
        return this;
    }

    protected abstract void Behave();

    //int Alarm23(int _b)
    //{ return _b; }
}
