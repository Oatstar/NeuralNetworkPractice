using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GenerationManager : MonoBehaviour
{
	float tickTime = 0.05f;
	[SerializeField] int simulationTimeModifier = 1;
	[SerializeField] GameObject[] livingAreas;
	[SerializeField] WorldManager worldManager;
	[SerializeField] CharacterSpawner characterSpawner;
	int currentTicks = 250;
	int startingTicks = 250;
	int currentGeneration = 0;
	[SerializeField] TMP_Text currentGenHeader;
	[SerializeField] TMP_Text timerText;
	[SerializeField] Button startGenButton;
	[SerializeField] Button repopulateButton;
	[SerializeField] Button simulateXButton;

	public List<int> survivorCountByGeneration = new List<int> { };

	[SerializeField] LayerMask charLayer;

	public int spawnCountPerGeneration = 100;
	public Vector3 m_DetectorOffset = Vector3.zero;
	public TMP_Text timeScaleText;
	public TMP_Text survivorCountText;
	List<GameObject> survivors;
	float currentTime;
	public TMP_InputField cycleCountInputField;

	[SerializeField] bool readyToStartSimulation = true;
	public bool generationOnGoing = false;

	private void Start()
	{
		currentTicks = startingTicks;
		Time.timeScale = 0;

		RefreshButtonListeners();
	}

	public float GetTickTime()
	{
		return tickTime;
	}

	public float GetSimulationTimeModifier()
	{
		return simulationTimeModifier;
	}

	public void SimulateForwardOnButton()
	{
		int cycleCount = 5;

		if (cycleCountInputField.text != "")
			cycleCount = int.Parse(cycleCountInputField.text);
		
		Debug.Log("cyclecount: " + cycleCount);
		StartCoroutine(SimulateFowardX(cycleCount));
	}

	public void ReloadScene()
	{
		SceneManager.LoadScene("SampleScene");
	}

	IEnumerator SimulateFowardX(int cycleCount)
	{
		for (int i = 0; i < cycleCount; i++)
		{
			StartGeneration();
			SetTimeScale(10);
			while (generationOnGoing)
			{
				yield return new WaitForEndOfFrame();
				//yield return new WaitForSeconds(0.1f);
			}

			if(i < cycleCount - 1)
				EndGeneration();
		}
	}

	void RefreshButtonListeners()
	{
		
		if (generationOnGoing)
		{
			startGenButton.interactable = false;
			repopulateButton.interactable = false;
			simulateXButton.interactable = false;
		}
		if (!readyToStartSimulation && !generationOnGoing & GetCurrentTicks() <= 0)
		{
			startGenButton.interactable = false;
			repopulateButton.interactable = true;
			simulateXButton.interactable = false;
		}
		else if(readyToStartSimulation && !generationOnGoing)
		{
			startGenButton.interactable = true;
			repopulateButton.interactable = false;
			simulateXButton.interactable = true;
		}
	}

	public void StartGeneration()
	{
		readyToStartSimulation = false;
		generationOnGoing = true;

		RefreshButtonListeners();

		currentTime = 0;
		if (currentGeneration== 0)
		{
			characterSpawner.SpawnInitialCharacters(spawnCountPerGeneration);
			currentGeneration = 1;
		}
		else
		{
			currentGeneration += 1;
		}

		currentGenHeader.text = "Gen: "+ currentGeneration.ToString();

		characterSpawner.StartSimulationOnAllChars();
		//Debug.Log("STARTING NEW GENERATION by player input");
		SetTimeScale(simulationTimeModifier);
		
		StartCoroutine(CountTime());
	}

	IEnumerator CountTime()
	{
		//yield return new WaitForSeconds(tickTime);
		yield return new WaitForSeconds(tickTime);
		int ticks = currentTicks;
		ticks--;
		SetTimer(ticks);

		if (currentTicks <= 0)
			EndGenerationOnTimer();
		else
			StartCoroutine(CountTime());
	}
		
	void Update()
    {
		if (Input.GetKeyDown(KeyCode.Alpha1))
			SetTimeScale(1);
		if (Input.GetKeyDown(KeyCode.Alpha2))
			SetTimeScale(2);
		if (Input.GetKeyDown(KeyCode.Alpha3))
			SetTimeScale(3);
		if (Input.GetKeyDown(KeyCode.Alpha4))
			SetTimeScale(5);
		if (Input.GetKeyDown(KeyCode.Alpha5))
			SetTimeScale(10);
		if (Input.GetKeyDown(KeyCode.Alpha6))
			SetTimeScale(20);
		//if (Input.GetKeyDown(KeyCode.R))
		//	EndGeneration();
		//if (Input.GetKeyDown(KeyCode.T))
		//	StartGeneration();

		//if (Input.GetKeyDown(KeyCode.Space))
		//	PauseToggle();	
	}

	void SetTimeScale(int timeScale)
	{
		if (generationOnGoing)
		{
			simulationTimeModifier = timeScale;
			Time.timeScale = timeScale;
			timeScaleText.text = "TimeScale: " + timeScale;
		}
	}

	void PauseToggle()
	{
		if (Time.timeScale != 0)
			Time.timeScale = 0;
		else
			Time.timeScale = simulationTimeModifier;
	}

	void EndGenerationOnTimer()
	{
		
		Time.timeScale = 0;
		generationOnGoing = false;
		RefreshButtonListeners();

		SetTimer(startingTicks);
		CountAllSurvivors();
		survivorCountByGeneration.Add(survivors.Count);

		survivorCountText.text = "Survivors: " + survivors.Count;
		
		Debug.Log("Generation ended on timer");
	}

	void CountAllSurvivors()
	{
		survivors = RayCastLivingArea();
		//WindowGraph.instance.RefreshGraph(survivorCountByGeneration, 0, "Survivors");
	}

	void SetTimer(int value)
	{
		currentTicks = value;
		timerText.text = "Timer: " + currentTicks.ToString();
	}

	public void EndGeneration()
	{
		//Debug.Log("Generation terminated on player input");
		currentGenHeader.text = "Next gen: "+(currentGeneration + 1).ToString();
		characterSpawner.Repopulate(survivors);

		readyToStartSimulation = true;

		RefreshButtonListeners();
	}

	List<GameObject> RayCastLivingArea()
	{
		List<GameObject> collidedCharacters = new List<GameObject>();

		for (int i = 0; i < livingAreas.Length; i++)
		{
			Collider2D[] areaObjects = Physics2D.OverlapBoxAll(livingAreas[i].transform.localPosition, livingAreas[i].transform.localScale, 0, charLayer);

			for (int j = 0; j < areaObjects.Length; j++)
			{
				if (areaObjects[j].tag == "Character")
					collidedCharacters.Add(areaObjects[j].transform.gameObject);
			}

			//Color all survivors green
			//for (int j = 0; j < collidedCharacters.Count; j++)
			//{
			//	//Debug.Log("collided: " + collidedCharacters[i].tag);
			//	collidedCharacters[j].transform.GetComponentInChildren<SpriteRenderer>().color = Color.green;
			//}
		}
		

		return collidedCharacters;
	}


	public float GetCurrentTicks()
	{
		return (float)currentTicks;
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		//Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
		//Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)

		for (int i = 0; i < livingAreas.Length; i++)
		{
			Gizmos.DrawWireCube(livingAreas[i].transform.localPosition, livingAreas[i].transform.localScale);
		}
	}

}
