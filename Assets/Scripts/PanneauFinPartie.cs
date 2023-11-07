using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
///Classe qui gère l'affichage de fin de partie pour le pointage et les meilleurs scores
/// /// Auteurs du code: Thomas Brunet et Michaël Malard || michael à remplacé les donné bidon par les vrais données
/// Auteur des commentaires: Thomas Brunet
/// #tp4
/// </summary>
public class PanneauFinPartie : MonoBehaviour
{
    [Header("Données")] //un header dans l'inspecteur pour les données
    [SerializeField] SOPerso _donneePerso;  //une référence aux données du personnage 
    [SerializeField] SOSauvegarde _donneesSauvegarde; //une référence aux données de sauvegarde local

    [Header("Niveau Completé")]//un header dans l'inspecteur pour les champs reliée aux données
    [SerializeField] private TextMeshProUGUI _champNiveauComplete;  //champ de texte pour le nombre de niveau complété
    [SerializeField] private TextMeshProUGUI _champPointNiveauComplete; //champ de texte pour la valeur des point donnée au niveau complété fois le nombre de niveau complété
    [SerializeField] private TextMeshProUGUI _champPointNiveauCompleteTotal;    //champ de texte de point total pour les niveau complété

    [Header("Nombre d'achat")]//un header dans l'inspecteur pour les champs reliée aux données
    [SerializeField] private TextMeshProUGUI _champAchat;   //champ de texte pour le nombre de niveau complété
    [SerializeField] private TextMeshProUGUI _champPointAchat;  //champ de texte pour la valeur des point donnée au achat réalisé fois d'achat réalisé complété
    [SerializeField] private TextMeshProUGUI _champPointAchatTotal; //champ de texte de point total pour les achat complété

    [Header("Joyaux Collecté")]//un header dans l'inspecteur pour les champs reliée aux joyaux
    [SerializeField] private TextMeshProUGUI _champJoyauxCollecte;   //champ de texte pour le nombre de niveau complété
    [SerializeField] private TextMeshProUGUI _champPointJoyauxCollecte;   //champ de texte pour la valeur des point donnée au joyaux colécté fois le nombre de joyaux colecté
    [SerializeField] private TextMeshProUGUI _champPointJoyauxCollecteTotal; //champ de texte de point total pour joyaux colecté

    [Header("Total")]
    [SerializeField] private TextMeshProUGUI _champPointTotalPoints;  //champ de texte des points total
    [SerializeField] private TextMeshProUGUI _champPointTotal; //champ de texte des points total final

    [Header("Usager")]//un header dans l'inspecteur pour les usagers
    [SerializeField] private TMP_InputField _champTexteUsager;  //Input field de l'usager
    public TMP_InputField champTexteUsager => _champTexteUsager;

    [Header("MeilleurScore")]//un header dans l'inspecteur pour les champ reliée aux meilleurs scores
    [SerializeField] private TextMeshProUGUI _champMeilleurNom1;   //champ de texte pour le meilleur score
    [SerializeField] private TextMeshProUGUI _champMeilleurScore1;   //champ de texte pour le meilleur score
    [SerializeField] private TextMeshProUGUI _champMeilleurNom2;   //champ de texte pour le deuxieme meilleur score
    [SerializeField] private TextMeshProUGUI _champMeilleurScore2;   //champ de texte pour le deuxieme meilleur score
    [SerializeField] private TextMeshProUGUI _champMeilleurNom3;   //champ de texte pour le troisieme meilleur score
    [SerializeField] private TextMeshProUGUI _champMeilleurScore3;   //champ de texte pour le troisieme meilleur score


    static PanneauFinPartie _instance;     //singleton
    static public PanneauFinPartie instance => _instance;   //singleton


    
    //Synthese Michaël
    [SerializeField] float _tempsAnimation = 0.5f; //Le temps d'animation du score

    private TypePiste _typePiste; //Le type des pistes #sythese Michael


    /// <summary>
    /// singleton dans le awake (avant le start)
    /// </summary>
    void Awake()
    {
        if (_instance != null) { Destroy(gameObject); return; } //CHANGEMENT 
        _instance = this;

        StartCoroutine(CoroutineChangerMusique()); //#sythese Michael
    }

    //#sythese Michael
    IEnumerator CoroutineChangerMusique()
    {

        TypePiste typePisteBase = TypePiste.musiqueBase; //Le type de piste pour la musique de base
        TypePiste typePisteEvenA = TypePiste.musiqueEvenA; //Le type de piste pour la musique d'evenement A
        TypePiste typePisteEvenB = TypePiste.musiqueEvenB; //Le type de piste pour la musique d'evenement B

        GestAudio.instance.ChangerEtatLecturePiste(typePisteBase, false); //Changer le type de piste
        GestAudio.instance.ChangerEtatLecturePiste(typePisteEvenA, false); //Changer le type de piste
        GestAudio.instance.ChangerEtatLecturePiste(typePisteEvenB, false); //Changer le type de piste


        yield return null;

        TypePiste typePisteFin = TypePiste.musiqueFin; //Le type de piste pour la musique de la Fin
        GestAudio.instance.ChangerEtatLecturePiste(typePisteFin, true); //Changer le type de piste
        GestAudio.instance.ChangerVolume(0.1f); //Changer le volume de la piste

    }
    /// <summary>
    /// fonction qui active la calculation de point, qui initialise les champs et données et qui desactive le champ de texte usager au besoin
    /// </summary>
    void Start()
    {
        Cursor.visible = true; //affiche le curseur #synthese Michael
        _donneesSauvegarde.LireFichier(); //lit les données du fichier
        // _donneesSauvegarde.EcrireFichier(); //ecrit les données par défault dans le fichier   #synthese Thomas
        _champTexteUsager.interactable = false;
        Initialiser();
        _donneesSauvegarde.Initialiser();
        // StartCoroutine(CoroutineAfficherPointage());
        StartCoroutine(CoroutineAfficherPointage());
    }
    /// <summary>
    /// fonction qui initialise les champ de texte
    /// </summary>
    private void Initialiser()
    {

        _champNiveauComplete.text = "";
        _champPointNiveauComplete.text = "";
        _champPointNiveauCompleteTotal.text = "";

        _champJoyauxCollecte.text = "";
        _champPointJoyauxCollecte.text = "";
        _champPointJoyauxCollecteTotal.text = "";

        _champAchat.text = "";
        _champPointAchat.text = "";
        _champPointAchatTotal.text = "";

        _champPointTotal.text = "";
        _champPointTotalPoints.text = "";
    }

    /// <summary>
    /// Coroutine qui fait une animation de pointage comme une machine de casino
    /// </summary>
    /// <returns>Les temps d'attente entres les animations</returns>
    IEnumerator CoroutineAfficherPointage()
    {
        int niveau = _donneePerso.niveau - 1;   //enleve le niveau non complété
        int achat = _donneePerso.nbAchat;   //nombre d'achat fait
        int joyaux = _donneePerso.cristal;  //nombre de joyaux colecté
        Debug.Log("niveau : " + niveau + " achat : " + achat + " joyaux : " + joyaux);

        _champNiveauComplete.text = "Niveau" + ((niveau >= 2) ? "x" : "") + " complété" + ((niveau >= 2) ? "s" : "") + " :";    //affiche le texte
        int pointsNiveaux = CalculerPointage(niveau, 1000);     //calcul le pointage
        _champPointNiveauComplete.text = $"{niveau} X 1000pts"; //affiche le texte
        AnimerScore(0, pointsNiveaux, _champPointNiveauCompleteTotal); //anime le score

        _champAchat.text = "Nombre d'objets achetés :"; //affiche le texte
        int pointsAchats = CalculerPointage(achat, 500);    //calcul le pointage
        _champPointAchat.text = $"{achat} X 500pts";    //affiche le texte
        AnimerScore(0, pointsAchats, _champPointAchatTotal); //anime le score


        _champJoyauxCollecte.text = "Cristaux collectés :";  //affiche le texte
        int pointsJoyaux = CalculerPointage(joyaux, 10);    //calcul le pointage
        _champPointJoyauxCollecte.text = $"{joyaux} X 10pts";    //affiche le texte
        AnimerScore(0, pointsJoyaux, _champPointJoyauxCollecteTotal); //anime le score

        int pointTotal; //variable pour le pointage total
        pointTotal = pointsNiveaux + pointsAchats + pointsJoyaux;   //calcul le pointage total
        _champPointTotal.text = "Point" + ((pointTotal >= 2) ? "s" : "") + " tota" + ((pointTotal >= 2) ? "ux" : "l");  //affiche le texte
        yield return new WaitForSeconds(2f);    //attend 2 seconde
        AnimerScore(0, pointTotal, _champPointTotalPoints); //anime le score
        _donneePerso.score = pointTotal;    //assigne le pointage total au score du joueur
        yield return new WaitForSeconds(2f);    //attend 2 seconde
        _donneesSauvegarde.InitialiserPointJoueur(pointTotal); //initialise le pointage du joueur
    }

    /// <summary>
    /// calcule le pointage en multipliant le nombre d'objet par le multiplicateur
    /// </summary>
    /// <param name="objet">la valeur de l'objet a multiplier</param>
    /// <param name="multiplicateur">par quoi multiplier l'objet</param>
    /// <returns>retourne la pointage d'un champ</returns>
    private int CalculerPointage(int objet, int multiplicateur)
    {
        int pointTotal; //variable pour le pointage total
        pointTotal = objet * multiplicateur;  //calcul le pointage total

        return pointTotal; //retourne le pointage total
    }
    /// <summary>
    /// affichage des meilleur score
    /// </summary>
    /// <param name="lesJoueursScores">la liste des meilleur scors trié en orde</param>
    public void AfficherMeilleurScore(List<JoueurScore> lesJoueursScores)
    {
        _champMeilleurNom1.text = $"{lesJoueursScores[0].joueur}";
        _champMeilleurScore1.text = $"{lesJoueursScores[0].score}";
        _champMeilleurNom2.text = $"{lesJoueursScores[1].joueur}";
        _champMeilleurScore2.text = $"{lesJoueursScores[1].score}";
        _champMeilleurNom3.text = $"{lesJoueursScores[2].joueur}";
        _champMeilleurScore3.text = $"{lesJoueursScores[2].score}";
    }
    /// <summary>
    /// activation du champ pur les usagers
    /// </summary>
    public void ActiverChampUsager()
    {
        _champTexteUsager.interactable = true;
    }


    //#synthese Michael

    /// <summary>
    /// Part la coroutine pour animer le score
    /// </summary>
    /// <param name="scoreInitial">le score de base</param>
    /// <param name="scoreFinal">le score a atteindre</param>
    /// <param name="champ">le champ où animer</param>
    public void AnimerScore(int scoreInitial, int scoreFinal, TextMeshProUGUI champ)
    {
        StartCoroutine(CoroutineAnimerScore(scoreInitial, scoreFinal, champ)); //Part la coroutine pour animer le score
    }

    

    /// <summary>
    /// Coroutine qui anime le score
    /// </summary>
    /// <param name="scoreInitial">le score de base</param>
    /// <param name="scoreFinal">le score a atteindre</param>
    /// <param name="champ">le champ où animer</param>
    /// <returns></returns>
    IEnumerator CoroutineAnimerScore(int scoreInitial, int scoreFinal, TextMeshProUGUI champ)
    {
        champ.text = scoreInitial.ToString(); //Affiche le score initial
        float tempsEcoule = 0; //Le temps écoulé

        //Tant que le temps écoulé est plus petit que le temps d'animation
        while (tempsEcoule < _tempsAnimation)
        {
            tempsEcoule += Time.deltaTime; //Ajoute le temps écoulé
            int scoreActuel = (int)Mathf.Lerp(scoreInitial, scoreFinal, tempsEcoule / _tempsAnimation); //Calcule le score actuel
            champ.text = scoreActuel.ToString() + "pts"; //Affiche le score pour faire une animation
            yield return null; //Attend une frame
        }

        champ.text = scoreFinal.ToString() + "pts"; //Affiche le score final
    }


}
