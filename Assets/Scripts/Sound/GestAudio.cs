using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe qui gère tout les sons du jeu
/// Auteurs du code: Michaël Malard
/// Auteur des commentaires: Michaël Malard
/// </summary>
public class GestAudio : MonoBehaviour
{
    [SerializeField] float _pitchMinEffetSonore = 0.95f;
    [SerializeField] float _pitchMaxEffetSonore = 1.05f;
    [SerializeField] SOPiste[] _tPistes; //les pistes musicales
    [SerializeField] float _volumeMusiqueRef = 0.5f; //le volume de référence pour la musique
    public float volumeMusiqueRef => _volumeMusiqueRef;
    AudioSource _sourceEffetsSonores;
    public AudioSource sourceEffetsSonores => _sourceEffetsSonores;
    static GestAudio _instance;
    static public GestAudio instance => _instance;
    public SOPiste[] tPistes => _tPistes;



    void Awake()
    {
        if (_instance == null) _instance = this;
        else
        {
            Debug.Log("Un gestionnaire audio existe déjà, donc celui sur la scène sera détruit");
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        _sourceEffetsSonores = gameObject.AddComponent<AudioSource>();
        _sourceEffetsSonores.volume = 0.5f; //on met le volume à 50%
        CreerLesSourcesMusicales();
    }

    void CreerLesSourcesMusicales()
    {
        foreach (SOPiste piste in tPistes)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            piste.Initialiser(source);
        }
    }

    public void JouerEffetSonore(AudioClip clip)
    {

        _sourceEffetsSonores.pitch = Random.Range(_pitchMinEffetSonore, _pitchMaxEffetSonore); //on change le pitch
        _sourceEffetsSonores.PlayOneShot(clip); //on joue le son

    }


    public void ChangerVolume(float volume)
    {
        StopAllCoroutines();
        foreach (SOPiste piste in tPistes)
        {
            if (piste.estActif) StartCoroutine(CoroutineChangerVolume(piste, volume, 1f / 2f));
        }
    }

    public IEnumerator CoroutineChangerVolume(SOPiste piste, float volumeFinal, float duree)
    {
        _volumeMusiqueRef = volumeFinal;

        //on calcule le temps initial, le temps actuel et le temps final
        float tempsInitial = Time.time;
        float tempsActuel = tempsInitial;
        float tempsFinal = tempsInitial + duree;

        float volumeInitial = piste.source.volume;

        while (tempsActuel < tempsFinal)
        {
            tempsActuel = Time.time;
            float pourcentage = (tempsActuel - tempsInitial) / duree;
            float nouveauVolume = Mathf.Lerp(volumeInitial, volumeFinal, pourcentage);
            piste.source.volume = nouveauVolume;
            yield return null;
        }

    }


    public void ChangerEtatLecturePiste(TypePiste type, bool estActif)
    {
        foreach (SOPiste piste in tPistes)
        {
            if (piste.type == type)
            {
                Debug.Log("Changement de l'état de la piste " + piste.type + " à " + estActif);
                piste.estActif = estActif;
                return; //on a trouvé la piste, on sort de la boucle
            }
        }
    }

}