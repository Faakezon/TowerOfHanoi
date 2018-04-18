using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Just a quick fix to make the game look a little more alive and fun.
/// The IEnumerator is code I found online, re-written for my purposes.
/// </summary>

public class MainMenuTextBehaviour : MonoBehaviour {

    public List<Color> ColorsList = new List<Color>();
    private Text title;

    void Start () {
        title = GetComponent<Text>();

        StartCoroutine("SetupMainMenuTextSpacing");

        if (ColorsList.Count > 0)
            StartCoroutine("SetupMainMenuTextColor");
    }

    IEnumerator SetupMainMenuTextColor()
    {
        float delayTime = 5;
        
            Color currentcolor = ColorsList[0]; ;
            Color nextcolor;

            title.color = currentcolor;

        while (true)
        {
            nextcolor = ColorsList[Random.Range(0, ColorsList.Count)];
            for (float t = 0; t < delayTime; t += Time.deltaTime)
            {
                if (ColorsList.Count > 0)
                {
                    title.color = Color.Lerp(currentcolor, nextcolor, t / delayTime);
                }
                yield return null;
            }
            currentcolor = nextcolor;
        }
    }
    IEnumerator SetupMainMenuTextSpacing()
    {
        float delayTime = 3;

        while (true)
        {
            for (float t = 0; t < delayTime; t += Time.deltaTime)
            {
                if (title.lineSpacing < 0.95f)
                {
                    title.lineSpacing = Mathf.Lerp(0, 1.0f, t / delayTime);
                }
                yield return null;
            }

        }
    }

}
