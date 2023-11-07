using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CarteTuiles : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)] private float _probabiliteApparition;   //declaration de la probabilite d'apparition d'une carte tuile
    public float probabiliteApparition => _probabiliteApparition;       // la variable juste en haut devient visible par d'autre script
    Tilemap _tm;    //declaration tilemap
    /// <summary>
    /// LA fonction awake s'occupe de verifier si une tuile doit est presente ou non
    /// </summary>
    void Awake()
    {
        _tm = GetComponent<Tilemap>();
        BoundsInt bounds = _tm.cellBounds;  //bouds de la tilemap
        float aleatoire = Random.Range(0f, 1f);
        bool estPresent = (aleatoire <= _probabiliteApparition);
        Niveau niveau = GetComponentInParent<Niveau>();     //chercher le script niveau
        for (int y = bounds.yMin; y < bounds.yMax; y++)
        {
            for (int x = bounds.xMin; x < bounds.xMax; x++)     // boucle qui prend les tuiles qui sont suppose etre present et les envoient se faire traiter
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                TraiterTuile(estPresent, niveau, pos);
            }
        }
        gameObject.SetActive(false);
    }
    /// <summary>
    /// La fonction traite les tuiles et les envoie a la tiilemap de niveau pour qu'il les placent dans sa tilemap
    /// </summary>
    /// <param name="estPresent"> un bool qui determine si les plateformes doient etre presente en jeu</param>
    /// <param name="niveau">Le script niveau</param>
    /// <param name="pos">  La position des tuiles </param>
    private void TraiterTuile(bool estPresent, Niveau niveau, Vector3Int pos)
    {
        TileBase tuile = _tm.GetTile(pos);
        Vector3Int ajustementPos = Vector3Int.FloorToInt(transform.position);
        if (estPresent && tuile != null)
        {
            _tm.color = new Color(1, 1, 1, 1);
            Vector3Int nouvellePos = pos + ajustementPos;
            niveau.ReceptionnerLesTuiles(nouvellePos, tuile);
        }
        else
        {
            _tm.SetTile(pos, null);
        }
    }
    /// <summary>
    /// S'occupe de changer le alpha en mode edition meme quand le jeux n'est pas actif
    /// </summary>
    void OnDrawGizmos()
    {
        if (Application.isPlaying == false)
        {
            _tm = GetComponent<Tilemap>();
            _tm.color = new Color(1, 1, 1, _probabiliteApparition);
        }
    }
}
