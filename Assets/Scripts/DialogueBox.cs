using UnityEngine;
using System.Collections;

public class DialogueBox : MonoBehaviour {
  public bool dialogue;
  public string text;
  public GUIStyle style;
  public string buttonText;
  void Start () {
  }

  
  void Update () {

  }
  
  void OnGUI() {
    if(dialogue == true) {
      Rect box = ResizeGUI(new Rect (200,400,400,125));
      GUI.BeginGroup (box);
      GUI.Box(new Rect(0,0,box.width,box.height),"");
      string newText = text.Replace("\\n","\n");
      GUI.Label(ResizeGUI(new Rect(0,0,box.width,box.height)),newText,style);
      if(GUI.Button(ResizeGUI(new Rect(325,80,65,30)),buttonText)) {
        GameObject.Find ("EventSystem").GetComponent<Tutorial>().NextScene();
      }
      GUI.EndGroup ();
    }
  }

  Rect ResizeGUI(Rect _rect)
  {
    float FilScreenWidth = _rect.width / 800;
    float rectWidth = FilScreenWidth * Screen.width;
    float FilScreenHeight = _rect.height / 600;
    float rectHeight = FilScreenHeight * Screen.height;
    float rectX = (_rect.x / 800) * Screen.width;
    float rectY = (_rect.y / 600) * Screen.height;
    
    return new Rect(rectX,rectY,rectWidth,rectHeight);
  }
}