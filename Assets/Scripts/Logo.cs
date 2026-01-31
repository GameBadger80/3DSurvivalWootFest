using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Logo : MonoBehaviour
{
    [Header("UI Settings")]
    public Image[] splashImages;     // Array of images to display
    public string nextScene;         // Scene to load after all splashes

    [Header("Timing Settings")]
    public float fadeInTime = 1.5f;
    public float holdTime = 1.0f;
    public float fadeOutTime = 2.5f;

    IEnumerator Start()
    {
        foreach (Image img in splashImages)
        {
            // Ensure image is active and starts invisible
            img.gameObject.SetActive(true);
            img.canvasRenderer.SetAlpha(0f);

            // Fade in
            img.CrossFadeAlpha(1f, fadeInTime, false);
            yield return new WaitForSeconds(fadeInTime + holdTime);

            // Fade out
            img.CrossFadeAlpha(0f, fadeOutTime, false);
            yield return new WaitForSeconds(fadeOutTime);

            // Optional: deactivate image to keep hierarchy clean
            img.gameObject.SetActive(false);
        }

        // Load next scene after all images
        SceneManager.LoadScene(nextScene);
    }
}
