using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class MBStageCamera : MonoBehaviour {

    private const float ROTATION_DURATION = 0.5f;

    public float angle = 30;


    private Transform anchorRotationTransform;
    public int Rotation { get; private set; }
    public bool Rotating { get { return rotationsQueue.Count > 0 || rotatingAnimation; } }
    private bool rotatingAnimation = false;
    private Queue<IEnumerator> rotationsQueue = new Queue<IEnumerator>();

    private void Awake() {
        anchorRotationTransform = gameObject.transform.parent.transform;
        Rotation = 0;
    }

#if UNITY_EDITOR
    void Update () {
        if (!Application.isPlaying) { // Does not update in play mode
            if (Mathf.Abs(this.transform.localRotation.eulerAngles.x - this.angle) > 0.1f) {
                SetAngle(angle);
            }
        }

        RotateCamera();
	}
#endif

    public void SetAngle(float angle) {
        this.angle = angle;
        this.transform.localPosition = StageCameraHelper.CalculateNewPosition(this.transform.localPosition, angle);
        Vector3 newRotation = this.transform.localEulerAngles;
        newRotation.x = angle;
        this.transform.localEulerAngles = newRotation;
    }

    private void RotateCamera() {
        if(!rotatingAnimation && rotationsQueue.Count > 0) {
            StartCoroutine(rotationsQueue.Dequeue());
        }
        
    }

    IEnumerator RotateCamera(bool clockwise) {
        rotatingAnimation = true;
        Vector3 startRotation = anchorRotationTransform.localEulerAngles;
        float targetAngle = startRotation.y + (clockwise ? -90.0f : 90.0f);
        Vector3 targetRotation = new Vector3(startRotation.x, targetAngle, startRotation.z);
        float time = 0.0f;
        while(time < ROTATION_DURATION) {
            yield return null;
            time += Time.deltaTime;
            float ratio = time / (ROTATION_DURATION);
            if (ratio > 1.0f) {
                ratio = 1.0f;
            }
            ratio = Mathf.Sin(ratio * Mathf.PI / 2.0f);
            anchorRotationTransform.localEulerAngles = Vector3.Lerp(startRotation, targetRotation, ratio);
        }
        rotatingAnimation = false;
    }

    public void Rotate(bool clockwise) {
        if(rotationsQueue.Count < 16) {
            rotationsQueue.Enqueue(RotateCamera(clockwise));
            if (clockwise) {
                if (++Rotation > 3) {
                    Rotation = 0;
                }
            } else {
                if (--Rotation < 0) {
                    Rotation = 3;
                }
            }
        }
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
