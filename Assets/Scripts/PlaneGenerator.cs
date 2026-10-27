using UnityEngine;

// Genere automatiquement un plan quadrille a partir de sommets et de triangles.
public class PlaneGenerator : MonoBehaviour
{
    // Nombre de cellules du quadrillage dans la direction X.
    public int resX = 10;
    // Nombre de cellules du quadrillage dans la direction Z.
    public int resZ = 10;

    // Dimensions globales du plan dans les directions X et Z.
    public float sizeX = 10f;
    public float sizeZ = 10f;

    void Start()
    {
        // Recupere le MeshFilter de l'objet et lui affecte le maillage genere.
        GetComponent<MeshFilter>().mesh = GeneratePlane();
    }

    // Construit et retourne un maillage de plan compose de cellules rectangulaires.
    Mesh GeneratePlane()
    {
        // Cree un nouveau maillage vide qui recevra les sommets, les UV et les triangles.
        Mesh mesh = new Mesh();
        mesh.name = "Procedural Plane";

        // Une grille de resX * resZ cellules contient une rangee de sommets
        // supplementaire : resX + 1 sommets en X et resZ + 1 en Z.
        int vertCountX = resX + 1;
        int vertCountZ = resZ + 1;

        // Reserve la memoire necessaire :
        // - un sommet et une coordonnee UV par point de la grille ;
        // - 6 indices par cellule, car chaque cellule est composee de 2 triangles.
        Vector3[] vertices = new Vector3[vertCountX * vertCountZ];
        Vector2[] uvs = new Vector2[vertices.Length];
        int[] triangles = new int[resX * resZ * 6];

        // Parcourt tous les points de la grille pour calculer leur position et leurs UV.
        for (int z = 0; z < vertCountZ; z++)
        {
            for (int x = 0; x < vertCountX; x++)
            {
                // Transforme les coordonnees 2D (x, z) en indice dans les tableaux 1D.
                int i = z * vertCountX + x;

                // Normalise les coordonnees entre 0 et 1.
                // Elles servent a la fois a positionner les sommets et a definir les UV.
                float u = (float)x / resX;
                float v = (float)z / resZ;

                // Centre le plan autour de l'origine :
                // u et v vont de 0 a 1, puis deviennent des positions de -0.5 a 0.5.
                vertices[i] = new Vector3((u - 0.5f) * sizeX, 0f, (v - 0.5f) * sizeZ);

                // Associe a chaque sommet une coordonnee de texture comprise entre 0 et 1.
                uvs[i] = new Vector2(u, v);
            }
        }

        // Construit les triangles : chaque cellule du quadrillage est divisee en 2 triangles.
        int t = 0;
        for (int z = 0; z < resZ; z++)
        {
            for (int x = 0; x < resX; x++)
            {
                // Recupere les quatre sommets de la cellule courante.
                int bl = z * vertCountX + x;       // bas gauche
                int br = bl + 1;                   // bas droite
                int tl = bl + vertCountX;          // haut gauche
                int tr = tl + 1;                   // haut droite

                // Triangle 1 : bas gauche -> haut gauche -> bas droite.
                triangles[t++] = bl;
                triangles[t++] = tl;
                triangles[t++] = br;

                // Triangle 2 : bas droite -> haut gauche -> haut droite.
                triangles[t++] = br;
                triangles[t++] = tl;
                triangles[t++] = tr;
            }
        }

        // Transmet les donnees calculees au maillage Unity.
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        // Calcule les normales pour que l'eclairage fonctionne correctement.
        mesh.RecalculateNormals();

        // Calcule la boite englobante utilisee notamment pour le rendu et le culling.
        mesh.RecalculateBounds();

        // Retourne le maillage termine a la methode Start().
        return mesh;
    }
}
