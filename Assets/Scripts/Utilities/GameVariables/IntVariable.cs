using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CreateAssetMenu(fileName = "intVar", menuName = "New/Simple Variables/Int Variable")]
public class IntVariable : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField] bool positiveValuesOnly;
    public int InitialValue;
    [SerializeField] int RuntimeValue;
    [SerializeField] List<DKGGameEvent> OnChangedDKGEvents = new List<DKGGameEvent>();
    private int lastChange = 0;


    public int GetValue()
    {
        return RuntimeValue;
    }

    public int GetLastChange()
    {
        return lastChange;
    }
    public void SetValue(int _value)
    {
        if ((positiveValuesOnly) && (_value < 0))
            _value = 0;

        RuntimeValue = _value;

        RaiseEvents();
    }

    public void IncrementValue()
    {
        lastChange = 1;
        SetValue(RuntimeValue + 1);
    }
    public void DecrementValue()
    {
        lastChange = -1;
        SetValue(RuntimeValue - 1);
    }
    public void IncrementValue(int _amount)
    {
        lastChange = _amount;
        SetValue(RuntimeValue + _amount);
    }
    public void DecrementValue(int _amount)
    {
        lastChange = -_amount;
        SetValue(RuntimeValue - _amount);
    }

    void RaiseEvents()
    {
        if(OnChangedDKGEvents.Count != 0)
            foreach (var _event in OnChangedDKGEvents)
                if(_event)
                    _event.Raise();
    }

#if UNITY_EDITOR
    protected void OnEnable()
    {
        EditorApplication.playModeStateChanged += OnPlayStateChange;
    }

    protected void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayStateChange;
    }

    void OnPlayStateChange(PlayModeStateChange state)
    {
        OnAfterDeserialize();
    }
#endif

    public void OnBeforeSerialize() { }
    public void OnAfterDeserialize() {
        lastChange = 0;
        SetValue(InitialValue);
    }
}
