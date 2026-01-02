using TMPro;
using UnityEngine;

public class FPSIndicatorScript : MonoBehaviour
{
    int framesThisSecond;
    TextMeshProUGUI text;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        InvokeRepeating("UpdateFramesPerSecondText", 1, 1);
    }

    void Update()
    {
        framesThisSecond++;
    }

    void UpdateFramesPerSecondText()
    {
        text.text = framesThisSecond.ToString();
        framesThisSecond = 0;
    }
}
