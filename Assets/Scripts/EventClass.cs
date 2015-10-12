using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct UIObj {
  public GameObject EventObject;
  public bool flag;
}

[System.Serializable]
public struct EventObj {
  public GameObject EventObject;
  public List<Vector3> spawnLocations;
}

[System.Serializable]
public class EventClass : Tutorial.SwitchOnOff {
  public UIObj[] UIObjects;
  private bool enabled;
  public EventObj[] EventObjects;
  public bool dialogue;
  public string text;
  public Vector2 textboxPos = new Vector2();
  //public DialogueBox textbox;

  void Start () {
    enabled = false;
  }

  public void enable () {
    for (int i = 0; i < UIObjects.Length; i++) {
      if (UIObjects [i].EventObject != null) {
        Tutorial.SwitchOnOff Interface = (Tutorial.SwitchOnOff)UIObjects [i].EventObject.GetComponent<Tutorial.SwitchOnOff> ();

        if (UIObjects [i].flag == false) {
          Interface.disable ();
        } else {
          Interface.enable ();
        }
      }
    }

    if (enabled == false) {
      for (int i = 0; i < EventObjects.Length; i++) {
        for (int j = 0; j < EventObjects[i].spawnLocations.Count; j++) {
          GameObject clone;
          clone = GameObject.Instantiate (EventObjects [i].EventObject, EventObjects [i].spawnLocations [j], Quaternion.identity) as GameObject;
          clone.name = clone.name.Replace ("(Clone)", " ");
          clone.name = "Tutorial_" + clone.name;
          GameObject.Find("EventSystem").GetComponent<ObjectCollection>().Add (clone);
        }
      }
    }
    enabled = true;
  }



  public void render () {
    enable ();
    DialogueBox dialogueBox = GameObject.Find ("Main Camera").GetComponent<DialogueBox> ();
    dialogueBox.dialogue = dialogue;
    dialogueBox.text = text;
    dialogueBox.textboxPos = textboxPos;
  }
  
  public void disable () {
    if (enabled == true) {
      GameObject.Find("EventSystem").GetComponent<ObjectCollection>().Clear ();
    }

    for (int i = 0; i < UIObjects.Length; i++) {
      if (UIObjects [i].EventObject != null) {
        Tutorial.SwitchOnOff Interface = (Tutorial.SwitchOnOff)UIObjects [i].EventObject.GetComponent<Tutorial.SwitchOnOff> ();
        if (UIObjects [i].flag == true) {
          Interface.disable ();
        } else {
          Interface.enable ();
        }
      }
    }

    dialogue = false;
    enabled = false;
  }
}