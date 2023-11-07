using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe qui gere la mana et la collison du bouclier du joueur
/// Auteurs du code: Michaël Malard
/// Auteur des commentaires: Michaël Malard
/// #synthese Michael
/// </summary>

public class Bouclier : MonoBehaviour
{
    [SerializeField] float _coutMana = 2f; //cout en mana du bouclier
    [SerializeField] GameObject _bouclier; //le bouclier

    Coroutine _manaCoroutine; //la coroutine qui deduit la mana

    Perso _perso; //le perso

    bool _peutActiverBouclier = true; //si le perso peut activer le bouclier

    public bool peutActiverBouclier { get => _peutActiverBouclier; set => _peutActiverBouclier = value; } //getter et setter de peutActiverBouclier




    /// <summary>
    /// Initialisation des variables et desactivation du bouclier au debut du jeu
    /// </summary>
    void Awake()
    {
        _bouclier.SetActive(false);

        _perso = GetComponentInParent<Perso>();

    }

    /// <summary>
    /// Fonction qui active le bouclier et deduit la mana
    /// </summary>
    public void InitialiserBouclier()
    {
        //si le perso peut activer le bouclier
        if (_peutActiverBouclier)
        {
            _perso.peutAttacker = false; //le perso ne peut plus attaquer

            //si le perso a assez de mana
            if (_perso.donnees.manaPresente >= _coutMana)
            {
                _bouclier.SetActive(true); //on active le bouclier
                _manaCoroutine = StartCoroutine(CoroutineDeduireMana()); //on deduit la mana avec la coroutine
            }
            //sinon on s'assure que le bouclier est bien desactive
            else
            {
                Debug.Log("Pas assez de mana");
                _bouclier.SetActive(false);
            }
        }
        else
        {
            Debug.Log("Pas assez de mana");
        }

    }

    /// <summary>
    /// Coroutine qui deduit la mana
    /// </summary>
    /// <returns>Delai entre chaques deduction</returns>
    IEnumerator CoroutineDeduireMana()
    {
        //tant que la mana est superieur a 0
        while (_perso.donnees.manaPresente > 0f)
        {
            _perso.donnees.manaPresente -= _coutMana; //on deduit la mana

            //si la mana est inferieur a 0, on s'assure qu'elle est bien a 0 et on arrete 
            if (_perso.donnees.manaPresente <= 0f)
            {
                _perso.donnees.manaPresente = 0f;
                break;
            }   
            yield return new WaitForSeconds(0.1f);
        }
        DesactiverBouclier();   //on desactive le bouclier
    }

    /// <summary>
    /// Fonction qui desactive le bouclier
    /// </summary>
    public void DesactiverBouclier()
    {
        _bouclier.SetActive(false);   //on desactive le bouclier
        _peutActiverBouclier = false;  //on empeche le perso d'activer le bouclier tant qu'il n'a pas toute sa mana
        _perso.peutAttacker = true;   //on permet au perso d'attaquer

        //si la coroutine est active, on l'arrete
        if (_manaCoroutine != null)
        {
            StopCoroutine(_manaCoroutine);
        }
    }
}
