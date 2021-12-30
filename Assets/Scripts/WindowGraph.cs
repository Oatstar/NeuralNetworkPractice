using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WindowGraph : MonoBehaviour
{

	public Sprite circleSprite;
	private RectTransform graphContainer;
	private RectTransform labelTemplateX;
	private RectTransform labelTemplateY;
	//private TMP_Text titleText;
	public int windowTypeActive;

	//public TMP_Text[] legendTexts;
	//public GameObject[] legendIcons;

	private int listType;

	private int maxValue;

	public static WindowGraph instance;

	private void Awake()
	{
		instance = this;

		//titleText = GameObject.Find("GraphTitle").GetComponent<TMP_Text>();
	}

	public void EmptyGraphs()
	{
		GameObject[] graphIcons = GameObject.FindGameObjectsWithTag("GraphIcon");
		for (int i = 0; i < graphIcons.Length; i++)
		{
			Destroy(graphIcons[i]);
		}

		//for (int i = 1; i < legendIcons.Length; i++)
		//{
		//	legendIcons[i].SetActive(false);
		//}
	}

	public void RefreshGraph(List<int> listGraph, int type, string textType)
	{

		//legendIcons[type].SetActive(true);


		//legendTexts[type].text = textType;

		CheckForMaxValue(listGraph, type);

		//Destroy all previous graph icons.
		graphContainer = this.GetComponent<RectTransform>();
		labelTemplateX = graphContainer.Find("LabelTemplateX").GetComponent<RectTransform>();
		labelTemplateY = graphContainer.Find("LabelTemplateY").GetComponent<RectTransform>();

		TitleTextPrinter(type, textType);
		listType = type;
		ShowGraph(listGraph, maxValue);

	}

	void CheckForMaxValue(List<int> listGraph, int type)
	{
		//Check for max Y value by checking the list for the maximum value. Increments at 100, 1000 and 10 000.
		//int topValue = 100;
		maxValue = 100;

		int[] topValues = new int[] { 100, 1000, 10000, 100000 };
		if (type == 1 && listGraph.Count > 1)
		{
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < listGraph.Count; j++)
				{
					if (listGraph[j] >= topValues[i])
					{
						//Debug.Log("maxvalue found in: " + j + "with value: " + listGraph[j]);
						maxValue = topValues[i + 1];
					}
				}
			}

		}
	}

	void TitleTextPrinter(int type, string textType)
	{
		//if (type == 1 && textType == "Total") { titleText.text = "Passengers"; }
		//if (type == 1 && textType == "Space Corn") { titleText.text = "Food"; }
	}

	private GameObject CreateCircle(Vector2 anchoredPosition)
	{
		GameObject gameObject = new GameObject("circle", typeof(Image));
		gameObject.tag = "GraphIcon";
		gameObject.transform.SetParent(graphContainer, false);
		gameObject.GetComponent<Image>().sprite = circleSprite;
		RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
		rectTransform.anchoredPosition = anchoredPosition;
		rectTransform.sizeDelta = new Vector2(2, 2);
		rectTransform.anchorMin = new Vector2(0, 0);
		rectTransform.anchorMax = new Vector2(0, 0);
		return gameObject;
	}

	private void ShowGraph(List<int> valueList, float maxYValue)
	{
		maxYValue = 100;
		int maxXValue = 100;

		float graphHeight = graphContainer.sizeDelta.y;
		float yMaximum = maxYValue;
		float xPosition;
		float xSize = graphContainer.sizeDelta.x / maxXValue;

		GameObject lastCircleGameObject = null;
		int j = 0;
		if (valueList.Count > maxXValue)
		{
			j = valueList.Count - maxXValue;
		}

		for (int i = j; i < valueList.Count; i++)
		{
			int monthLabel = 1;
			int yearLabel = 1;

			if (valueList.Count < maxXValue)
			{
				xPosition = xSize + i * xSize;
			}
			else
			{
				xPosition = xSize + (i - j) * xSize;
			}
			float yPosition = (valueList[i] / yMaximum) * graphHeight;
			GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition));
			if (lastCircleGameObject != null)
			{
				CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition);
			}
			lastCircleGameObject = circleGameObject;
			if (listType == 1)
			{
				RectTransform labelX = Instantiate(labelTemplateX);
				labelX.tag = "GraphIcon";
				labelX.SetParent(graphContainer);
				labelX.gameObject.SetActive(true);
				labelX.anchoredPosition = new Vector2(xPosition, -10f);

				monthLabel = i + 1;

				for (int k = 1; k < monthLabel; k++)
				{
					if (k % 12 == 0)
					{
						yearLabel++;
					}
				}
				monthLabel = monthLabel - ((yearLabel - 1) * 12);

				labelX.GetComponent<TMP_Text>().text = (monthLabel).ToString() + "/" + (yearLabel).ToString();

			}

		}
		//ONLY if listType == 1, then generate Y-axis and templates. Always add 1 type when opening/creating graph.
		if (listType == 1)
		{
			int separatorCount = 10;
			for (int i = 0; i < separatorCount + 1; i++)
			{
				RectTransform labelY = Instantiate(labelTemplateY);
				labelY.tag = "GraphIcon";
				labelY.SetParent(graphContainer);
				labelY.gameObject.SetActive(true);
				float normalizedValue = i * 1f / separatorCount;
				labelY.anchoredPosition = new Vector2(-23f, normalizedValue * graphHeight);
				labelY.GetComponent<TMP_Text>().text = Mathf.RoundToInt(normalizedValue * yMaximum).ToString();
			}
		}


	}

	private void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB)
	{
		GameObject gameObject = new GameObject("dotConnection", typeof(Image));
		gameObject.tag = "GraphIcon";
		gameObject.transform.SetParent(graphContainer, false);

		if (listType == 1)
		{
			gameObject.GetComponent<Image>().color = new Color(0, 0, 1, .5f);
		}
		else
		if (listType == 2)
		{
			gameObject.GetComponent<Image>().color = new Color(0, 1, 0, .5f);
		}
		else
		if (listType == 3)
		{
			gameObject.GetComponent<Image>().color = new Color(1, 0, 1, .5f);
		}
		else
		if (listType == 4)
		{
			gameObject.GetComponent<Image>().color = new Color(1, 0.92f, 0.016f, .5f);
		}

		RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
		Vector2 dir = (dotPositionB - dotPositionA).normalized;
		float distance = Vector2.Distance(dotPositionA, dotPositionB);
		rectTransform.anchorMin = new Vector2(0, 0);
		rectTransform.anchorMax = new Vector2(0, 0);
		rectTransform.sizeDelta = new Vector2(distance, 1f);
		rectTransform.anchoredPosition = dotPositionA + dir * distance * .5f;

		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		rectTransform.localEulerAngles = new Vector3(0, 0, angle);
	}

}
