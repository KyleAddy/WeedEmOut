using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "gameObjectVar", menuName = "New/Simple Variables/GameObject Variable")]
public class GameObjectVariable : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField] GameObject InitialValue;
    [SerializeField] GameObject RuntimeValue;
    [SerializeField] List<DKGGameEvent> OnChangedDKGEvents = new List<DKGGameEvent>();

    public void OnBeforeSerialize()
    {

    }

    public void OnAfterDeserialize()
    {
        RuntimeValue = InitialValue;
    }

    public GameObject GetValue()
    {
        return RuntimeValue;
    }

    public void SetValue(GameObject _gameObject)
    {
        RuntimeValue = _gameObject;
        RaiseEvents();
    }
    public void ClearValue()
    {
        RuntimeValue = null;
    }
    void RaiseEvents()
    {
        if (OnChangedDKGEvents.Count != 0)
            foreach (var _event in OnChangedDKGEvents)
                _event.Raise();
    }
}
