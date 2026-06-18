#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// One-shot editor tool: right-click this component > "Rebuild Collider".
/// Reads the sprite's Custom Physics Shape and bakes it into the MeshCollider.
/// Safe to remove this script after baking — the mesh stays.
/// </summary>
[RequireComponent(typeof(MeshCollider))]
public class SpriteMeshCollider : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

#if UNITY_EDITOR
    [ContextMenu("Rebuild Collider")]
    public void BuildCollider()
    {
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteMeshCollider: No SpriteRenderer assigned.", this);
            return;
        }

        Sprite sprite = spriteRenderer.sprite;
        if (sprite == null)
        {
            Debug.LogError("SpriteMeshCollider: SpriteRenderer has no sprite.", this);
            return;
        }

        int shapeCount = sprite.GetPhysicsShapeCount();
        if (shapeCount == 0)
        {
            Debug.LogError(
                "SpriteMeshCollider: No physics shapes found on sprite.\n" +
                "Open Sprite Editor > Custom Physics Shape, generate a shape, then Apply.",
                this);
            return;
        }

        var allVerts = new List<Vector3>();
        var allTris = new List<int>();

        for (int s = 0; s < shapeCount; s++)
        {
            var path = new List<Vector2>();
            sprite.GetPhysicsShape(s, path);

            if (path.Count < 3)
                continue;

            int baseIndex = allVerts.Count;

            foreach (var p in path)
                allVerts.Add(new Vector3(p.x, p.y, 0f));

            // Fan triangulation — works for convex shapes.
            // For concave shapes Unity's physics outline is usually already
            // split into convex sub-shapes, so this handles most sprites fine.
            for (int i = 0; i < path.Count - 2; i++)
            {
                allTris.Add(baseIndex);
                allTris.Add(baseIndex + i + 1);
                allTris.Add(baseIndex + i + 2);
            }
        }

        if (allVerts.Count == 0)
        {
            Debug.LogError("SpriteMeshCollider: No valid geometry could be built.", this);
            return;
        }

        var mesh = new Mesh();
        mesh.name = sprite.name + "_ColliderMesh";
        mesh.vertices = allVerts.ToArray();
        mesh.triangles = allTris.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        var col = GetComponent<MeshCollider>();
        col.sharedMesh = mesh;

        // Save the mesh as a project asset so it survives reloads
        string path2 = $"Assets/{sprite.name}_ColliderMesh.asset";
        AssetDatabase.CreateAsset(mesh, AssetDatabase.GenerateUniqueAssetPath(path2));
        AssetDatabase.SaveAssets();

        Debug.Log($"SpriteMeshCollider: Collider mesh baked and saved to {path2}", this);
        EditorUtility.SetDirty(col);
    }
#endif
}