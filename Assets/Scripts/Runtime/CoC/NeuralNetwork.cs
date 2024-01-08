using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

namespace Default
{
    // make network comparable for sorting purposes, f.e. sort by fitness and serializable for storing into a database
    [Serializable]
    public class NeuralNetwork : IComparable<NeuralNetwork>
    {
        private const float DEFAULT_LEARNING_RATE = 0.4f;
        private const float DEFAULT_WEIGHT_DECAY = 0.001f;

        public int[] layers; //layers
        public float[][] neurons; //neuron matix
        public float[][] biases; //biases per neuron
        public float[][][] weights; //weight matrix

        public float[][] desiredNeurons;
        public float[][] biasesSmudge;
        public float[][][] weightsSmudge;

        [SerializeField] private float fitness = 0f; //fitness of the network


        /// <summary>
        /// Initilizes a neural network with random weights
        /// </summary>
        /// <param name="layers">layers to the neural network</param>
        public NeuralNetwork(int[] layers, bool randomBiases = false)
        {
            this.layers = layers;

            //generate matrix
            InitNeurons();
            InitWeights();
            InitBiases(randomBiases);

            fitness = 0f;
        }

        /// <summary>
        /// Create Network from Json-Data
        /// </summary>
        public NeuralNetwork(string jsonData)
        {
            var data = JsonConvert.DeserializeObject<SerializedNetworkData>(jsonData);

            this.layers = new int[data.layers.Length];
            Debug.Log(jsonData + "\n\n\n\n" + data);

            for (int i = 0; i < data.layers.Length; i++)
            {
                this.layers[i] = data.layers[i];
            }

            InitNeurons();
            InitWeights();
            InitBiases(false);

            CopyBiases(data.biases);
            CopyWeights(data.weights);

            fitness = data.fitness == float.NaN || !float.IsFinite(data.fitness) ? 0f : data.fitness;
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
            InitBiases(false);
            CopyWeights(copyNetwork.weights);
            CopyBiases(copyNetwork.biases);

            fitness = copyNetwork.GetFitness();
        }

        /// <summary>
        /// Mating two networks and mixing the weights and biases
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
            List<float[]> desiredNeuronsList = new();

            for (int i = 0; i < layers.Length; i++) //run through all layers
            {
                neuronsList.Add(new float[layers[i]]); //add layer to neuron list
                desiredNeuronsList.Add(new float[layers[i]]); //add layer to neuron list
            }

            //convert list to array
            neurons = neuronsList.ToArray();
            desiredNeurons = desiredNeuronsList.ToArray();
        }

        /// <summary>
        /// Create Biases Matrix
        /// </summary>
        private void InitBiases(bool randomBiases)
        {
            List<float[]> biasList = new();

            for (int i = 0; i < layers.Length; i++) //run through all layers
            {
                biasList.Add(new float[layers[i]]); //add layer to bias list

                if (randomBiases)
                {
                    for (int j = 0; j < biasList[i].Length; j++)
                    {
                        biasList[i][j] = UnityEngine.Random.Range(-0.5f, 0.5f);
                    }
                }
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
            var xavierInit = XavierInitialization(layers[0], layers[layers.Length - 1]);

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
                        //give random weights to neuron weights in the range of the xavier init function
                        neuronWeights[k] = UnityEngine.Random.Range(-xavierInit, xavierInit);
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
        public async UniTask Train(List<Dataset> trainingData, float learningRate = DEFAULT_LEARNING_RATE, float weightDecay = DEFAULT_WEIGHT_DECAY)
        {
            for (var i = 0; i < trainingData.Count; i++)
            {
                var dataset = trainingData[i];
                var actualOutput = FeedForward(dataset.Inputs);

                // if invalid training data, discontinue training
                if (actualOutput == null) { return; }

                // set desired output from trainingdata to backpropagate
                for (var j = 0; j < desiredNeurons[desiredNeurons.Length - 1].Length; j++)
                {
                    desiredNeurons[desiredNeurons.Length - 1][j] = dataset.Outputs[j];
                }

                // calculate how much to smudge the weight and bias of each and every neuron and weight connection
                for (var j = neurons.Length - 1; j >= 1; j--)
                {
                    for (var k = 0; k < neurons[j].Length; k++)
                    {
                        var costForSingleNeuron = neurons[j][k] * (desiredNeurons[j][k] - neurons[j][k]);

                        var biasSmudge = j >= neurons.Length - 1 ?
                            SigmoidDerivative(costForSingleNeuron)
                            : ReLUDerivative(costForSingleNeuron);
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
                    biasesSmudge[i][j] = 0f; // reset smudges

                    for (var k = 0; k < neurons[i - 1].Length; k++)
                    {
                        weights[i - 1][j][k] += weightsSmudge[i - 1][j][k] * learningRate;
                        weights[i - 1][j][k] *= 1 - weightDecay;
                        weightsSmudge[i - 1][j][k] = 0f; // reset smudges
                    }

                    desiredNeurons[i][j] = 0f;
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

            var outputs = new float[neurons[layers.Length - 1].Length]; // output layer format 

            // Set inputs of the neuron matrix
            for (var i = 0; i < neurons[0].Length; i++) neurons[0][i] = inputs[i];

            // Iterate over the rest neurons, including the output
            for (var i = 1; i < neurons.Length; i++)
            {
                for (var j = 0; j < neurons[i].Length; j++)
                {
                    var activationValue = i >= neurons.Length - 1 ? // only use sigmoid for the output layer
                        Sigmoid(Sum(neurons[i - 1], weights[i - 1][j]) + biases[i][j])
                        : ReLU(Sum(neurons[i - 1], weights[i - 1][j]) + biases[i][j]);

                    neurons[i][j] = activationValue;
                    desiredNeurons[i][j] = neurons[i][j];

                    // if at last layer/set of neurons
                    if (i >= layers.Length - 1)
                    {
                        outputs[j] = activationValue;
                    }
                }
            }

            return outputs; //return copy of activated output layer
        }

        private static float Sum(IEnumerable<float> values, IReadOnlyList<float> weights) =>
            values.Select((v, i) => v * weights[i]).Sum();


        // Tanh 
        public static float Tanh(float value)
        {
            return (float)Math.Tanh(value);
        }

        public static float TanhDerivative(float value)
        {
            var tanhValue = (float)Math.Tanh(value);
            return 1f - tanhValue * tanhValue;
        }

        // ReLU 
        public static float ReLU(float value)
        {
            return Math.Max(0f, value);
        }

        public static float ReLUDerivative(float value)
        {
            return value > 0f ? 1f : 0f;
        }

        // Sigmoid 
        private static float Sigmoid(float x)
        {
            return 1f / (1f + (float)Math.Exp(-x));
        }

        private static float SigmoidDerivative(float x) => x * (1f - x);

        private static float HardSigmoid(float x)
        {
            if (x < -2.5f)
                return 0;
            if (x > 2.5f)
                return 1;
            return 0.2f * x + 0.5f;
        }

        // determines the weight randomization range according to the Xavier (Glorot) Initialization, which considers the input and output size of the network
        private static float XavierInitialization(int numberInputNeurons, int numberOutputNeurons)
        {
            return Mathf.Sqrt(6f / (numberInputNeurons + numberOutputNeurons));
        }

        /// <summary>
        /// Mutate weights and biases, epochs without improvement increases the chance of mutation for every weight
        /// </summary>
        public void Mutate(float chanceMultiplier = 1f, float strengthMultiplier = 1f)
        {
            for (int i = 0; i < weights.Length; i++)
            {
                for (int j = 0; j < weights[i].Length; j++)
                {
                    for (int k = 0; k < weights[i][j].Length; k++)
                    {
                        float weight = weights[i][j][k];

                        // mutate chance
                        float randomNumber = UnityEngine.Random.Range(0f, 100f);

                        if (randomNumber <= 2f * chanceMultiplier) // 2% Chance * chancemultiplier
                        {
                            weight += UnityEngine.Random.Range(-strengthMultiplier, strengthMultiplier); // addition pos/neg
                        }
                        else if (randomNumber <= 3f * chanceMultiplier) // 1% Chance * chancemultiplier
                        {
                            weight = UnityEngine.Random.Range(-0.5f, 0.5f); // reset 
                        }
                        else if (randomNumber <= 6f * chanceMultiplier) // 3% Chance * chancemultiplier
                        {
                            weight *= UnityEngine.Random.Range(1f, 1f + strengthMultiplier); // multiplication 1 to strengthmultiplier
                        }
                        else if (randomNumber <= 9f * chanceMultiplier) // 3% Chance * chancemultiplier
                        {
                            weight *= UnityEngine.Random.Range(1f / (2f + strengthMultiplier), 0.9f); // division by 0.9 to 0.5 and smaller depending on strengthmultiplier
                        }

                        weights[i][j][k] = weight;
                    }
                }
            }

            // for (int i = 0; i < biases.Length; i++)
            // {
            //     for (int j = 0; j < biases[i].Length; j++)
            //     {
            //         float bias = biases[i][j];

            //         //mutate bias value 
            //         float randomNumber = UnityEngine.Random.Range(0f, 100f);

            //         if (randomNumber <= 1f)
            //         {
            //             bias *= -1f;
            //         }
            //         else if (randomNumber <= 3f)
            //         {
            //             bias = UnityEngine.Random.Range(-0.5f, 0.5f);
            //         }
            //         else if (randomNumber <= 5f)
            //         {
            //             bias += UnityEngine.Random.Range(-1f, 1f);
            //         }

            //         biases[i][j] = bias;
            //     }
            // }
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

        public string GetJsonData()
        {
            var data = new SerializedNetworkData(this);
            var jsonData = JsonConvert.SerializeObject(data);

            return jsonData;
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