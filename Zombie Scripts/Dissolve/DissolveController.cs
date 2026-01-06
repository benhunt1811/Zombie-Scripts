using System.Collections;
using UnityEngine;

public class DissolveController : MonoBehaviour
{
    [Header("Mesh info")]
    public MeshRenderer Mesh;
    public SkinnedMeshRenderer SkinnedMesh;
    private Material[] Materials;
    public DissolveMeshType MeshType;

    [Header("Values")]
    public float DissolveRate = 0.0125f;
    public float Refresh = 0.025f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Used to get the mesh or skinnedmesh
        if (MeshType == DissolveMeshType.Mesh)
        {
            if (Mesh != null)
            {
                Materials = Mesh.materials;
            }
        }

        if (MeshType == DissolveMeshType.SkinnedMesh)
        {
            if (SkinnedMesh != null)
            {
                Materials = SkinnedMesh.materials;
            }
        }
    }

    public IEnumerator Dissolve()
    {
        if (Materials.Length > 0)
        {
            float counter = 0;

            while (Materials[0].GetFloat("_DissolveAmount") < 1)
            {
                counter += DissolveRate;
                for (int i = 0; i < Materials.Length; i++)
                {
                    Materials[i].SetFloat("_DissolveAmount", counter);
                }
                

                yield return new WaitForSeconds(Refresh);
            }

            transform.root.gameObject.SetActive(false);
        }
    }

    public IEnumerator Appear()
    {
        Materials = Mesh.sharedMaterials;
        if (Materials.Length > 0)
        {
            float counter = 1;
            Materials[0].SetFloat("_DissolveAmount", 1);

            while (Materials[0].GetFloat("_DissolveAmount") > 0)
            {
                counter -= DissolveRate;
                for (int i = 0; i < Materials.Length; i++)
                {
                    Materials[i].SetFloat("_DissolveAmount", counter);
                }

                yield return new WaitForSeconds(Refresh);
            }
        }
    }
}
