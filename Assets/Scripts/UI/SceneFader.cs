using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    public Image image;
    public AnimationCurve curve;    

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    public void FadeTo(string scene)
    {
        StartCoroutine(FadeOut(scene));
    }

    // De l'ecran noir vers la scene
    IEnumerator FadeIn()
    {
        float t = 1f;

        while (t > 0f)
        {
            t -= Time.unscaledDeltaTime;
            float a = curve != null ? curve.Evaluate(t) : t;
            if (image != null) image.color = new Color(0f, 0f, 0f, a);
            yield return null;
        }
    }
    
    // De la scene vers l'ecran noir
    IEnumerator FadeOut(string scene)
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime;
            float a = curve != null ? curve.Evaluate(t) : t;
            if (image != null) image.color = new Color(0f, 0f, 0f, a);
            yield return null;
        }

        SceneManager.LoadScene(scene);
    }
}
