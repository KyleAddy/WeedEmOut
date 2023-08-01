using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CreateAssetMenu(fileName = "stringVar", menuName = "New/Simple Variables/String Variable")]
public class StringVariable : ScriptableObject
{
    public string InitialValue;
    [SerializeField] string RuntimeValue;
    [SerializeField] List<DKGGameEvent> OnChangedDKGEvents = new List<DKGGameEvent>();


    public string GetValue()
    {
        return RuntimeValue;
    }
    public void SetValue(string _value)
    {
        RuntimeValue = _value;
        RaiseEvents();
    }

    void RaiseEvents()
    {
        if (OnChangedDKGEvents.Count != 0)
            foreach (var _event in OnChangedDKGEvents)
                if (_event)
                    _event.Raise();
    }
}
