using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///Classe qui gère le projectile de l'ennemiThomas
///Auteurs du code: Thomas Brunet 
/// Auteur des commentaires: Thomas Brunet
/// #synthese thomas
/// </summary>
public class ProjectileEnnemiThomas : MonoBehaviour
{
    [SerializeField] private float _forceProjectile = 12f;  //force du projectile   
    float _degat = 5f; //degat de l'ennemi
    public float degat => _degat;   //accesseur de degat
    private GameObject _perso;  //la référence du perso
    private Rigidbody2D _rb;    //le rigidbody du projectile
    /// <summary>
    /// Fonction qui initialise le projectile et lui donne une direction
    /// </summary>
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();  //on récupère le rigidbody
        _perso = GameObject.FindWithTag("Perso");   //on récupère le perso
        Vector3 direction = _perso.transform.position - transform.position; //on calcule la direction
        _rb.velocity = new Vector2(direction.x, direction.y).normalized * _forceProjectile;     //on donne une vitesse au projectile    
    }

    /// <summary>
    /// Sent when an incoming collider makes contact with this object's
    /// collider (2D physics only).
    /// </summary>
    /// <param name="other">The Collision2D data associated with this collision.</param>
    void OnCollisionEnter2D(Collision2D other)
    {
        EnnemiThomas ennemi = other.collider.GetComponent<EnnemiThomas>();  //si l'ennemi est touché
        if (ennemi != null)     
        {
            return;
        }
        Destroy(gameObject);
    }
    void Update()
    {
        
    }
}
