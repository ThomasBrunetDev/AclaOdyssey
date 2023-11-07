using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe qui gère l'ennemi de Michael synthese
/// Auteurs du code: Michaël Malard
/// Auteur des commentaires: Michaël Malard
/// </summary>
public class EnnemieMichael : MonoBehaviour
{
    [Header("Informations")]
    [SerializeField] float _vitesse = 3f; //vitesse de l'ennemi

    [SerializeField] int _nbPointVie = 3; //nombre de point de vie de l'ennemi

    [Header("Sons")]
    [SerializeField] AudioClip _sonBlesse; //audioclip quand l'ennemi est blessé
    [SerializeField] AudioClip _sonMort; //audioclip quand l'ennemi est mort

    [Header("Joyaux")]
    [SerializeField] Joyaux[] _tJoyauxMort; //tableau de joyaux qui apparait quand l'ennemi meurt

    float _degat = 5f; //degat de l'ennemi
    public float degat => _degat;
    
    GameObject _perso; //le personnage
    Rigidbody2D _rb; //rigidbody de l'ennemi
    bool _chasse = false; //si l'ennemi chasse le perso
    Vector2 _pointApparition; //point d'apparition de l'ennemi pour qu'il puisse y retourner

    Animator _anim; //animator de l'ennemi pour les animations

    /// <summary>
    /// Initialisation des variables et du point d'apparition
    /// </summary>
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _perso = GameObject.FindGameObjectWithTag("Perso");
        _anim = GetComponent<Animator>();

        _pointApparition = transform.position;
    }

    /// <summary>
    /// Fonction qui fait bouger l'ennemi avec la physique
    /// </summary>
    void FixedUpdate()
    {
        if (_perso == null) return; //si le perso est null, on ne fait rien

        if (_chasse == true) TrouverPerso(); //si l'ennemi chasse le perso il le suit
        else RetrounerPointApparition(); //sinon il retourne à son point d'apparition

    }

    /// <summary>
    /// Fonction qui fait bouger l'ennemi vers le perso
    /// </summary>
    void TrouverPerso()
    {
        //prend la position de l'ennemi et la position du perso et le fait bouger vers le perso avec la bonne vitesse
        Vector2 newPos = Vector2.MoveTowards(transform.position, _perso.transform.position, _vitesse * Time.fixedDeltaTime);
        _rb.MovePosition(newPos);
    }


    /// <summary>
    /// Fonction qui fait bouger l'ennemi vers son point d'apparition
    /// </summary>
    void RetrounerPointApparition()
    {
        gameObject.GetComponent<CapsuleCollider2D>().enabled = false;  //fait en sorte que l'ennemi puisse traverser les murs

        //prend la position de l'ennemi et la position du point d'apparition et le fait bouger vers le point d'apparition avec la bonne vitesse
        Vector2 newPos = Vector2.MoveTowards(transform.position, _pointApparition, _vitesse * Time.fixedDeltaTime);
        _rb.MovePosition(newPos);
    }

    /// <summary>
    /// Fonction qui regarde si le perso est dans la zone de detection de l'ennemi
    /// </summary>
    /// <param name="other">Le perso</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        Perso perso = other.GetComponent<Perso>();
        gameObject.GetComponent<CapsuleCollider2D>().enabled = true; //fait en sorte que l'ennemi ne puisse plus traverser les murs et qu'il puisse attaquer le perso

        if (perso != null)
        {
            _chasse = true; //l'ennemi chasse le perso
        }
    }

    /// <summary>
    /// Fonction qui regarde si le perso sort de la zone de detection de l'ennemi
    /// </summary>
    /// <param name="other">Le perso</param>
    void OnTriggerExit2D(Collider2D other)
    {
        Perso perso = other.GetComponent<Perso>();

        if (perso != null)
        {
            _chasse = false; //l'ennemi ne chasse plus le perso
        }
    }


    /// <summary>
    /// Fonction qui fait subir des dégats à l'ennemi et qui regarde si il est mort
    /// </summary>
    public void SubirDegat()
    {
        _nbPointVie--; //fait perdre un point de vie à l'ennemi
        _anim.SetTrigger("estBlesser"); //fait jouer l'animation de blessure
        GestAudio.instance.JouerEffetSonore(_sonBlesse); //fait jouer le son de blessure

        //si l'ennemi n'a plus de point de vie, il meurt avec la coroutine
        if (_nbPointVie <= 0)
        {
            StartCoroutine(CoroutineMourir());
        }
    }

    /// <summary>
    /// Coroutine qui fait mourir l'ennemi par etapes
    /// </summary>
    /// <returns>le temps de l'annimation de mort</returns>
    IEnumerator CoroutineMourir()
    {
        gameObject.GetComponent<Collider2D>().enabled = false; //fait en sorte que l'ennemi ne puisse plus attaquer le perso
        _anim.SetBool("estMort", true); //fait jouer l'animation de mort
        GestAudio.instance.JouerEffetSonore(_sonMort); //fait jouer le son de mortx
        FaireApparaitreBonus(); //fait apparaitre des joyaux
        yield return new WaitForSeconds(1.07f); //attend le temps de l'annimation de mort
        Destroy(gameObject); //detruit l'ennemi pour qu'il ne soit plus visible, surtout dans la minimap
    }

    /// <summary>
    /// Fonction qui fait apparaitre des joyaux quand l'ennemi meurt
    /// </summary>
    void FaireApparaitreBonus()
    {
        int nbAleaApparition = Random.Range(0, 21); //nombre aleatoire d'apparition de joyaux
        int joyauxAlea = Random.Range(0, _tJoyauxMort.Length); //nombre aleatoire de joyaux

        //fait apparaitre des joyaux en fonction du nombre aleatoire
        for (int i = 0; i < nbAleaApparition; i++)
        {
            Instantiate(_tJoyauxMort[joyauxAlea], transform.position, Quaternion.identity);
        }
    }

    /// <summary>
    /// Coroutine qui fait en sorte que l'ennemi ne puisse pas attaquer le perso pendant un certain temps
    /// </summary>
    /// <returns></returns>
    IEnumerator CoroutineDesactiverCollider()
    {
        gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
        yield return new WaitForSeconds(1f);
        gameObject.GetComponent<CapsuleCollider2D>().enabled = true;
    }

    /// <summary>
    /// Fonction qui regarde si l'ennemi entre en collision avec le perso
    /// </summary>
    /// <param name="other"></param>
    void OnCollisionEnter2D(Collision2D other)
    {
        Perso perso = other.collider.GetComponent<Perso>();

        //si l'ennemi entre en collision avec le perso, son collider se desactive pendant un certain temps pour ne pas faire de degat en boucle
        if (perso != null)
        {
            StartCoroutine(CoroutineDesactiverCollider());
        }
    }




}
