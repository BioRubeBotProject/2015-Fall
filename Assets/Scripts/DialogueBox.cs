using UnityEngine;
using System.Collections;

public class DialogueBox : MonoBehaviour {
  public bool dialogue;
  public string text;
  public Vector2 textboxPos = new Vector2();
  public float offset;
  public GUIStyle style;
  public string buttonText;
  public Vector4 buttonPos;

  void Start () {
  }
  
  void Update () {
    
  }
  
  void OnGUI() {
    if(dialogue == true) {
      GUI.BeginGroup (new Rect (Screen.width/2 - textboxPos.x,
                                Screen.height/2 - textboxPos.y + offset,
                                Screen.width/2 + textboxPos.x,
                                Screen.height/2 + textboxPos.y + offset));
      GUI.Box(new Rect(0,0,textboxPos.x*2,textboxPos.y*2),"");
      string newText = text.Replace("\\n","\n");
      GUI.Label(new Rect(0,0,0,0),newText,style);
      if(GUI.Button(new Rect( buttonPos.x, buttonPos.y, buttonPos.z, buttonPos.w),buttonText)) {
        GameObject.Find ("EventSystem").GetComponent<Tutorial>().NextScene();
      }
      GUI.EndGroup ();
    }
  }
}