using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// Classe qui active les bonus dans le niveau à l'aide d'un événement lorsque qu'il rentre en collision avec le peronnage.
/// Auteurs du code: Thomas Brunet et Michaël Malard
/// Auteur des commentaires: Michaël Malard
/// #Tp3
/// </summary>
public class Activateur : MonoBehaviour
{
    private UnityEvent _evenementBonus = new UnityEvent();  //Un événement pour activer les bonus dans les salle
    public UnityEvent evenementBonus => _evenementBonus;    //Un getter pour avoir accès à l'événement

    private SpriteRenderer _sr; //SpriteRenderer pour changer le sprite du gameObject

    [SerializeField] Sprite _spriteInactif; //Sprite du bonus lorqu'il est inactif
    [SerializeField] Sprite _spriteActif;   //Sprite du bonus lorsqu'il est actif

    static private Activateur _instance;    //Singleton
    static public Activateur instance => _instance; //getter pour le Singleton

    /// <summary>
    /// Vérification du singleton et composant "SpriteRenderer" au début du jeu.
    /// </summary>
    void Awake()
    {
        // Vérification du Singleton
        if (_instance != null) { Destroy(gameObject); return; }
        _instance = this;

        _sr = GetComponent<SpriteRenderer>();   //Va chercher le composant "SpriteRenderer"

    }


    /// <summary>
    /// Lorsque l'activateur entre en collision avec un autre gameObject avec un Collider2D. Gère principalement avec la collision avec le personnage.
    /// </summary>
    /// <param name="other">L'autre gameObject avec un Collider2D</param>
    void OnCollisionEnter2D(Collision2D other)
    {
        //Va chercher le script perso de l'objet de la collision pour faire une vérification
        Perso perso = other.gameObject.GetComponent<Perso>();

        //Si le perso n'est pas null l'événement se lance, le sprite de l'activateur change et son collider est désactivé
        if (perso != null)
        {
            evenementBonus.Invoke();
            _sr.sprite = _spriteInactif;    //Changement de sprite pour celui inactif
            GetComponent<CapsuleCollider2D>().enabled = false;
        }
    }


    /// <summary>
    /// Lorsque que le joueur quitte l'application le sprite change
    /// </summary>
    void OnApplicationQuit()
    {
        _sr.sprite = _spriteActif;  //Changement de sprite pour celui actif
    }


}
