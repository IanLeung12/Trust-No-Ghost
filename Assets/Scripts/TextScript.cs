using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TextScript : MonoBehaviour
{
    [Header("Tutorial Text Settings")]
    public List<string> tutorialMessages;
    public TMP_Text uiText; // Assign your TMP UI Text component in Inspector
    public CanvasGroup canvasGroup; // Assign CanvasGroup in Inspector
    public float fadeDuration = 1f;
    public float displayDuration = 2f;

    private int currentIndex = 0;
    private float fadeTimer = 0f;
    private float displayTimer = 0f;
    private enum State { Idle, FadingIn, Displaying, FadingOut }
    private State state = State.Idle;

    void Start()
    {
        if (tutorialMessages != null && tutorialMessages.Count > 0)
        {
            currentIndex = 0;
            uiText.text = tutorialMessages[currentIndex];
            canvasGroup.alpha = 0f;
            state = State.FadingIn;
            fadeTimer = 0f;
        }
        else
        {
            uiText.text = "";
            canvasGroup.alpha = 0f;
            state = State.Idle;
        }
    }

    void Update()
    {
        switch (state)
        {
            case State.FadingIn:
                fadeTimer += Time.deltaTime;
                canvasGroup.alpha = Mathf.Clamp01(fadeTimer / fadeDuration);
                if (canvasGroup.alpha >= 1f)
                {
                    state = State.Displaying;
                    displayTimer = 0f;
                }
                break;
            case State.Displaying:
                displayTimer += Time.deltaTime;
                if (displayTimer >= displayDuration)
                {
                    state = State.FadingOut;
                    fadeTimer = 0f;
                }
                break;
            case State.FadingOut:
                fadeTimer += Time.deltaTime;
                canvasGroup.alpha = Mathf.Clamp01(1f - (fadeTimer / fadeDuration));
                if (canvasGroup.alpha <= 0f)
                {
                    currentIndex++;
                    if (tutorialMessages != null && currentIndex < tutorialMessages.Count)
                    {
                        uiText.text = tutorialMessages[currentIndex];
                        state = State.FadingIn;
                        fadeTimer = 0f;
                    }
                    else
                    {
                        uiText.text = "";
                        state = State.Idle;
                    }
                }
                break;
        }
    }
}
