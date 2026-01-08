using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Stopwatch : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI stopwatchText;
	
	private float elapsedTime;
	private bool isStart = false;
	
    void Update()
    {
		if (isStart){
			elapsedTime += Time.deltaTime;
			int minutes = Mathf.FloorToInt(elapsedTime / 60);
			int seconds = Mathf.FloorToInt(elapsedTime % 60);
			stopwatchText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
		}
    }
	
	public void startStopwatch(){
		isStart = true;
	}
	
	public void stopStopwatch(){
		isStart = false;
	}
}
