using UnityEngine;

/// <summary>
/// Classe à ajouter sur la caméra pour suivre une cible
/// (Inutile d'améliorer ce script: il sera éventuellement remplacer par Cinemachine.)
/// Auteur du code et des commentaires: Jonathan Tremblay
/// </summary>
public class CameraCible : MonoBehaviour
{
    [SerializeField] private Transform _cible; //la cible à suivre (ex. perso)

    private Vector3 _decalage; //le décalage a maintenir entre la camera et sa cible

    // Start is called before the first frame update
    void Start()
    {
        //cueillette des positions initiales:
        Vector3 posCamInitiale = transform.position;
        Vector3 posCibleInitiale = _cible.position;
        //calcul du décalage:
        _decalage = posCamInitiale - posCibleInitiale;
    }

    // Update is called once per frame
    void Update()
    {
        //ajustement de la position de la caméra:
        transform.position = _cible.position + _decalage;
    }
}