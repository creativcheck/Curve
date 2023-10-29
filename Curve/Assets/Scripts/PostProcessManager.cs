using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessManager : MonoBehaviour
{
    [SerializeField] Volume volume;
    Bloom bloom;
    Vignette vignette;
    PaniniProjection paniniProjection;

    [SerializeField] AnimationCurve vignetteIntensityCurveRight;
    [SerializeField] AnimationCurve vignetteIntensityCurveLeft;
    [SerializeField] AnimationCurve vignetteIntensityCurveBoth;
    [SerializeField] AnimationCurve paniniDistanCurve;
    [SerializeField] float animatioRightDuration;
    [SerializeField] float animationLeftDuration;
    [SerializeField] float animationBothDuration;

    private float vignetteIntensityMultiplier;
    private float paniniIntensityMultiplier;

    private void OnEnable()
    {
        CacheObjects();
    }
    void Start()
    {
      
      //  StartCoroutine(AnimationTimeLeft());
      //  StartCoroutine(AnimationTimeRight());
      StartCoroutine(AnimationTimeBoth());
    }

    void CacheObjects ()
    {
        volume.profile.TryGet<PaniniProjection>(out paniniProjection);
        volume.profile.TryGet<Vignette>(out vignette);
    }

    IEnumerator AnimationTimeRight()
    {
        float time = 0f;
        float duration = animatioRightDuration;
        Vector2 newCenter = new Vector2(0.2f, 0.5f);
        vignette.center.Override(newCenter);
        while (time<duration)
        {
            paniniProjection.distance.value = paniniDistanCurve.Evaluate(time);
            vignette.intensity.value = vignetteIntensityCurveRight.Evaluate(time);
            time += Time.deltaTime; 
            yield return null;
        }
        paniniProjection.distance.value = paniniDistanCurve.keys[paniniDistanCurve.length - 1].value;
        vignette.intensity.value = vignetteIntensityCurveRight.keys[vignetteIntensityCurveRight.length - 1].value;

    }

    IEnumerator AnimationTimeLeft()
    {
        float time = 0f;
        float duration = animationLeftDuration;
        Vector2 newCenter = new Vector2(0.8f, 0.5f);
        vignette.center.Override(newCenter);
        while (time < duration)
        {
            paniniProjection.distance.value = paniniDistanCurve.Evaluate(time);
            vignette.intensity.value = vignetteIntensityCurveLeft.Evaluate(time);
            time += Time.deltaTime;
            yield return null;
        }
        paniniProjection.distance.value = paniniDistanCurve.keys[paniniDistanCurve.length - 1].value;
        vignette.intensity.value = vignetteIntensityCurveLeft.keys[vignetteIntensityCurveLeft.length - 1].value;

    }

    IEnumerator AnimationTimeBoth()
    {
        float time = 0f;
        float duration = animationBothDuration;
        //Vector2 newCenter = new Vector2(0.5f, 0.5f);
        //vignette.center.Override(newCenter);
        while (time < duration)
        {
            paniniProjection.distance.value = paniniDistanCurve.Evaluate(time) * paniniIntensityMultiplier;
            vignette.intensity.value = vignetteIntensityCurveBoth.Evaluate(time) * vignetteIntensityMultiplier;
            time += Time.deltaTime;
            yield return null;
        }
        paniniProjection.distance.value = paniniDistanCurve.keys[paniniDistanCurve.length - 1].value * paniniIntensityMultiplier;
        vignette.intensity.value = vignetteIntensityCurveBoth.keys[vignetteIntensityCurveBoth.length - 1].value * vignetteIntensityMultiplier;

    }

    public void SetMultiplier(float value)
    {
        vignetteIntensityMultiplier = value;
        paniniIntensityMultiplier = value;
    }
}
