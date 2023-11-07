using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// classe qui place les objet dans une salle si elle a ete pige aleatoirement dans le script niveau
/// /// Auteurs du code: Thomas Brunet et Michaël Malard
/// Auteur des commentaires: Thomas Brunet
/// </summary>
public class Salle : MonoBehaviour
{
    static Vector2Int _tailleAvecBordures = new Vector2Int(32, 18);  // la taille de chaque salle
    static public Vector2Int tailleAvecBordures => _tailleAvecBordures;     // static de la taille de la salle

    //#tp3 Thomas
    [SerializeField] Transform _reperePorte;    //le repere de la porte
    [SerializeField] Transform _repereCle;  //le repere de la cle
    [SerializeField] Transform _reperePerso;    //le repere du perso
    [SerializeField] Transform _repereActivateur;   //le repere de l'Activateur
    [SerializeField] Transform _repereEffector;   //le repere de l'effector


    [SerializeField] Transform _repereEnnemiMichael; //position de spawn de l'ennemi de Michael synthese

    /// <summary>
    /// Une fonction qui instantie la porte dans la salle choisie
    /// </summary>
    /// <param name="model">Le model de la porte</param>
    /// <returns>la position occupé par l'objet instantier</returns>
    public Vector2Int PlacerPorteSurRepere(GameObject model)
    {
        Vector3 posPorte = _reperePorte.position;
        Instantiate(model, posPorte, Quaternion.identity, transform.parent);
        Debug.Log("Porte apparue");
        return Vector2Int.FloorToInt(posPorte);
    }
    /// <summary>
    /// Une fonction qui instantie l'activateur dans la salle choisie
    /// </summary>
    /// <param name="model">Le model de l'activateur'</param>
    /// <returns>la position occupé par l'objet instantier</returns>
    public Vector2Int PlacerActivateurSurRepere(GameObject model)
    {
        Vector3 posActivateur = _repereActivateur.position;
        Instantiate(model, posActivateur, Quaternion.identity, transform.parent);
        Debug.Log("Activateur apparue");
        return Vector2Int.FloorToInt(posActivateur);
    }

    /// <summary>
    /// Une fonction qui instantie la clé dans la salle choisie
    /// </summary>
    /// <param name="model">Le model de la clé</param>
    /// <returns>la position occupé par l'objet instantier</returns>
    public Vector2Int PlacerCleSurRepere(GameObject model)
    {
        Vector3 pos = _repereCle.position;
        Instantiate(model, pos, Quaternion.identity, transform.parent);
        Debug.Log("Clé apparue");
        return Vector2Int.FloorToInt(pos);
    }
    /// <summary>
    /// Une fonction qui instantie le perso dans la salle choisie
    /// </summary>
    /// <param name="model">Le model du perso</param>
    /// <returns>la position occupé par l'objet instantier</returns>
    public GameObject PlacerObjetSurRepere(GameObject model)
    {
        Vector3 pos = _repereEffector.position;
        if (model.GetComponent<Perso>() != null)
        {
            pos = _reperePerso.position;
        }

        GameObject modelInstantialte = Instantiate(model, pos, Quaternion.identity);
        Debug.Log("Position perso changé");
        return modelInstantialte;
    }
    /// <summary>
    /// Une fonction qui instantie l'effector dans la salle choisie
    /// </summary>
    /// <param name="model">Le model du perso</param>
    /// <returns>la position occupé par l'objet instantier</returns>


    public Vector2Int PlacerEnnemieMichaelSurRepere(EnnemieMichael model)
    {
        Transform conteneurEnnemiesMichael = new GameObject("EnnemiMichael").transform;   //Groupe pour les bonus
        conteneurEnnemiesMichael.parent = transform;   //Mettre le groupe dans le niveau
        Vector3 pos = _repereEnnemiMichael.position;
        Instantiate(model, pos, Quaternion.identity, conteneurEnnemiesMichael);
        Debug.Log("Ennemi Michael apparue");
        return Vector2Int.FloorToInt(pos);
    }






    /// <summary>
    /// Gizmo qui dessine la contour de la salle
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, (Vector3Int)_tailleAvecBordures);
    }
}
