using System;
using System.Collections.Generic;
using Default;

// Training

[Serializable]
public class TrainingEvaluationData
{
    public List<TrainingEvaluation> Evaluations;

    public TrainingEvaluationData()
    {
        Evaluations = new();
    }
}

[Serializable]
public class TrainingEvaluation
{
    public float LearningRate;
    public List<float> FitnessConvergence;

    public TrainingEvaluation(float learningRate)
    {
        LearningRate = learningRate;

        FitnessConvergence = new();
    }
}

[Serializable]
public class EvolutionEvaluationData
{
    public string _info;
    public string _additionalNotes;
    public List<EvolutionEvaluation> Evaluations;

    public EvolutionEvaluationData()
    {
        Evaluations = new();
    }
}

[Serializable]
public class EvolutionEvaluation
{
    public List<int> LayerStructure;
    public List<float> FitnessConvergence;
    public SerializedNetworkData fittestNetwork;

    public EvolutionEvaluation()
    {
        LayerStructure = new();
        FitnessConvergence = new();
    }

    public EvolutionEvaluation(NeuralNetwork fittestNetwork)
    {
        LayerStructure = new();
        FitnessConvergence = new();

        this.fittestNetwork = new SerializedNetworkData(fittestNetwork);
    }
}
