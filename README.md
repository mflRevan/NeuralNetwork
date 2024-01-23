[logo]: https://github.com/mflRevan/NeuralNetwork/blob/main/Public/title.png "Self Driving Car"
[nn]: https://github.com/mflRevan/NeuralNetwork/blob/main/Public/nn.png "Neural Net Visual"

# Self-Driving Car Project in Unity

![Self Driving Car][logo]

## Introduction

This Unity-based project integrates Neural Networks (NNs) and Evolutionary Algorithms (EAs) to create a self-driving car simulation. By simulating natural selection, the project aims to optimize autonomous driving behaviors.

## Technology Stack

- Unity: Provides a robust simulation environment for testing the self-driving car.
- Neural Networks: Employs a layered NN structure for decision-making processes.
- Evolution Algorithms: Optimizes NN by simulating evolutionary processes like mutation and selection.

# Project Structure & Documentation

## Using Neural Networks

![Neural Network][nn]

### Overview
The Neural Network class in this Unity project implements a custom neural network for simulating a self-driving car. It uses a layered structure, with customizable neurons, biases, and weights, and includes methods for training with back-propagation and EAs coupled with mutation and crossover logic.

### Key Features
- Layered Neural Network: Flexible architecture allowing various configurations.
- Evolutionary Algorithm Integration: For optimizing the neural network using genetic algorithm concepts.
- Training and Mutation: Methods to train the network and mutate weights and biases.

### Usage

#### Initialization
```csharp
// Using a layer structure represented by an integer array
int[] layers = new int[] { inputSize, hiddenLayerSize, outputSize };
NeuralNetwork network1 = new NeuralNetwork(layers);

// Using serialized json data from the SerializableNetworkData class
string networkData = "Json Data";
NeuralNetwork network2 = new NeuralNetwork(networkData);

// Using an existing NeuralNetwork instance to deep-copy from (including weights, neurons, fitness etc.)
NeuralNetwork network3 = new NeuralNetwork(network1); // Deep copy of network1
```

#### Training

```csharp
List<Dataset> trainingData = //... load or create your data
await network.Train(trainingData);
```
#### Feed Forward (Inference)

```csharp
float[] inputs = //... input data
float[] outputs = network.FeedForward(inputs);
```

#### Mutation

```csharp
network.Mutate(chanceMultiplier, strengthMultiplier);
```

## Evaluating & Testing



## Results & Observations

- Configuration Comparisons: Effectiveness of different NN structures.
- Optimization Results: Insights into the evolutionary process and NN performance.
- Improvements and Observations: Discussion of observed enhancements and potential areas for further development.

## Conclusion & Outlook

- Project Review: Assessment of current progress compared to initial goals.
- Future Work: Prospective enhancements and areas of further research.

## Sources & Inspirations

- https://machinelearningmastery.com/learning-rate-for-deep-learning-neural-networks
- https://towardsdatascience.com/weight-initialization-techniques-in-neural-networks-26c649eb3b78
- https://www.youtube.com/watch?v=-WjKICvAOsY
- https://www.youtube.com/watch?v=Yq0SfuiOVYE (Basis for the NeuralNetwork)
- From Asset Store: 
    - https://assetstore.unity.com/packages/tools/physics/tiny-car-controller-151827
    - https://assetstore.unity.com/packages/3d/environments/roadways/simple-roads-212360
- https://www.youtube.com/watch?v=eZOHA6Uy52k&t=32
- https://www.analyticsvidhya.com/blog/2023/01/why-is-sigmoid-function-important-in-artificial-neural-networks/
- https://homes.di.unimi.it/azzini/wwwmat/TesiAzziniAntonia.pdf
- A _little_ help and guidance from ChatGPT

### License

This project is open source and available under the Apache 2.0 License, encouraging free use and distribution for both personal and commercial purposes.
