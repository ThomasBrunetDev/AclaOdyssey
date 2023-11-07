using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geyser : MonoBehaviour
{
    Particules _particules; //référence au script particules pour pouvoir avoir le controle sur les particules

    [SerializeField] AudioClip _sonGeyser; //audioclip quand le geyser explose
    

    /// <summary>
    /// Prend les composants nécessaire au démarrage. Comme le Particules.
    /// </summary>
    void Awake()
    {
        _particules = GetComponentInChildren<Particules>();
    }

    /// <summary>
    /// Activer lorsqu'il y a une collision de type "Trigger".
    /// </summary>
    /// <param name="other">le perso</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        Perso perso = other.gameObject.GetComponent<Perso>(); //Va chercher le script perso de l'objet de la collision pour faire une vérification

        //Si le perso n'est pas null les effect s'activent
        if(perso != null)
        {
            GestAudio.instance.JouerEffetSonore(_sonGeyser); //joue le son du geyser
            StartCoroutine(_particules.CoroutineAugmenterForce()); //lance la coroutine pour augmenter la force des particules
        }
    }
}
