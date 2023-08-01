using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
[CreateAssetMenu(fileName = "floatVar", menuName = "New/Simple Variables/Float Variable")]
public class FloatVariable : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField] bool positiveValuesOnly;
    [SerializeField] float maximumValue = 0f;
    [SerializeField] float InitialValue;
    [SerializeField] float RuntimeValue;

    [SerializeField] DKGGameEvent OnChangedDKGEvent;

    [SerializeField] float matchValue = 0;

    [SerializeField] DKGGameEvent OnValueMatchDKGEvent;


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
    public void OnAfterDeserialize()
    {
        RuntimeValue = InitialValue;
    }
    public float GetValue()
    {
        return RuntimeValue;
    }
    public void SetValue(float _value)
    {
        if ((positiveValuesOnly) && (_value < 0))
            _value = 0;
        if ((maximumValue != 0) && (_value > maximumValue))
            _value = maximumValue;
        RuntimeValue = _value;
        RaiseEvents();

        if(RuntimeValue == matchValue)
        {
            if (OnValueMatchDKGEvent != null) OnValueMatchDKGEvent.Raise();
        }
    }
    public void IncrementValue()
    {
        SetValue(RuntimeValue + 1);
        RaiseEvents();
    }

    public void IncrementValue(float _num)
    {
        SetValue(RuntimeValue + _num);
        RaiseEvents();
    }
    public void DecrementValue()
    {
        SetValue(RuntimeValue - 1);
        RaiseEvents();
    }
    public void IncrementValueByAmount(float _amount)
    {
        SetValue(RuntimeValue + _amount);
        RaiseEvents();
    }
    public void DecrementValue(float _amount)
    {
        SetValue(RuntimeValue - _amount);
        RaiseEvents();
    }

    void RaiseEvents()
    {
        if (OnChangedDKGEvent != null) OnChangedDKGEvent.Raise();
    }

    public float GetMaximumValue()
    {
        return maximumValue;
    }
}
