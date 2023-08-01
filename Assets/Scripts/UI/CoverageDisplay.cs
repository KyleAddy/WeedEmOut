using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoverageDisplay : MonoBehaviour
{
    [SerializeField]
    Image greenImage;

    [SerializeField]
    Image redImage;

    [SerializeField]
    TextMeshProUGUI text;

    [SerializeField]
    IntVariable weedCountVar;

    [SerializeField]
    IntVariable flowerCountVar;

    int gridSpacesCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        Vector2 temp = GridManager.instance.GetComponent<GridManager>().GetGridSize();
        gridSpacesCount = (int)temp.x * (int)temp.y;
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        float weedPercentage = (float)weedCountVar.GetValue() / (float)gridSpacesCount;

        if (weedPercentage < .01f && weedCountVar.GetValue() > 0)
        {
            weedPercentage = .01f;
        }

        greenImage.fillAmount = weedPercentage;

        text.text = Mathf.RoundToInt(weedPercentage * 100) + "%";

        float flowerPercentage = (float)flowerCountVar.GetValue() / (float)gridSpacesCount;

        if (flowerPercentage < .01f && flowerCountVar.GetValue() > 0)
        {
            flowerPercentage = .01f;
        }

        redImage.fillAmount = flowerPercentage;
    }
}
