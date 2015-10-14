﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct UIObj {
  public GameObject EventObject;
  public bool transparent;
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
  public string text;

  void Start () {
    enabled = false;
  }

  public void enable () {
    for (int i = 0; i < UIObjects.Length; i++) {
      if (UIObjects [i].EventObject != null) {
        Tutorial.SwitchOnOff Interface = (Tutorial.SwitchOnOff)UIObjects [i].EventObject.GetComponent<Tutorial.SwitchOnOff> ();
        Interface.enable ();
        Interface.transparent(UIObjects[i].transparent);
        if (UIObjects [i].flag == false) {
          Interface.disable ();
        } else {
          Interface.enable ();
        }
        Interface.transparent(UIObjects[i].transparent);
      }
    }

    if (enabled == false) {
      for (int i = 0; i < EventObjects.Length; i++) {
        for (int j = 0; j < EventObjects[i].spawnLocations.Count; j++) {
          GameObject clone;
          clone = GameObject.Instantiate (EventObjects [i].EventObject, EventObjects [i].spawnLocations [j], Quaternion.identity) as GameObject;
          clone.name = clone.name.Replace ("(Clone)", " ");
          clone.name = "Tutorial_" + clone.name;
          GameObject.Find ("EventSystem").GetComponent<ObjectCollection> ().Add (clone);
        }
      }
    }
    enabled = true;
  }

  void Tutorial.SwitchOnOff.transparent (bool value) {
  }

  public void render () {
    enable ();
    DialogueBox dialogueBox = GameObject.Find ("Main Camera").GetComponent<DialogueBox> ();
    dialogueBox.dialogue = true;
    dialogueBox.text = text;
  }
  
  public void disable () {
    if (enabled == true) {
      GameObject.Find("EventSystem").GetComponent<ObjectCollection>().Clear ();
    }

    for (int i = 0; i < UIObjects.Length; i++) {
      if (UIObjects [i].EventObject != null) {
        Tutorial.SwitchOnOff Interface = (Tutorial.SwitchOnOff)UIObjects [i].EventObject.GetComponent<Tutorial.SwitchOnOff> ();

        Interface.enable ();
        Interface.transparent(!UIObjects[i].transparent);
        if (UIObjects [i].flag == true) {
          Interface.disable ();
        } else {
          Interface.enable ();
        }

      }
    }

    GameObject.Find ("Main Camera").GetComponent<DialogueBox> ().dialogue = false;
    enabled = false;
  }
}