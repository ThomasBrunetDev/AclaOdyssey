using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Classe qui gère les particules dans le jeu. Les partir, arrêter, etc.
/// Auteurs du code: Thomas Brunet et Michaël Malard
/// Auteur des commentaires: Michaël Malard
/// #Tp3
/// </summary>
public class Particules : MonoBehaviour
{
    [SerializeField] Color _teinteAmeliorationBonus = new Color(0f, 1f, 10, 1f); //couleur que les particules auront #thomas

    private ParticleSystem _part;   //Chercher les particules en tant que tel
    private ParticleSystem.MainModule _main;    //Module le "main" module des particule pour changer leurs couleurs
    private ParticleSystem.EmissionModule _emission;   //Module de l'émission pour changer l'émission des particule
    private ParticleSystem.ForceOverLifetimeModule _force; //Module de la force pour changer la force des particules #synthese Michael

    void Awake()
    {
        _part = GetComponent<ParticleSystem>(); //Va chercher le composant "ParticuleSystem"
        _main = _part.main; //Va chercher le module "main" des particules 
        _emission = _part.emission; //Va chercher le module "emission" des particules 

        _force = _part.forceOverLifetime; //Va chercher le module "force" des particules #synthese Michael
    }

    /// <summary>
    /// Démarre les particules
    /// </summary>
    public void Demarrer()
    {
        _part.Play();
    }

    /// <summary>
    /// Arrête les particules
    /// </summary>
    public void Arreter()
    {
        _part.Stop();
    }
    /// <summary>
    /// appelle la coroutine qui fait teinter les particules #thomas
    /// </summary>
    public void Teinter()
    {
        StartCoroutine(CoroutineTeinter());
    }

    /// <summary>
    /// Augmente l'émission des particules
    /// </summary>
    public void AugmenterEmission()
    {
        _emission.rateOverTime = 20;    //Augmente l'émisson à 20
    }

    /// <summary>
    /// Coroutine qui augmente la force des particules pour le geyser
    /// </summary>
    /// <returns></returns>
    public IEnumerator CoroutineAugmenterForce()
    {
        _force.y = 5;  //Augmente la force à 5
        yield return new WaitForSeconds(.5f); //Attend 5 secondes
        _force.y = -3;   //Rétablit la force à 0
    }

    /// <summary>
    /// Rétablit l'émission de base des particules
    /// </summary>
    public void RetablirEmission()
    {
        _emission.rateOverTime = 8; //Rétablit l'émission par défaut
    }
    /// <summary>
    /// croutine qui s'occupe de teinter les particules pedant 5 seconde    #thomas
    /// </summary>
    /// <returns>temps d'arret</returns>
    private IEnumerator CoroutineTeinter()
    {
        Debug.Log("coroutine teinter démare");
        ParticleSystem.MinMaxGradient couleurOrigine = _main.startColor;
        _main.startColor = _teinteAmeliorationBonus;
        yield return new WaitForSeconds(5f);
        _main.startColor = couleurOrigine;
    }
}
