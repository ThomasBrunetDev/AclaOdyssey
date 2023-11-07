using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Classe qui peut servir a tous personnage qui se deplace
/// Auteurs du code: Thomas Brunet et Michaël Malard
/// Auteur des commentaires: Thomas Brunet et Michaël Malard
/// </summary>
public class BasePerso : MonoBehaviour
{
    [Header("Base Perso ---------------")]
    [Header("Saut")]
    [SerializeField, Range(0f, 2f)] private float _distanceGizmo = 1.18f;  //la distance du gizmo par rapport au centre du personnage
    [SerializeField, Range(0f, 0.5f)] private float _longueurRayonSol = 0.25f;   //longueur rayon
    [SerializeField] private LayerMask _layerMask;  //Declaration du layermask
    protected bool _estAuSol = false;   // Bool pour savoir si le perso est au sol


    // #tp4
    protected bool _estEnLair = false;   // Bool pour savoir si le perso est dans les airs

    [Header("Son")]
    [SerializeField] private AudioClip _sonAtterissage; //son d'atterissage
    [SerializeField] protected AudioClip _sonSaut; //son de saut





    /// <summary>
    /// fixed update virtual protected pour faire en sorte qu'ils soit appeler dans le script fille
    /// </summary>
    virtual protected void FixedUpdate()
    {
        VerifierSol(); //appel de la fonction qui verifie si le perso est au sol
    }



    /// <summary>
    /// Fonction qui détecte le sol pour pouvoir faire sauter le personnage
    /// </summary>
    private void VerifierSol()
    {

        Vector2 pointDepart = (Vector2)transform.position - new Vector2(0, _distanceGizmo); //point de départ du rayon
        Collider2D col = Physics2D.OverlapCircle(pointDepart, _longueurRayonSol, _layerMask);   //detection de sol
        _estAuSol = (col != null); //si le collider touche le sol, le bool est vrai
    }

    /// <summary>
    /// Fonction qui gère le son du saut et de l'atterissage d'un personnage
    /// </summary>
    protected void GererSonAtterissage()
    {
        //si le perso n'est pas au sol et qu'il n'a pas encore joué son saut
        if (!_estAuSol)
        {
            _estEnLair = true; //le perso est en l'air
        }
        //si le perso est au sol et qu'il était en l'air
        else if (_estAuSol && _estEnLair)
        {
            GestAudio.instance.JouerEffetSonore(_sonAtterissage); //joue le son d'atterissage
            _estEnLair = false; //le perso n'est plus en l'air
        }
        


    }

    /// <summary>
    ///Dessine le Gizmo en dessous du personnage pour voir si il touche le sol
    /// </summary>
    void OnDrawGizmos()
    {

        if (Application.isPlaying == false) VerifierSol();
        if (_estAuSol) Gizmos.color = Color.green; //le gizmo devien vert si le collider touche  au sol
        else Gizmos.color = Color.red;//le gizmo devien rouge si le collider ne touche pas au sol
        Vector2 pointDepart = (Vector2)transform.position - new Vector2(0, _distanceGizmo);
        Gizmos.DrawWireSphere(pointDepart, _longueurRayonSol);
    }


}
