using UnityEngine;
using System.Collections;

public class DialogueBox : MonoBehaviour {
  public bool dialogue;
  public string text;
  public GUIStyle style;
  public string buttonText;

  private int originalFontSize;
  void Start () {
    originalFontSize = style.fontSize; 
  }

  
  void Update () {

  }
  
  void OnGUI() {
    if(dialogue == true) {
      Rect box = ResizeGUI(new Rect (200,375,400,150));
      GUI.BeginGroup (box);
      GUI.Box(new Rect(0,0,box.width - 5,box.height - 5),"");
      string newText = text.Replace("\\n","\n");
      //style.fontSize = (int)(.0085 * box.height * originalFontSize);
      //GUI.Box(new Rect(0,0,box.width - 5,box.height - 5),newText,style);


      GUI.Label(new Rect(0,0,box.width-10,box.height-10),newText,style);
      if(GUI.Button(ResizeGUI(new Rect(325,118,65,20)),buttonText)) {
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