using System;
using System.Collections.Generic;

namespace Default
{
    [Serializable]
    public class SerializedNetworkData
    {
        public int[] layers;
        public float[][] biases;
        public float[][][] weights;

        public float fitness;

        public SerializedNetworkData()
        {
            
        }

        public SerializedNetworkData(NeuralNetwork nn)
        {
            fitness = nn.GetFitness();

            // layers
            layers = new int[nn.layers.Length];

            for (int i = 0; i < nn.layers.Length; i++)
            {
                layers[i] = nn.layers[i];
            }


            // biases
            var biasList = new List<float[]>();

            for (int i = 0; i < nn.biases.Length; i++)
            {
                biasList.Add(new float[nn.biases[i].Length]);

                for (int j = 0; j < nn.biases[i].Length; j++)
                {
                    biasList[i][j] = nn.biases[i][j];
                }
            }

            biases = biasList.ToArray();

            // weights
            List<float[][]> weightsList = new();

            for (int i = 1; i < nn.layers.Length; i++)
            {
                List<float[]> layerWeightsList = new();

                int neuronsInPreviousLayer = nn.layers[i - 1];

                //itterate over all neurons in this current layer
                for (int j = 0; j < nn.neurons[i].Length; j++)
                {
                    float[] neuronWeights = new float[neuronsInPreviousLayer]; //neruons weights

                    layerWeightsList.Add(neuronWeights); //add neuron weights of this current layer to layer weights
                }

                weightsList.Add(layerWeightsList.ToArray()); //add this layers weights converted into 2D array into weights list
            }

            weights = weightsList.ToArray(); // 3D array of uninitialized weights (all weights = 0) 

            // copy weights
            for (int i = 0; i < nn.weights.Length; i++)
            {
                for (int j = 0; j < nn.weights[i].Length; j++)
                {
                    for (int k = 0; k < nn.weights[i][j].Length; k++)
                    {
                        weights[i][j][k] = nn.weights[i][j][k];
                    }
                }
            }
        }
    }
}