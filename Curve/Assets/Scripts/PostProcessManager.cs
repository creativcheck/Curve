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

    [SerializeField] AnimationCurve vignetteIntensityCurve;
    [SerializeField] AnimationCurve paniniDistanCurve;
    [SerializeField] float animationStartLeftDuration;

    private void OnEnable()
    {
        CacheObjects();
    }
    void Start()
    {
        StartCoroutine(AnimationTimeStartLeft());
    }

    void CacheObjects ()
    {
      //  volume.profile.TryGet<Bloom>(out bloom);
        volume.profile.TryGet<PaniniProjection>(out paniniProjection);
        volume.profile.TryGet<Vignette>(out vignette);
    }

    IEnumerator AnimationTimeStartLeft()
    {
        float time = 0f;
        float duration = animationStartLeftDuration;
        while (time<duration)
        {
            paniniProjection.distance.value = paniniDistanCurve.Evaluate(time);
            vignette.intensity.value = vignetteIntensityCurve.Evaluate(time);
            time += Time.deltaTime; 
            yield return null;
        }
        paniniProjection.distance.value = paniniDistanCurve.keys[paniniDistanCurve.length - 1].value;
        vignette.intensity.value = vignetteIntensityCurve.keys[vignetteIntensityCurve.length - 1].value;

    }
    
    void Update()
    {
        
    }
}
