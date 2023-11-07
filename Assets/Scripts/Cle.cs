using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/// <summary>
/// Classe de la clé qui se gere lorsque le perso la recupère et qui se detruit
/// /// Auteurs du code: Thomas Brunet
/// Auteur des commentaires: Thomas Brunet
/// #tp3
/// </summary>
public class Cle : MonoBehaviour
{
    [SerializeField] private AudioClip _sonClee;//audioClip du son de la clée #tp4 Thomas

    [SerializeField] private TypePiste typePiste = TypePiste.musiqueEvenA; //Le type de piste de la cle #tp4 Michael
    [SerializeField] private SOPiste donneePiste; //Les données de la piste de la cle #tp4 Michael

    /// <summary>
    /// Lors d'une colision
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        Perso perso = other.gameObject.GetComponent<Perso>();
        if (perso != null)
        {
            perso.possedeCle = true; // le boolean pour savoir si le perso a la cle devient true 


            Niveau.instance.AfficherCleObtenue(perso); //Affiche le texte de la cle obtenue #tp4 Thomas
            GestAudio.instance.JouerEffetSonore(_sonClee); //#tp4 Thomas

            GestAudio.instance.ChangerEtatLecturePiste(typePiste, true); //Changer le type de piste #tp4 Michael
            GestAudio.instance.ChangerVolume(.1f); //Changer le volume de la piste #tp4 Michael

            Destroy(gameObject);
        }
    }
}
