using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioStuff : MonoBehaviour
{
    [SerializeField] AudioSource _audioSource;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PlayRandomPitchedAudio());
    }

    IEnumerator PlayRandomPitchedAudio(){
        while(true){
            _audioSource.volume = Random.Range(0.3f,0.5f);
            _audioSource.pitch = Random.Range(0.7f,1.4f);
            _audioSource.Play();
            yield return new WaitWhile(()=>_audioSource.isPlaying);
        }
    }
}
