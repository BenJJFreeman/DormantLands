using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour {

    public GameObject blockingPanel;
    public GameObject nextButton;
    public GameObject[] tutorialTextObj;
    public GameObject[] featureObj;



    int currentSection;
	void Start () {
		for (int i = 0; i< tutorialTextObj.Length; i++)
        {

            tutorialTextObj[i].SetActive(false);
        }

        for (int i = 0; i < featureObj.Length; i++)
        {
            featureObj[i].SetActive(false);

        }
        nextButton.SetActive(false);
        StartCoroutine(LoadTutorial(2));
    }

    IEnumerator LoadTutorial(float sec)
    {
        yield return new WaitForSeconds(sec);

        nextButton.SetActive(true);


        tutorialTextObj[0].SetActive(true);



        featureObj[0].SetActive(true);


    }
    void Update () {
		



	}

    public void NextTutorialSection()
    {


        tutorialTextObj[currentSection].SetActive(false);


        if (featureObj.Length > currentSection)
        {
            featureObj[currentSection].SetActive(false);
        }


        currentSection++;



        if (tutorialTextObj.Length > currentSection)
        {
            tutorialTextObj[currentSection].SetActive(true);
        }
        else
        {
            for (int i = 0; i < featureObj.Length; i++)
            {
                featureObj[i].SetActive(true);

            }
            blockingPanel.SetActive(false);
            nextButton.SetActive(false);
            
        }
        if (featureObj.Length > currentSection)
        {
            featureObj[currentSection].SetActive(true);
        }
    }
}
