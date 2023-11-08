using System.Collections.Generic;
using System;
using UnityEngine;

namespace Default
{
    // make network comparable for sorting purposes, f.e. sort by fitness
    [Serializable]
    public class NeuralNetwork : IComparable<NeuralNetwork>
    {
        public int[] layers; //layers
        public float[][] neurons; //neuron matix
        public float[][][] weights; //weight matrix

        [SerializeField] private float fitness; //fitness of the network


        /// <summary>
        /// Initilizes a neural network with random weights
        /// </summary>
        /// <param name="layers">layers to the neural network</param>
        public NeuralNetwork(int[] layers)
        {
            //deep copy of layers of this network 
            this.layers = new int[layers.Length];
            for (int i = 0; i < layers.Length; i++)
            {
                this.layers[i] = layers[i];
            }

            //generate matrix
            InitNeurons();
            InitWeights();
        }

        public NeuralNetwork(NeuralNetworkData data)
        {
            this.layers = data.Network.layers;
            InitNeurons();
            InitWeights();
            CopyWeights(data.Network.weights);
        }

        /// <summary>
        /// Deep copy constructor 
        /// </summary>
        /// <param name="copyNetwork">Network to deep copy</param>
        public NeuralNetwork(NeuralNetwork copyNetwork)
        {
            this.layers = new int[copyNetwork.layers.Length];
            for (int i = 0; i < copyNetwork.layers.Length; i++)
            {
                this.layers[i] = copyNetwork.layers[i];
            }

            InitNeurons();
            InitWeights();
            CopyWeights(copyNetwork.weights);
        }

        /// <summary>
        /// Mating two networks and mixing the weights
        /// </summary>
        /// <param name="otherParent">Other mate</param>
        /// <returns>Returns the child of the two networks!</returns>
        public NeuralNetwork Crossover(NeuralNetwork otherParent, float otherParentGeneticSuperiority = 0.5f)
        {
            NeuralNetwork child = new NeuralNetwork(this.layers);

            for (int i = 0; i < weights.Length; i++)
            {
                for (int j = 0; j < weights[i].Length; j++)
                {
                    for (int k = 0; k < weights[i][j].Length; k++)
                    {
                        child.weights[i][j][k] = UnityEngine.Random.value < otherParentGeneticSuperiority ? otherParent.weights[i][j][k] : this.weights[i][j][k];
                    }
                }
            }

            return child;
        }

        private void CopyWeights(float[][][] copyWeights)
        {
            for (int i = 0; i < weights.Length; i++)
            {
                for (int j = 0; j < weights[i].Length; j++)
                {
                    for (int k = 0; k < weights[i][j].Length; k++)
                    {
                        weights[i][j][k] = copyWeights[i][j][k];
                    }
                }
            }
        }

        /// <summary>
        /// Create neuron matrix
        /// </summary>
        private void InitNeurons()
        {
            //Neuron Initilization
            List<float[]> neuronsList = new List<float[]>();

            for (int i = 0; i < layers.Length; i++) //run through all layers
            {
                neuronsList.Add(new float[layers[i]]); //add layer to neuron list
            }

            neurons = neuronsList.ToArray(); //convert list to array
        }

        /// <summary>
        /// Create weights matrix.
        /// </summary>
        private void InitWeights()
        {

            List<float[][]> weightsList = new(); //weights list which will later will converted into a weights 3D array

            //itterate over all neurons that have a weight connection
            for (int i = 1; i < layers.Length; i++)
            {
                List<float[]> layerWeightsList = new(); //layer weight list for this current layer (will be converted to 2D array)

                int neuronsInPreviousLayer = layers[i - 1];

                //itterate over all neurons in this current layer
                for (int j = 0; j < neurons[i].Length; j++)
                {
                    float[] neuronWeights = new float[neuronsInPreviousLayer]; //neruons weights

                    //itterate over all neurons in the previous layer and set the weights randomly between 0.5f and -0.5
                    for (int k = 0; k < neuronsInPreviousLayer; k++)
                    {
                        //give random weights to neuron weights
                        neuronWeights[k] = UnityEngine.Random.Range(-0.5f, 0.5f);
                    }

                    layerWeightsList.Add(neuronWeights); //add neuron weights of this current layer to layer weights
                }

                weightsList.Add(layerWeightsList.ToArray()); //add this layers weights converted into 2D array into weights list
            }

            weights = weightsList.ToArray(); //convert to 3D array
        }

        /// <summary>
        /// Feed forward this neural network with a given input array
        /// </summary>
        /// <param name="inputs">Inputs to network</param>
        /// <returns></returns>
        public float[] FeedForward(float[] inputs)
        {
            //Add inputs to the neuron matrix
            for (int i = 0; i < inputs.Length; i++)
            {
                neurons[0][i] = inputs[i];
            }

            //itterate over all neurons and compute feedforward values 
            for (int i = 1; i < layers.Length; i++)
            {
                for (int j = 0; j < neurons[i].Length; j++)
                {
                    float value = 0f;

                    for (int k = 0; k < neurons[i - 1].Length; k++)
                    {
                        value += weights[i - 1][j][k] * neurons[i - 1][k]; //sum off all weights connections of this neuron weight their values in previous layer
                    }

                    neurons[i][j] = (float)Math.Tanh(value); //Hyperbolic tangent activation

                    if (i == layers.Length - 1)
                    {
                        neurons[i][j] = 8f * (neurons[i][j] + 1) - 8;  // Remap from [-1, 1] to [-8, 8]
                    }
                }
            }

            return neurons[neurons.Length - 1]; //return output layer
        }

        /// <summary>
        /// Mutate neural network weights
        /// </summary>
        public void Mutate()
        {
            for (int i = 0; i < weights.Length; i++)
            {
                for (int j = 0; j < weights[i].Length; j++)
                {
                    for (int k = 0; k < weights[i][j].Length; k++)
                    {
                        float weight = weights[i][j][k];

                        //mutate weight value 
                        float randomNumber = UnityEngine.Random.Range(0f, 100f);

                        if (randomNumber <= 2f)
                        {
                            weight *= -1f;
                        }
                        else if (randomNumber <= 4f)
                        {
                            weight = UnityEngine.Random.Range(-0.5f, 0.5f);
                        }
                        else if (randomNumber <= 6f)
                        {
                            float factor = UnityEngine.Random.Range(0f, 1f) + 1f;
                            weight *= factor;
                        }
                        else if (randomNumber <= 8f)
                        {
                            float factor = UnityEngine.Random.Range(0f, 1f);
                            weight *= factor;
                        }

                        weights[i][j][k] = weight;
                    }
                }
            }
        }

        public void AddFitness(float fit)
        {
            fitness += fit;
        }

        public void SetFitness(float fit)
        {
            fitness = fit;
        }

        public float GetFitness()
        {
            return fitness;
        }

        /// <summary>
        /// Compare two neural networks and sort based on fitness
        /// </summary>
        /// <param name="other">Network to be compared to</param>
        /// <returns></returns>
        public int CompareTo(NeuralNetwork other)
        {
            if (other == null) return 1;

            if (fitness > other.fitness)
                return 1;
            else if (fitness < other.fitness)
                return -1;
            else
                return 0;
        }
    }
}