using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class EvolutionManager : MonoBehaviour {
    public int agentsPerGeneration = 50;
    public Agent agentPrefab;
    public int[] layers = { 3, 3, 2 };

    public TextMeshProUGUI generationText;
    public TextMeshProUGUI highscooreText;
    public GameObject ListContent;
    public GameObject entryPrefab;

    private List<Agent> activeAgents;
    private List<NeuralNetwork> activeNetworks;
    private int generationCounter = 0;
    private bool isManageing = false;

    private float highscoore = 0f;
    private float firstScoore = 0f;

    // Use this for initialization
    void Start () {
        activeAgents = new List<Agent>();
        activeNetworks = new List<NeuralNetwork>();

        //add initial networks
        for (int i = 0; i < agentsPerGeneration; i++) {
            activeNetworks.Add(new NeuralNetwork(layers));
        }
        Debug.Log(activeNetworks.Count);

        //start the first generation
        updateUI();
        StartCoroutine(StartNextGeneration());
    }
	
	// Update is called once per frame
	void Update () {
       if (IsGenerationOver()) {
            StartCoroutine(StartNextGeneration());
       }
    }

    //Starts the next generation
    System.Collections.IEnumerator StartNextGeneration() {
        isManageing = true;
        generationCounter++;    //increment generaiton counter

        //collect data from all agents to save it for the next generation
        collectDataFromAgents();
        updateUI();
        evaluateGeneration();

        yield return new WaitForSeconds(2f);

        //remove previous entries
        foreach (Transform child in ListContent.transform) {
            Destroy(child.gameObject);
        }

        //create new agents
        for (int i = 0; i < agentsPerGeneration; i++) {
            Agent agent = Instantiate(agentPrefab);
            agent.transform.position = transform.position;
            agent.transform.rotation = transform.rotation;

            agent.SetNetwork(activeNetworks[i]);
            agent.infoUI = Instantiate(entryPrefab);
            agent.infoUI.transform.SetParent(ListContent.transform);
            activeAgents.Add(agent);
        }
        isManageing = false;
    }

    //collects the data from the agents and gets them ready for evaluation and delets the agents
    private void collectDataFromAgents() {

        foreach (Agent agent in activeAgents) {
            //delete previous agents
            if (agent != null)
                Destroy(agent.gameObject);
        }
    }

    //updates the ui
    private void updateUI() {
        if (generationText != null) {
            generationText.text = "Generaiton: " + generationCounter;
        }

        //order the networks based on theis fitness
        activeNetworks = activeNetworks.OrderByDescending(o => o.GetFitness()).ToList();

        //display fitness of each network
        if (ListContent != null && entryPrefab != null) {
            //remove previous entries
            foreach (Transform child in ListContent.transform) {
                Destroy(child.gameObject);
            }

            //add new ui elements ones for the fitness of each networ
            int i = 0;
            foreach (NeuralNetwork network in activeNetworks) {
                GameObject uiElem = Instantiate(entryPrefab);
                uiElem.GetComponent<TextMeshProUGUI>().text = i++ + " - Fitness: " + network.GetFitness();
                uiElem.transform.SetParent(ListContent.transform);
            }
        }
    }

    //check if generation is over
    private bool IsGenerationOver() {
        if (isManageing)
            return false;

        bool isDone = true;
        foreach (Agent agent in activeAgents) {
            if (agent.isActive) {
                isDone = false;
                break;
            }
        }
        return isDone;
    }

    //evaluates the neural networks of a generation based on their fitness an replaces/mutates some of them
    private void evaluateGeneration() {
        //order the networks based on theis fitness
        activeNetworks = activeNetworks.OrderByDescending(o => o.GetFitness()).ToList();

        updateHighscoore();

        /*ditch and replace the bottom 85% of the population
        int ditchAmmountStart = Mathf.FloorToInt(agentsPerGeneration - agentsPerGeneration * 0.85f);
        for (int i = ditchAmmountStart; i < activeNetworks.Count; i++) {
            activeNetworks[i] = new NeuralNetwork(layers);
        }

        //copy the best network and keep it in a slightly mutated form
        for (int i = agentsPerGeneration / 2; i < activeNetworks.Count; i++) {
            activeNetworks[i] = new NeuralNetwork(activeNetworks[0]);
            activeNetworks[i].Mutate();
        }

        //mutate the top 15 networks slightly
        for (int i = 1; i < agentsPerGeneration / 2; i++) {
            if (activeNetworks[i] != null)
                activeNetworks[i].Mutate();
        }*/

        //duplicate the top network
        for (int i = 1 + 5; i < agentsPerGeneration / 2; i++) {
            activeNetworks[i] = new NeuralNetwork(activeNetworks[0]);
            activeNetworks[i].Mutate();
            if (i == 0)
                Debug.Log("Warning...... 0 accessed");
        }

        //generate 30 new networks
        for (int i = agentsPerGeneration / 2; i < activeNetworks.Count; i++) {
            activeNetworks[i] = new NeuralNetwork(layers);
            if (i == 0)
                Debug.Log("Warning...... 0 accessed");
        }
    }

    //updates the highscoore
    private void updateHighscoore() {
        if (highscoore < activeNetworks[0].GetFitness()) {
            highscoore = activeNetworks[0].GetFitness();

            //update firstscoore for monitoring
            if (firstScoore == 0) {
                firstScoore = highscoore;
            }

            if (highscooreText != null)
                highscooreText.text = "Highscoore: " + highscoore + " -> + " + (((highscoore / firstScoore) * 100) - 100) + "%";
        }
    }
}
