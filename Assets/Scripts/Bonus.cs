using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Classe qui gère les bonus dans le niveau. Elle active les bonus et gère les collisions avec perso.
/// Auteurs du code: Thomas Brunet et Michaël Malard
/// Auteur des commentaires: Michaël
/// #Tp3
/// </summary>
public class Bonus : MonoBehaviour
{
    //#tp3 Michael

    static List<Bonus> _lesBonus = new List<Bonus>();   //liste vide des bonus
    public static int nbBonus => _lesBonus.Count;   //Nombre de bonus dans une salle

    [Header("Sprites du Bonus")]
    [SerializeField] Sprite _spriteActif;   //Sprite du bonus lorsqu'il est actif
    [SerializeField] Sprite _spriteInactif; //Sprite du bonus lorqu'il est inactif

    [Header("Type du bonus")]
    [SerializeField] private TypeBonus typeBonus;   //Un enum pour savoir quel est le type du bonus

    [Header("Temps du bonus")]
    [SerializeField, Range(0f, 50f)] float _tempsBonus; //Le temps que le bonus dure #syntese Michael

    private SpriteRenderer _sr; //SpriteRenderer pour changer le sprite du gameObject
    CapsuleCollider2D _collider;    //Collider du bonus pour le désactiver/activer

    //Va chercher le script "Particule" dans son enfant pour utiliser l'une de ses fonctions 
    Particules _particules;

    //tp4

    [Header("Musique du bonus")]
    [SerializeField] TypePiste typePiste;   //Le type de piste du bonus
    [SerializeField] SOPiste donneePiste;   //Les données de la piste du bonus


    //#syntese Michael
    float _orthoInitial = 7f;    //La taille de la caméra au début de la scène
    float _orthoBonus = 10f;   //La taille de la caméra à la fin de la scène


    /// <summary>
    /// Prend les composants nécessaire au démarrage. Comme le Collider et le SpriteRenderer. Il ajoute le bonus à la liste.
    /// </summary>
    void Awake()
    {
        _lesBonus.Add(this);    //Ajoute le bonus à la liste
        _sr = GetComponent<SpriteRenderer>();   //Va chercher le composant "SpriteRenderer"
        _collider = GetComponent<CapsuleCollider2D>();  //Va chercher le composant "CapsuleCollider2D"
        _particules = GetComponentInChildren<Particules>();
        //S'abonne à l'événement de l'activateur avec la fonction ActiverBonus pour pouvoir acitver les bonus
        Activateur.instance.evenementBonus.AddListener(ActiverBonus);

    }


    /// <summary>
    /// Change le sprite du bonus et son collider pour qu'ils soient actif.
    /// </summary>
    public void ActiverBonus()
    {
        _sr.sprite = _spriteActif;  //Change le sprite du bonus à celui de actif
        _collider.enabled = true;   //Active le collider du bonus pour pouvoir le récupérer
    }


    /// <summary>
    /// Une coroutine pour le temps des bonus. Elle fais une minuterie en fonction de son paramètre "tempsBonus" et détruit le gameObject du bonus par le suite.
    /// </summary>
    /// <param name="tempsBonus">Le temps que le bonus dure</param>
    /// <param name="perso">Script du perso pour mettre a jour le statut du bonus (bool)</param>
    /// <returns>Il retroune un minuterie qui est le temps du bonus</returns>
    private IEnumerator CoroutineTempsBonus(Perso perso)
    {

        Debug.Log("Timer actif pour: " + typeBonus);    //Indique que la minuterie est active pour tel bonus
        yield return new WaitForSeconds(_tempsBonus);    //Déclanche la minuterie

        //Si le type du bonus et "Argent" le booléen "bonusPiece" du perso est affecté.
        if (typeBonus == TypeBonus.Argent)
        {
            perso.bonusPiece = false;   //Met le booléen du script Perso false pour ne plus qu'il ait le bonus
            Niveau.instance.AfficherBonus(perso.bonusVie, _spriteActif, _spriteInactif, typeBonus);

            GestAudio.instance.ChangerEtatLecturePiste(typePiste, false); //Arrête la piste du bonus
            StartCoroutine(GestAudio.instance.CoroutineChangerVolume(donneePiste, 0f, 1f / 2f)); //Diminue le volume de la piste du bonus #tp4 Michael

        }
        _particules.RetablirEmission(); //Rétablit l'émission des particules

        //tp4 Michael
        Niveau.instance.DemarrerTransitionCM(_orthoInitial, 0.5f); //Rétablit la taille de la caméra #tp4 Michael

        Destroy(this.gameObject);   //Détruit le gameObject du bonus
    }

    /// <summary> #tp3 THOMAS
    /// coroutine pour le bonus de vie. elle demare un compteur et a chaque seconde le perso recoit 5 point de vie jusqua temps
    /// que le compteur soit rendu a 5 
    /// </summary>
    /// <param name="perso">Script du perso pour mettre a jour le statut du bonus (bool)</param>
    /// <returns></returns>
    private IEnumerator CoroutineSoigner(Perso perso)
    {
        int compteur = 1;    //compteur
        while (compteur < _tempsBonus)   //quand le compteur est inferieur a 5
        {
            perso.donnees.vie += 5f;
            Niveau.instance.AfficherVie();  //Affiche la vie du perso
            Debug.Log("Le joueur à " + perso.donnees.vie + " point de vie");
            yield return new WaitForSeconds(1f);    // a chaque seconde le code va s'executé

            compteur += 1;
        }
        perso.bonusVie = false;
        Niveau.instance.AfficherBonus(perso.bonusVie, _spriteActif, _spriteInactif, typeBonus);

        Debug.Log("Coroutine finis");
        //fin #tp3 THOMAS

        //#tp4 Michael
        GestAudio.instance.ChangerEtatLecturePiste(typePiste, false); //Arrête la piste du bonus
         StartCoroutine(GestAudio.instance.CoroutineChangerVolume(donneePiste, 0f, 1f / 2f)); //Diminue le volume de la piste du bonus #tp4 Michael
        Niveau.instance.DemarrerTransitionCM(_orthoInitial, 0.5f); //Rétablit la taille de la caméra #tp4 Michael

        Destroy(this.gameObject);   //Détruit le gameObject du bonus
    }


    /// <summary>
    /// Démarre les particules du bonus
    /// </summary>
    private void PartirParticules()
    {
        _particules.AugmenterEmission();    //Augmente l'émission des particules
        _particules.Demarrer();  //Appel de la fonction "Demarrer" du script des particules pour partir les particules
    }


    /// <summary>
    /// Activer lorsqu'il y a une collision de type "Trigger".
    /// </summary>
    /// <param name="other">L'objet de la collision</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        //Va chercher le script perso de l'objet de la collision pour faire une vérification
        Perso perso = other.gameObject.GetComponent<Perso>();

        //Si le perso n'est pas null les effect s'activent
        if (perso != null)
        {

            _sr.enabled = false;    //Désactivation du SpriteRenderer pour ne plus voir le bonus (Effect de récupération)
            _collider.enabled = false;  //Désactivation du Collider pour ne plus pouvoir le "récpérer" 
            PartirParticules(); //Appel de la fonction pour activer les particules après que le bonus soit récupérer
            Particules particules = perso.GetComponentInChildren<Particules>();
            //Si le bonus est lui de vie la couroutine de vie se lance
            if (typeBonus == TypeBonus.Vie)
            {
                BonusVie(perso, particules);
            }
            //Si le bonus est un bonus de vie le bolléen du perso qui indique le que bonus est activer deviens "true" et le timer se lance
            else if (typeBonus == TypeBonus.Argent)
            {
                BonusPiece(perso, particules);
            }
            Niveau.instance.DemarrerTransitionCM(_orthoBonus, 0.5f); //Augmente la taille de la caméra #tp4 Michael
        }


    }

    /// <summary>
    /// Active le bonus de vie
    /// </summary>
    /// <param name="perso"></param>
    /// <param name="particules"></param>
    public void BonusPiece(Perso perso, Particules particules)
    {
        perso.bonusPiece = true;    //Le booléen du script "Perso" deviens "true" pour que le bonus des pièce fasse effet
        StartCoroutine(CoroutineTempsBonus(perso)); //Lance le timer du bonus

        GestAudio.instance.ChangerEtatLecturePiste(typePiste, true); //Joue la piste du bonus #tp4 Michael
        GestAudio.instance.ChangerVolume(.1f); //Augmente le volume de la piste du bonus #tp4 Michael

        if (particules != null) particules.Teinter();  //si les particules sont different de null, les particules vont se teinter #tp3 Thomas
        Niveau.instance.AfficherBonus(perso.bonusPiece, _spriteActif, _spriteInactif, typeBonus); //Affiche le bonus avec tout ses paramètres #tp4 Michael
    }

    // <summary>
    /// fonction qui appelle la coroutine pour activer les effets du bonus de vie
    /// #tp3 Thomas
    /// </summary>
    /// <param name="other">L'objet de la collision</param>
    private void BonusVie(Perso perso, Particules particules)
    {
        perso.bonusVie = true;
        StartCoroutine(CoroutineSoigner(perso));


        GestAudio.instance.ChangerEtatLecturePiste(typePiste, true); // Joue la piste du bonus #tp4 Michael
        GestAudio.instance.ChangerVolume(.1f); //Augmente le volume de la piste du bonus #tp4 Michael

        if (particules != null) particules.Teinter();   //si les particules sont different de null, les particules vont se teinter #tp3 Thomas
        Niveau.instance.AfficherBonus(perso.bonusVie, _spriteActif, _spriteInactif, typeBonus); //Affiche le bonus avec tout ses paramètres #tp4 Michael
    }


    /// <summary>
    /// Quand l'appplication est quittée le sprite du bonus est changé et la liste est reset
    /// </summary>
    void OnApplicationQuit()
    {
        _sr.sprite = _spriteInactif;    //Changer de sprite pour celui du inactif
        _lesBonus.Clear();  // Réinitialisation de la liste des bonus
    }

}
