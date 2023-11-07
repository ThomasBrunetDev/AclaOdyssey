using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Classe qui controle les déplacements du personnage
/// Auteurs du code: Thomas Brunet et Michaël Malard
/// Auteur des commentaires: Thomas Brunet
/// /// </summary>
public class Perso : BasePerso
{
    [Header("Perso---------------")]
    [SerializeField] private bool _veutSauter = false; //Le bool qui detecte si le joueur veut sauter

    bool _sautEnCours = false;  //Le bool qui detecte si le joueur saute

    [SerializeField] private int _nbFramesMax = 10; //Le int du nombre de frame maximum pour le saut
    [SerializeField] SOPerso _donnees;  //Utilisation du SO pour les valeur comme vitesse, etc #tp3
    public SOPerso donnees { get => _donnees; set => _donnees = value; }
    [SerializeField] SONavigation _navigation;  //utilisation du scriptable object de navigation pour le changement de scene #tp3 Thomas

    private int _nbFramesRestants = 0;  //Le int du nombre de frame restant pour le saut
    private Rigidbody2D _rb;    //Déclaration du rigidBody
    private SpriteRenderer _sr;     //Déclaration du spriteRenderer
    private float _axeHorizontal;   //float qui permet au mouvement lorsqu'une touche est activé

    private bool _possedeCle;    //un boolean pour nous indiquer si le personnage a ramsser la cle #tp3 Thomas
    public bool possedeCle { get => _possedeCle; set => _possedeCle = value; }  //getter + setter pour savoir si le preso posede la clé #tp3 Thomas


    //#tp4 michael

    private bool _bonusVie; //si le perso a recuperer un bonus pour récupérer de la vie
    public bool bonusVie { get => _bonusVie; set => _bonusVie = value; } //getter + setter pour le bool de bonus vie 
    private bool _bonusPiece; //si le perso a recuperer un bonus pour le double pièce
    public bool bonusPiece { get => _bonusPiece; set => _bonusPiece = value; }  //getter + setter pour le bool de bonus piece #tp3 Michael

    Animator _animator; //Declaration de l'animator #tp4 Michaël




    //#synthese Michael

    [Header("Bouclier")]
    [SerializeField] Bouclier _bouclier; //Declaration du script bouclier
    [SerializeField] GameObject _gOBarreMana;  //gameObject de la barre de mana
    [SerializeField] GameObject _gOMana;   //gameObject de la mana

    [Header("Attaque")]
    [SerializeField] Transform[] _tPointsAttaques;  //point d'attaque du perso
    [SerializeField] LayerMask _ennemiLayers;  //layer des ennemis
    float _rayonAttaque = 1f;  //rayon d'attaque du perso
    float _vitesseAttaque = 0.9f;  //rayon d'attaque du perso
    float _prochaineAttaque = 0f;  //temps avant la prochaine attaque
    bool _peutAttacker = true;  //bool pour savoir si le perso peut attaquer
    public bool peutAttacker { get => _peutAttacker; set => _peutAttacker = value; } //getter + setter pour avoir accès dans le bouclier #synthese Michael

    bool _estMort = false;  //bool pour savoir si le perso est mort

    Coroutine _manaCoroutine;
    //#synthese Michael

    /// <summary>
    /// recuperation du sprite renderer, du rigid body et de l'animator
    /// </summary>
    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>(); //recuperation de l'animator #tp4 Michaël
        _possedeCle = false; //#tp3

        bonusVie = false;
        bonusPiece = false;

        donnees.manaPresente = donnees.manaMax; //initialisation de la mana #synthese Michael

        _estMort = false;  //initialisation du bool de mort #synthese Michael
    }


    /// <summary>
    /// Fonction qui permet de verifier si le perso a le bouclier et si oui, l'initialise
    /// </summary>
    void VerificationBouclier()
    {
        //Si les bouclier est initialisé
        if (_bouclier != null)
        {
            _bouclier.InitialiserBouclier();   //initialise le bouclier 
            //Si le gameObject du bouclier est activé
            if (_bouclier.gameObject.activeSelf)
            {
                _rb.mass = 100f; //augmente la masse du perso pour qu'il ne puisse pas bouger   
                _animator.SetBool("estProteger", true); //lance l'animation de protection
            }
        }

    }


    /// <summary>
    /// Permet de recharger la mana du perso juste une fois pour pas que la coroutine se lance en boucle
    /// </summary>
    void CommencerRechargerMana()
    {
        //Si la coroutine n'est pas lancé
        if (_manaCoroutine == null)
        {
            _manaCoroutine = StartCoroutine(CoroutineRechargerMana()); //lance la coroutine de rechargement de mana
            _animator.SetBool("estProteger", false); //enlève l'animation de protection
        }
    }

    /// <summary>
    /// Coroutine qui permet de recharger la mana du perso
    /// </summary>
    /// <returns>Le temps d'attente avant que le mana commence à se regénérer et le temps entre chaques regénération</returns>
    IEnumerator CoroutineRechargerMana()
    {
        yield return new WaitForSeconds(2f); //attend 2 secondes avant de commencer à recharger la mana

        //Tant que la mana du perso est plus petite que la mana max
        while (donnees.manaPresente < donnees.manaMax)
        {
            donnees.manaPresente += 1f; //ajoute 1 à la mana du perso

            //Si la mana du perso est plus grande ou égale à la mana max
            if (donnees.manaPresente >= donnees.manaMax)
            {
                donnees.manaPresente = _donnees.manaMax; //la mana du perso est égale à la mana max
                break; //sort de la boucle
            }
            yield return new WaitForSeconds(.1f); //attend 0.1 seconde avant de recommencer la boucle

        }
        //Si la mana du perso est égale à la mana max
        if (donnees.manaPresente == donnees.manaMax)
        {
            Debug.Log("Mana rechargé");
            _bouclier.peutActiverBouclier = true; //le perso peut activer le bouclier
        }
        _manaCoroutine = null; //la coroutine est null pour pouvoir la relancer

    }


    /// <summary>
    /// Fonction qui permet de vérifier si le perso est en mouvement et si oui, désactive le bouclier
    /// </summary>
    void VerifierMovement()
    {
        float tolerance = 5f; //tolerance de mouvement du perso

        //Si le le gameObject du bouclier est activé
        if (_bouclier.gameObject.activeSelf)
        {
            //Si la vitesse du perso est plus grande ou égale à la tolerance
            if (_rb.velocity.magnitude >= tolerance)
            {
                Debug.Log("Bouclier désactivé"); //affiche dans la console que le bouclier est désactivé
                _bouclier.DesactiverBouclier(); //désactive le bouclier
                _rb.mass = 1f; //remet la masse du perso a 1
                _animator.SetBool("estProteger", false); //enlève l'animation de protection
                _manaCoroutine = StartCoroutine(CoroutineRechargerMana()); //lance la coroutine de rechargement de mana et la stock dans une variable
            }

        }
    }


    /// <summary>
    /// Fonction qui permet d'afficher la mana du perso
    /// </summary>
    void AfficherMana()
    {
        //Si la mana du perso est plus petite que la mana max
        if (_donnees.manaPresente < _donnees.manaMax)
        {
            _gOBarreMana.SetActive(true); //active la barre de mana
        }
        //Si la mana du perso est plus grande ou égale à la mana max
        else
        {
            _gOBarreMana.SetActive(false); //desactive la barre de mana
        }

        float ratio = donnees.manaPresente / donnees.manaMax; //calcule le ratio de la mana du perso

        Vector2 nouvScale = _gOMana.transform.localScale; //recupere la scale de la mana
        nouvScale.y = ratio; //change la scale de la mana
        _gOMana.transform.localScale = nouvScale; //change la scale de la mana dans le jeu

    }

    /// <summary>
    /// Fonction qui permet de gérer les collisions avec les ennemis et subir des dégats #synthese Michael Thomas
    /// </summary>
    /// <param name="col">L'ennemi en collision avec le perso</param>
    void OnCollisionEnter2D(Collision2D col)
    {
        EnnemieMichael ennemieMick = col.gameObject.GetComponent<EnnemieMichael>();   //recuperation du script ennemieMichael
        if (ennemieMick != null && !_bouclier.gameObject.activeSelf)    //si le perso n'a pas le bouclier et si l'ennmie est pas null
        {
            SubirDegats(ennemieMick.degat); //appel de la fonction subir degat avec le nombre de degat de l'ennemi Michael
        }

        EnnemiThomas ennemi = col.collider.GetComponent<EnnemiThomas>();  //recuperation du script ennemiThomas
        if (ennemi != null && !_bouclier.gameObject.activeSelf)   //si le perso n'a pas le bouclier et si l'ennmie est pas null
        {
            SubirDegats(ennemi.degat);  //appel de la fonction subir degat avec le nombre de degat de l'ennemi Thomas
        }

        ProjectileEnnemiThomas projectile = col.collider.GetComponent<ProjectileEnnemiThomas>();  //recuperation du script projectileEnnemiThomas
        if (projectile != null && !_bouclier.gameObject.activeSelf) //si le perso n'a pas le bouclier et si le projectile est pas null
        {
            SubirDegats(projectile.degat);  //appel de la fonction subir degat avec le nombre de degat du projectile          
        }
    }

    /// <summary>
    /// fonction qui permet de faire subir des degats au perso
    /// debut #synthese thomas
    /// </summary>
    /// <param name="degat">Le nombre de dégat que le perso recoit</param>
    public void SubirDegats(float degat)
    {
        _animator.SetTrigger("estBlesser"); //lance l'animation de blessure
        GestAudio.instance.JouerEffetSonore(_donnees.sonBlesse); //#syhtese Michael
        _donnees.vie -= degat;  //enleve les degats de la vie du perso
        Niveau.instance.AfficherVie();  //appel la fonction afficher vie du script niveau
        if (_donnees.vie <= 0)  //si la vie du perso est inferieur ou egale a 0
        {
            _donnees.vie = 0;   //la vie du perso est egale a 0
            _estMort = true;    //le perso est mort #synthese Michael
            StartCoroutine(CoroutineTerminerJeux());    //appel de la coroutine qui termine le jeu
        }

    }

    /// <summary>
    /// Coroutine qui laisse la temps au perso de faire son animation de mort avant de terminer le jeu
    /// </summary>
    /// <returns>fait une pause de 3 seconde</returns>
    private IEnumerator CoroutineTerminerJeux()
    {
        _animator.SetBool("estMort", true); //lance l'animation de mort  #synthese Michael
        _rb.simulated = false;  //desactive la physique du perso #synthese Michael
        yield return new WaitForSeconds(3f);    //fait une pause de 3 seconde
        _navigation.TerminerLeJeu();    //appel de la fonction qui termine le jeu
        yield return new WaitForSeconds(1f);    //fait une pause de 1 seconde
        Destroy(gameObject);    //detruit le perso
    }
    //fin synthese Thomas


    /// <summary>
    /// Fonction qui gère le flip horizontal du perso et la valeur du axeHorizontal qu sert au mouvement du perso
    /// </summary>
    void Update()
    {
        if (!_estMort)
        {
            _axeHorizontal = Input.GetAxis("Horizontal"); //changement de la valeur pour le deplacement horizontale

            //Troune le sprite du perso tout dépendant le valeur du X
            if (_axeHorizontal < 0) _sr.flipX = true;
            else if (_axeHorizontal > 0) _sr.flipX = false;

            _veutSauter = Input.GetButton("Jump");  //changement de valeur du bool quand la barre d'espace est activé
            GererSonAtterissage();  //appel de la fonction qui gère le son d'atterissage #tp3 Michaël

            //Gestion du bouclier #synthese Michael
            VerifierMovement(); //appel de la fonction qui vérifie si le perso est en mouvement
            AfficherMana();   //appel de la fonction qui affiche la mana du perso

            //Si le perso a le bouclier d'acheté, il peut l'activer
            if (donnees.estAcheteBulle)
            {
                //si le joueur appuie sur la touche S
                if (Input.GetKeyDown(KeyCode.S))
                {
                    VerificationBouclier(); //appel de la fonction qui vérifie si le perso a le bouclier et si oui, l'initialise
                }

                //Si le perso a le bouclier d'activé, mais qu'il n'a plus de mana
                if (donnees.manaPresente == 0)
                {
                    Debug.Log("Bouclier désactivé");
                    _rb.mass = 1f; //remet la masse du perso a 1
                    //appel de la fonction qui permet de recharger la mana du perso juste une fois pour pas que la coroutine se lance en boucle
                    CommencerRechargerMana();
                }
            }

            //Attaque du personnage #synthese Michael
            ////appel de la fonction qui permet de recharger la mana du perso juste une fois pour pas que la coroutine se lance en boucle
            if (Time.time >= _prochaineAttaque)
            {
                //si le joueur appuie sur la touche pour attaquer
                if (Input.GetButtonDown("Fire1"))
                {
                    Attaquer(); //appel de la fonction qui permet au perso d'attaquer
                    _prochaineAttaque = Time.time + 1f / _vitesseAttaque; //temps avant la prochaine attaque
                }
            }
        }



    }

    /// <summary>
    /// Gère l'attaque du perso #synthese Michael
    /// </summary>
    void Attaquer()
    {
        if (!_peutAttacker) return; //si le perso ne peut pas attaquer, on sort de la fonction
        //sinon on peut attaquer
        else
        {
            _animator.SetTrigger("attaque"); //lance l'animation d'attaque
            GestAudio.instance.JouerEffetSonore(_donnees.sonAttaque); //jouer le son d'attaque

            //si le perso est tourné vers la gauche
            if (_sr.flipX)
            {
                DetruirEnnemis(0); //appel de la fonction qui permet de faire des degat au ennemis vers la gauche
            }
            else
            {
                DetruirEnnemis(1); //appel de la fonction qui permet de faire des degat au ennemis vers la droite
            }
        }
    }

    /// <summary>
    /// Fonction qui permet de faire des degat au ennemis #synthese Michael
    /// </summary>
    /// <param name="pointAttaque"></param>
    void DetruirEnnemis(int pointAttaque)
    {
        //collecte les informations des ennemis dans le rayon d'attaque
        Collider2D[] ennemisTouches = Physics2D.OverlapCircleAll(_tPointsAttaques[pointAttaque].position, _rayonAttaque, _ennemiLayers);  //detecte les ennemis dans le rayon d'attaque

        //pour chaque ennemis dans le rayon d'attaque
        foreach (Collider2D ennemi in ennemisTouches)
        {
            //si l'ennemi est un ennemi thomas
            if (ennemi.GetComponent<EnnemiThomas>() != null)
            {
                //le joueur attaque dans le collider de détection de l'ennemi (un trigger)
                if (ennemi.isTrigger)
                {
                    continue; //on continue dans la fonction
                }
                else
                {
                    ennemi.GetComponent<EnnemiThomas>().SubirDegat(); //appel de la fonction qui permet de faire des degat au ennemis
                }
            }
            //si l'ennemi est un ennemi michael
            else if (ennemi.GetComponent<EnnemieMichael>() != null)
            {
                //le joueur attaque dans le collider de détection de l'ennemi (un trigger)
                if (ennemi.isTrigger)
                {
                    continue; //on continue dans la fonction
                }
                else
                {
                    ennemi.GetComponent<EnnemieMichael>().SubirDegat(); //appel de la fonction qui permet de faire des degat au ennemis
                }
            }
        }
    }

    /// <summary>
    /// Fonction pour faire des gizmo dans la scene pour voir le rayon d'attaque du perso #synthese Michael
    /// </summary>
    void OnDrawGizmosSelected()
    {
        //si le tableau des points d'attaque est null, on sort de la fonction
        foreach (Transform t in _tPointsAttaques)
        {
            if (t == null)
            {
                return;
            }

            Gizmos.DrawWireSphere(t.position, _rayonAttaque); //dessine un cercle pour voir le rayon d'attaque du perso
        }

    }



    /// <summary>
    /// gère la physique du deplacemen du perso et le saut
    /// </summary>
    override protected void FixedUpdate()
    {
        _rb.velocity = new Vector2(_axeHorizontal * _donnees.vitesse, _rb.velocity.y);  //Ajout de l'utilisation du SO

        base.FixedUpdate();  //appel de la fonction du fixedUpdate dans le script mère
        Particules part = GetComponentInChildren<Particules>();

        if (_estAuSol)  //Si le perso est au sol, detection du sol
        {
            _animator.SetBool("estAuSol", true); //change la valeur de l'animator
            if (part != null) part.Demarrer();

            if (_veutSauter)    //la condition pour pouvoir faire sauter le personnage
            {
                //si le saut n'est pas en cours
                if (!_sautEnCours)
                {
                    GestAudio.instance.JouerEffetSonore(_donnees.sonSaut); //#syhtese Michael
                    StartCoroutine(CoroutineReinitialiserSauter()); //appel de la coroutine qui permet de réinitialiser le saut
                }
                Sauter();
            }
            else _nbFramesRestants = _nbFramesMax;

        }
        // Si le joueur est dans les air et lui reste des "frames" de saut
        else
        {
            _animator.SetBool("estAuSol", false); //change la valeur de l'animator
            if (part != null) part.Arreter();
            bool peutSauterPlus = (_nbFramesRestants > 0);//bool que la valeur de frame restant

            if (_veutSauter && peutSauterPlus)//si la barre d'espace est enfoncé et il reste des frames de saut
            {
                Sauter();
            }
            else
            {
                _nbFramesRestants = 0;
            }

        }

        Animation(); //appel de la fonction qui gère les animations #tp4 Michaël
    }
    /// <summary>
    /// Fonction qui va permettre au perso de sauter
    /// </summary>
    private void Sauter()
    {

        _sautEnCours = true;  //le perso saute
        float fractionDeForce = (float)_nbFramesRestants / _nbFramesMax;
        float puissance = _donnees.forceSaut * fractionDeForce; //Ajout de l'utilisation du SO
        _rb.AddForce(Vector2.up * puissance);   //Ajoute de la force au rigidbody
        _nbFramesRestants--;


        if (_nbFramesRestants < 0)  //si le nombre de frame quand on tien la barre d'espace est plus petit que 0
        {
            _nbFramesRestants = 0;
        }

    }


    /// <summary>
    /// Fonction qui permet de réinitialiser le saut après le temps du son de saut
    /// </summary>
    /// <returns></returns>
    IEnumerator CoroutineReinitialiserSauter()
    {
        yield return new WaitForSeconds(_donnees.sonSaut.length);
        _sautEnCours = false;
    }

    /// <summary>
    /// Fonction qui gère les animation du personnage #tp4 Michaël
    /// </summary>
    void Animation()
    {
        _animator.SetFloat("vitesseX", _rb.velocity.x); //recupere la valeur de l'axe horizontal
        _animator.SetFloat("vitesseY", _rb.velocity.y); //recupere la valeur de l'axe vertical
    }

    /// <summary>
    /// Lorsque l'application quitte
    /// </summary>
    void OnApplicationQuit()
    {
        _donnees.Initialiser();
        _donnees.InitialiserBonus();
        _donnees.estAcheteBulle = false;
        _donnees.estAcheteMap = false;

    }
}