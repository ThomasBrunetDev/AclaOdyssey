using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Objet", menuName = "Objet")]
/// <summary>
/// Scriptable object pour les donnees d'un amelioration¸
/// /// Auteurs du code: Thomas Brunet
/// Auteur des commentaires:Thomas BRunet
/// #tp3
/// </summary>
public class SOObjet : ScriptableObject
{
    [SerializeField] TypeAmelioration _typeAmelioration;    //enum pour savoir le type d'amelioration
    public TypeAmelioration typeAmelioration => _typeAmelioration;  //getter pour l'enum
    
    [Header("LES DONNÉES")]//une categorie dans l'inspecteur pour separer les champs
    [SerializeField] string _nom = "Objet"; //le nom de l'objet
    [SerializeField][Tooltip("Image de l'icone à afficher")] Sprite _sprite;   // l'image a afficher de l'amelioration 
    [SerializeField][Range(0, 200)] int _prixDeBase = 30;  //le prix de l'amelioration 
    [SerializeField] AudioClip _sonAchat; //le son a jouer quand on achete l'amelioration
    [SerializeField][TextArea] string _description;    //la descriptionde l'amelioration 

    public string nom { get => _nom; set => _nom = value; } //getter + setter du nom
    public Sprite sprite { get => _sprite; set => _sprite = value; }//getter + setter du sprite
    public int prixDeBase { get => _prixDeBase; set => _prixDeBase = Mathf.Clamp(value, 0, int.MaxValue); }//getter + setter du prix et limitte du prix a la valeur maximum des int
    public string description { get => _description; set => _description = value; }//getter + setter de la description

    public AudioClip sonAchat => _sonAchat; //getter du son d'achat

}
