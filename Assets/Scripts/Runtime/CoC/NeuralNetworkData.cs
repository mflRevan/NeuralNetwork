using UnityEngine;

namespace Default
{
    [CreateAssetMenu(fileName = "NeuralNetworkData", menuName = "Neural Network/Data")]
    public class NeuralNetworkData : ScriptableObject
    {
        public NeuralNetwork Network;

        public void StoreNetwork(NeuralNetwork network)
        {
            Network = new NeuralNetwork(network);
        }
    }
}