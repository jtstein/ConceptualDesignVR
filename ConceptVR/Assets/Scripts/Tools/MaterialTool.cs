using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialTool : Tool {

    public DCGMaterial newDCGMat;
    HUDManager HUD;

    // Use this for initialization
    void Start()
    {
        HUD = GameObject.Find("Managers").GetComponent<HUDManager>();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }

    public override bool Tap(Vector3 position)
    {
        Face nearestFace = DCGBase.NearestFace(position, 0.07f);
        if (nearestFace != null && !nearestFace.isLocked)
        {
            if (!DCGBase.matList.Contains(newDCGMat))   //make sure mat is in the list
                DCGBase.matList.Add(newDCGMat);

            if (nearestFace.mat != null)
                nearestFace.mat.RemoveFace(nearestFace);    //unset previous mat
            newDCGMat.AddFace(nearestFace); //set new mat
            return true;
        }
        return false;
    }

    /*public override bool Tap(Vector3 position)
    {
        Face nearestFace = DCGBase.NearestFace(position, 0.07f);
        if (nearestFace != null && !nearestFace.isLocked)
        {
            // remove the nearestFace from any pre-existing DCGMat lists.
            bool doublebreak = false;
            foreach (DCGMaterial dcgm in DCGBase.matList)
            {
                foreach (Face face in dcgm.facesUsingMat)
                {
                    if(nearestFace == face)
                    {
                        dcgm.facesUsingMat.Remove(face);
                        // Remove the DCGMat from the DCGBase.matList if there are no more faces using the mat
                        if (dcgm.facesUsingMat.Count == 0)
                            DCGBase.matList.Remove(dcgm);
                        doublebreak = true;
                        break;
                    }
                }
                if (doublebreak)
                    break;
            }

            // add the nearestFace to the DCGMat.facesUsingMat list
            newDCGMat.facesUsingMat.Add(nearestFace);
            // reset that DCGMaterial in the DCGBase.matList if it already exists
            foreach(DCGMaterial dcgm in DCGBase.matList)
            {
                if(dcgm.mat == newDCGMat.mat)
                {
                    // reset the DCGMaterial in the DCGBase.matList
                    DCGBase.matList.Remove(dcgm);
                    DCGBase.matList.Add(newDCGMat);
                    break;
                }
            }

            // add the new DCG material to the list of all used DCG materials if it is not in there already
            if (!(DCGBase.matList.Contains(newDCGMat)))
            {
                DCGBase.matList.Add(newDCGMat);
            }

            return true;
        }
        else
        {
            return false;
        }
    }*/

    public void changeMaterial(DCGMaterial DCGMat)
    {
        newDCGMat = DCGMat;
    }

}
