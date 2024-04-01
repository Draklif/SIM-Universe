using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarGenerator : MonoBehaviour
{
    [SerializeField] MeshRenderer starPrefab;

    public int count = 1000;
    public Vector2 radiusRange;
    public Vector2 brightnessRange;

    const float distance = 2000;

    Camera cam;

    void Start()
    {
        cam = Camera.main;

        float starDistance = cam.farClipPlane - radiusRange.y;
        float starScale = starDistance / distance;

        for (int i = 0; i < count; i++)
        {
            MeshRenderer star = Instantiate(starPrefab, Random.onUnitSphere * starDistance, Quaternion.identity, transform);
            star.transform.localScale = Vector3.one * Mathf.Lerp(radiusRange.x, radiusRange.y, SmallestRandomValue(6)) * starScale;
            star.material.color = Color.Lerp(Color.black, star.material.color, Mathf.Lerp(brightnessRange.x, brightnessRange.y, SmallestRandomValue(6)));
        }
    }

    float SmallestRandomValue(int iterations)
    {
        float r = 1;
        for (int i = 0; i < iterations; i++)
        {
            r = Mathf.Min(r, Random.value);
        }
        return r;
    }

    void LateUpdate()
    {
        if (cam != null)
        {
            transform.position = cam.transform.position;
        }   
    }
}
