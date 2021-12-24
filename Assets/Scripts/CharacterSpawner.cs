using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CharacterSpawner : MonoBehaviour
{
	float maxX = 99;
	float maxY = 99;

	[SerializeField] GameObject characterContainer;
	[SerializeField] GameObject characterPrefab;
	GenerationManager genManager;
	public TMP_Text allDeadText;
	public TMP_Text identifierTexts1;
	public TMP_Text identifierTexts2;
	public TMP_Text identifierTexts3;


	public List<int> allIdentifiers = new List<int> { };

	List<GameObject> allCharacters = new List<GameObject>();

	public void ReCollectIdentifiers()
	{
		allIdentifiers.Clear();

		foreach (GameObject character in allCharacters)
		{
			int currentIdentifier = character.GetComponent<CharacterManager>().GetIdentifier();
			if (!allIdentifiers.Contains(currentIdentifier))
				allIdentifiers.Add(currentIdentifier);
		}
	}

	private void Awake()
	{
		genManager = GameObject.Find("MANAGERS").GetComponent<GenerationManager>();
	}

	public void StartSimulationOnAllChars()
	{
		for (int i = 0; i < allCharacters.Count; i++)
		{
			StartCoroutine(allCharacters[i].transform.GetComponent<CharacterManager>().RefreshMovement());
		}
	}

	public void SpawnInitialCharacters(int spawnCount)
	{
		allCharacters.Clear();
		for (int i = 0; i < spawnCount; i++)
		{
			int xPos = Mathf.RoundToInt(Random.Range(-maxX, maxX));
			int yPos = Mathf.RoundToInt(Random.Range(-maxY, maxY));
			Vector2 randomPos = new Vector2(xPos, yPos);

			GameObject spawnedChar = Instantiate(characterPrefab, randomPos, Quaternion.identity);
			spawnedChar.transform.SetParent(characterContainer.transform);

			allCharacters.Add(spawnedChar);
			spawnedChar.transform.GetComponent<CharacterManager>().InitializeSelf();
		}
	}

	//Repopulate the world for the next generation with previous generations neural networks
	public void Repopulate(List<GameObject> survivors)
	{
		List<NeuralNetwork> nextGen = BuildNextGeneration(survivors);
		KillAllCharacters();
		CreateNewGeneration(nextGen);
	}


	void KillAllCharacters()
	{
		Debug.Log("Killing current generation");

		//Kill each character that is not in survivors-list.
		foreach (GameObject character in allCharacters)
		{
			Destroy(character);
		}
	}

	List<NeuralNetwork> BuildNextGeneration(List<GameObject> survivors)
	{
		List<NeuralNetwork> nextGenNeuralNetworks = new List<NeuralNetwork>();

		//Build up a cache of neural networks that had the surviving brains
		for (int i = 0; i < survivors.Count; i++)
		{
			nextGenNeuralNetworks.Add(survivors[i].GetComponent<CharacterManager>().neuralNetwork);
		}

		return nextGenNeuralNetworks;
	}

	void CreateNewGeneration(List<NeuralNetwork> nextGenNeuralNetworks)
	{
		allDeadText.text = " ";

		allCharacters.Clear();

		Debug.Log("----");
		Debug.Log("Creating and spawning new generation from "+nextGenNeuralNetworks.Count + " survivors");
		Debug.Log("----");


		int spawnCount = genManager.spawnCountPerGeneration;
		int networkIndex = 0;

		for (int i = 0; i < spawnCount; i++)
		{
			if (nextGenNeuralNetworks.Count == 0)
			{
				allDeadText.text = "All characters died. Restart simulation";
			}
			else
			{
				//If i is larger than previous generations neural network count,
				//iterate trough the previousGeneration again starting from i = 0
				if (i >= nextGenNeuralNetworks.Count)
				{
					//Check how many times i is included in previousGeneration.Count and use
					// it as the amount of cycles when floored to int.
					int cycleCount = 0;
					cycleCount = Mathf.FloorToInt(i / nextGenNeuralNetworks.Count);

					//Remove amount of cycles per previousGenerations count from i.
					networkIndex = i - (nextGenNeuralNetworks.Count * cycleCount);
					//Debug.Log("networkindex: " + networkIndex);
				}
				else
				{
					//if i < previousGeneration.Count use 
					networkIndex = i;
				}

				int xPos = Mathf.RoundToInt(Random.Range(-maxX, maxX));
				int yPos = Mathf.RoundToInt(Random.Range(-maxY, maxY));
				Vector2 randomPos = new Vector2(xPos, yPos);

				GameObject spawnedChar = Instantiate(characterPrefab, randomPos, Quaternion.identity);
				spawnedChar.transform.SetParent(characterContainer.transform);
				//Debug.Log("using index: " + networkIndex);

				spawnedChar.transform.GetComponentInChildren<SpriteRenderer>().color = nextGenNeuralNetworks[networkIndex].characterColor;

				allCharacters.Add(spawnedChar);
				spawnedChar.transform.GetComponent<CharacterManager>().LoadParentsNeuralNetwork(nextGenNeuralNetworks[networkIndex]);
			}
		}

		ReCollectIdentifiers();
		RefreshIdentifierText();
	}

	void RefreshIdentifierText()
	{
		identifierTexts1.text = "";
		identifierTexts2.text = "";
		identifierTexts3.text = "";


		for (int i = 0; i < allIdentifiers.Count; i++)
		{
			if(i <33)
				identifierTexts1.text = identifierTexts1.text + allIdentifiers[i] + " \n";
			else if(i < 66)
				identifierTexts2.text = identifierTexts2.text + allIdentifiers[i] + " \n";
			else
				identifierTexts3.text = identifierTexts3.text + allIdentifiers[i] + " \n";

		}
	}
}
