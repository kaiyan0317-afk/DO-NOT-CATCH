using UnityEngine;

public class ButtonSE : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip se;

    public void PlaySE()
    {
        audioSource.PlayOneShot(se);
    }
}
