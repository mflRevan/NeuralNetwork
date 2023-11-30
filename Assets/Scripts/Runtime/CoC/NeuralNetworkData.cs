using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Default
{
    [CreateAssetMenu(fileName = "NeuralNetworkData", menuName = "Neural Network/Data")]
    public class NeuralNetworkData : ScriptableObject
    {
        public NeuralNetwork FittestNetwork;
        public TrainingData TrainingData;


        public void StoreNetwork(NeuralNetwork network)
        {
            FittestNetwork = new NeuralNetwork(network);

            Debug.Log($"Stored Network with:\nFitness: {FittestNetwork.GetFitness()}\nWeights length: {FittestNetwork.weights.Length} + {FittestNetwork.weights[0].Length}\nNeurons: {FittestNetwork.neurons}\nBiases: {FittestNetwork.biases}");
        }

        public void CreateNeuralNetwork(int[] layers)
        {
            FittestNetwork = new NeuralNetwork(layers);
        }

        public void ClearTrainingData()
        {
            TrainingData = new();
        }
    }

    [Serializable]
    public class TrainingData
    {
        public List<Dataset> Data;


        public TrainingData()
        {
            Data = new();
        }

        public void AddData(Dataset dataset)
        {
            Data.Add(dataset);
        }
    }

    [Serializable]
    public class Dataset
    {
        [SerializeField] public float[] Inputs;
        [SerializeField] public float[] Outputs;

        public Dataset(float[] inputs, float[] outputs)
        {
            this.Inputs = inputs;
            this.Outputs = outputs;
        }
    }
}