using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.InteropServices;

/// <summary>
///Classe qui gère l'enregistrement des meilleur scores et les données de sauvegarde
/// /// Auteurs du code: Thomas Brunet 
/// Auteur des commentaires: Thomas Brunet
/// #tp4
/// </summary>
[CreateAssetMenu(menuName = "Sauvegarde", fileName = "Sauvegarde")] //Créer un menu pour créer un ScriptableObject de type SOSauvegarde
public class SOSauvegarde : ScriptableObject
{
    [SerializeField] SOPerso _donnees;  //une référence aux données du perso
    [SerializeField] int _points;   //le nombre de point du joueur
    [SerializeField] string _nomFictif = "Votre Nom";  //le nom fictif du joueur

    [SerializeField]
    private List<JoueurScore> _lesJoueurScore; //la liste des meilleurs scores
    public List<JoueurScore> lesJoueurScore { get => _lesJoueurScore; set => _lesJoueurScore = value; } //getter/setter de la liste des meilleurs scores
    private string _champtexte; //champ texte pour le nom du joueur


    [DllImport("__Internal")]   //Permet d'appeler une fonction javascript
    static extern void SynchroniserWebGL();  //Fonction javascript qui permet de synchroniser les fichiers entre le navigateur et le disque dur
    string fichier = "Scores.tim";  //Le nom du fichier de sauvegarde


    /// <summary>
    /// fonction qui initialise le les points du joueur
    /// </summary>
    /// <param name="points">point que le joueur à fait durant la partie</param>
    public void InitialiserPointJoueur(int points)
    {
        LireFichier();  //Lire le fichier de sauvegarde
        _points = points;   //Initialiser les points du joueur
        AjouterJoueurScore(_points);
    }


    [ContextMenu("Lire fichier")]   //Créer un bouton dans l'inspecteur pour lire le fichier de sauvegarde
    /// <summary>
    /// Lecture du fichier de sauvegarde externe 
    /// </summary>
    public void LireFichier()
    {
        string fichierEtChemin = Application.persistentDataPath + "/" + fichier; //Le chemin du fichier de sauvegarde
        if (!File.Exists(fichierEtChemin))
        {
            _lesJoueurScore = new List<JoueurScore>
            {new JoueurScore { joueur = "Thomas", score = 1111 }, new JoueurScore { joueur = "Michaël", score = 1110 }, new JoueurScore { joueur = "Oli", score = 1109 }}; //Liste des meilleurs scores par défaut};
            EcrireFichier();
        }
        if (File.Exists(fichierEtChemin))   //Si le fichier de sauvegarde existe
        {
            string contenu = File.ReadAllText(fichierEtChemin); //Lire le fichier de sauvegarde
            JsonUtility.FromJsonOverwrite(contenu, this);  //Remplacer les données du ScriptableObject par les données du fichier de sauvegarde

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);   //Sauvegarder les changements dans l'éditeur
            UnityEditor.AssetDatabase.SaveAssets();   //Sauvegarder les changements dans l'éditeur
#endif
        }

    }
    /// <summary>
    /// fonction qui intialise la liste des meilleurs scores
    /// </summary>
    public void Initialiser()
    {
        lesJoueurScore = new List<JoueurScore>();
    }

    [ContextMenu("Écrire fichier")]
    /// <summary>
    /// fonction qui ecrit dans le fichier de sauvegarde externe
    /// </summary>
    public void EcrireFichier()
    {
        lesJoueurScore.Sort((a, b) => b.score.CompareTo(a.score));  //Trier la liste des meilleurs scores
        string fichierEtChemin = Application.persistentDataPath + "/" + fichier;    //Le chemin du fichier de sauvegarde
        string contenu = JsonUtility.ToJson(this, true);    //Convertir les données du ScriptableObject en JSON
        File.WriteAllText(fichierEtChemin, contenu);    //Écrire dans le fichier de sauvegarde

        if (Application.platform == RuntimePlatform.WebGLPlayer)    //si le jeu est en webGL
        {
            Debug.Log("SynchroniserWebGL");
            SynchroniserWebGL(); //Appeller la fonction pour snchroniser les fichiers entre le navigateur et le disque dur
        }
    }
    /// <summary>
    /// Ajouter le score du joueur en fin de partie à la liste de meilleur score , trier la liste et suprimer le dernier score
    /// </summary>
    /// <param name="score">le score du joueur</param>
    public void AjouterJoueurScore(int score)
    {
        JoueurScore joueurScore = new JoueurScore();    //Créer un nouveau joueurScore
        joueurScore.joueur = _nomFictif;    //Initialiser le nom du joueur
        joueurScore.score = score;  //Initialiser le score du joueur
        joueurScore.estJoueur = true;
        lesJoueurScore.Add(joueurScore);    //Ajouter le joueurScore à la liste des meilleurs scores
        lesJoueurScore.Sort((a, b) => b.score.CompareTo(a.score));  //Trier la liste des meilleurs scores
        if (lesJoueurScore.Count > 3)   //Si la liste des meilleurs scores contient plus de 3 éléments
        {
            lesJoueurScore.RemoveAt(3); //Suprimer le dernier score
        }
        foreach (JoueurScore js in lesJoueurScore)   //Pour chaque joueurScore dans la liste des meilleurs scores
        {
            if (js.estJoueur) AjouterInput(js);    //Si le nom du joueur est Bob, afficher le champ texte pour entrer le nom du joueur

        }
        PanneauFinPartie.instance.AfficherMeilleurScore(lesJoueurScore);     //Sinon, afficher la liste des meilleurs scores
    }
    /// <summary>
    /// Ajouter le inputField pour entrer le nom du joueur et appeller la fonction pour afficher la liste des meilleurs scores
    /// </summary>
    /// <param name="js"></param>
    private void AjouterInput(JoueurScore js)
    {
        PanneauFinPartie.instance.ActiverChampUsager(); //Afficher le champ texte pour entrer le nom du joueur
        PanneauFinPartie.instance.AfficherMeilleurScore(lesJoueurScore);   //Afficher la liste des meilleurs scores
        js.joueur = PanneauFinPartie.instance.champTexteUsager.text;    //Remplacer le nom du joueur par le nom entré dans le champ texte

    }
    /// <summary>
    /// fonction qui lit le champ texte pour le nom du joueur et qui l'aplique au joueur si il a fait asser de point
    /// </summary>
    /// <param name="nom">le champ ecrit dans le inputfield</param>
    public void LireChampTexte(string nom)
    {
        foreach (JoueurScore js in lesJoueurScore)   //Pour chaque joueurScore dans la liste des meilleurs scores
        {
            if (js.estJoueur)//Si le nom du joueur est Bob, afficher le champ texte pour entrer le nom du joueur 
            {
                js.joueur = nom;
                js.estJoueur = false;
            }
        }
        PanneauFinPartie.instance.AfficherMeilleurScore(lesJoueurScore);
        EcrireFichier();
    }
}


[System.Serializable]   //Permet de voir les données dans l'inspecteur
public class JoueurScore    //Classe qui contient le nom du joueur et son score
{
    public string joueur;   //Le nom du joueur
    public int score;       //Le score du joueur
    public bool estJoueur;  //bool qui determine si c'est un joueur
}

// lire, ajouter, sort, suprimer, cherche joueur, inputfield,  remplace nom, estjoeur false ecrirefichier
