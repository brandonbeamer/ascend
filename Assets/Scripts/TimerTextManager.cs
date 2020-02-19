using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerTextManager : MonoBehaviour
{
    public LevelManager lm;
    public Color expiredColor;
    public Color safeColor;
    private Color defaultColor;

    private Text text;

    private void Awake()
    {
        text = GetComponent<Text>();
        defaultColor = text.color;

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float rem = lm.ExpireTime - Time.time;

        if (lm.qtm.QuestComplete)
            text.color = safeColor;
        else
        {
            if (rem < 0)
                text.color = expiredColor;
            else
                text.color = defaultColor;
        }

        if (rem < 0 || lm.ExpireTime == 0)
        {
            text.text = "00:00";
            return;
        }



        //int min = (int)(rem / 60);
        //rem -= 60f * min;

        int sec = (int)rem;
        rem -= sec;

        int cs = (int)(rem * 100);

        string timeString = "";
        //if (min > 0)
        //    timeString += min + ":";

        timeString += string.Format("{0:d2}:{1:d2}", sec, cs);
        text.text = timeString;
    }
}
