using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[ExecuteInEditMode]
public class MBStageCamera : MonoBehaviour {
    private Transform anchorTransform;
    private Camera camera;

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
    private float CurrentAngle { get { return transform.localEulerAngles.x; } }
    public bool Tilting { get { return tiltCoroutine != null; } }
    private Coroutine tiltCoroutine;

    private const float ZOOM_DURATION = 0.5f;
    private int zoom = 1;
    private float[] zoomMap = new float[] { 2f, 3f, 4.5f, 6f };
    private float ZoomDistance { get { return zoomMap[zoom]; } }
    public bool Zooming { get { return zoomCoroutine != null; } }
    private Coroutine zoomCoroutine;

    private Vector3 newPosition;

    private void Awake() {
        anchorTransform = transform.parent;
        camera = GetComponent<Camera>();
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
        transform.localPosition = StageCameraHelper.CalculateNewPosition(transform.localPosition, angle);
        Vector3 newRotation = transform.localEulerAngles;
        newRotation.x = angle;
        transform.localEulerAngles = newRotation;
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
            time += Time.deltaTime;
            float ratio = time / ROTATION_DURATION;
            ratio = Mathf.Sin(ratio * Mathf.PI / 2.0f);
            anchorTransform.localEulerAngles = Vector3.Lerp(startRotation, targetRotation, ratio);
            yield return null;
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
            time += Time.deltaTime;
            float ratio = time / TILT_DURATION;
            ratio = Mathf.Sin(ratio * Mathf.PI / 2.0f);
            SetAngle(Mathf.Lerp(startAngle, targetAngle, ratio));
            yield return null;
        }
        SetAngle(targetAngle);
        tiltCoroutine = null;
    }

    public void Zoom(bool inward) {
        if (inward) {
            if (++zoom >= zoomMap.Length) {
                zoom = 0;
            }
        } else {
            if (--zoom < 0) {
                zoom = zoomMap.Length - 1;
            }
        }
        if (zoomCoroutine != null) {
            StopCoroutine(zoomCoroutine);
        }
        zoomCoroutine = StartCoroutine(ZoomCamera());
    }

    IEnumerator ZoomCamera() {
        float startZoom = camera.orthographicSize;
        float endZoom = ZoomDistance;
        float time = 0.0f;
        while (time < ZOOM_DURATION) {
            time += Time.deltaTime;
            float ratio = time / ZOOM_DURATION;
            ratio = Mathf.Sin(ratio * Mathf.PI / 2.0f);
            camera.orthographicSize = Mathf.Lerp(startZoom, endZoom, ratio);
            yield return null;
        }
        camera.orthographicSize = endZoom;
        zoomCoroutine = null;
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
