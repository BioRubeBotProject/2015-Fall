using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ButtonOnOff : MonoBehaviour , Tutorial.SwitchOnOff {

	void Tutorial.SwitchOnOff.enable () {
    this.GetComponent<Button> ().interactable = true;
  }

  void Tutorial.SwitchOnOff.disable () {
    this.GetComponent<Button> ().interactable = false;
  }
}
