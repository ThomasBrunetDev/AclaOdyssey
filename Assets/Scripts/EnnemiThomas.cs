using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///Classe qui gère le comportement, la vie, les animation, les sons de l'ennemiThomas
///Auteurs du code: Thomas Brunet 
/// Auteur des commentaires: Thomas Brunet
/// #synthese thomas
/// </summary>
public class EnnemiThomas : MonoBehaviour
{   
    [Header("Projectile")]  //header du projectile de l'ennemi
    [SerializeField] GameObject _prefabProjectile;//prefab du projectile
    [SerializeField] Transform _projectileDepartDroit; //position de départ du projectile
    [SerializeField] Transform _projectileDepartGauche; //position de départ du projectile

    [Header("Destination")] //header de la destination de l'ennemi
    [SerializeField] Transform[] _tDest;//tableau destinations de l'ennemi 

    [Header("Vitesse")] //header de la vitesse de l'ennemi
    [SerializeField] float _delaiPremierDepart = 0.5f;  //delai du premier départ
    [SerializeField] float _delaiDepartsSuivants = 1f;  //delai des départs suivants
    [SerializeField] float _vitesseMax = 2f;    //vitesse max de l'ennemi
    [SerializeField] float _toleranceDest = 0.5f;   //tolerance de la destination

    [Header("Son")] //header des sons de l'ennemi
    [SerializeField] AudioClip _sonBlesse; //audioclip quand l'ennemi est blessé
    [SerializeField] AudioClip _sonMort; //audioclip quand l'ennemi est mort

    [Header("Joyaux")]  //header des joyaux de l'ennemi
    [SerializeField] Joyaux[] _tJoyauxMort;  //tableau des joyaux que l'ennemi peut faire apparaitre

    private Coroutine _coroutineGererTrajet;    //coroutine qui gère le trajet de l'ennemi
    private Animator _anim; //animator de l'ennemi
    private int _nbPointVie = 3; //nombre de point de vie de l'ennemi
    private bool _estEnDanger = false; //si l'ennemi est en danger
    private GameObject _perso; //la référence du perso
    float _degat = 10f; //degat que fait l'ennemi
    public float degat => _degat;   //accesseur du degat de l'ennemi
    int _iDest = 0; //index de la destination de l'ennemi
    Rigidbody2D _rb;    //rigidbody de l'ennemi
    private SpriteRenderer _sr; //sprite renderer de l'ennemi

    /// <summary>
    /// Fonction start qui initialise les variables et qui lance la coroutine de gestion du trajet
    /// </summary>
    void Start()
    {
        _iDest = 0;
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _rb.MovePosition(_tDest[_iDest].position);
        _coroutineGererTrajet = StartCoroutine(CoroutineGererTrajet());
        _anim = GetComponent<Animator>();
    }

    /// <summary>
    ///  coroutine de gestion du trajet de l'ennemi
    /// </summary>
    /// <returns>temps d'attente</returns>
    IEnumerator CoroutineGererTrajet()
    {
        yield return new WaitForSeconds(_delaiPremierDepart);   //attend le delai du premier départ
        while (true)    //boucle infinie
        {
            Vector2 posDest = ObtenirPosProchaineDestination(); //obtient la prochaine destination
            while (Vector2.Distance(transform.position, posDest) > _toleranceDest)  //tant que l'ennemi n'est pas rendu à sa destination
            {
                if (_iDest == 0) _sr.flipX = true;  //si l'ennemi est à sa première destination, il regarde vers la gauche  
                else _sr.flipX = false; //sinon il regarde vers la droite

                yield return new WaitForFixedUpdate();  //attend la prochaine frame du fixed update
                BougerAvecMoveToward(posDest);  //bouge l'ennemi vers sa destination
            }
            yield return new WaitForSeconds(_delaiDepartsSuivants); //attend le delai des départs suivants
        }
    }

    /// <summary>
    /// Fonction qui bouge l'ennemi vers sa destination
    /// </summary>
    /// <param name="posDest">la destination de l'ennemi</param>
    private void BougerAvecMoveToward(Vector2 posDest)  
    {
        float distMax = _vitesseMax * Time.fixedDeltaTime;  //distance max que l'ennemi peut parcourir
        Vector2 nouvPos = Vector2.MoveTowards(transform.position, posDest, distMax);    //nouvelle position de l'ennemi
        _rb.MovePosition(nouvPos);  //bouge l'ennemi à sa nouvelle position
    }

    /// <summary>
    /// Fonction qui obtient la prochaine destination de l'ennemi
    /// </summary>
    /// <returns>la prochaine position de l'ennemi</returns>
    Vector2 ObtenirPosProchaineDestination()
    {
        _iDest++;   //augmente l'index de la destination
        if (_iDest >= _tDest.Length) _iDest = 0;    //si l'index est plus grand que la longueur du tableau, il retourne à 0
        Vector2 pos = _tDest[_iDest].position;  //position de la prochaine destination
        return pos; //retourne la position de la prochaine destination
    }

    /// <summary>
    /// Fonction qui fait subir des dégats à l'ennemi et qui regarde si il est mort
    /// </summary>
    public void SubirDegat()
    {
        _nbPointVie--;  //diminue le nombre de point de vie
        _anim.SetTrigger("estBlesser"); //lance l'annimation de blessure
        GestAudio.instance.JouerEffetSonore(_sonBlesse);    //joue le son de blessure

        if (_nbPointVie <= 0)   //si l'ennemi n'a plus de point de vie, il meurt
        {   
            StartCoroutine(CoroutineMourir());  //lance la coroutine de mort
        }
    }

    /// <summary>
    /// Coroutine qui fait mourir l'ennemi par etapes
    /// </summary>
    /// <returns>le temps que l'annimation de mort se fasse</returns>
    IEnumerator CoroutineMourir()   
    {
        _anim.SetBool("estMort", true); //lance l'annimation de mort
        GestAudio.instance.JouerEffetSonore(_sonMort);  //joue le son de mort
        FaireApparaitreBonus(); //fait apparaitre des bonus à la mort de l'ennemi
        yield return new WaitForSeconds(1.5f);      //attend 1.5 secondes
        Destroy(gameObject);    //detruit l'ennemi
    }

    /// <summary>
    /// Fonction qui fait apparaitre des bonus à la mort de l'ennemi
    /// </summary>
    void FaireApparaitreBonus()
    {
        int nbAleaApparition = Random.Range(0, 21); //nombre aléatoire d'apparition de bonus
        int joyauxAlea = Random.Range(0, _tJoyauxMort.Length);  //nombre aléatoire de joyaux
        for (int i = 0; i < nbAleaApparition; i++)  //boucle qui fait apparaitre des bonus
        {
            Instantiate(_tJoyauxMort[joyauxAlea], transform.position, Quaternion.identity); //fait apparaitre un joyau
        }
    }

/// <summary>
/// Fonction qui regarde si le perso est dans la zone de détéction de l'ennemi
/// </summary>
/// <param name="other"> le colider en contact avec la zone de detection de l'ennemi</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        Perso perso = other.GetComponent<Perso>();  //le perso en contact avec la zone de detection
        if (perso != null)  //si le perso est en contact avec la zone de detection
        {
            if (_coroutineGererTrajet != null)  //si la coroutine de gestion du trajet est en cours
            {
                StopCoroutine(_coroutineGererTrajet);   //arrete la coroutine de gestion du trajet
                _coroutineGererTrajet = null;   //la coroutine de gestion du trajet est null
                _anim.SetBool("estAttaque", true);  //lance l'annimation d'attaque
            }
            _estEnDanger = true;    //l'ennemi est en danger
        }
    }
    /// <summary>
    /// Fonction qui regarde si le perso à quitte la zone de detection de l'ennemi
    /// </summary>
    /// <param name="other"> le colidder en contact avec la zone de détéction</param>
    void OnTriggerExit2D(Collider2D other)  
    {
        Perso perso = other.GetComponent<Perso>();  //le perso en contact avec la zone de detection
        if (perso != null)  //si le perso est en contact avec la zone de detection
        {
            _anim.SetBool("estAttaque", false); //arrete l'annimation d'attaque
            if (_coroutineGererTrajet == null)  //si la coroutine de gestion du trajet est null
            {
                _coroutineGererTrajet = StartCoroutine(CoroutineGererTrajet()); //lance la coroutine de gestion du trajet
            }
            _estEnDanger = false;   //l'ennemi n'est plus en danger      
        }
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (_estEnDanger)   //si l'ennemi est en danger
        {
            _perso = GameObject.FindWithTag("Perso");   //trouve le perso
            if (_perso.transform.position.x < transform.position.x) //si le perso est à gauche de l'ennemi
            {
                _sr.flipX = true;   //l'ennemi regarde à gauche
            }
            else    //si le perso est à droite de l'ennemi
            {
                _sr.flipX = false;  //l'ennemi regarde à droite
            }
        }
    }
    /// <summary>
    /// Fonction qui lance le son de l'attaque de l'ennemi fonction appelée dans l'annimation
    /// </summary>
    public void LancerRoche()
    {
        if (_sr.flipX)  //si l'ennemi regarde à gauche
        {
            Instantiate(_prefabProjectile, _projectileDepartGauche.transform.position, Quaternion.identity);    //fait apparaitre un projectile à gauche
        }
        else Instantiate(_prefabProjectile, _projectileDepartDroit.transform.position, Quaternion.identity);    //fait apparaitre un projectile à droite
    }
}   
