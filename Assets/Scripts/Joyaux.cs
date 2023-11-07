using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Classe qui gère les joyaux dans le niveau. Elle active les détruits et gère les collisions avec perso.
/// Auteurs du code: Michaël Malard
/// Auteur des commentaires: Michaël
/// #Tp3
/// </summary>
public class Joyaux : MonoBehaviour
{

    [SerializeField] TypeArgent typeArgent; //Enum pour savoir quel est le type de l'argent

    static List<Joyaux> _lesJoyaux = new List<Joyaux>();    //liste vide des joyaux
    public static int nbJoyaux => _lesJoyaux.Count; //Nombre de joyayx dans une salle

    private CircleCollider2D _collider; //Collider du joyaux pour le désactiver/activer


    /// <summary>
    /// Prend les composants nécessaire au démarrage. Comme le Collider et le SpriteRenderer. Il ajoute le joyaux à la liste.
    /// </summary>
    void Awake()
    {
        _lesJoyaux.Add(this);   //Ajoute le joyaux à la liste
        _collider = GetComponent<CircleCollider2D>();   //Va chercher le composant "CircleCollider2D"
    }


    /// <summary>
    /// Activer lorsqu'il y a une collision de type "Trigger".
    /// </summary>
    /// <param name="other">L'objet de la collision</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        //Va chercher le script perso de l'objet de la collision pour faire une vérification
        Perso perso = other.gameObject.GetComponent<Perso>();

        //Si le perso n'est pas null les effect s'activent
        if (perso != null)
        {
            //Si l'argent est de type "Lumiere", la mécanique du bonus est active si celui-ci est actif
            if (typeArgent == TypeArgent.Lumiere)
            {
                // Si le bonus de piece est actif le perso reçoi 2 lumière
                if (perso.bonusPiece) perso.donnees.lumiere += 2;

                // Si le bonus de piece est inactif le perso reçoi 1 lumière
                else perso.donnees.lumiere++;
                Destroy(this.gameObject);   //Détruit le gameObject du joyaux 

                Niveau.instance.AfficherJoyauxObtenue(TypeArgent.Lumiere);   //#tp4
            }
            //Si l'argent est de type "Cristal", le perso recoi un cristal
            else if (typeArgent == TypeArgent.Cristal)
            {
                perso.donnees.cristal++;    //le perso reçoi 1 cristal
                Destroy(this.gameObject);   //détruir le gameObject du joyaux

                Niveau.instance.AfficherJoyauxObtenue(TypeArgent.Cristal);   //#tp4
            }
        }
    }

    /// <summary>
    /// Quand l'appplication est quittée la liste de joyaux se vide
    /// </summary>
    void OnApplicationQuit()
    {
        _lesJoyaux.Clear(); //La liste de joyaux se vide
    }
}
