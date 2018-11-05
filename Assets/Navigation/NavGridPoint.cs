using UnityEngine;
using System.Collections.Generic;

public class NavGridPoint 
{
    // Entfernung zum Startpunkt
    public float m_GroundCost;
    public List<NavGridPoint> m_Neighbours = new List<NavGridPoint>();
    public Vector3 Position { get; private set; }
    public bool Active { get; set; }

    public NavGridPoint m_Previous;

    public NavGridPoint(Vector3 _pos, bool _active)
    {
        Position = _pos;
        Active = _active;

        Reset();
    }

    public void Reset()
    {
        m_GroundCost = float.MaxValue;
        m_Previous = null;
    }

    // Geschätzte Entfernung zum Ziel
    public float GetHeuristicCosts(Vector3 _target)
    {
        return (Position - _target).magnitude;
    }
}
