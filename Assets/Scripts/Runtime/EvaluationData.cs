using System;
using System.Collections.Generic;

[Serializable]
public class EvaluationData
{
    public List<Evaluation> Evaluations;

    public EvaluationData()
    {
        Evaluations = new();
    }
}

[Serializable]
public class Evaluation
{
    public float LearningRate;
    public List<float> FitnessConvergence;

    public Evaluation(float learningRate)
    {
        LearningRate = learningRate;

        FitnessConvergence = new();
    }
}