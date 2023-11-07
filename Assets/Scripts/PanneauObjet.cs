using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Classe du panneau d'un objet qui va va gerer l'achat du objet, ce que le UI va afficher et gerrer les disponibilité d'un amelioration
/// Auteurs du code: Thomas Brunet
/// Auteur des commentaires: Thomas Brunet
/// #tp3
/// </summary>
public class PanneauObjet : MonoBehaviour
{
    [Header("DONNÉES")] //une categorie dans l'inspecteur pour separer les champs
    [SerializeField] SOObjet _donnees; //les donnée de l'amelioration
    public SOObjet donnees => _donnees;  //getter des donnée de l'objet

    [Header("CONTENEURS")] //une categorie dans l'inspecteur pour separer les champs
    [SerializeField] TextMeshProUGUI _champNom;     //champ de text du nom de l'amelioration
    [SerializeField] TextMeshProUGUI _champPrix;    //champ de text du prix de l'amelioration
    [SerializeField] TextMeshProUGUI _champDescription; //champ de text de la description de l'amelioration
    [SerializeField] Image _image;  //l'image de l'amelioration
    [SerializeField] CanvasGroup _canvasGroup;  //le canvas group d'un panneau
    /// <summary>
    /// le start met a jour les information du UI et s'abonnent a l'evenement de donnePerso pour mettre a jour les infos
    /// </summary>
    void Start()
    {
        MettreAJourInfos(); //met a jour les infos du UI
        Boutique.instance.donneesPerso.evenementMiseAJour.AddListener(MettreAJourInfos);    //s'abonne a l'evenement de donnePerso pour mettre a jour les infos
        GererDispo();   //gere les disponibilité des objets en vente
    }
    /// <summary>
    /// met à jour les element de UI
    /// </summary>
    private void MettreAJourInfos()
    {
        _champNom.text = _donnees.nom;
        _champPrix.text = _donnees.prixDeBase + " $";
        _champDescription.text = _donnees.description;
        _image.sprite = _donnees.sprite;
        _image.preserveAspect = true;

        GererDispo();
    }
    /// <summary>
    /// gere les disponibilité des objets en vente
    /// </summary>
    public void GererDispo()
    {
        //#synthese thomas

        //si l'objet est une map et qu'elle est acheté le canvas group n'est pas interactible (on ne peut pas acheter la map 2 fois)
        if (_donnees.typeAmelioration == TypeAmelioration.MiniMap)
        {
            if (Boutique.instance.donneesPerso.estAcheteMap)
            {
                Destroy(gameObject);
            }
            else _canvasGroup.interactable = true; _canvasGroup.alpha = 1;
        }
        
        if (_donnees.typeAmelioration == TypeAmelioration.Bulle)
        {
            if (Boutique.instance.donneesPerso.estAcheteBulle)
            {
                Destroy(gameObject);
            }
            else _canvasGroup.interactable = true; _canvasGroup.alpha = 1;
        }
        //fin #synthese thomas

        bool aAssezArgent = Boutique.instance.donneesPerso.lumiere >= _donnees.prixDeBase;
        if (aAssezArgent) //si le joueur a assez d'argent le canvas group est interactible et il peut acheter l'objet
        {
            _canvasGroup.interactable = true;
            _canvasGroup.alpha = 1;
        }
        else    //sinon la canvas group n'est pas interactible
        {
            _canvasGroup.interactable = false;
            _canvasGroup.alpha = 0.5f;
        }
    }
    //achat d'un objet
    public void Acheter()
    {
        _canvasGroup.interactable = false;  //on peut achater l'objet qu'une seul fois par niveau donc a l'achat le canvas group se desactive
        _canvasGroup.alpha = 0.5f;
        GererDispo();   //gere les disponibilité des objets en vente
        Boutique.instance.donneesPerso.Acheter(_donnees);
    }
}
