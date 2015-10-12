using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {

  public bool tutorial;
  public EventClass[] Events;
  public int currentScene;

  public interface SwitchOnOff {
    void enable();
    void disable();
  }

	// Use this for initialization
	void Start () {
    currentScene = -1;
    tutorial = false;
	}

  public void switchTutorial () {
    if (tutorial == true) {
      endTutorial ();
    } else {
      startTutorial ();
    }
  }

  public void startTutorial () {
    GameObject.Find ("EventSystem").GetComponent<ObjectCollection> ().Clear ();
    tutorial = true;
    currentScene = 0;
    Events [currentScene].enable ();
  }

  public void endTutorial () {
    tutorial = false;

    Events [currentScene].disable ();
    currentScene = -1;
    //GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIControl>().ChangeScene("Models");
  }
	
  public void NextScene() {
    if (currentScene + 1 < Events.Length && currentScene >= 0) {
      Events[currentScene].disable();
      currentScene++;
      Events[currentScene].enable ();
    } else {
      endTutorial ();
    }
  }

	// Update is called once per frame
	void Update () {
    if (tutorial == true) {
      Events [currentScene].render ();
    }
	}
}