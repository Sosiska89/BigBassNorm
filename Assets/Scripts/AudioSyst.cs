using UnityEngine;

public class AudioSyst : MonoBehaviour
{
    [HideInInspector] public static AudioSyst Instanse;
    [SerializeField] private AudioSource _backMusic;

    private void Start()
    {
        if (Instanse == null)
        {
            Instanse = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetMusic(bool isMusic)
    {
        CompRoot.Instanse.IsSound = isMusic;
        if (isMusic)
        {
            _backMusic.Play();
        }
        else
        {
            _backMusic.Stop();
        }
    }

}
