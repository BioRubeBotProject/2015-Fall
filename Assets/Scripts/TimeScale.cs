using UnityEngine;
using System.Collections;

public class TimeScale : MonoBehaviour 
{
	public void Start()
	{
		Time.timeScale = 0;
	}
	
	public void PauseButton()
	{
		doPauseToggle ();
	}
	public void FastForwardButton()
	{
		doFastForward ();
	}

	public void PlayButton()
	{
		doPlay ();
	}
	void doPlay()
	{
		Time.timeScale = 1;
	}
	void doPauseToggle()
	{
		if (Time.timeScale > 0) 
		{
			Time.timeScale = 0;
		} 
		else 
		{
			Time.timeScale = 1;
		}
	}
	void doFastForward()
	{
		if (Time.timeScale >= 1 && Time.timeScale < 2) 
		{
			Time.timeScale = Time.timeScale + 1;
		}
		else 
		{
			Time.timeScale = 1;		
		}

	}
	
}