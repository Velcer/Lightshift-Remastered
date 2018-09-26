using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffectComponent : MonoBehaviour
{
    private Vector3 positionTmp;
    public float MaxDistance = 100;
    public float ShakeMult = 0.2f;

    void Start()
    {
        if (transform.parent)
            positionTmp = transform.localPosition;
        else positionTmp = transform.position;
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, CameraEffects.Shaker.PositionShaker);
        float damping = (1.0f / MaxDistance) * Mathf.Clamp(MaxDistance - distance, 0, MaxDistance);

        if (transform.parent)
            transform.localPosition = positionTmp + (CameraEffects.Shaker.ShakeMagnitude * damping * ShakeMult);
        else transform.position = positionTmp + (CameraEffects.Shaker.ShakeMagnitude * damping * ShakeMult);
    }
}
