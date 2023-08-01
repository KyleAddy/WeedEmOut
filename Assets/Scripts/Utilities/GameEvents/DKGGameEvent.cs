using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "event", menuName = "New/Simple Events/Event")]
public class DKGGameEvent : ScriptableObject
{
    public List<DKGGameEventListener> listeners = new List<DKGGameEventListener>();
    public bool debug = false;
    public void Raise(string _debugInfo = null)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            if(listeners[i] != null)
                listeners[i].OnEventRaised();
    }

    public void RegisterListener(DKGGameEventListener listener)
    {
        listeners.Add(listener);
    }

    public void UnRegisterListener(DKGGameEventListener listener)
    {
        listeners.Remove(listener);
    }
}
