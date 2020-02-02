using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTest : MonoBehaviour
{
    public AudioClip clip;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            AudioManager.PlaySingleShot(AudioManager.Sounds.NewCharSound);
        }
    }
}
