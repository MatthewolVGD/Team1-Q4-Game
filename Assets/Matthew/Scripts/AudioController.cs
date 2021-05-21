using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioSource controller;
    public AudioClip[] songs;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<AudioSource>();
        controller.clip = songs[3];
        controller.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
