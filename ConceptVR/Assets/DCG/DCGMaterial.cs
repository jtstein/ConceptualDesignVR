using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New DCG Material", menuName = "DCGMaterial")]
public class DCGMaterial : ScriptableObject {

    public new string name;
    public new Material mat;
    public List<Face> facesUsingMat = new List<Face>();

    public void AddFace(Face f)
    {
        if (!facesUsingMat.Contains(f))
        {
            facesUsingMat.Add(f);
            f.mat = this;
        }
    }

    public void RemoveFace(Face f)
    {
        if (facesUsingMat.Contains(f))
        {
            facesUsingMat.Remove(f);
            f.mat = null;
        }
    }
}
