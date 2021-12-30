using System.Collections.Generic;
using System;
using UnityEngine;


public class NeuralNetwork : IComparable<NeuralNetwork>
{
	CharacterSpawner charSpawner;
	private int[] layers;
	private float[][] neurons;
	private float[][][] weights;
	public int identifier;
	public Color characterColor;

	private float fitness;

	public NeuralNetwork(int[] layers)
	{
		charSpawner = GameObject.Find("MANAGERS").GetComponent<CharacterSpawner>();
		characterColor = new Color(UnityEngine.Random.Range(0.15f, 0.85f), UnityEngine.Random.Range(0.15f, 0.85f), UnityEngine.Random.Range(0.15f, 0.85f), 1.0f);

		do
		{
			identifier = UnityEngine.Random.Range(10000, 99999);
		} while (charSpawner.allIdentifiers.Contains(identifier));

		this.layers = new int[layers.Length];
		for (int i = 0; i < layers.Length; i++)
		{
			this.layers[i] = layers[i];
		}

		//Generate arrays and matrixes
		InitNeurons(); //Initialize neurons 
		InitWeights(); //Initialize weights
	}

	/// <summary>
	/// Deep copy constructor
	/// </summary>
	/// <param name="copyNetwork"></param>
	public NeuralNetwork(NeuralNetwork copyNetwork)
	{
		characterColor = copyNetwork.characterColor;

		identifier = copyNetwork.identifier;
		//Debug.Log("Identifier: " + identifier);
		this.layers = new int[copyNetwork.layers.Length];
		for (int i = 0; i < copyNetwork.layers.Length; i++)
		{
			this.layers[i] = copyNetwork.layers[i];
		}

		//Generate arrays and matrixes
		InitNeurons();
		InitWeights();
		CopyWeights(copyNetwork.weights);
	}

	public int GetIdentifier()
	{
		return identifier;
	}

	private void CopyWeights(float[][][] copyWeights)
	{
		for (int i = 0; i < weights.Length; i++)
		{
			for (int j = 0; j < weights[i].Length; j++)
			{
				for (int k = 0; k < weights[i][j].Length; k++)
				{
					weights[i][j][k] = copyWeights[i][j][k];
				}
			}
		}
	}

	private void InitNeurons()
	{ 
		List<float[]> neuronsList = new List<float[]>();
		
		for (int i = 0; i < layers.Length; i++)
		{
			neuronsList.Add(new float[layers[i]]);
		}

		neurons = neuronsList.ToArray();
	}

	private void InitWeights()
	{
		List<float[][]> weightsList = new List<float[][]>();

		for (int i = 1; i < layers.Length; i++)
		{
			List<float[]> layerWeightList = new List<float[]>();

			int neuronsInPreviousLayer = layers[i - 1];

			for (int j = 0; j < neurons[i].Length; j++)
			{
				float[] neuronWeights = new float[neuronsInPreviousLayer]; //neuron weights

				//Set the weights randomly between 1 and -1
				for (int k = 0; k < neuronsInPreviousLayer; k++)
				{
					//Give random weights to neuron weights
					neuronWeights[k] = UnityEngine.Random.Range(-0.5f, 0.5f); //Get a random number between -0.5f and 0.5f
				}

				layerWeightList.Add(neuronWeights); //Add neuron weights of this layer to layer weights
			}

			weightsList.Add(layerWeightList.ToArray()); //Add this layers weights converted into 2D array into weights list
		}

		weights = weightsList.ToArray(); //convert to 3D array
	}

	public float[] FeedForward(float[] inputs)
	{
		for (int i = 0; i < inputs.Length; i++)
		{
			neurons[0][i] = inputs[i];
		}

		for (int i = 1; i < layers.Length; i++)
		{
			for (int j = 0; j < neurons[i].Length; j++)
			{
				float value = 0f;

				for (int k = 0; k < neurons[i-1].Length; k++)
				{
					value += weights[i - 1][j][k] * neurons[i - 1][k];
				}

				neurons[i][j] = (float)Math.Tanh(value);
			}
		}

		//Return the last neurons (ie. lenght -1 = LAST LAYER as in OUTPUT layer).
		return neurons[neurons.Length-1];
	}

	//Mutate networks weights based on random chance
	public int Mutate()
	{
		int mutateCount = 0;
		//Iterate through all the layers in the weight matrix
		for (int i = 0; i < weights.Length; i++)
		{
			//Iterate through all the current neruons
			for (int j = 0; j < weights[i].Length; j++)
			{
				//Iterate through all connections that neuron is connected to in the previous layer
				for (int k = 0; k < weights[i][j].Length; k++)
				{
					float weight = weights[i][j][k];

					//Mutate the weight value 
					float randomNumber = UnityEngine.Random.Range(0,1000);

					if(randomNumber <= 2f)
					{
						mutateCount++;
						//Debug.Log("Mutate 1");
						RerollIndentifier();

						//if 1
						//flip sign of weight
						weight *= -1f;
					}
					else if (randomNumber <= 4f)
					{
						mutateCount++;
						//Debug.Log("Mutate 2");
						RerollIndentifier();

						//if 1
						//Set a random number between -1 and 1
						weight = UnityEngine.Random.Range(-0.5f,0.5f);
					}
					else if (randomNumber <= 6f)
					{
						mutateCount++;
						//Debug.Log("Mutate 3");
						RerollIndentifier();

						//if 1
						//randomly increase weight by 0% to 100%
						float factor = UnityEngine.Random.Range(0f, 1f) + 1f;
						weight *= factor;
					}
					else if (randomNumber <= 8f)
					{
						mutateCount++;
						//Debug.Log("Mutate 4");
						RerollIndentifier();
						//if 1
						//randomly decrease by 0% to 100%
						float factor = UnityEngine.Random.Range(0f, 1f);
						weight *= factor;
					}

					weights[i][j][k] = weight;
				}
			}
		}
		return mutateCount;
	}

	void RerollIndentifier()
	{
		identifier = UnityEngine.Random.Range(10000, 99999);
	}

	public void AddFitness(float fit)
	{
		fitness += fit;
	}

	public void SetFitness(float fit)
	{
		fitness = fit;
	}

	public float GetFitness()
	{
		return fitness;
	}

	public int CompareTo(NeuralNetwork other)
	{
		if (other == null) return 1;

		if (fitness > other.fitness)
			return 1;
		else if (fitness < other.fitness)
			return -1;
		else
			return 0;
	}
}
