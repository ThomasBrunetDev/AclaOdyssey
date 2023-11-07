using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Classe qui gère la collision entre la porte et le joueur
/// Auteurs du code: Thomas Brunet
/// Auteur des commentaires: Michaël
/// #Tp3
/// </summary>
public class Porte : MonoBehaviour
{
    [SerializeField] SONavigation _navigation;  //Utilisation du scriptable object de navigation pour le changement de scene #tp3 Thomas
    [SerializeField] private AudioClip _sonPorte; //audioClip du son de la porte #tp4 Thomas
    [SerializeField] private TypePiste typePiste = TypePiste.musiqueEvenA; //Le type de piste de la cle #tp4 Michael
    [SerializeField] private SOPiste donneePiste; //Les données de la piste de la cle #tp4 Michael

    /// <summary>
    /// La collision de type "Trigger", principalement avec le personnage.
    /// </summary>
    /// <param name="other">L'objet de la collision</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        //Va chercher le script perso de l'objet de la collision pour faire une vérification
        Perso perso = other.gameObject.GetComponent<Perso>();

        //Si le perso n'est pas null une condition se lance
        if (perso != null)
        {
            ///Si le perso à la clé il change de salle (vers Boutique)
            if (perso.possedeCle)
            {
                GestAudio.instance.JouerEffetSonore(_sonPorte); //#tp4 Thomas

                GestAudio.instance.ChangerEtatLecturePiste(typePiste, false); //Changer le type de piste #tp4 Michael
                GestAudio.instance.ChangerVolume(0f); //Changer le volume de la piste #tp4 Michael

                StartCoroutine(CoroutineChangerSalle(perso));    //Démarrage du changement de salle vers boutique
            }
        }
    }

    /// <summary>
    /// Une coroutine pour créer un délai entre la collision et le changement de salle
    /// </summary>
    /// <returns>Temps d'attendre</returns>
    private IEnumerator CoroutineChangerSalle(Perso perso)
    {
        yield return new WaitForSeconds(1f);    //Attente de 1 seconde
        perso.donnees.vie += 20f;  //Ajout de 20 points de vie au perso #synthese Michael
        _navigation.AllerSceneSuivante();   //Changement de salle vers boutique
    }
}
