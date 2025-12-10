using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AgentInfoUI : MonoBehaviour
{
    [SerializeField] private Image agentColorImage;
    [SerializeField] private TextMeshProUGUI positionText;
    [SerializeField] private TextMeshProUGUI gradientText;

    public void Initialize(Color agentColor, Vector2 position, Vector2 gradient)
    {
        agentColorImage.color = agentColor;
        positionText.text = "Position: " + position.ToString();
        gradientText.text = "Gradient: " + gradient.ToString();
    }
    public void UpdatePositionAndGradient(Vector2 position, Vector2 gradient)
    {
        positionText.text = "Position: " + position.ToString();
        gradientText.text = "Gradient: " + gradient.ToString();
    }
}
