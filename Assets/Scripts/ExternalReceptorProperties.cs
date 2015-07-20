using UnityEngine;

public class ExternalReceptorProperties : MonoBehaviour
{
    #region Public Fields + Properties + Events + Delegates + Enums

    public Color ActiveColor = Color.white;
    public bool allowMovement = true;
    public bool isActive = true;
    public Color NonActiveColor = Color.gray;

    #endregion Public Fields + Properties + Events + Delegates + Enums

    #region Public Methods

    public void changeState(bool message)
    {
        this.isActive = message;
        if (this.isActive == false)
        {
            foreach (Transform child in this.transform)
            {
                if (child.name == "Receptor Body")
                {
                    child.GetComponent<Renderer>().material.color = NonActiveColor;
                }
            }
        }
        else
        {
            this.allowMovement = true;
            foreach (Transform child in this.transform)
            {
                if (child.name == "Receptor Body")
                {
                    child.GetComponent<Renderer>().material.color = ActiveColor;
                }
            }
        }
    }

    #endregion Public Methods

    #region Private Methods

    private void Start()
    {
        changeState(true);
    }

    #endregion Private Methods
}