using System;
using System.Collections.Generic;
using UnityEngine;

namespace Default
{
    [CreateAssetMenu(fileName = "NeuralNetworkData", menuName = "Neural Network/Data"), Serializable]
    public class NeuralNetworkData : ScriptableObject
    {
        [TextArea] public string fittestNetworkData_Evolution;
        [TextArea] public string fittestNetworkData_Training;

        [Space]
        public List<Dataset> TrainingData;


        public void StoreNetwork(NeuralNetwork network, bool evolutionNetwork)
        {
            if (evolutionNetwork)
            {
                fittestNetworkData_Evolution = network.GetJsonData();
            }
            else
            {
                fittestNetworkData_Training = network.GetJsonData();
            }

            Debug.Log($"Saved data:\n{fittestNetworkData_Evolution}\nas " + (evolutionNetwork ? "evolution" : "training"));
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