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
		neuralNetwork = parentNetwork;
	}

	private void Start()
	{
		StartCoroutine(RefreshMovement());
	}

	void InitializeNeuralNetwork()
	{
		int[] layers = new int[4] {4, 5, 5, 4 };

		neuralNetwork = new NeuralNetwork(layers);

		
	}

	//Get inputs as distances from edges
	float[] GetInputs()
	{
		//Directions 0 = top, 1 = right, 2 = bot, 3 = left
		float[] inputs = new float[4];
		Vector2 characterPos = this.gameObject.transform.position;

		//Iterate directions and compare only x or y depending on the edge. ie. x = left and right. y = top and bot
		for (int i = 0; i < 4; i++)
		{
			if(i == 0)
				inputs[i] = worldManager.GetEdgePosition(i).y - characterPos.y;
			else
				inputs[i] = worldManager.GetEdgePosition(i).x - characterPos.x;
		}

		inputs[2] = genManager.GetCurrentTicks();
		inputs[3] = worldManager.GetEdgePosition(3).x - characterPos.x;

		return inputs;
	}

	int GetCloseCharactersCount()
	{
		Collider2D[] closestChars = Physics2D.OverlapCircleAll(this.transform.position, 10);
		return 0;
	}

	IEnumerator RefreshMovement()
	{
		yield return new WaitForSeconds(0.05f);

		//Get inputs and outputs
		float[] distancesToEdges = GetInputs();
		output = neuralNetwork.FeedForward(distancesToEdges);
		//Debug.Log("Output[0]: " + output[0]);
		//Debug.Log("Output[1]: " + output[1]);
		//Debug.Log("Output[2]: " + output[2]);
		//Debug.Log("erotus: " + (Mathf.Abs(output[0]) - Mathf.Abs(output[1])));

		float xMovement = 0;
		float yMovement = 0;

		xMovement = output[0];
		yMovement = output[1];

		float absDifferennce = Mathf.Abs(xMovement) - Mathf.Abs(yMovement);

		if (Mathf.Abs(absDifferennce) < 0.01f)
		{
			MoveRandomly();
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

		if (UnityEngine.Random.Range(0, 1) < output[3])
			MoveAgain();

		StartCoroutine(RefreshMovement());
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
		Gizmos.DrawWireSphere(this.transform.position, 10);
	}

}
