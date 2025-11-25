using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderText : MonoBehaviour
{
    [SerializeField] private Slider slider;
    private TextMeshProUGUI text;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        text.text = slider.value.ToString("F2");

        slider.onValueChanged.AddListener((value) =>
        {
            text.text = value.ToString("F2");
        });
    }
}
