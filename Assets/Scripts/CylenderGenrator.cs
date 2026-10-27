using UnityEngine;

public class CylenderGenrator : MonoBehaviour
{
    public float rayon = 2.5f;
    public float hauteur = 10f;
    public int nbFaces = 10;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Recupere le MeshFilter de l'objet et lui affecte le maillage genere.
        GetComponent<MeshFilter>().mesh = GenerateCylender();
    }
    Mesh GenerateCylender()
    {
        Mesh mesh = new Mesh();
        mesh.name = "Procedural Cylender";

        // Organisation du tableau des sommets
        int nbCorps = 2 * (nbFaces + 1);
        int centreBas = nbCorps;                  // puis nbFaces sommets de bord
        int centreHaut = centreBas + nbFaces + 1;  // puis nbFaces sommets de bord
        int total = centreHaut + nbFaces + 1;

        Vector3[] vertices = new Vector3[total];
        Vector2[] uvs = new Vector2[total];
        int[] triangles = new int[12 * nbFaces];

        float pas = 2f * Mathf.PI / nbFaces;
        float demi = hauteur / 2f;

        // --- Sommets du corps (la colonne nbFaces duplique la colonne 0) ---
        for (int i = 0; i <= nbFaces; i++)
        {
            float theta = i * pas;
            float x = rayon * Mathf.Cos(theta);
            float z = rayon * Mathf.Sin(theta);

            int bas = 2 * i;
            int haut = bas + 1;

            vertices[bas] = new Vector3(x, -demi, z);
            vertices[haut] = new Vector3(x, demi, z);

            uvs[bas] = new Vector2((float)i / nbFaces, 0f);
            uvs[haut] = new Vector2((float)i / nbFaces, 1f);
        }

        // --- Centres des disques ---
        vertices[centreBas] = new Vector3(0, -demi, 0);
        vertices[centreHaut] = new Vector3(0, demi, 0);
        uvs[centreBas] = new Vector2(0.5f, 0.5f);
        uvs[centreHaut] = new Vector2(0.5f, 0.5f);

        // --- Sommets de bord des disques ---
        for (int k = 0; k < nbFaces; k++)
        {
            float theta = k * pas;
            float c = Mathf.Cos(theta);
            float s = Mathf.Sin(theta);
            Vector2 uvDisque = new Vector2(0.5f + 0.5f * c, 0.5f + 0.5f * s);

            vertices[centreBas + 1 + k] = new Vector3(rayon * c, -demi, rayon * s);
            vertices[centreHaut + 1 + k] = new Vector3(rayon * c, demi, rayon * s);
            uvs[centreBas + 1 + k] = uvDisque;
            uvs[centreHaut + 1 + k] = uvDisque;
        }

        // --- Triangles ---
        int t = 0;

        // Corps : plus besoin de modulo, la colonne i+1 existe toujours
        for (int i = 0; i < nbFaces; i++)
        {
            int basI = 2 * i;
            int hautI = basI + 1;
            int basJ = 2 * (i + 1);
            int hautJ = basJ + 1;

            triangles[t++] = basI;
            triangles[t++] = hautI;
            triangles[t++] = hautJ;

            triangles[t++] = basI;
            triangles[t++] = hautJ;
            triangles[t++] = basJ;
        }

        // Disques : ici le modulo sert, car le bord n'a que nbFaces sommets
        for (int k = 0; k < nbFaces; k++)
        {
            int kSuiv = (k + 1) % nbFaces;

            // Bas
            triangles[t++] = centreBas;
            triangles[t++] = centreBas + 1 + k;
            triangles[t++] = centreBas + 1 + kSuiv;

            // Haut
            triangles[t++] = centreHaut;
            triangles[t++] = centreHaut + 1 + kSuiv;
            triangles[t++] = centreHaut + 1 + k;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }
}
