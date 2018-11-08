using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionListObject : MonoBehaviour {

    public Text actionTypeText;
    public Text[] buttonText;
    public Main main;
    public float[] time;

    public int section;

    public void SetUpActionList(Main _main, float[] _time,string _actionTypeText,int _section)
    {
        main = _main;
        time = _time;
        buttonText[0].text = time[0].ToString();
        buttonText[1].text = time[1].ToString();
        actionTypeText.text = _actionTypeText;
        section = _section;
    }
    public void SetTime(int t)
    {

        main.SetTimeFromActionList(t, section);

        //main.SetTimelineCurrentValue(time[t]);

    }   


	
}
