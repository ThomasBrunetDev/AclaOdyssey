using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(fileName = "Perso", menuName = "Perso")]
/// <summary>
/// ScriptableObject qui gère les bonus dans le niveau. Elle active les bonus et gère les collisions avec perso.
/// Auteurs du code: Thomas Brunet et Michaël Malard
/// Auteur des commentaires: Michaël
/// #Tp3
/// </summary>
public class SOPerso : ScriptableObject
{
    int _niveauInitial = 1;   //Niveau Initial
    int _lumiereInitial = 0;   //Lumière Initial
    int _cristalInitial = 0;   //Lumière Initial
    float _vieInitial = 100f;   //Vie Initial
    float _forceSautInitial = 150f;   //Force de saut Initial
    float _vitesseInitial = 6f;   //Vitesse Initial
    int _nbAchatInitial = 0;   //Nombre d'achat

    [Header("Données du perso")]
    [SerializeField, Range(1, 5)] int _niveau = 1;   //Niveau rendu
    [SerializeField, Range(0, 500)] int _lumiere = 0;   //Lumière en possesion
    [SerializeField, Range(0, 500)] int _cristal = 0;   //Lumière en possesion
    [SerializeField, Range(0, 100)] float _vie = 50f;   //Vie du personnage
    [SerializeField, Range(0, 200)] float _forceSaut = 150f;   //Force de saut du personnage
    [SerializeField, Range(0, 10)] float _vitesse = 6f;   //Vitesse du personnage
    private float _forceSautAmeliorer = 200f;   //Force de saut du personnage lors de l'achat du bonus #synthese thomas
    private float _vitesseAmeliorer = 9f;   //Vitesse du personnage lors de l'achat du bonus #synthese thomas
    [SerializeField] int _score = 0;   //Score du joueur
    [SerializeField] int _nbAchat = 0;   //Nombre d'achat total





    //#tp4 michael

    [Header("Sons")]

    [SerializeField] AudioClip _sonAtterissage;  //Le son d'atterissage

    public AudioClip sonAtterissage => _sonAtterissage;  //Le son d'atterissage

    // #sythese Michael
    [SerializeField] AudioClip _sonSaut;  //Le son d'atterissage
    public AudioClip sonSaut => _sonSaut;  //Le son d'atterissage

    [SerializeField] AudioClip _sonBlesse;  //Le son d'atterissage
    public AudioClip sonBlesse => _sonBlesse;  //Le son d'atterissage


    [SerializeField] bool _ameliorationVitesse = false;
    [SerializeField] bool _ameliorationSaut = false;

    //#tp4 michael fin

    // #syntese Michael
    [Header("Mana")]
    [SerializeField] float _manaMax = 100f;
    [SerializeField] float _manaRegeneration = 10f;
    [SerializeField] float _manaRegenerationDelai = 2f;
    [SerializeField] float _coutManaBouclier = 2f;

    [SerializeField] float _manaPresente;
    float _manaRegenDelaiTimer;
    bool _bouclierEstActif;

    [Header("Attaque")]

    [SerializeField] AudioClip _sonAttaque;  //Le son d'attaque #syntese Michael
    public AudioClip sonAttaque => _sonAttaque;  //Le son d'attaque #syntese Michael

    // #syntese Michael fin

    //#synthese Thomas
    [Header("Améliorations achetées")]
    [SerializeField] private bool _estAcheteMap = false; //Si la map est acheté
    public bool estAcheteMap { get => _estAcheteMap; set => _estAcheteMap = value; }    //getter + setter pour le bool de la map
    [SerializeField] private bool _estAcheteBulle = false; //Si la bulle est acheté
    public bool estAcheteBulle { get => _estAcheteBulle; set => _estAcheteBulle = value; }  //getter + setter pour le bool de la bulle
    //#fin synthese Thomas



    //Un getter, setter pour le niveau pour pouvoir le modifier et le lire
    public int niveau
    {
        get => _niveau;
        set
        {
            _niveau = Mathf.Clamp(value, 1, int.MaxValue);
            _evenementMiseAJour.Invoke();   //Invoke l'événement qui indique de mettre à jour le UI
        }
    }

    //Un getter, setter pour le niveau pour pouvoir le modifier et le lire
    public int lumiere
    {
        get => _lumiere;
        set
        {
            _lumiere = Mathf.Clamp(value, 0, int.MaxValue);
            _evenementMiseAJour.Invoke();   //Invoke l'événement qui indique de mettre à jour le UI
        }
    }

    //Getter, setter pour les données du perso
    public int cristal { get => _cristal; set => _cristal = Mathf.Clamp(value, 0, int.MaxValue); }
    public float vieInitial => _vieInitial;
    public float vie { get => _vie; set => _vie = Mathf.Clamp(value, 0, 100); }
    public float forceSaut { get => _forceSaut; set => _forceSaut = Mathf.Clamp(value, 0, float.MaxValue); }
    public float vitesse { get => _vitesse; set => _vitesse = Mathf.Clamp(value, 0, float.MaxValue); }
    public int score { get => _score; set => _score = Mathf.Clamp(value, 0, int.MaxValue); }
    public int nbAchat { get => _nbAchat; set => _nbAchat = Mathf.Clamp(value, 0, int.MaxValue); }

    //tp4 michael amelioration affichage
    public bool ameliorationVitesse { get => _ameliorationVitesse; set => _ameliorationVitesse = value; }
    public bool ameliorationSaut { get => _ameliorationSaut; set => _ameliorationSaut = value; }

    private UnityEvent _evenementMiseAJour = new UnityEvent();  //événement qui indique de mettre à jour le UI
    public UnityEvent evenementMiseAJour => _evenementMiseAJour;    //événement qui indique de mettre à jour le UI
    List<SOObjet> _lesObjets = new List<SOObjet>(); //Une liste d'objet acheter (Inventaire)

    //#tp4 thomas
    int _sallesX = 3; //Nombre de salles en X
    public int sallesX { get => _sallesX; set => _sallesX = value; }    //getter + setter pour le nombre de salles en X

    int _sallesY = 3; //Nombre de salles en Y
    public int sallesY { get => _sallesY; set => _sallesY = value; }    //getter + setter pour le nombre de salles en Y


    private int _saleDepart = 3;//nb Salle de départ

    //#tp4 fin thomas

    //#syntese Michael

    public float manaMax => _manaMax;
    public float manaRegeneration => _manaRegeneration;
    public float manaRegenerationDelai => _manaRegenerationDelai;
    public float coutManaBouclier => _coutManaBouclier;
    public float manaPresente { get => _manaPresente; set => _manaPresente = Mathf.Clamp(value, 0, _manaMax); }
    public float manaRegenDelaiTimer { get => _manaRegenDelaiTimer; set => _manaRegenDelaiTimer = Mathf.Clamp(value, 0, float.MaxValue); }
    public bool bouclierEstActif { get => _bouclierEstActif; set => _bouclierEstActif = value; }



    public void InitialiserTout()
    {
        Initialiser();
        InitialiserBonus();
        InitialiserAmeliorations();
        InitialiserAvantages();
    }


    /// <summary>
    /// Initialise les données du perso et la liste l'objet. Elle fait un "reset" des données au valeurs de bases.
    /// </summary>
    public void Initialiser()
    {
        _niveau = _niveauInitial;   //Initialsation des niveaux
        _lumiere = _lumiereInitial; //Initialsation des lumières
        _cristal = _cristalInitial; //Initialsation des cristaux
        _vie = _vieInitial; //Initialsation de la vie
        _lesObjets = new List<SOObjet>(); //Initialsation de la liste d'objets

        _nbAchat = _nbAchatInitial; //Initialsation du nombre d'achat

        //#tp4 thomas
        _sallesX = _saleDepart; //Initialsation du nombre de salle en X
        _sallesY = _saleDepart; //Initialsation du nombre de salle en Y
        //#tp4 fin thomas
    }

    /// <summary>
    /// Initialise les bonus. Une fonction à part pour que les bonus dûre le niveaux
    /// </summary>
    public void InitialiserBonus()
    {
        _forceSaut = _forceSautInitial; //Initialsation de la force de saut
        _vitesse = _vitesseInitial; //Initialsation de la vitesse
    }
    /// <summary>
    /// Initialise les améliorations. Une fonction à part pour que les améliorations dûre le niveaux
    /// </summary>
    public void InitialiserAmeliorations()
    {
        //#synthese michael
        _ameliorationVitesse = false;
        _ameliorationSaut = false;
    }
    /// <summary>
    /// Initialise les avantages. Une fonction à part pour que les avantages dûre le jeux et non le niveau
    /// </summary>
    public void InitialiserAvantages()
    {
        _estAcheteBulle = false; //Initialsation de la bulle
        _estAcheteMap = false; //Initialsation de la map
    }

    /// <summary>
    /// Met à jour les données quand les données du script son changer dans l'inspecteur ou dans le jeu
    /// </summary>
    void OnValidate()
    {
        _evenementMiseAJour.Invoke();   //Invoke l'événement qui indique de mettre à jour le UI
    }

    /// <summary> #THOMAS
    /// l'achat d'un objet, l'ajout des amelioration pour le niveau suivant et le retrait du nombre de lumiere
    /// </summary>
    /// <param name="donneObjet"></param>
    public void Acheter(SOObjet donneObjet)
    {

        GestAudio.instance.JouerEffetSonore(donneObjet.sonAchat); //Joue le son d'achat
        lumiere -= donneObjet.prixDeBase;
        _lesObjets.Add(donneObjet);
        AfficherListeInventaire();
        if (donneObjet.typeAmelioration == TypeAmelioration.Saut)
        {
            _forceSaut = _forceSautAmeliorer;
            _ameliorationSaut = true;
        }

        else if (donneObjet.typeAmelioration == TypeAmelioration.Vitesse)
        {
            _vitesse = _vitesseAmeliorer;
            _ameliorationVitesse = true;
        }
        //#synthese Thomas
        else if (donneObjet.typeAmelioration == TypeAmelioration.MiniMap)   //Si l'objet est une minimap
        {
            _estAcheteMap = true;   //La minimap est acheté
        }
        else if (donneObjet.typeAmelioration == TypeAmelioration.Bulle) //Si l'objet est une bulle
        {
            _estAcheteBulle = true; //La bulle est acheté
        }   
        _nbAchat = _lesObjets.Count; //Ajoute un achat dans le nombre d'achat total #tp4
    }

    /// <summary>#THOMAS
    /// Affichage de la liste des objet acheter dans la boutique
    /// </summary>
    private void AfficherListeInventaire()
    {
        string inventaire = ""; //Initialisation de l'inventaire
        foreach (SOObjet objet in _lesObjets)   //Pour chaque objet dans la liste d'objet
        {
            if (inventaire != "") inventaire += ", ";   //Ajoute une virgule si il y a plus d'un objet
            inventaire += objet.nom;
        }
        Debug.Log("Inventaire : " + inventaire);
    }

    /// <summary>
    ///#synthese Thomas
    /// Lorsque le jeux quitte, la map est remis à false
    /// </summary>
    void OnApplicationQuit()
    {
        _estAcheteMap = false; //Initialsation de la map
        _estAcheteBulle = false; //Initialsation de la bulle
        _vie = _vieInitial; //Initialsation de la vie

    }
    //fin synthese Thomas
}