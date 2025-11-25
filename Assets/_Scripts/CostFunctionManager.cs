using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CostFunctionManager : MonoBehaviour
{
    [SerializeField] private MeshGenerator meshGenerator;
    [SerializeField] private Slider sliderA;
    [SerializeField] private Slider sliderB;
    [SerializeField] private Slider sliderC;
    [SerializeField] private TextMeshProUGUI costFunctionText;
    private void Start()
    {
        sliderA.onValueChanged.AddListener((value) =>
        {
            UpdateMeshParameter();
            UpdateCostFunctionText();
        });
        sliderB.onValueChanged.AddListener((value) =>
        {
            UpdateMeshParameter();
            UpdateCostFunctionText();
        });
        sliderC.onValueChanged.AddListener((value) =>
        {
            UpdateMeshParameter();
            UpdateCostFunctionText();
        });
    }
    private void UpdateMeshParameter()
    {
        meshGenerator.SetFunctionParameters(sliderA.value, sliderB.value, sliderC.value);
    }
    private void UpdateCostFunctionText()
    {
        string aStr = FormatParameter(sliderA.value, "x");
        string bStr = FormatParameter(sliderB.value, "z");
        string cStr = FormatParameter(sliderC.value, "");

        costFunctionText.text = $"F(x,z) = sin({aStr}) + cos({bStr}) + {cStr}sin(x + z)";
    }
    private string FormatParameter(float value, string variable)
    {
        int intVal = Mathf.RoundToInt(value);

        if (Mathf.Approximately(value, 1f))
            return variable;

        if (Mathf.Approximately(value, intVal))
            return $"{intVal}{variable}";

        return $"{value:F2}{variable}";
    }
}
