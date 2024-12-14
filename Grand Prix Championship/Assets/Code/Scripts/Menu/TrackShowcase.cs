using System.Collections;
using UnityEngine;

public class TrackShowcase : MonoBehaviour
{
    public CanvasGroup FadeCanvasGroup; 
    public GameObject[] Tracks; 
    public float DisplayTime = 60f; 
    public float FadeDuration = 2f; 

    private int CurrentTrackIndex = 0;
    private bool IsTransitioning = false;

    void Start()
    {
        SetActiveTrack(CurrentTrackIndex);
        StartCoroutine(FadeIn());
    }

    void Update()
    {
        if (!IsTransitioning)
        {
            DisplayTime -= Time.deltaTime;

            if (DisplayTime <= 0f)
            {
                StartCoroutine(TransitionToNextTrack());
            }
        }
    }

    private void SetActiveTrack(int index)
    {
        for (int i = 0; i < Tracks.Length; i++)
        {
            Tracks[i].SetActive(i == index);
        }
    }

    private IEnumerator TransitionToNextTrack()
    {
        IsTransitioning = true;
        yield return StartCoroutine(FadeOut());
        CurrentTrackIndex = (CurrentTrackIndex + 1) % Tracks.Length;
        SetActiveTrack(CurrentTrackIndex);
        yield return StartCoroutine(FadeIn());
        DisplayTime = 60f; 
        IsTransitioning = false;
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;

        while (elapsedTime < FadeDuration)
        {
            elapsedTime += Time.deltaTime;
            FadeCanvasGroup.alpha = Mathf.Lerp(1f, 0.3f, elapsedTime / FadeDuration);
            yield return null;
        }

        FadeCanvasGroup.alpha = 0.3f; 
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;

        while (elapsedTime < FadeDuration)
        {
            elapsedTime += Time.deltaTime;
            FadeCanvasGroup.alpha = Mathf.Lerp(0.3f, 1f, elapsedTime / FadeDuration);
            yield return null;
        }

        FadeCanvasGroup.alpha = 1f; 
    }
}
