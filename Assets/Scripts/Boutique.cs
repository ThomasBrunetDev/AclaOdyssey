using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Classe de la boutique qui gere l'affichage des champs qui ont rapport a l'usager #tp3
/// /// Auteurs du code: Thomas Brunet
/// Auteur des commentaires: Thomas Brunet
/// #tp3
/// </summary>
public class Boutique : MonoBehaviour
{

    [SerializeField] SOPerso _donneesPerso;     //les donnees du perso
    public SOPerso donneesPerso => _donneesPerso;   //getter des donnes du perso
    [SerializeField] TextMeshProUGUI _champNiveau;  //champ de texte du niveau UI
    [SerializeField] TextMeshProUGUI _champArgent;  //champ de l'argent UI
    private bool _estEnPlay = true; //bool pour savoir si le jeu est en play 
    static Boutique _instance;   //creation d'un singleton pour la classe boutique
    static public Boutique instance => _instance;   //getter pour le singleton de Boutique 

    private TypePiste typePiste; //Le type de pistes #sythese Michael

    /// <summary>
    /// start qui initialise les bonus de vitesse ou de saut des amelioration, verifie le singletin, met a jour les info du UI et abonnement a un evenement qui s'Active lors d'un changement
    /// </summary>
    void Awake()
    {

        if (_instance != null) { Destroy(gameObject); return; }
        _instance = this;

        Cursor.visible = true; //rendre le curseur visible //#sythese Michael
        StartCoroutine(CoroutineChangerMusique()); //#sythese Michael

        _donneesPerso.InitialiserBonus();
        _donneesPerso.InitialiserAmeliorations();

        MiseAJourInfos();
        _donneesPerso.evenementMiseAJour.AddListener(MiseAJourInfos); //abonnement evenement
    }

    //#sythese Michael
    IEnumerator CoroutineChangerMusique()
    {


        TypePiste typePisteBase = TypePiste.musiqueBase; //Le type de piste pour la musique de base
        GestAudio.instance.ChangerEtatLecturePiste(typePisteBase, false); //Changer le type de piste

        yield return null;

        TypePiste typePisteBoutique = TypePiste.musiqueBoutique; //Le type de piste pour la musique de la boutique
        GestAudio.instance.ChangerEtatLecturePiste(typePisteBoutique, true); //Changer le type de piste
        GestAudio.instance.ChangerVolume(0.1f); //Changer le volume de la piste



    }
    /// <summary>
    /// Mise a jour des infos du UI
    /// </summary>
    private void MiseAJourInfos()
    {
        Debug.Log("Infos mise a jour");
        // _champArgent.text = "Lumiere : "+_donneesPerso.lumiere;
        _champArgent.text = "Lumière" + ((_donneesPerso.lumiere >= 2) ? "s" : "") + " : " + _donneesPerso.lumiere;
        _champNiveau.text = "Boutique du niveau : " + _donneesPerso.niveau;
    }
    /// <summary>
    /// Lorsque l'application quitte
    /// </summary>
    void OnApplicationQuit()
    {
        _donneesPerso.Initialiser();
        _estEnPlay = false;
    }
    /// <summary>
    /// lorsque la classe est detruite
    /// </summary>
    void OnDestroy()
    {
        _donneesPerso.evenementMiseAJour.RemoveAllListeners();      //desabonement quand changer scene
        if (_estEnPlay) _donneesPerso.niveau++;
    }
}
