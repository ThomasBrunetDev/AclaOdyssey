using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Navigation", menuName = "Navigation")]
/// <summary>
/// ScriptableObject qui gère les déplacements entre les scènes du jeu
/// Auteurs du code: Michaël Malard
/// Auteur des commentaires: Michaël Malard
/// #Tp3
/// </summary>
public class SONavigation : ScriptableObject
{
    [SerializeField] SOPerso _donneesPerso; //Les données du personnage


    /// <summary>
    /// Change de scène pour retourner au jeu principal (Scène de jeu)
    /// </summary>
    public void SortirBoutique()
    {
        AllerScenePrecedente(); //Va à la scène principal (Scène de jeu)
    }

    public void TerminerLeJeu()
    {
        AllerSceneSuivante(2);  //Va à la scène de mort
    }   

    public void RecommencerLeJeu()
    {
        AllerScenePrecedente(3);
    }

    /// <summary>
    /// Utilise le système de Unity pour aller dans la scène suivante
    /// </summary>
    public void AllerSceneSuivante(int index = 1)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + index);   //Ajoute 1 à l'index de la scène pour changer scène
    }

    /// <summary>
    /// Utilise le système de Unity pour aller dans la scène précédente
    /// </summary>
    public void AllerScenePrecedente(int index = 1)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - index);   //Enlève 1 à l'index de la scène pour changer scène
    }

    /// <summary>
    /// Reinitialise les données du personnage pour la prochaine partie // activer par les bouton de debut de partie et de fin de partie
    /// </summary>
    public void InitialiserToutPourLaProchainePartie()
    {
        _donneesPerso.InitialiserTout(); //appel de la fonction qui initialise les valeurs du perso
    }


}
