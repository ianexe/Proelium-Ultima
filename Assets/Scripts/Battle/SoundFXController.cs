using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXController : MonoBehaviour
{
    public List<AudioSource> sfx_list;
    public AudioSource light_damage_sfx;
    public AudioSource high_damage_sfx;
    private List<AudioSource> sfx_instances = new List<AudioSource>();

    void Update()
    {
        if (sfx_instances.Count > 0)
        {
            List<AudioSource> to_iterate = new List<AudioSource>(sfx_instances);

            foreach (AudioSource audio in to_iterate)
            {
                if (!audio.isPlaying)
                {
                    sfx_instances.Remove(audio);
                    Destroy(audio.gameObject);
                }
            }

            to_iterate.Clear();
        }  
    }
    public void Play(int sfx_id)
    {
        AudioSource instance = Instantiate(sfx_list[sfx_id]);
        sfx_instances.Add(instance);
        instance.Play();
    }

    public void PlayLightDamage()
    {
        AudioSource instance = Instantiate(light_damage_sfx);
        sfx_instances.Add(instance);
        instance.Play();
    }

    public void PlayHighDamage()
    {
        AudioSource instance = Instantiate(high_damage_sfx);
        sfx_instances.Add(instance);
        instance.Play();
    }
}
