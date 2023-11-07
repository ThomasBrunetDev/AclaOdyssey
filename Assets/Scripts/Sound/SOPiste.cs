using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Piste musicale", fileName = "DonneesPiste")]
/// <summary>
/// Classe qui instantie les salle, place les bonus , la porte, la cle, le perso, les joyaux
/// Auteur du code: Johnathan Tremblay
/// Auteur des modifications: Michaël Malard
/// </summary des commentaires: Michaël Malardmmary>
public class SOPiste : ScriptableObject
{
    [Header("Données de la piste")]
    [SerializeField] TypePiste _type; //le type de piste
    [SerializeField] AudioClip _clip; //le clip à jouer

    [Header("État actif")]
    [SerializeField] bool _estActifParDefaut; //permet de choisir l'état initial
    [SerializeField] bool _estActif; //c'est l'état actuel
    AudioSource _source; //la source audio qui jouera le clip
    public AudioSource source => _source; //propriété pour accéder à la source audio

    public TypePiste type => _type; //propriété pour accéder au type de piste
    public AudioClip clip => _clip; //propriété pour accéder au clip


    public bool estActif 
    {
        get => _estActif;
        set
        {
            _estActif = value;
            AjusterVolume();
        }
    }
    
    /// <summary>
    /// Initialise la piste avec les propriétés de base de la piste
    /// </summary>
    /// <param name="source">Le AudioSource de la piste</param>
    public void Initialiser(AudioSource source)
    {
        _source = source; //on assigne la source audio
        _source.clip = _clip; //on assigne le clip
        _source.loop = true; //on met le clip en boucle
        _source.playOnAwake = false; //on désactive le playOnAwake
        _source.Play(); //on joue le clip
        _estActif = _estActifParDefaut; //on assigne l'état initial
        AjusterVolume(); //on ajuste le volume
    }

    /// <summary>
    /// Ajuste le volume de la piste en fonction de la valeur dans le gestionnaire audio
    /// </summary>
    public void AjusterVolume()
    {   
        //si la piste est active, on ajuste le volume avec la valeur du gestionnaire audio
        if(estActif) _source.volume = GestAudio.instance.volumeMusiqueRef;
        else _source.volume = 0; //sinon, on met le volume à 0
    }

}