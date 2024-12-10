using System.Collections;
using UnityEngine;

public class TrackShowcase : MonoBehaviour
{
    public CanvasGroup FadeCanvasGroup; // Canvas group for fading effect
    public GameObject[] Tracks; // Array of track GameObjects
    public float DisplayTime = 60f; // Time each track stays visible
    public float FadeDuration = 2f; // Duration of fade effect

    private int CurrentTrackIndex = 0;
    private bool IsTransitioning = false;

    void Start()
    {
        // Initialize
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
        // Deactivate all tracks
        for (int i = 0; i < Tracks.Length; i++)
        {
            Tracks[i].SetActive(i == index);
        }
    }

    private IEnumerator TransitionToNextTrack()
    {
        IsTransitioning = true;

        // Fade out
        yield return StartCoroutine(FadeOut());

        // Switch to the next track
        CurrentTrackIndex = (CurrentTrackIndex + 1) % Tracks.Length;
        SetActiveTrack(CurrentTrackIndex);

        // Fade in
        yield return StartCoroutine(FadeIn());

        // Reset timer and allow updates
        DisplayTime = 60f; // Reset the display time
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

        FadeCanvasGroup.alpha = 0.3f; // Fully visible
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

        FadeCanvasGroup.alpha = 1f; // Fully hidden
    }
}
