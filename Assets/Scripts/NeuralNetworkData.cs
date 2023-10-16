using UnityEngine;

[CreateAssetMenu(fileName = "NeuralNetworkData", menuName = "Neural Network/Data")]
public class NeuralNetworkData : ScriptableObject
{
    public int[] layers;
    public float[][][] weights;

    public NeuralNetwork Network;

    public void StoreNetwork(NeuralNetwork network)
    {
        Network = new NeuralNetwork(network);
    }
}
