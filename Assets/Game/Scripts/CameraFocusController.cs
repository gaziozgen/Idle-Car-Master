using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFocusController : MonoBehaviour
{
    public static CameraFocusController Instance { get; private set; }
    public Transform Transform { get; private set; }
    public Transform Focus = null;

    [SerializeField] private float cameraSpeed = 0;

    [SerializeField] private Camera carSpecialCamera = null;

    [SerializeField] private Vector3 basePosition = Vector3.zero;
    [SerializeField] private Vector3 focusOffset = Vector3.zero;

    [SerializeField] private float baseOthographicSize = 0;
    [SerializeField] private float focusOthographicSize = 0;

    void Awake()
    {
        Transform = transform;
        Instance = this;
    }

    void Update()
    {
        if (Focus)
        {
            float orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, focusOthographicSize, Time.deltaTime * cameraSpeed);
            Camera.main.orthographicSize = orthographicSize;
            carSpecialCamera.orthographicSize = orthographicSize;
            Transform.position = Vector3.Lerp(Transform.position, Focus.position + focusOffset, Time.deltaTime * cameraSpeed);
        }
        else
        {
            float orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, baseOthographicSize, Time.deltaTime * cameraSpeed);
            Camera.main.orthographicSize = orthographicSize;
            carSpecialCamera.orthographicSize = orthographicSize;
            Transform.position = Vector3.Lerp(Transform.position, basePosition, Time.deltaTime * cameraSpeed);
        }
    }
}
