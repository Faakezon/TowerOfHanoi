using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    //Instantiate a static singleton for easy access.
    public static AudioManager instance = null;

    private AudioSource sfxSource;

    [SerializeField]
    AudioClip WonGameClip;
    [SerializeField]
    AudioClip PickUp;
    [SerializeField]
    AudioClip CollideWithOtherRing;

    // Use this for initialization
    void Awake () {
        InitAudioManager();
        sfxSource = GetComponent<AudioSource>();
	}
	
    private void InitAudioManager()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void PlaySfx(string name)
    {
        switch (name)
        {
            case "WonGame":
                sfxSource.volume = 0.3f;
                sfxSource.PlayOneShot(WonGameClip);
                break;
            case "PickUp":
                sfxSource.volume = 0.4f;
                sfxSource.PlayOneShot(PickUp);
                break;
            case "Collide":
                sfxSource.volume = 0.5f;
                sfxSource.PlayOneShot(CollideWithOtherRing);
                break;
        }
    }
}
