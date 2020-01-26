﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GameManager : Singleton<GameManager>
{
    [Header("Post Processing")]
    public PostProcessVolume globalVolume;
    public float startShutterAngle = 1f;
    public float endShutterAngle = 360f;

    private Vignette vignette;
    private MotionBlur motionBlur;
    private Grain filmGrain;

    private void Start()
    {
        globalVolume.profile.TryGetSettings(out vignette);
        globalVolume.profile.TryGetSettings(out motionBlur);
        globalVolume.profile.TryGetSettings(out filmGrain);
        UIManager.Instance.staminaBar.OnChange += UpdateVignette;
        UIManager.Instance.staminaBar.OnChange += UpdateMotionBlur;
        UIManager.Instance.staminaBar.OnChange += UpdateFilmGrain;
    }

    void UpdateVignette(float fillAmount)
    {
        // Disabling for now - Finnbarr
        //vignette.intensity.value = StaminaBar.STAMINA_MAX - fillAmount;
    }
    
    void UpdateMotionBlur(float fillAmount)
    {
        // We have end --> start because: fillAmount == 0 means no stamina (end state), and == 1 means full (start state)
        motionBlur.shutterAngle.value = Mathf.Lerp(endShutterAngle, startShutterAngle, fillAmount / StaminaBar.STAMINA_MAX);
    }

    void UpdateFilmGrain(float fillAmount)
    {
        filmGrain.intensity.value = fillAmount;
    }
}
