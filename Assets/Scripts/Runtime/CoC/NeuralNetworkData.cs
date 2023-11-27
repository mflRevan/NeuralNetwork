using System;
using System.Collections.Generic;
using UnityEngine;

namespace Default
{
    [CreateAssetMenu(fileName = "NeuralNetworkData", menuName = "Neural Network/Data")]
    public class NeuralNetworkData : ScriptableObject
    {
        public NeuralNetwork FittestNetwork;
        public List<float[]> TrainingDataset;


        public void StoreNetwork(NeuralNetwork network)
        {
            FittestNetwork = new NeuralNetwork(network);
        }
    }

    [Serializable]
    public class TrainingData
    {
        public readonly float[] InputData;
        public readonly float[] OutputData;


        public TrainingData(float[] inputData, float[] outputData)
        {
            InputData = inputData;
            OutputData = outputData;
        }
    }
}