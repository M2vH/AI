using UnityEngine;
using System.Collections;

public class Gras : MonoBehaviour 
{
    public float Food { get; private set; }

    void Awake()
    {
        Food = 80;
        Destroy(gameObject, Random.Range(40, 500));
    }

	// Update is called once per frame
	void Update () 
	{
        Food = Mathf.Clamp(Food + Time.deltaTime * 2, 0, 100);
        transform.localScale = new Vector3(1, Food / 100, 1);
	}

    public float Eat(float _hunger)
    {
        float amount = Mathf.Min(_hunger, Food);
        Food -= amount;
        return amount;
    }

    void OnDestroy()
    {
        if (NavGrid.Instance != null)
        {
            NavGrid.Instance.m_allGras.Remove(this);
        }
    }
}
