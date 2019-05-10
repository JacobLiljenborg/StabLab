﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponModelManager : MonoBehaviour
{

    public List<GameObject> models;
    private bool modelActive = false;
    private static GameObject tempModel;

    public GameObject injuryAddingObj;
    private InjuryAdding injuryAdding;

    public RuntimeGizmos.TransformGizmo gizmo;

    Quaternion previousRotation;

    private bool staticChange;

    public void Start()
    {
        injuryAdding = injuryAddingObj.GetComponent<InjuryAdding>();
        gizmo = Camera.main.gameObject.GetComponent<RuntimeGizmos.TransformGizmo>();
    }

    public void Reset() {
        if (tempModel == null || InjuryManager.activeInjury == null || !InjuryManager.activeInjury.HasMarker()) return;
        tempModel.transform.rotation = previousRotation;
        InjuryManager.activeInjury.AddModel(tempModel);

        Destroy(tempModel);
        tempModel = null;
    }

    public void ToggleCurrentModel() {
        SetParentIfNonExisting();
        InjuryModelGizmos modelGizmo = tempModel.GetComponentInChildren<InjuryModelGizmos>();
        if (tempModel != null)
        {
            if(InjuryManager.activeInjury.HasMarker()){
                InjuryManager.activeInjury.Marker.activeInPresentation = !tempModel.activeSelf;
            }

            tempModel.SetActive(!tempModel.activeSelf);
            modelActive = tempModel.activeSelf;
            //gizmo.enabled = parent.activeSelf;
            if(!tempModel.activeSelf){
                RemoveGizmoFromModel();
                modelGizmo.gizmoActive = false;
            }
        }

    }

    public void ToggleGizmo() {
        SetParentIfNonExisting();
        if (tempModel == null || tempModel.activeSelf == false) return;
        InjuryModelGizmos modelGizmo = tempModel.GetComponentInChildren<InjuryModelGizmos>();
        if (modelGizmo.gizmoActive) {
            //SaveRotation();
            RemoveGizmoFromModel();
        }
        else {
            AddGizmoToModel();
        }
    }

    public void AddGizmoToModel()
    {
        SetParentIfNonExisting();
        GameObject curModel = InjuryManager.activeInjury.Marker.GetWeaponModel(); ;
        if (curModel == null || tempModel.activeSelf == false) return;
        InjuryModelGizmos comp = curModel.GetComponentInChildren<InjuryModelGizmos>();
        RuntimeGizmos.TransformGizmo transGizmo = comp.gizmo;
        comp.AddGizmo(curModel);
        transGizmo.bodyPartsMovement = false;
    }

    public void RemoveGizmoFromModel()
    {
        SetParentIfNonExisting();
        GameObject curModel = InjuryManager.activeInjury.Marker.GetWeaponModel();
        if (curModel == null) return;
        InjuryModelGizmos comp = curModel.GetComponentInChildren<InjuryModelGizmos>();
        RuntimeGizmos.TransformGizmo transGizmo = comp.gizmo;
        comp.RemoveGizmo(curModel);
        transGizmo.bodyPartsMovement = true;
    }

    public void AddModel()
    {
        if (tempModel != null) tempModel.SetActive(true);
        modelActive = true;
    }

    public void RemoveModel()
    {
        if (tempModel != null) tempModel.SetActive(false);
        modelActive = false;
    }

    public void UpdateModel()
    {
        if (InjuryManager.activeInjury == null) return;
        if (tempModel != null)
        {
            Destroy(tempModel);
        }

        if (InjuryManager.activeInjury.HasMarker())
        {
            InjuryManager.activeInjury.Marker.GetWeaponModel().SetActive(false);
        }

        tempModel = InjuryManager.activeInjury.InstantiateModel(Vector3.zero, Quaternion.identity, null);
        tempModel.GetComponent<InjuryModelGizmos>().gizmo = this.gizmo;

        UpdateParentAndChild();
        RotateModel();
    }

    public void UpdateParentAndChild() {
        tempModel.transform.parent = injuryAdding.hitPart;
        tempModel.transform.position = injuryAdding.markerPos;
        tempModel.SetActive(true);
        modelActive = true;
    }

    public void SetParentIfNonExisting() {
        if (tempModel == null && InjuryManager.activeInjury != null &&InjuryManager.activeInjury.HasMarker())
        {
            tempModel = InjuryManager.activeInjury.Marker.GetWeaponModel();

        }
    }

    public void RotateModel()
    {
        // Rotates the model towards the user

        Vector3 cameraPos = Camera.main.transform.position;

        tempModel.transform.rotation = Quaternion.FromToRotation(Vector3.left, cameraPos - injuryAdding.markerPos);
    }

    public void SaveRotation() {
        // Save the rotation from the gizmo to the temporary model.
        if (InjuryManager.activeInjury != null && InjuryManager.activeInjury.HasMarker())
        {
            InjuryManager.activeInjury.Marker.SetModelRotation(tempModel.transform.rotation);
        }
    }


    public void SaveModel() {
        // Add the temporary model to the injury and reset variables

        SetParentIfNonExisting();
        if (tempModel == null) return;

        RemoveGizmoFromModel();
        InjuryManager.activeInjury.Marker.activeInPresentation = tempModel.activeSelf;
        InjuryManager.activeInjury.Marker.SetWeaponModel(tempModel);
        tempModel = null;
    }

    public void SetActiveInjuryColor(int colorIndex) {
        SetModelColor(colorIndex, InjuryManager.activeInjury);
    }

    public void SetModelColor(int colorIndex,Injury injury) {
        Color color;
        Material m;
        switch (colorIndex) {
            case 0:
                color = Color.red;
                break;
            case 1:
                color = Color.yellow;
                break;
            case 2:
                color = Color.green;
                break;
            default:
                color = Color.white;
                break;
        }
        if (!InjuryManager.activeInjury.HasMarker()) return;

        MeshRenderer mesh = injury.Marker.GetWeaponModel().transform.GetComponentInChildren<MeshRenderer>();
        if (mesh == null) return;
        m = mesh.material;
        m.color = color;
        mesh.material = m;
        injury.Marker.modelColorIndex = colorIndex;
    }

    public void SetPreviousRotation() {
        if (InjuryManager.activeInjury == null || !InjuryManager.activeInjury.HasMarker()) return;
        previousRotation = InjuryManager.activeInjury.Marker.MarkerRotation;
    }

    public static void SetParent(GameObject newParent) {
        tempModel = newParent;
    }

    public void ResetModel(){
        Destroy(tempModel);
        tempModel = null;
    }
}
