using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GradientAgentSpawner : MonoBehaviour
{
    [SerializeField] private MeshGenerator surface;
    [SerializeField] private GradientDescentAgent agentPrefab;
    [SerializeField] private Slider learningRateSlider;
    [SerializeField] private bool useMomentum = false;
    [SerializeField] private TextMeshProUGUI momentumText;
    [SerializeField] private AgentInfoUI agentInfoUI;
    [SerializeField] private Transform agentInfoContainer;

    private Camera cam;
    private List<GradientDescentAgent> gradientDescentAgentList = new();

    private void Start()
    {
        cam = Camera.main;
        learningRateSlider.onValueChanged.AddListener(SetLearningRate);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) TrySpawnAgent();
    }

    private void TrySpawnAgent()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.GetComponent<MeshGenerator>() != null)
            {
                SpawnAgentOnSurface(hit.point);
            }
        }
    }

    private void SpawnAgentOnSurface(Vector3 worldPoint)
    {
        Vector3 local = worldPoint - surface.transform.position;

        GradientDescentAgent agent = Instantiate(agentPrefab);
        agent.OnInstatiated(surface, local.x, local.z, learningRateSlider.value, useMomentum, Instantiate(agentInfoUI, agentInfoContainer));
        agent.ResetToStart();
        gradientDescentAgentList.Add(agent);
    }

    public void RunFullAllAgents()
    {
        foreach (GradientDescentAgent agent in gradientDescentAgentList)
            agent.Run();
    }

    public void RunNextStepAllAgents()
    {
        foreach (GradientDescentAgent agent in gradientDescentAgentList)
            agent.NextStep();
    }

    public void ResetAndRunAllAgents()
    {
        foreach (GradientDescentAgent agent in gradientDescentAgentList)
        {
            agent.ResetToStart();
            agent.Run();
        }
    }

    public void ResetAllAgents()
    {
        foreach (GradientDescentAgent agent in gradientDescentAgentList)
        {
            agent.ResetToStart();
            agent.Stop();
        }
    }
    public void ClearAllAgents()
    {
        foreach (GradientDescentAgent agent in gradientDescentAgentList)
        {
            Destroy(agent.gameObject);
        }
        gradientDescentAgentList.Clear();
    }
    public void ToggleMomentum()
    {
        useMomentum = !useMomentum;
        foreach (GradientDescentAgent agent in gradientDescentAgentList)
        {
            agent.SetUseMomentum(useMomentum);
        }
        momentumText.text = useMomentum ? "MOMENTUM" : "VANILLA";
    }
    private void SetLearningRate(float value)
    {
        foreach (GradientDescentAgent agent in gradientDescentAgentList)
        {
            agent.SetLearningRate(value);
        }
    }
}
