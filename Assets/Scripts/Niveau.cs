using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;   //tp4
using TMPro;    //tp4
using Cinemachine; //tp4
/// <summary>
/// Classe qui instantie les salle, place les bonus , la porte, la cle, le perso, les joyaux
/// /// Auteurs du code: Thomas Brunet et Michaël Malard
/// Auteur des commentaires: Thomas Brunet et Michaël Malard
/// </summary>
public class Niveau : MonoBehaviour
{
    [SerializeField] Tilemap _tileMap;      //declaration de la tilemap
    public Tilemap tilemap => _tileMap;     //metre la tilemap en setter

    [SerializeField] Vector2Int _nbSalles = new Vector2Int(3, 3);       //le x equivaut au nombre de salle en X et le y equivaut au nombre de salle en Y
    [SerializeField] Salle[] _tSallesModeles;       //tableau des salles qui ont le script salle
    [SerializeField] TileBase _tuileModele;     //modele des tuiles de contour

    //#tp3 Thomas 
    [SerializeField] private GameObject _porteModele; // le modele de la porte du niveau
    [SerializeField] private GameObject _cleModele;  // le modele de la clé du niveau
    [SerializeField] private GameObject _persoModele;  // le modele du perso du niveau
    [SerializeField] private GameObject _activateurModele;  // le modele de l'activateur du niveau
    [SerializeField] private GameObject _effectorModele;  // le modele de l'effector du niveau
    //fin #tp3 Thomas

    //tp3 Michael
    [SerializeField, Range(10, 50)] private int _nbJoyauxParSalle = 50; //le nombre de joyaux par salle
    [SerializeField] Joyaux[] _tJoyauxModeles;  //le tableau des joyaux
    [SerializeField, Range(1, 3)] private int _nbBonusParSalle = 50;    //le nombre de bonus par salle
    [SerializeField] Bonus[] _tBonusModeles;    //le tableau de bonus
    private bool _activateur = false; //boolier pour déterminer si l'activateur est activé ou non
    public bool activateur { get => _activateur; set => _activateur = value; } //getter + setter de l'activateur
    //tp3 Michael fin


    //list #tp3 
    private List<Vector2Int> _lesPosLibres = new List<Vector2Int>(); //liste des position libre de la tilemap du niveau
    private List<Vector2Int> _lesPosSurRepere = new List<Vector2Int>();//liste des position occupé de la tilemap du niveau
    //fin list #tp3 

    // singleton #tp3
    static private Niveau _instance; //creation du singleton pour la classe niveau
    static public Niveau instance => _instance; //getter du singleton
    //fin singleton #tp3



    //#tp4 Michael IU
    [SerializeField] SOPerso _donneesPerso;     //Scriptable object pour les données du perso
    [Header("INTERFACE UTILISATEUR")]   //un header dans l'inspecteur pour l'interface utilisateur

    [Header("BONUS")]   //un header dans l'inspecteur pour les images des bonus
    [SerializeField] Image _imgBonusPiece;  //image du bonus piece
    [SerializeField] Image _imgBonusVie;   //image du bonus vie
    [Header("NIVEAU")]//un header dans l'inspecteur pour un champ de texte du niveau
    [SerializeField] TextMeshProUGUI _champNiveau; //Champ pour le texte du niveau

    [Header("VIE")] //un header dans l'inspecteur pour les champ de la vie du perso
    [SerializeField] TextMeshProUGUI _champNbVie; //Champ pour le texte de la vie
    [SerializeField] RectTransform _barreDeVie; //Barre de vie du perso

    [Header("JOYAUX")]  //un header dans l'inspecteur pour les champs de texte des joyaux
    [SerializeField] TextMeshProUGUI _champNbLumiere; //Champ pour le texte du nombre de lumiere
    [SerializeField] TextMeshProUGUI _champNbCristal; //Champ pour le texte du nombre de cristal

    [Header("CLE")] //un header dans l'inspecteur pour l'image de la clé
    [SerializeField] Image _imageCle; //Image de la clé

    [Header("TEMPS")]   //un header dans l'inspecteur pour les champs et variables du temps
    [SerializeField] TextMeshProUGUI _champTempsNiveau;   //Champ pour le texte du temps
    float _tempsRestantNiveau = 210f; // Temps total de la minuterie
    private bool _tempsEnFonction = false; // Savoir si la minuterie est fini

    [Header("AMÉLIORATION")]    //un header dans l'inspecteur pour les images des améliorations
    [SerializeField] Image _imageAmeliorationSaut; //Image de l'amélioration du saut
    [SerializeField] Image _imageAmeliorationVitesse; //Image de l'amélioration de la vitesse


    [Header("Navigation")]  //un header dans l'inspecteur pour les données de navigation
    [SerializeField] SONavigation _navigation; //Scriptable object pour les données de navigation


    //#tp4 Michael Cinemachine
    [Header("Cinémachine")] //un header dans l'inspecteur pour la cinemachine
    [SerializeField] Transform _cmConfiner; //Transform pour le confiner du cinemachine

    //Lien avec la caméra cinemachine de la scène pour modifier la grosseur par prograpmation
    [SerializeField] CinemachineVirtualCamera _cmCam;
    [SerializeField] CinemachineVirtualCamera _cmCamMiniMap;


    Coroutine _coroutineTransition; //coroutine transition de la camera


    //#synthese Michael
    [Header("Ennemie Michael")]
    [SerializeField] EnnemieMichael _ennemiMichael; //ennemie de Michael
    [SerializeField] int nbEnnemieMichaelDansNiveau = 3; //nombre d'ennemie de Michael


    //#synthese Thomas
    [SerializeField] GameObject _miniMap; //la minimap
    //fin synthese Thomas

    private TypePiste typePiste; //Le type de piste de la cle #sythese Michael

    /// <summary>
    /// La fonction awake verifie le singleton et appelle lees fonction pour creer les salle, trouver les pos libre et placer les objet
    /// </summary>
    void Awake()
    {
        Cursor.visible = false; //rendre le curseur invisible #synthese Michael
        // _miniMap.SetActive(false); //desactiver la minimap #synthese Thomas
        StartCoroutine(CoroutineChangerMusique()); //#sythese Michael


        //singleton #tp3
        if (_instance != null) { Destroy(gameObject); return; }
        _instance = this;
        // fin singleton #tp3
        CreerLesSalles(); //appel de la fonction pour creer les salles #tp3
        TrouverPosLibres();  //appel de la fonction pour trouver les positions libres #tp3
        PlacerLesObjets(); //appel de la fonction pour placer les objets #tp3

        _tempsEnFonction = true; //le temps pour complété le niveau commence #tp4 Michael

        //Afficher les données de l'interface utilisateur #tp4 Michael
        AfficherJoyauxObtenue(); //appel de la fonction pour afficher les joyaux obtenue
        AfficherVie(); //appel de la fonction pour afficher la vie
        AfficherNiveau(); //appel de la fonction pour afficher le niveau
        AfficherAmelioration(); //appel de la fonction pour afficher les améliorations
        //fin #tp4 Michael



    }


    IEnumerator CoroutineChangerMusique()
    {
        TypePiste typePisteBoutique = TypePiste.musiqueBoutique; //Le type de piste pour la musique de la boutique
        GestAudio.instance.ChangerEtatLecturePiste(typePisteBoutique, false); //Changer le type de piste
        TypePiste typePisteFin = TypePiste.musiqueFin; //Le type de piste pour la musique de la fin
        GestAudio.instance.ChangerEtatLecturePiste(typePisteFin, false); //Changer le type de piste

        yield return null;

        TypePiste typePisteBase = TypePiste.musiqueBase; //Le type de piste pour la musique de base
        GestAudio.instance.ChangerEtatLecturePiste(typePisteBase, true); //Changer le type de piste
        GestAudio.instance.ChangerVolume(0.1f); //Changer le volume de la piste

    }
    private Vector2Int RecevoirGameObject(Salle salle, GameObject gameObject)
    {
        GameObject gameObjectInstantiate = salle.PlacerObjetSurRepere(gameObject);
        int x = (int)gameObjectInstantiate.transform.position.x;
        int y = (int)gameObjectInstantiate.transform.position.y;
        Vector2Int pos = new Vector2Int(x, y);

        //#synthese Michael
        if (gameObjectInstantiate.GetComponent<Perso>() != null)
        {
            _cmCam.Follow = gameObjectInstantiate.transform;
            _cmCamMiniMap.Follow = gameObjectInstantiate.transform;
        }


        return pos;
    }





    /// <summary>
    /// Fonction qui s'occupe d'instancier les salles aléatoire dans le niveau
    /// </summary>
    private void CreerLesSalles()
    {

        //#synthese Thomas
        if (_donneesPerso.estAcheteMap)  //si le joueur a acheté la map
        {
            _miniMap.SetActive(true); //active la minimap si le joueur a acheté la map
        }
        //fin #synthese Thomas

        //#tp4 Thomas

        if (_donneesPerso.niveau > 1) //si le joueur a completé le niveau 1
        {
            if (_donneesPerso.niveau % 2 == 0) //si le niveau est pair
            {
                _donneesPerso.sallesX++; // Si le niveau est pair, augmentez la coordonnée x
            }
            else //si le niveau est impair
            {
                if (_donneesPerso.sallesY < 4)  //si le nombre de salle en y est inférieur à 4
                {
                    _donneesPerso.sallesY++; // augmentez la coordonnée y
                }
                else    //si le nombre de salle est supérieur ou égal à 4
                {
                    _donneesPerso.sallesX++; // augmentez la coordonnée x
                }
            }
        }
        _nbSalles = new Vector2Int(_donneesPerso.sallesX, _donneesPerso.sallesY);   

        //#tp4 Thomas fin

        //#tp3 Thomas
        Vector2Int placementPorte = new Vector2Int(Random.Range(0, _nbSalles.x), 0); //semi-aletoire pour placer la porte 
        Vector2Int placementCle = new Vector2Int(Random.Range(0, _nbSalles.x), _nbSalles.y - 1);  //semi-aletoire pour placer la cle 
        Vector2Int placementActivateur = new Vector2Int(Random.Range(0, _nbSalles.x), Random.Range(1, _nbSalles.y - 2));   //semi-aletoire pour placer l'activateur 
        //fin #tp3 Thomas   
        Vector2Int tailleAvecUneBordure = Salle.tailleAvecBordures - Vector2Int.one;
        for (int y = 0; y < _nbSalles.y; y++)   //generer les salle en y
        {
            for (int x = 0; x < _nbSalles.x; x++)   //generer les salle en x
            {
                Vector2Int placementSalle = new Vector2Int(x, y);

                Vector2 pos = new Vector2(tailleAvecUneBordure.x * x, tailleAvecUneBordure.y * y);
                Salle salle = Instantiate(_tSallesModeles[ChoisirSalleAleatoire()], pos, Quaternion.identity, transform);
                salle.name = $"Salle_{x}_{y}";

                Vector2Int decalageEffector = Vector2Int.CeilToInt(_tileMap.transform.position);    //#tp3
                Vector2Int posEffector = RecevoirGameObject(salle, _effectorModele) - decalageEffector;  //#tp3

                //#tp3 Thomas
                if (placementSalle == placementPorte) // placement de la porte dans la salle equivalent au random et deplacement du perso
                {
                    Vector2Int decalage = Vector2Int.CeilToInt(_tileMap.transform.position);
                    Vector2Int posRep = salle.PlacerPorteSurRepere(_porteModele) - decalage;//instantiate qui retourne la pos occupe par la porte
                    Vector2Int posRepPerso = RecevoirGameObject(salle, _persoModele) - decalage;//instantiate qui retourne la pos occupe par le perso
                    _lesPosSurRepere.Add(posRep);
                }
                if (placementCle == placementSalle)// placement de la porte dans la cle equivalent au random 
                {
                    Vector2Int decalage = Vector2Int.CeilToInt(_tileMap.transform.position);
                    Vector2Int posRep = salle.PlacerCleSurRepere(_cleModele) - decalage; //instantiate qui retourne la pos occupe par la cle
                    _lesPosSurRepere.Add(posRep);

                }
                if (placementActivateur == placementSalle)   // placement de l'activateur dans la salle equivalent au random 
                {
                    Vector2Int decalage = Vector2Int.CeilToInt(_tileMap.transform.position);
                    Vector2Int posRep = salle.PlacerActivateurSurRepere(_activateurModele) - decalage;//instantiate qui retourne la pos occupe par l'Activateur
                    _lesPosSurRepere.Add(posRep);
                }
                //fin #tp3 Thomas

                //#synthese Michael

                for (int i = 0; i < nbEnnemieMichaelDansNiveau; i++)
                {
                    Vector2Int placementEnnemieMichael = new Vector2Int(Random.Range(0, _nbSalles.x), Random.Range(0, _nbSalles.y));  //semi-aletoire pour placer les ennemies de Michael
                    if (placementEnnemieMichael == placementSalle)
                    {

                        Vector2Int decalage = Vector2Int.CeilToInt(_tileMap.transform.position);
                        Vector2Int posRep = salle.PlacerEnnemieMichaelSurRepere(_ennemiMichael) - decalage;//instantiate qui retourne la pos occupe par l'Activateur
                        _lesPosSurRepere.Add(posRep);
                    }
                }

            }
        }


        Vector2Int tailleNiveau = new Vector2Int(_nbSalles.x, _nbSalles.y) * tailleAvecUneBordure;
        Vector2Int tailleNiveauFix = tailleNiveau + Vector2Int.one;

        Vector2Int min = Vector2Int.zero - Salle.tailleAvecBordures / 2;
        Vector2Int max = min + tailleNiveau;
        for (int y = min.y; y <= max.y; y++) //boucle a la verticale  sur toute les range de tuile
        {
            for (int x = min.x; x <= max.x; x++)//boucle a la horizontale sur toute les range de tuile
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (x == min.x || x == max.x) _tileMap.SetTile(pos, _tuileModele);
                if (y == min.y || y == max.y) _tileMap.SetTile(pos, _tuileModele);
            }
        }

        //#tp4 CM confiner Michael   
        _cmConfiner.localScale = (Vector3Int)tailleNiveau + Vector3.one; //Ajuster la taille du confiner avec la taille du niveau + 1 de décalage
        _cmConfiner.position = new Vector2(min.x, min.y); //Ajuster la position du confiner avec la position du niveau
        //fin #tp4 Michael
        Vector2 centreNiveau = new Vector2(tailleNiveauFix.x / 2f, tailleNiveauFix.y / 2f);

    }

    //tp3 michael
    /// <summary>
    /// Placement des objets aleatoirement sur les espace vides
    /// </summary>
    void PlacerLesObjets()
    {
        PlacerObjets("Joyaux"); //appel de la fonction pour placer les joyaux
        PlacerObjets("Bonus"); //appel de la fonction pour placer les bonus
    }

    private void PlacerObjets(string type)
    {
        Transform conteneur = new GameObject(type).transform;   //Groupe pour les joyaux
        conteneur.parent = transform;   //Mettre le groupe dans le niveau
        int nbObjets = 0;
        if (type == "Joyaux")
        {
            nbObjets = Random.Range(10, _nbJoyauxParSalle) * (_nbSalles.x + _nbSalles.y);
        }
        else if (type == "Bonus")
        {
            nbObjets = Random.Range(2, _nbBonusParSalle) * (_nbSalles.x + _nbSalles.y);
        }

        for (int i = 0; i < nbObjets; i++)
        {
            int index;
            Joyaux joyauxModele;
            Bonus bonusModele;

            Vector2Int pos = ObtenirUnePosLibre();  //Obtenir une position pour placer le joyaux

            Vector3 pos3 = (Vector3)(Vector2)pos + _tileMap.transform.position + _tileMap.tileAnchor;   //Transformer la position en vecteur3

            if (type == "Joyaux")
            {
                index = Random.Range(0, _tJoyauxModeles.Length);  //Prendre un joyaux aleatoire
                joyauxModele = _tJoyauxModeles[index]; //choisir le joyaux
                Instantiate(joyauxModele, pos3, Quaternion.identity, conteneur);
            }
            else if (type == "Bonus")
            {
                index = Random.Range(0, _tBonusModeles.Length);  //Prendre un bonus aleatoire
                bonusModele = _tBonusModeles[index]; //choisir le bonus
                Instantiate(bonusModele, pos3, Quaternion.identity, conteneur);
            }

            if (_lesPosLibres.Count == 0) { Debug.LogWarning("Aucun espace libre"); break; }
        }
    }

    /// <summary>
    /// La fonction receptionne les tuiles envoye par carte tuile et les place dans sa tilemap
    /// </summary>
    /// <param name="pos"> position des tuiles</param>
    /// <param name="tuile">les tuiles a positionner</param>
    public void ReceptionnerLesTuiles(Vector3Int pos, TileBase tuile)
    {
        _tileMap.SetTile(pos, tuile);
    }

    /// <summary>
    /// La fonction fait un random range avec comme maximum la longueur du tableau des salle et retourne la valeur
    /// </summary>
    /// <returns></returns>
    private int ChoisirSalleAleatoire()
    {
        int aleatoire = Random.Range(0, _tSallesModeles.Length);
        return aleatoire;
    }


    //#tp3 ----------

    /// <summary>
    /// trouver les pos libre dans la tilemap
    /// </summary>
    private void TrouverPosLibres()
    {
        BoundsInt bornes = _tileMap.cellBounds;
        for (int y = bornes.yMin; y < bornes.yMax; y++) //recherche de pos libre en y 
        {
            for (int x = bornes.xMin; x < bornes.xMax; x++)  //recherche de pos libre en x
            {
                Vector2Int posTuile = new Vector2Int(x, y);
                TileBase tuile = _tileMap.GetTile((Vector3Int)posTuile);
                if (tuile == null) _lesPosLibres.Add(posTuile);
            }
        }
        foreach (Vector2Int pos in _lesPosSurRepere) //pour tous les pos ocupe, on enleve dans la liste de pos libre
        {
            _lesPosLibres.Remove(pos);
        }
    }

    /// <summary>
    /// obtention d'une pos libre avec notre liste de pos libre
    /// </summary>
    /// <returns> une pos libre</returns>
    private Vector2Int ObtenirUnePosLibre()
    {
        int indexPosLibre = Random.Range(0, _lesPosLibres.Count);
        Vector2Int pos = _lesPosLibres[indexPosLibre]; //pos libre dans la liste de pos libre
        _lesPosLibres.RemoveAt(indexPosLibre);
        return pos;
    }
    //fin #tp3 -----------

    //#tp4 Michaël --------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Afficher le temps restant pour le niveau
    /// </summary>
    private void TempsNiveau()
    {
        //Si le temps du niveau est en cours
        if (_tempsEnFonction)
        {
            //Si le temps restant est plus grand que 0
            if (_tempsRestantNiveau > 0)
            {
                _tempsRestantNiveau -= Time.deltaTime; //Diminuer le temps restant
                _champTempsNiveau.text = _tempsRestantNiveau.ToString("F0"); //Afficher le temps restant
            }
            else //Si le temps est ecoule
            {
                _tempsRestantNiveau = 0; //Le temps restant est 0
                _tempsEnFonction = false; //Le temps du niveau n'est plus en cours
                Debug.Log("Fin du temps pour le niveau"); //Afficher dans la console que le temps est écoulé
            }
        }
        else //Si le temps est fini
        {
            _navigation.TerminerLeJeu(); //Aller à la scene de fin de jeu avec le SO Navigation
        }
    }
    /// <summary>
    /// Fonction qui affiche les joyaux collecté durant le niveau
    /// </summary>
    /// <param name="typeArgent">type d'argent collecté soit une lumiere ou un joyaux</param>
    public void AfficherJoyauxObtenue(TypeArgent typeArgent = TypeArgent.Aucun)
    {
        //Si le type d'argent est une lumiere
        if (typeArgent == TypeArgent.Lumiere)
        {
            _champNbLumiere.text = _donneesPerso.lumiere.ToString("F0"); //Afficher le nombre de lumiere du donneesPerso
        }
        //Si le type d'argent est un cristal
        else if (typeArgent == TypeArgent.Cristal)
        {
            _champNbCristal.text = _donneesPerso.cristal.ToString("F0"); //Afficher le nombre de cristaux du donneesPerso
        }
        //Si le type d'argent est aucun
        else
        {
            _champNbLumiere.text = _donneesPerso.lumiere.ToString("F0"); //Afficher le nombre de lumiere du donneesPerso au début du niveau
            _champNbCristal.text = _donneesPerso.cristal.ToString("F0"); //Afficher le nombre de cristaux du donneesPerso au début du niveau
        }
    }

    /// <summary>
    /// Afficher la clé dans l'interface du joueur quand il la possède
    /// </summary>
    /// <param name="perso"></param>
    public void AfficherCleObtenue(Perso perso)
    {
        //Si le joueur ne possède pas la clé la couleur de l'image de la clé est plus pâle
        if (perso.possedeCle == false) _imageCle.color = new Color(1, 1, 1, 0.2f);
        //Si le joueur possède la clé la couleur de l'image de la clé est plus foncée
        else _imageCle.color = new Color(1, 1, 1, 1);
    }

    /// <summary>
    /// Afficher la vie du joueur dans l'interface du joueur
    /// </summary>
    public void AfficherVie()
    {
        float viesMax = _donneesPerso.vieInitial; //La vie maximum du joueur est sa vie initial
        float nbVie = _donneesPerso.vie; //La vie du joueur est sa vie actuelle
        _champNbVie.text = nbVie.ToString("F0"); //Afficher le nombre de vie du joueur
        float ratio = (nbVie * .5f / viesMax); //Calcul du ratio de la vie du joueur
        _barreDeVie.localScale = new Vector3(ratio, 0.5f, 0.5f); //Changer la taille de la barre de vie du joueur avec le calcul du ratio
    }

    /// <summary>
    /// Afficher le niveau actuel dans l'interface du joueur
    /// </summary>
    public void AfficherNiveau()
    {
        _champNiveau.text = $"Niveau {_donneesPerso.niveau}"; //Afficher le niveau actuel du joueur
    }

    /// <summary>
    /// Afficher lorsque le joueur au un bonus en rapport avec le type de bonus
    /// </summary>
    /// <param name="actif">Si le bonus est actif ou non</param>
    /// <param name="spriteActif">Le sprite du bonus actif</param>
    /// <param name="spriteInactif">Le sprite du bonus inactif</param>
    /// <param name="typeBonus">Le type de bonus pour savoir lequel activer dans l'interface</param>
    public void AfficherBonus(bool actif, Sprite spriteActif, Sprite spriteInactif, TypeBonus typeBonus)
    {
        //Si le type de bonus est un bonus d'argent
        if (typeBonus == TypeBonus.Argent)
        {
            //Si le bonus est actif
            if (actif)
            {
                _imgBonusPiece.sprite = spriteActif; //Afficher le sprite du bonus actif
            }
            else //Si le bonus est inactif
            {
                _imgBonusPiece.sprite = spriteInactif; //Afficher le sprite du bonus inactif
            }
        }
        //Si le type de bonus est un bonus de vie
        else if (typeBonus == TypeBonus.Vie)
        {
            //Si le bonus est actif
            if (actif)
            {
                _imgBonusVie.sprite = spriteActif; //Afficher le sprite du bonus actif
            }
            else //Si le bonus est inactif
            {
                _imgBonusVie.sprite = spriteInactif; //Afficher le sprite du bonus inactif
            }
        }

    }

    /// <summary>
    /// Afficher les améliorations du joueur achetée dans la boutique dans l'interface du joueur
    /// </summary>
    void AfficherAmelioration()
    {
        //Si l'amélioration de saut est achetée
        if (_donneesPerso.ameliorationSaut)
        {
            _imageAmeliorationSaut.color = new Color(1, 1, 1, 1); //Afficher l'amélioration de saut avec la bonne couleur
            Debug.Log(_donneesPerso.ameliorationSaut);
        }
        else
        {
            _imageAmeliorationSaut.color = new Color(1, 1, 1, 0.2f); //Afficher l'amélioration de saut avec la bonne couleur
        }

        //Si l'amélioration de vitesse est achetée
        if (_donneesPerso.ameliorationVitesse)
        {
            _imageAmeliorationVitesse.color = new Color(1, 1, 1, 1); //Afficher l'amélioration de vitesse avec la bonne couleur
            Debug.Log(_donneesPerso.ameliorationVitesse);
        }
        else
        {
            _imageAmeliorationVitesse.color = new Color(1, 1, 1, 0.2f); //Afficher l'amélioration de vitesse avec la bonne couleur
        }
    }

    /// <summary>
    /// Démarre une transision pour changer la taille de la caméra
    /// </summary>
    /// <param name="cibleOrthoSize">La taille orthographique souaitée</param>
    /// <param name="vitesseTransition">La vitesse de transition entre les deux tailles de caméra</param>
    public void DemarrerTransitionCM(float cibleOrthoSize, float vitesseTransition)
    {
        //Si la coroutine est en cours, l'arrêter
        if (_coroutineTransition != null)
            StopCoroutine(_coroutineTransition);

        _coroutineTransition = StartCoroutine(CoroutineChangerTailleCM(cibleOrthoSize, vitesseTransition)); //Lancer la coroutine si elle est vrai
    }

    /// <summary>
    /// Une coroutine pour changer la taille de la caméra avec une transition, pour faire plus fluide
    /// </summary>
    /// <param name="cibleOrthoSize">La taille orthographique souaitée</param>
    /// <param name="vitesseTransition">La vitesse de transition entre les deux tailles de caméra</param>
    /// <returns></returns>
    private IEnumerator CoroutineChangerTailleCM(float cibleOrthoSize, float vitesseTransition)
    {
        float tailleOrthoActuelle = _cmCam.m_Lens.OrthographicSize; //La taille orthographique actuelle de la caméra

        //Tant que la taille orthographique actuelle est différente de la taille orthographique souhaitée
        while (Mathf.Abs(tailleOrthoActuelle - cibleOrthoSize) > 0.01f)
        {
            //Changer la taille orthographique de la caméra avec une transition en utilisant un Mathf.Lerp
            tailleOrthoActuelle = Mathf.Lerp(tailleOrthoActuelle, cibleOrthoSize, Time.deltaTime * vitesseTransition);
            _cmCam.m_Lens.OrthographicSize = tailleOrthoActuelle; //Changer la taille orthographique de la caméra pour la nouvelle taille

            yield return null; //Attendre une frame
        }

        _cmCam.m_Lens.OrthographicSize = cibleOrthoSize; //Changer la taille orthographique de la caméra pour la nouvelle taille
        _coroutineTransition = null; //La coroutine est terminée
    }

    /// <summary>
    /// Afficher le temps restant dans le niveau et la vie du joueur
    /// </summary>
    void Update()
    {
        TempsNiveau(); //Afficher le temps restant dans le niveau
    }

    //fin #tp4 Michaël --------------------------------------------------------------------------------------------------------------
}
