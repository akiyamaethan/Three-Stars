using UnityEngine;

public class BackgroundMotion : MonoBehaviour
{
    public float zoomSpeed = 0.01f;

    void Update()
    {
        transform.localScale += Vector3.one * zoomSpeed * Time.deltaTime;
    }
}