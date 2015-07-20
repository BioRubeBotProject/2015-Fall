using UnityEngine;
using System.Collections;

public class TimeScale : MonoBehaviour 
{
	private float maxFFW = 64.0f;
	private float saveTimeScale = 1.0f;

	public void Start()
	{ 
		Time.timeScale = 0; 
	}

	public void PlayButton()
	{
		Time.timeScale = 1;
	}

	public void PauseButton()
	{ 
		if (Time.timeScale > 0)
		{
			//Save our time scale to return to later, and then set to 0
			saveTimeScale = Time.timeScale;
			Time.timeScale = 0;
		} 
		else 
		{
			//Return the timeScale to the original value
			Time.timeScale = saveTimeScale;
		}
	}

	public void FastForwardButton()
	{ 
		if (Time.timeScale < maxFFW)
			Time.timeScale *= 2; //double unitl maxFFW reached
		else
			Time.timeScale = 1;
	}	
}