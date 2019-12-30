using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectFadeInAndOut : MonoBehaviour
{
    public float timerMax;
    private float timer;
    private bool fade;
    public bool loop;
    public Image thisImage;

    private void Start()
    {
        timer = timerMax;
        fade = false;
    }

    public void toggleFade()
    {
        if (thisImage.color.a == 1)
        {
            StartCoroutine(FadeCanvas(thisImage, 1, 0, timerMax));
        }
        else if (thisImage.color.a == 0)
        {
            StartCoroutine(FadeCanvas(thisImage, 0, 1, timerMax));
        }
    }

    private static IEnumerator FadeCanvas(Image image, float startAlpha, float endAlpha, float duration)
    {
        // keep track of when the fading started, when it should finish, and how long it has been running&lt;/p&gt; &lt;p&gt;&a
        var startTime = Time.time;
        var endTime = Time.time + duration;
        var elapsedTime = 0f;

        // set the canvas to the start alpha – this ensures that the canvas is ‘reset’ if you fade it multiple times
        image.color = new Vector4(1,1,1,startAlpha);
        // loop repeatedly until the previously calculated end time
        while (Time.time <= endTime)
        {
            elapsedTime = Time.time - startTime; // update the elapsed time
            var percentage = 1 / (duration / elapsedTime); // calculate how far along the timeline we are
            if (startAlpha > endAlpha) // if we are fading out/down 
            {
                image.color = new Vector4(1, 1, 1,startAlpha - percentage); // calculate the new alpha
            }
            else // if we are fading in/up
            {
                image.color = new Vector4(1, 1, 1, startAlpha + percentage); // calculate the new alpha
            }

            yield return new WaitForEndOfFrame(); // wait for the next frame before continuing the loop
        }
        image.color = new Vector4(1, 1, 1, endAlpha); // force the alpha to the end alpha before finishing – this is here to mitigate any rounding errors, e.g. leaving the alpha at 0.01 instead of 0
    }

    private void Update()
    {
        if (loop)
        {
            if (!fade && thisImage.color.a == 1)
            {
                StartCoroutine(FadeCanvas(thisImage, 1, 0, timerMax));
                fade = true;
            }

            if (fade && thisImage.color.a == 0)
            {
                StartCoroutine(FadeCanvas(thisImage, 0, 1, timerMax));
                fade = false;
            }
        }
    }
}