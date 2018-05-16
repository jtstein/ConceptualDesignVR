using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Exporter : HUDButton {

    public Material exportMaterial;
    int exportNumber;
    int initialExportNumber;
    public GameObject exportedText;

	void Start () {
        // Grab the export number by counting the number of .objs in the export directory 
        DirectoryInfo dir = new DirectoryInfo("Assets/Resources/Exports/Meshes/");
        FileInfo[] info = dir.GetFiles("*.obj");
        exportNumber = info.Length+1;
        initialExportNumber = exportNumber;
        exportedText.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
        base.Update();
	}

    public override void OnPress()
    {
        // re-initialize export number so user can re-export solids if they wish to
        exportNumber = initialExportNumber;

        foreach (Solid solid in DCGBase.solids)
        {
            string filePath = "Assets/Resources/Exports/Meshes/";
            string fileName = "export" + exportNumber;
            filePath += fileName + ".obj";

            /* create empty game object, add mesh filter and mesh renderer to it
               initialize mesh filter with meshs from dcgbase >> solids */
            GameObject go = new GameObject();
            MeshFilter meshFilter = go.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = solid.getMesh();
            MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
            meshRenderer.material = exportMaterial;
            Debug.Log("Exporting solid to " + filePath);
            ObjExporter.MeshToFile(meshFilter, filePath);

            exportNumber++;
        }
        StartCoroutine(displayExportedText());
        base.OnPress();
    }

    private IEnumerator displayExportedText()
    {
        exportedText.SetActive(true);
        yield return new WaitForSeconds(2);
        exportedText.SetActive(false);
    }
}
