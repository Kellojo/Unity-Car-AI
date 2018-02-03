using System;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetwork : IComparable<NeuralNetwork> {

    private int[] layers;
    private float[][] neurons;
    private float[][][] weights;
    private float fitness;
    private Species species;

    public NeuralNetwork(int[] layers) {
        this.layers = layers;
        species = new Species();
        for (int i = 0; i < layers.Length; i++) {
            this.layers[i] = layers[i];
        }

        InitNeurons();
        InitWeights();
    }

    //constructor that deeply copies another neural network
    public NeuralNetwork(NeuralNetwork CopyFrom) {
        layers = new int[CopyFrom.layers.Length];
        species = CopyFrom.GetSpecies();
        for (int i = 0; i < CopyFrom.layers.Length; i++) {
            layers[i] = CopyFrom.layers[i];
        }

        InitNeurons();
        InitWeights();
        CopyWeights(CopyFrom.weights);
    }

    //copy weights to this network
    private void CopyWeights(float[][][] copyWeights) {
        for (int i = 0; i < weights.Length; i++) {
            for (int j = 0; j < weights[i].Length; j++) {
                for (int k = 0; k < weights[i][j].Length; k++) {
                    weights[i][j][k] = copyWeights[i][j][k];
                }
            }
        }
    }

    //generate initial neurons
    private void InitNeurons() {
        List<float[]> neuronsList = new List<float[]>();

        for(int i = 0; i < layers.Length; i++) {
            neuronsList.Add(new float[layers[i]]);
        }

        neurons = neuronsList.ToArray();
    }

    //generate initial weights with random value
    private void InitWeights() {
        List<float[][]> weightList = new List<float[][]>();

        for (int i = 1; i < layers.Length; i++) {
            List<float[]> layerWeightList = new List<float[]>();

            int neuronsInPreviousLayer = layers[i - 1];

            for(int j = 0; j < neurons[i].Length; j++) {
                float[] neuronWeights = new float[neuronsInPreviousLayer];

                for (int k = 0; k < neuronsInPreviousLayer; k ++) {

                    //give random value to the neuron weight
                    neuronWeights[k] = UnityEngine.Random.Range(-1f, 1f);
                }

                layerWeightList.Add(neuronWeights);
            }

            weightList.Add(layerWeightList.ToArray());
        }

        weights = weightList.ToArray();
    }

    //feed forward this neural network with a given input array
    public float[] FeedForward(float[] inputs) {
        //set inputs to the first layer
        for (int i = 0; i < inputs.Length; i ++) {
            neurons[0][i] = inputs[i];
        }

        //iterate over all neurons and feed forward the values
        for (int i = 1; i < layers.Length; i++) {
            for (int j = 1; j < neurons[i].Length; j++) {

                float value = 0.25f;
                for (int k = 1; k < neurons[i - 1].Length; k++) {
                    value += weights[i - 1][j][k] * neurons[i - 1][k];
                }

                neurons[i][j] = (float) System.Math.Tanh(value); //Apply activation
            }
        }

        return neurons[neurons.Length - 1]; //return output layer
    }

    //mutate the network slightly
    public void Mutate() {
        for (int i = 0; i < weights.Length; i++) {
            for (int j = 0; j < weights[i].Length; j++) {
                for (int k = 0; k < weights[i][j].Length; k++) {

                    float weight = weights[i][j][k];
                    float randomNumber = UnityEngine.Random.Range(0f, 100);

                    //mutate the weight value
                    if (randomNumber <= 2f) {
                        weight *= -1f;                                      //flip value
                    } else if (randomNumber <= 4f) {
                        weight = UnityEngine.Random.Range(-1, 1);           //randomize value
                    } else if (randomNumber <= 6f) {
                        weight *= (UnityEngine.Random.Range(0f, 1f) + 1f);  //increase value by 0 to 100%
                    } else if (randomNumber <= 8f) {
                        weight *= UnityEngine.Random.Range(0f, 1f);         //decrease value by 0 to 100%
                    }

                    weights[i][j][k] = weight;
                }
            }
        }

    }

    //set the fitness of the network
    public void SetFitness(float fitness) {
        this.fitness = fitness;
    }

    //get the fitness of the network
    public float GetFitness() {
        return fitness;
    }

    //Add fitness to the network
    public void AddFitness(float fitnessToAdd) {
        fitness += fitnessToAdd;
    }

    //used for sorting networks based on their fitness value
    public int CompareTo(NeuralNetwork other) {
        if (other == null) return 1;
        if (fitness > other.fitness) return 1;
        if (fitness < other.fitness) return 0;
        return 0;
    }

    //get species of this network
    public Species GetSpecies() {
        return species;
    }

    //gets the input layer size
    public int GetInputLayerSize() {
        return layers[0];
    }
}
