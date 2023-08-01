using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DKGGameEventListener : MonoBehaviour
{
    [SerializeField] public List<DKGGameEvent> Events = new List<DKGGameEvent>();
    public UnityEvent Response;

    private void OnEnable()
    {
        foreach (DKGGameEvent e in Events)
        {
            e.RegisterListener(this);
        }
    }

    private void OnDisable()
    {
        foreach (DKGGameEvent e in Events)
        {
            e.UnRegisterListener(this);
        }
    }

    public void OnEventRaised()
    {
        Response.Invoke();
    }
}
