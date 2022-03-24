using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Layer : MonoBehaviour
{
    public float[][] weights;
    public float[] biases;
    public float[] neurons;
    public Image[] neuronsImage;
    public TextMeshProUGUI[] neuronsText;
    public int lastLayerNbr;
    public int layerNbr;

    
    public void InitNeurons(string neuronName, int last_nbr, int current_nbr, float[][] weights , float[] bias)
    {
        this.lastLayerNbr = last_nbr;
        this.layerNbr = current_nbr;

        this.neuronsImage = new Image[layerNbr];
        this.neuronsText = new TextMeshProUGUI[layerNbr];
        for (int i = 0; i < layerNbr; i++)
        {
            this.neuronsImage[i] = transform.Find(neuronName + " (" + i + ")").GetComponent<Image>();
            this.neuronsText[i] = neuronsImage[i].GetComponentInChildren<TextMeshProUGUI>();
        }

        this.neurons = new float[layerNbr];
        this.biases = new float[layerNbr];
        this.weights = new float[layerNbr][];
        for (int i = 0; i < layerNbr; i++)
        {
            this.weights[i] = new float[lastLayerNbr];
        }

        for (int i = 0; i < layerNbr; i++)
        {
            this.biases[i] = bias[i];
            for (int j = 0; j < lastLayerNbr; j++)
            {
                this.weights[i][j] = weights[i][j];
            }
        }
    }

    public void Calculate(float[] inputs)
    {
        for (int i = 0; i < layerNbr; i++)
        {
            float value = 0;
            for (int j = 0; j < lastLayerNbr; j++)
            {
                value += weights[i][j] * inputs[j];
            }
            neurons[i] = Sigmoid(value + biases[i]);
        }
        UpdateVisual();
    }

    private float Sigmoid(float x)
    {
        float k = (float)Math.Exp(-1f * x);
        return 1.0f / (1.0f + k);
    }

    private void UpdateVisual()
    {
        for (int i = 0; i < layerNbr; i++)
        {
            neuronsImage[i].color = new Color(neurons[i], 0, 0); ;
            neuronsText[i].text = (neurons[i]*100f).ToString("F");
        }
    }
}
