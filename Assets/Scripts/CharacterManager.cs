using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
	Vector2 movementDirection;
	public NeuralNetwork neuralNetwork;
	WorldManager worldManager;
	GenerationManager genManager;
	float[] output;

	LayerMask characterLayer;

	Rigidbody2D rb;

	private void Awake()
	{
		rb = this.GetComponent<Rigidbody2D>();
		worldManager = GameObject.Find("MANAGERS").GetComponent<WorldManager>();
		genManager = GameObject.Find("MANAGERS").GetComponent<GenerationManager>();
	}

	public void InitializeSelf()
	{
		InitializeNeuralNetwork();
	}

	public void LoadParentsNeuralNetwork(NeuralNetwork parentNetwork)
	{
		neuralNetwork = new NeuralNetwork(parentNetwork);

		//neuralNetwork = parentNetwork;
		int mutateCount = neuralNetwork.Mutate();
		if (mutateCount != 0)
			RefreshColorOnMutate();
	}

	void RefreshColorOnMutate()
	{
		Color currentColor = neuralNetwork.characterColor;
		neuralNetwork.characterColor = new Color(currentColor.r+UnityEngine.Random.Range(-0.25f, 0.25f), currentColor.g+ UnityEngine.Random.Range(-0.25f, 0.25f), currentColor.b+ UnityEngine.Random.Range(-0.25f, 0.25f), 1.0f);
		this.transform.GetComponentInChildren<SpriteRenderer>().color = neuralNetwork.characterColor;
	}

	public int GetIdentifier()
	{
		return neuralNetwork.identifier;
	}

	private void Start()
	{
		//StartCoroutine(RefreshMovement());
	}

	void InitializeNeuralNetwork()
	{
		int[] layers = new int[4] {3, 6, 6, 3 };
		neuralNetwork = new NeuralNetwork(layers);

		this.transform.GetComponentInChildren<SpriteRenderer>().color= neuralNetwork.characterColor;
	}

	//Get inputs as distances from edges
	float[] GetInputs()
	{
		int inputAmount = 3;
		//Directions 0 = top, 1 = right, 2 = closeChars, 3 = timeticks
		float[] inputs = new float[inputAmount];
		Vector2 characterPos = this.gameObject.transform.position;

		float input0raw = worldManager.GetEdgePosition(0).y - characterPos.y;
		float input1raw = worldManager.GetEdgePosition(1).x - characterPos.x;
		float input2raw = GetCloseCharactersCount();

		//inputs[3] = genManager.GetCurrentTicks();
		inputs[0] = Scale(input0raw, 0, 200, -1, 1);
		inputs[1] = Scale(input1raw, 0, 200, -1, 1);
		inputs[2] = Scale(input2raw, 0, 25, -1, 1);

		//Debug.Log("Input 0: " + inputs[0]);
		//Debug.Log("Input 1: " + inputs[1]);
		//Debug.Log("Input 2: " + inputs[2]);
		return inputs;
	}


	public float Scale(float OldValue, float OldMin, float OldMax, float NewMin, float NewMax)
	{

		float OldRange = (OldMax - OldMin);
		float NewRange = (NewMax - NewMin);
		float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;

		return (NewValue);
	}


	int GetCloseCharactersCount()
	{
		Collider2D[] closestChars = Physics2D.OverlapCircleAll(this.transform.position, 20);
		//Debug.Log("charcount close: " + closestChars.Length);
		return closestChars.Length;
	}

	public IEnumerator RefreshMovement()
	{
		//Debug.Log("Refresh movement");
		//yield return new WaitForSeconds(genManager.GetTickTime());
		yield return new WaitForSeconds(genManager.GetTickTime());

		//Get inputs and outputs
		float[] distancesToEdges = GetInputs();
		output = neuralNetwork.FeedForward(distancesToEdges);
		//Debug.Log("Output[0]: " + output[0]);
		//Debug.Log("Output[1]: " + output[1]);
		//Debug.Log("Output[2]: " + output[2]);
		//Debug.Log("erotus: " + (Mathf.Abs(output[0]) - Mathf.Abs(output[1])));

		float xMovement = output[0];
		float yMovement = output[1];
		float dontMoveChance = output[2];

		//float absDifferennce = Mathf.Abs(xMovement) - Mathf.Abs(yMovement);
		if (Mathf.Abs(dontMoveChance) < 0.001f)
		{
			//Debug.Log("Output: " + Mathf.Abs(dontMoveChance));
			DontMove();
		}
		else if (Mathf.Abs(xMovement) > Mathf.Abs(yMovement))
		{
			if (xMovement > 0)
				MoveRight();
			else
				MoveLeft();
		}
		else
		{
			if (yMovement > 0)
				MoveUp();
			else
				MoveDown();
		}

		//if (UnityEngine.Random.Range(0, 1) < output[3])
		//	MoveAgain();

		if(genManager.generationOnGoing)
			StartCoroutine(RefreshMovement());
	}

	void DontMove()
	{

	}

	void MoveRandomly()
	{
		int randomDirection = UnityEngine.Random.Range(0, 4);
		switch (randomDirection)
		{
			case 0:
				MoveUp();
				break;
			case 1:
				MoveRight();
				break;
			case 2:
				MoveDown();
				break;
			case 3:
				MoveLeft();
				break;

		}
	}

	bool CharacterInFront()
	{
		//Physics2D.Raycast(this.transform.position, Vector2.down, inf)
		return false;
	}

	void MoveUp()
	{
		movementDirection = new Vector2(0, 1);
		CommitMovement(movementDirection);
	}

	void MoveRight()
	{

		movementDirection = new Vector2(1, 0);
		CommitMovement(movementDirection);
	}

	void MoveDown()
	{
		movementDirection = new Vector2(0, -1);
		CommitMovement(movementDirection);
	}

	void MoveLeft()
	{
		movementDirection = new Vector2(-1, 0);
		CommitMovement(movementDirection);
	}

	void MoveAgain()
	{
		CommitMovement(movementDirection);
	}

	void CommitMovement(Vector2 movementDir)
	{
		Vector3 movementVector = new Vector3(movementDir.x, movementDir.y, 0);
		rb.MovePosition(transform.position + movementVector);
	}



	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		//Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
		//Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
		Gizmos.DrawWireSphere(this.transform.position, 20);
	}

}
