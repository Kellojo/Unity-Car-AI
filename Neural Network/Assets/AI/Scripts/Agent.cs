using TMPro;
using UnityEngine;

public abstract class Agent : MonoBehaviour {

    [HideInInspector] public bool isActive = true;
    [HideInInspector] public NeuralNetwork neuralNetwork;
    [HideInInspector] public GameObject infoUI;
    private Species species;

    // Use this for initialization
    void Start () {
        isActive = true;
        ExtendedStart();
    }
	
	// Update is called once per frame
	void Update () {
		if (isActive) {
            runAgentUpdate();

            if (infoUI != null) {
                infoUI.GetComponent<TextMeshProUGUI>().text = species.GetName() + " - " + CalcFitnessOnUpdate();
            }
        } else {
            finished();
        }
	}

    public abstract void runAgentUpdate();          //updates the agent every frame
    public abstract void finished();                //runs every frame when the agent is finished/done
    public abstract float CalcFitnessOnUpdate();    //calc current fitness on update
    public abstract float CalcFitnessOnFinish();    //calc fitness on finish
    public abstract void ApplyColor();              //apply color to gameobject
    public abstract void ExtendedStart();           //Gets called on Start
    
    //Gets called when the agent is done and can be deleted
    public void Done() {
        neuralNetwork.SetFitness(CalcFitnessOnFinish());
        isActive = false;
    }
    //sets the neural network of this agent
    public void SetNetwork(NeuralNetwork network) {
        neuralNetwork = network;
        species = network.GetSpecies();
        ApplyColor();
    }

    //gets the species of this agent
    public Species GetSpecies() {
        return neuralNetwork.GetSpecies();
    }
}
