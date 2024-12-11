using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SliderWithText : MonoBehaviour
{
    private Slider slider;
    private TextMeshProUGUI text;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(OnSliderValueChanged);
        text = transform.Find("Count").GetComponent<TextMeshProUGUI>();
        text.text = slider.value.ToString();
    }
    private void OnSliderValueChanged(float value)
    {
        text.text = value.ToString();
    }
}
