using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitingImage : MonoBehaviour
{
	private float totalProgress;
	private Image image;

	public float newFillTime;

	private void Awake()
	{
		image = GetComponent<Image>();
	}
	void Start()
	{
		StartWaitingTime();
	}


	private void StartWaitingTime()
	{
		StartCoroutine(WaitingTime());
	}
	private IEnumerator WaitingTime()
	{
		totalProgress = 0;
		while (totalProgress <= newFillTime)
		{
			totalProgress += Time.unscaledDeltaTime;
			image.fillAmount = totalProgress / newFillTime;
			yield return null;
		}

		StartWaitingTime();
	}
}
