using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace Default
{
    // make network comparable for sorting purposes, f.e. sort by fitness and serializable for storing into a database
    [Serializable]
    public class NeuralNetwork : IComparable<NeuralNetwork>
    {
        private float learningRate = 1f;
        private float weightDecay = 0.001f;

        public int[] layers; //layers
        public float[][] neurons; //neuron matix
        public float[][] biases; //biases per neuron
        public float[][][] weights; //weight matrix

        private float[][] desiredNeurons;
        private float[][] biasesSmudge;
        private float[][][] weightsSmudge;

        [SerializeField] private float fitness; //fitness of the network


        /// <summary>
        /// Initilizes a neural network with random weights
        /// </summary>
        /// <param name="layers">layers to the neural network</param>
        public NeuralNetwork(int[] layers)
        {
            this.layers = layers;

            //generate matrix
            InitNeurons();
            InitWeights();
            InitBiases();
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
            InitBiases();
            CopyWeights(copyNetwork.weights);
            CopyBiases(copyNetwork.biases);
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
                        // take more weights from the genetically more superior parent
                        child.weights[i][j][k] = UnityEngine.Random.value < otherParentGeneticSuperiority ? otherParent.weights[i][j][k] : this.weights[i][j][k];
                    }
                }
            }

            for (int i = 0; i < biases.Length; i++)
            {
                for (int j = 0; j < biases[i].Length; j++)
                {
                    // take more biases from the genetically more superior parent
                    biases[i][j] = UnityEngine.Random.value < otherParentGeneticSuperiority ? otherParent.biases[i][j] : this.biases[i][j];
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

        private void CopyBiases(float[][] copyBiases)
        {
            for (int i = 0; i < biases.Length; i++)
            {
                for (int j = 0; j < biases[i].Length; j++)
                {
                    biases[i][j] = copyBiases[i][j];
                }
            }
        }

        /// <summary>
        /// Create neuron matrix
        /// </summary>
        private void InitNeurons()
        {
            //Neuron Initilization
            List<float[]> neuronsList = new();

            for (int i = 0; i < layers.Length; i++) //run through all layers
            {
                neuronsList.Add(new float[layers[i]]); //add layer to neuron list
            }

            var array = neuronsList.ToArray();

            neurons = array; //convert list to array
            desiredNeurons = array;
        }

        /// <summary>
        /// Create Biases Matrix
        /// </summary>
        private void InitBiases()
        {
            List<float[]> biasList = new();

            for (int i = 0; i < layers.Length; i++) //run through all layers
            {
                biasList.Add(new float[layers[i]]); //add layer to bias list
            }

            var biasArray = biasList.ToArray();

            biases = biasArray;
            biasesSmudge = biasArray;
        }

        /// <summary>
        /// Create weights matrix.
        /// </summary>
        private void InitWeights()
        {

            List<float[][]> weightsList = new(); //weights list which will later will converted into a weights 3D array
            List<float[][]> weightsSmudgeList = new(); //smudge list format only 

            //itterate over all neurons that have a weight connection
            for (int i = 1; i < layers.Length; i++)
            {
                List<float[]> layerWeightsList = new(); //layer weight list for this current layer (will be converted to 2D array)
                List<float[]> layerWeightsSmudgeList = new();

                int neuronsInPreviousLayer = layers[i - 1];

                //itterate over all neurons in this current layer
                for (int j = 0; j < neurons[i].Length; j++)
                {
                    float[] neuronWeights = new float[neuronsInPreviousLayer]; //neruons weights
                    float[] neuronWeightsSmudge = new float[neuronsInPreviousLayer];

                    //itterate over all neurons in the previous layer and set the weights randomly between 0.5f and -0.5
                    for (int k = 0; k < neuronsInPreviousLayer; k++)
                    {
                        //give random weights to neuron weights
                        neuronWeights[k] = UnityEngine.Random.Range(-0.5f, 0.5f);
                    }

                    layerWeightsList.Add(neuronWeights); //add neuron weights of this current layer to layer weights
                    layerWeightsSmudgeList.Add(neuronWeightsSmudge);
                }

                weightsList.Add(layerWeightsList.ToArray()); //add this layers weights converted into 2D array into weights list
                weightsSmudgeList.Add(layerWeightsSmudgeList.ToArray());
            }

            weights = weightsList.ToArray(); //convert to 3D array
            weightsSmudge = weightsSmudgeList.ToArray(); //convert to 3D array
        }

        /// <summary>
        /// Train the underlying network using backpropagation 
        /// </summary>
        public async UniTask Train(List<(float[], float[])> trainingData)
        {
            for (var i = 0; i < trainingData.Count; i++)
            {
                (float[] trainingInputs, float[] trainingOutputs) = trainingData[i];

                if (FeedForward(trainingInputs) == null) // feed forward data, but if invalid training data, discontinue training
                {
                    return;
                }

                // set desired output from trainingdata to backpropagate
                for (var j = 0; j < desiredNeurons[desiredNeurons.Length - 1].Length; j++)
                {
                    desiredNeurons[desiredNeurons.Length - 1][j] = trainingOutputs[j];
                }

                // calculate how much to smudge the weight and bias of each and every neuron and weight connection
                for (var j = neurons.Length - 1; j >= 1; j--)
                {
                    for (var k = 0; k < neurons[j].Length; k++)
                    {
                        var biasSmudge = SigmoidDerivative(neurons[j][k]) *
                                        (desiredNeurons[j][k] - neurons[j][k]);
                        biasesSmudge[j][k] += biasSmudge;

                        for (var l = 0; l < neurons[j - 1].Length; l++)
                        {
                            var weightSmudge = neurons[j - 1][l] * biasSmudge;
                            weightsSmudge[j - 1][k][l] += weightSmudge;

                            var neuronValueSmudge = weights[j - 1][k][l] * biasSmudge;
                            desiredNeurons[j - 1][l] += neuronValueSmudge;
                        }
                    }
                }
            }

            for (var i = neurons.Length - 1; i >= 1; i--)
            {
                for (var j = 0; j < neurons[i].Length; j++)
                {
                    biases[i][j] += biasesSmudge[i][j] * learningRate;
                    biases[i][j] *= 1 - weightDecay;
                    biasesSmudge[i][j] = 0; // reset smudges

                    for (var k = 0; k < neurons[i - 1].Length; k++)
                    {
                        weights[i - 1][j][k] += weightsSmudge[i - 1][j][k] * learningRate;
                        weights[i - 1][j][k] *= 1 - weightDecay;
                        weightsSmudge[i - 1][j][k] = 0; // reset smudges
                    }

                    desiredNeurons[i][j] = 0;
                }
            }

            await UniTask.Yield(); // dont block the main thread
        }

        /// <summary>
        /// Feed forward this neural network with a given input array
        /// </summary>
        /// <param name="inputs">Inputs to network</param>
        /// <returns></returns>
        public float[] FeedForward(float[] inputs)
        {
            if (inputs.Length != layers[0])
            {
                Debug.LogError($"Inputs length ({inputs.Length}) does not match input neurons count ({neurons.Length})!");
                return null;
            }

            //Add inputs to the neuron matrix
            for (var i = 0; i < neurons[0].Length; i++) neurons[0][i] = inputs[i];

            for (var i = 1; i < neurons.Length; i++)
            {
                for (var j = 0; j < neurons[i].Length; j++)
                {
                    neurons[i][j] = Sigmoid(Sum(neurons[i - 1], weights[i - 1][j]) + biases[i][j]);
                    desiredNeurons[i][j] = neurons[i][j];
                }
            }

            return neurons[neurons.Length - 1]; //return output layer
        }

        private static float Sum(IEnumerable<float> values, IReadOnlyList<float> weights) =>
            values.Select((v, i) => v * weights[i]).Sum();

        // activation function
        private static float Sigmoid(float x) => 1f / (1f + (float)Math.Exp(-x));

        private static float SigmoidDerivative(float x) => x * (1 - x);

        private static float HardSigmoid(float x)
        {
            if (x < -2.5f)
                return 0;
            if (x > 2.5f)
                return 1;
            return 0.2f * x + 0.5f;
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