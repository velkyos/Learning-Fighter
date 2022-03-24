using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualisationManager : MonoBehaviour
{

    public InputLayer inputLayer;
    public Layer outputLayer;
    public Layer[] interLayer;

    private byte[] layers = { 31, 8, 4 };
    public string LoadName;
    public NeuronNetwork network;


    void Start()
    {
        network = new NeuronNetwork(layers);
        network.Load("Assets/" + LoadName + ".txt");
        float[][][] weights = network.GetWeights();
        float[][] biases = network.GetBiases();

        outputLayer.InitNeurons("NeuronO", 8, 4, weights[1], biases[1]);
        interLayer[0].InitNeurons("Neuron1", 31, 8, weights[0], biases[0]);
        //interLayer[1].InitNeurons("Neuron2", 8, 8, weights[2], biases[2]);
        inputLayer.UpdateChange();
    }

    internal void UpdateGaphic(float[] inputs)
    {
        interLayer[0].Calculate(inputs);
        //interLayer[1].Calculate(interLayer[0].neurons);
        outputLayer.Calculate(interLayer[0].neurons);
    }


}
