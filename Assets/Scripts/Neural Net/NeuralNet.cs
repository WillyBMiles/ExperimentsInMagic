using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NeuralNet
{
    readonly List<InputNode> inputs = new();
    readonly List<OutputNode> outputs = new();
    readonly List<List<Node>> layers = new();

    System.Random random;

    public void CreateNet(IReadOnlyList<InputNode> inputNodes, IReadOnlyList<OutputNode> outputNodes, int hiddenLayers)
    {
        random = new();

        inputs.Clear();
        inputs.AddRange(inputNodes);
        outputs.Clear();
        outputs.AddRange(outputNodes);

        List<Node> lastLayer = inputs.Select(node => node as Node).ToList();

        //For each hidden layer
        for (int i =0; i < hiddenLayers; i++)
        {
            List<Node> listOfNodes = new();
            //make a number of nodes equal to the input
            inputs.ForEach(_ => { 
                Node node = new();
                //Set their weights randomly
                node.SetInputWeights(lastLayer, random);
                listOfNodes.Add(node);
                
            });
            //add that layer
            layers.Add(listOfNodes);
            lastLayer = listOfNodes;
        }
        //Outputs use the last layer
        //Outputs are NOT in the layer list
        outputs.ForEach(node => node.SetInputWeights(lastLayer, random));
    }

    public Dictionary<OutputNode, double> GenerateOutput()
    {
        Dictionary<Node, double> allWeights = new();
        foreach (var node in inputs)
        {
            allWeights[node] = node.GetValue();
        }
        foreach (var layer in layers)
        {
            foreach (var node in layer)
            {
                allWeights[node] = node.CalculateNode(allWeights);
            }
        }
        foreach (var output in outputs)
        {
            allWeights[output] = output.CalculateNode(allWeights);
        }

        return allWeights.Where(kvp => outputs.Contains(kvp.Key)).ToDictionary(kvp => kvp.Key as OutputNode, kvp => kvp.Value);
    }

}


public class Node
{
    //Weight can be changed if certain nodes should have more effects on things,
    //Should be rather low because of weight propogation
    public double weightMult = 1.0; 
    public Dictionary<Node, double> inputWeights = new(); //none if Node is an inputNode
    public double CalculateNode(IReadOnlyDictionary<Node, double> allWeights)
    {
        double output = 0.0;
        foreach (var node in inputWeights)
        {
            output += allWeights[node.Key] * node.Value;
        }
        return Sigmoid(output);
    }

    public void SetInputWeights(List<Node> inputNodes, System.Random random)
    {
        foreach (var node in inputNodes)
        {
            inputWeights[node] = GenerateBaseWeight(node, random);
        }

        double sum = inputWeights.Select(kvp => Math.Abs(kvp.Value)).Sum();
        double scale = sum / 2.0;
        foreach (var node in inputWeights.Keys.ToArray())
        {
            inputWeights[node] /= scale;
        }
    }
    
    static double Sigmoid(double input)
    {
        //Sigmoid Centered around 0
        return (1.0 / (1.0 + Math.Pow(Math.E, -input)) - .5) * 2.0;
    }

    //Higher means more like a trained model, more predictable
    const double WEIGHT_POWER = 3.0; 
    public double GenerateBaseWeight(Node inputNode, System.Random random)
    {
        return Math.Pow((random.NextDouble() - .5) * 2.0, WEIGHT_POWER) * inputNode.weightMult;
    }
}

public abstract class InputNode : Node
{
    public abstract double GetValue();
}

public class OutputNode : Node
{
}