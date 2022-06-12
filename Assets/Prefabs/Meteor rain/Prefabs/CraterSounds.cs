using UnityEngine;

public class CraterSounds : MonoBehaviour
{
    [SerializeField] private AudioSource _source;
    [SerializeField] private AudioClip[] _clips;
    

    private void Start()
    {
        _source.panStereo = Random.Range(0, 1) == 1 ? -1 : 1;
        int index = Random.Range(0, _clips.Length);
        _source.PlayOneShot(_clips[index]);
    }
}
