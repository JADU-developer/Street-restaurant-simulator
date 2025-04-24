using UnityEngine;

public class soundEffectsManager : MonoBehaviour
{

    [SerializeField] private AudioSource soundEffectsObject3D;
    [SerializeField] private AudioSource soundEffectsObject2D;

    [SerializeField] private float VolumeMultiplier = 1f;

    public static soundEffectsManager instance {get; private set; }

    private void Awake()
    {
        instance = this;
    }

    public void playSoundEffectsClip3D(AudioClip audioClip, Transform spawnTransform, float volume = 1)
    {

    
        // Sets up an audio source.
        AudioSource audioSource = Instantiate(soundEffectsObject3D, spawnTransform.position, Quaternion.identity);

        // Assigns the audio clip.
        audioSource.clip = audioClip;

        // Assigns the volume.
        audioSource.volume = volume * VolumeMultiplier;


        // Plays the sound.
        audioSource.Play();

        // Gets the length of the sound effects clip.
        float clipLength = audioSource.clip.length;

    
        // Destroys the game object after the clip length.
        Destroy(audioSource.gameObject, clipLength);
    }
    public void playSoundEffectsClip2D(AudioClip audioClip, Transform spawnTransform, float volume = 1)
    {

    
        // Sets up an audio source.
        AudioSource audioSource = Instantiate(soundEffectsObject2D, spawnTransform.position, Quaternion.identity);

        // Assigns the audio clip.
        audioSource.clip = audioClip;

        // Assigns the volume.
        audioSource.volume = volume * VolumeMultiplier;


        // Plays the sound.
        audioSource.Play();

        // Gets the length of the sound effects clip.
        float clipLength = audioSource.clip.length;

    
        // Destroys the game object after the clip length.
        Destroy(audioSource.gameObject, clipLength);
    }

    public void playRandomSoundEffectsClip3D(AudioClip[] audioClip, Transform transform, float volume)
    {

        // Grabs a random index.
        int rand = Random.Range(0, audioClip.Length);

        playSoundEffectsClip3D(audioClip[rand], transform, volume);
    }

    public void playRandomSoundEffectsClip2D(AudioClip[] audioClip, Transform transform, float volume)
    {

        // Grabs a random index.
        int rand = Random.Range(0, audioClip.Length);

        playSoundEffectsClip2D(audioClip[rand], transform, volume);
    }
}
