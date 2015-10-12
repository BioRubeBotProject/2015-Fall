using UnityEngine;
using System.Collections;

public class TimeScale : MonoBehaviour {
	private static float saveTimeScale;

	public void Start()
	{
		Time.timeScale = 0;
	}

	public void PlayButton()
	{
		//Set TimeScale to 1, regardless of current state
		Time.timeScale = 1.0f;
	}

	public void PauseButton()
	{
		if (Time.timeScale > 0) {//Save our time scale to return to later, and then set to 0
			saveTimeScale = Time.timeScale;
			Time.timeScale = 0;
		} 
		else {
			Time.timeScale = saveTimeScale;//Return the timeScale to the original value
		}

	}

	public void FastForwardButton() {

		//When TimeScale = 1, TimeScale will Scale to 2
		//Depreciated comment, When TimeScale = 2, TimeScale will Scale to 4
		//Any TimeScale >= 2, does not compute
		//Change to Time.timeScale <= 2 to make maximum 4, currently allows objects to break through coliders on the membrane
		if (Time.timeScale >= 1 && Time.timeScale < 2) {
			Time.timeScale = Time.timeScale + Time.timeScale;
		}
		//Not user friendly TimeScale Modification
		else if(Time.timeScale > 2) {
			Time.timeScale = Time.timeScale / 2;
		}
	}

  public void ResetTime() {
    saveTimeScale = 0;
    Time.timeScale = 0;
  }
}
