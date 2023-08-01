using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChloroplastCountDisplay : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI text;

    [SerializeField]
    IntVariable chloroplastCountVar;

    [SerializeField]
    private string PreText = "";

    [SerializeField]
    GameObject valueChangeDisplayPrefab;

    [SerializeField]
    GameObject valueChangeDisplaySpawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        UpdateDispaly();
    }

    public void UpdateDispaly()
    {
        text.text = PreText + chloroplastCountVar.GetValue().ToString();

        if (chloroplastCountVar.GetLastChange() == 0) return;
        GameObject newDisply = Instantiate(valueChangeDisplayPrefab, valueChangeDisplaySpawnPoint.transform.position, transform.rotation.normalized);
        newDisply.GetComponent<ValueChangedDisplay>().UpdateDisplay(chloroplastCountVar.GetLastChange());
        newDisply.transform.parent = transform;
    }
}
