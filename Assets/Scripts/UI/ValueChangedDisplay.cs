using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ValueChangedDisplay : MonoBehaviour
{
    [SerializeField]
    Color greenColor;

    [SerializeField]
    Color redColor;

    public float targetOffset = 50f;
    [SerializeField]
    public float duration = 3f;

    private Vector2 originalPosition;
    private Vector2 targetPosition;

    private void Start()
    {
        originalPosition = transform.position;
        targetPosition = originalPosition + new Vector2(targetOffset, 0f);

        StartCoroutine(MoveElement());
    }

    private IEnumerator MoveElement()
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.position = Vector2.Lerp(originalPosition, targetPosition, t);
            Color tempColor = GetComponent<TextMeshProUGUI>().color;
            tempColor.a = 1 - (elapsedTime / duration);
            GetComponent<TextMeshProUGUI>().color = tempColor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        Destroy(gameObject);
    }

    public void UpdateDisplay(int _value)
    {
        GetComponent<TextMeshProUGUI>().text = _value.ToString();

        if (_value >= 0)
        {
            GetComponent<TextMeshProUGUI>().color = greenColor;
        }
        else
        {
            GetComponent<TextMeshProUGUI>().color = redColor;
        }
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

}
