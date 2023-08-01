using System;
using UnityEngine;

[CreateAssetMenu(fileName = "boolVar", menuName = "New/Simple Variables/Bool Variable")]
public class BoolVariable : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField] bool InitialValue;
    [SerializeField] bool RuntimeValue;
    [SerializeField] DKGGameEvent OnChangedDKGEvent;
    [SerializeField] DKGGameEvent OnActiveDKGEvent;
    [SerializeField] DKGGameEvent OnDeactiveDKGEvent;
    public void OnBeforeSerialize() { }

    public void OnAfterDeserialize()
    {
        RuntimeValue = InitialValue;
    }
    private void Awake()
    {
        RuntimeValue = InitialValue;
    }
    public void ToggleBool()
    {
        if (RuntimeValue == true)
            SetValue(false);
        else
            SetValue(true);
    }
    public bool GetValue()
    {
        return RuntimeValue;
    }
    public void SetValue(bool state)
    {
        if (RuntimeValue == state) // Already at value; no need to update or trigger events
            return;

        RuntimeValue = state;
        RaiseEvents();
    }
    void RaiseEvents()
    {
        if (OnChangedDKGEvent != null) OnChangedDKGEvent.Raise();
        if ((OnActiveDKGEvent != null) && (RuntimeValue == true))
        {
            OnActiveDKGEvent.Raise();
        }
        if ((OnDeactiveDKGEvent != null) && (RuntimeValue == false)) OnDeactiveDKGEvent.Raise();
    }
}
