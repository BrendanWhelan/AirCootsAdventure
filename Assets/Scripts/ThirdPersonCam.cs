using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class ThirdPersonCam : MonoBehaviour
{
    [SerializeField]
    private Transform orientation;
    [SerializeField]
    private Transform player;

    [SerializeField]
    private CinemachineFreeLook freeCam;
    
    private float defaultFOV = 50;

    private bool fovUpdating = false;

    private float startFov;
    private float targetFov;

    private float shiftTime = 0f;
    private float shiftTargetTime = 1f;

    private bool dutchRotating = false;

    private float targetDutch;
    private float startDutch;

    private float dutchShiftTime = 0f;
    private float dutchShiftTargetTime = 0.75f;
    
    
    [SerializeField]
    private CinemachineImpulseSource explosionSource;
    
    CinemachineBasicMultiChannelPerlin noise;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        noise = freeCam.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Update()
    {
        Vector3 movedTransform = player.position - (transform.position - player.position);
        var distanceToPlane = Vector3.Dot(player.up, (movedTransform - player.position));
       var planePoint = movedTransform - (player.up * distanceToPlane);
       orientation.LookAt(planePoint, player.up);

       if (fovUpdating)
        {
            freeCam.m_Lens.FieldOfView = Mathf.Lerp(startFov, targetFov, shiftTime / shiftTargetTime);
            if (shiftTime >= shiftTargetTime)
            {
                fovUpdating = false;
                freeCam.m_Lens.FieldOfView = targetFov;
            }
            shiftTime += Time.deltaTime;
        }

        if (dutchRotating)
        {
            freeCam.m_Lens.Dutch = Mathf.SmoothStep(startDutch, targetDutch, dutchShiftTime / dutchShiftTargetTime);
            if (dutchShiftTime >= dutchShiftTargetTime)
            {
                dutchRotating = false;
                freeCam.m_Lens.Dutch = targetDutch;
            }
            dutchShiftTime += Time.deltaTime;
        }
    }

    public void DoFov(float endValue)
    {
        targetFov = endValue;
        startFov = freeCam.m_Lens.FieldOfView;
        shiftTime = 0f;
        fovUpdating = true;
    }

    public void ResetFov()
    {
        targetFov = defaultFOV;
        startFov = freeCam.m_Lens.FieldOfView;
        shiftTime = 0f;
        fovUpdating = true;
    }

    public void Tilt(Vector3 tilt)
    {
        if (Math.Abs(targetDutch - tilt.x) > 0.1f)
        {
            targetDutch = tilt.x;
            dutchRotating = true;
            startDutch = freeCam.m_Lens.Dutch;
            dutchShiftTime = 0;
        }
    }

    public void SetView(Vector2 viewValues)
    {
        freeCam.m_XAxis.Value = viewValues.x;
        freeCam.m_YAxis.Value = viewValues.y;
    }
    
    public void CameraExplosion()
    {
        explosionSource.GenerateImpulse(new Vector3(Random.Range(-1f,1f),Random.Range(-1f,1f),Random.Range(-1f,1f)));
    }

    public void SetNoise(float noiseValue)
    {
        if (noise == null) return;
        noise.m_AmplitudeGain = Mathf.Clamp(noiseValue,0,0.6f);
    }
    
    public void SetNoiseAndFOV(float noiseValue)
    {
        if (noise == null) return;
        noise.m_AmplitudeGain = noiseValue;
    }
}
