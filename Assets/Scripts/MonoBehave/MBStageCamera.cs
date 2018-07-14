using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[ExecuteInEditMode]
public class MBStageCamera : MonoBehaviour {
    private Transform anchorTransform;

    private const float ROTATION_DURATION = 0.5f;
    public int Rotation { get; private set; }
    private float[] rotationMap = new float[] { 0f, 90f, 180f, 270f };
    private float RotationAngle { get { return rotationMap[Rotation]; } }
    public bool Rotating { get { return rotationCoroutine != null; } }
    private Coroutine rotationCoroutine;
    
    private const float TILT_DURATION = 0.5f;
    private int tilt = 1;
    private float[] angleMap = new float[] { 20f, 30f, 45f, 60f };
    private float TiltAngle { get { return angleMap[tilt]; } }
    public bool Tilting { get { return tiltCoroutine != null; } }
    private Coroutine tiltCoroutine;

    private Vector3 newPosition;

    private void Awake() {
        anchorTransform = transform.parent;
        Rotation = 0;
        newPosition = transform.parent.localPosition;
    }

    private void Start() {
        SetAngle(TiltAngle);
    }

    void Update () {        
        MoveCamera();
	}

    private void MoveCamera() {
        transform.parent.localPosition = Vector3.Lerp(transform.parent.localPosition, newPosition, 0.5f);
    }

    public void MoveTo(Vector3 newPosition) {
        this.newPosition = newPosition;
    }

    private void SetAngle(float angle) {
        this.transform.localPosition = StageCameraHelper.CalculateNewPosition(this.transform.localPosition, angle);
        Vector3 newRotation = this.transform.localEulerAngles;
        newRotation.x = angle;
        this.transform.localEulerAngles = newRotation;
    }

    IEnumerator RotateCamera(bool clockwise) {
        Vector3 startRotation = anchorTransform.localEulerAngles;
        float targetAngle = RotationAngle;
        if (clockwise) {
            if (targetAngle < startRotation.y) {
                targetAngle += 360f;
            }
        } else {
            if (targetAngle > startRotation.y) {
                targetAngle -= 360f;
            }
        }
        Vector3 targetRotation = new Vector3(startRotation.x, targetAngle, startRotation.z);
        float time = 0.0f;
        while(time < ROTATION_DURATION) {
            yield return null;
            time += Time.deltaTime;
            float ratio = time / (ROTATION_DURATION);
            ratio = Mathf.Sin(ratio * Mathf.PI / 2.0f);
            anchorTransform.localEulerAngles = Vector3.Lerp(startRotation, targetRotation, ratio);
        }
        anchorTransform.localEulerAngles = targetRotation;
        rotationCoroutine = null;
    }

    public void Rotate(bool clockwise) {
        if (clockwise) {
            if (++Rotation >= rotationMap.Length) {
                Rotation = 0;
            }
        } else {
            if (--Rotation < 0) {
                Rotation = rotationMap.Length - 1;
            }
        }
        if (rotationCoroutine != null) {
            StopCoroutine(rotationCoroutine);
        }
        rotationCoroutine = StartCoroutine(RotateCamera(clockwise));
    }

    public void Tilt(bool up) {
        if (up) {
            if (++tilt >= angleMap.Length) {
                tilt = 0;
            }
        } else {
            if (--tilt < 0) {
                tilt = angleMap.Length - 1;
            }
        }
        if (tiltCoroutine != null) {
            StopCoroutine(tiltCoroutine);
        }
        tiltCoroutine = StartCoroutine(TiltCamera());
    }

    IEnumerator TiltCamera() {
        float startAngle = transform.localEulerAngles.x;
        float targetAngle = TiltAngle;
        float time = 0.0f;
        while (time < TILT_DURATION) {
            yield return null;
            time += Time.deltaTime;
            float ratio = time / (ROTATION_DURATION);
            ratio = Mathf.Sin(ratio * Mathf.PI / 2.0f);
            SetAngle(Mathf.Lerp(startAngle, targetAngle, ratio));
        }
        SetAngle(targetAngle);
        tiltCoroutine = null;
    }
}

static class StageCameraHelper {
    /// <summary>
    /// Takes the a position (its x and z) and determines how high (y) it is for the camera
    /// at the defined angle to still be pointing at the local 0,0.
    /// </summary>
    /// <param name="currentPosition">position of the camera</param>
    /// <param name="angle">angle the camera needs to be looking down at</param>
    /// <returns>the new position the camera needs to be to accomodate the angle</returns>
    public static Vector3 CalculateNewPosition(Vector3 currentPosition, float angle) {
        float baseDistance = new Vector2(currentPosition.x, currentPosition.z).magnitude;
        float y = baseDistance * Mathf.Tan(Mathf.Deg2Rad * angle);
        Vector3 result = new Vector3(
            currentPosition.x,
            y,
            currentPosition.z);
        return result;
    }
}
