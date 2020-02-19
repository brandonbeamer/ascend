using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialBox : MonoBehaviour
{
    public RectTransform mainRect;
    public Toggle showTutorialToggle;
    public Button advanceButton;
    public Text titleText;
    public Text contentText;

    public delegate void ButtonClickDelegate();
    public ButtonClickDelegate advanceButtonClickedHandler;

    public string Title
    {
        get
        {
            return titleText.text;
        }
        set
        {
            titleText.text = value;
        }
    }

    public string Content
    {
        get
        {
            return contentText.text;
        }
        set
        {
            contentText.text = value;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        showTutorialToggle.isOn = GameManager.Instance.ShowTutorial;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowTutorialToggled(bool value)
    {
        GameManager.Instance.ShowTutorial = value;
        GameManager.Instance.SavePlayerData();
    }

    public void AdvanceButtonClicked()
    {
        advanceButtonClickedHandler?.Invoke();
    }

    public void SetSize(float width, float height)
    {
        mainRect.offsetMin = new Vector2(-width / 2, -height / 2);
        mainRect.offsetMax = new Vector2(width / 2, height / 2);
    }
}
