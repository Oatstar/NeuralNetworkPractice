using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GenerationManager : MonoBehaviour
{
	[SerializeField] GameObject livingArea;
	[SerializeField] WorldManager worldManager;
	[SerializeField] CharacterSpawner characterSpawner;
	int currentTicks = 100;
	int startingTicks = 100;
	int currentGeneration = 0;
	[SerializeField] TMP_Text currentGenHeader;
	[SerializeField] TMP_Text timerText;
	[SerializeField] Button startGenButton;
	[SerializeField] Button repopulateButton;

	[SerializeField] LayerMask charLayer;

	public int spawnCountPerGeneration = 100;
	public Vector3 m_DetectorOffset = Vector3.zero;


	private void Start()
	{
		Time.timeScale = 0;
	}


	public void StartGeneration()
	{
		if(currentGeneration== 0)
		{
			characterSpawner.SpawnInitialCharacters(spawnCountPerGeneration);
			currentGeneration = 1;
		}
		else
		{
			currentGeneration += 1;
		}

		currentGenHeader.text = "Gen: "+ currentGeneration.ToString();


		Debug.Log("STARTING NEW GENERATION by player input");
		Time.timeScale = 1;
		StartCoroutine(CountTime());
	}

	IEnumerator CountTime()
	{
		yield return new WaitForSeconds(0.05f);
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
		if (Input.GetKeyDown(KeyCode.R))
			EndGeneration();
		if (Input.GetKeyDown(KeyCode.T))
			StartGeneration();

		if (Input.GetKeyDown(KeyCode.Space))
			PauseToggle();	
    }

	void PauseToggle()
	{
		if (Time.timeScale != 0)
			Time.timeScale = 0;
		else
			Time.timeScale = 1;
	}

	void EndGenerationOnTimer()
	{
		SetTimer(startingTicks);
		Time.timeScale = 0;
		Debug.Log("Generation ended on timer");
	}

	void SetTimer(int value)
	{
		currentTicks = value;
		timerText.text = "Timer: " + currentTicks.ToString();
	}

	public void EndGeneration()
	{
		Debug.Log("Generation terminated on player input");
		currentGenHeader.text = (currentGeneration + 1).ToString();
		List<GameObject> survivors = RayCastLivingArea();
		characterSpawner.Repopulate(survivors);
	}

	List<GameObject> RayCastLivingArea()
	{
		Collider2D[] areaObjects = Physics2D.OverlapBoxAll(livingArea.transform.localPosition, livingArea.transform.localScale,0,  charLayer);

		List<GameObject> collidedCharacters = new List<GameObject>();

		for (int i = 0; i < areaObjects.Length; i++)
		{
			if (areaObjects[i].tag == "Character")
				collidedCharacters.Add(areaObjects[i].transform.gameObject);
		}

		for (int i = 0; i < collidedCharacters.Count; i++)
		{
			//Debug.Log("collided: " + collidedCharacters[i].tag);
			collidedCharacters[i].transform.GetComponentInChildren<SpriteRenderer>().color = Color.green;
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
		Gizmos.DrawWireCube(livingArea.transform.localPosition, livingArea.transform.localScale);
	}

}
