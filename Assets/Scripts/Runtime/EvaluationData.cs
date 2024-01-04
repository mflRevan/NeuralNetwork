using System;
using System.Collections.Generic;

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
public class EvolutionEvaluation
{
    public List<int> LayerStructure;
    public List<float> FitnessConvergence;

    public EvolutionEvaluation()
    {
        LayerStructure = new();
        FitnessConvergence = new();
    }
}
