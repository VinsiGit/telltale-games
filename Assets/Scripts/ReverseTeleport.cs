using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class ReverseTeleport : MonoBehaviour
{
    public GameObject videoObject;
    private VideoPlayer videoPlayer;
    private float fadeDuration = 3.0f;
    private Vector2 targetScaleXZ = new Vector2(5f, 5f);
    private float rotationSpeedMax = 100f;
    private Material videoMaterial;
    private float a = 0.0f;
    private float alpha = 1.0f;
    private bool isFading = false;
    private Vector3 initialScale;

    void Start()
    {
        videoObject.SetActive(false);
        videoPlayer = videoObject.GetComponentInChildren<VideoPlayer>();

        Renderer renderer = videoObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            videoMaterial = renderer.material;
        }

        if (videoMaterial != null)
        {
            Color color = videoMaterial.color;
            color.a = 1.0f;
            videoMaterial.color = color;
        }

        initialScale = videoObject.transform.localScale;
        StartReverseVideoFade();
    }

    public void StartReverseVideoFade()
    {
        videoObject.SetActive(true);

        VideoPlayer videoPlayer = videoObject.GetComponentInChildren<VideoPlayer>();
        if (videoPlayer != null)
        {
            Debug.Log("video started");
            videoPlayer.time = 2;
            videoPlayer.Play();
        }

        isFading = true;
        alpha = 1.0f;
    }

    void Update()
    {
        if (isFading && videoMaterial != null)
        {
            a += Time.deltaTime / fadeDuration;
            alpha = 1.0f - a;
            if (alpha <= 0.0f)
            {
                alpha = 0.0f;
                isFading = false;
            }
            Debug.Log("alpha " + alpha);
            Color color = videoMaterial.color;
            color.a = alpha;
            videoMaterial.color = color;

            float rotationSpeed = Mathf.Lerp(rotationSpeedMax, 0f, a);
            videoObject.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

            Vector3 currentScale = videoObject.transform.localScale;
            Vector3 newScale = new Vector3(
                Mathf.Lerp(initialScale.x, targetScaleXZ.x, a),
                currentScale.y,
                Mathf.Lerp(initialScale.z, targetScaleXZ.y, a)
            );
            videoObject.transform.localScale = newScale;
        }
    }
}
