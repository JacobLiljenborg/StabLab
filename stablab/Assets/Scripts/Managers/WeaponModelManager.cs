﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponModelManager : MonoBehaviour
{

    public List<GameObject> models;
    private bool modelActive = false;
    private static GameObject parent;

    public GameObject injuryAddingObj;
    private InjuryAdding injuryAdding;

    public GameObject transformGizmoObj;
    public RuntimeGizmos.TransformGizmo gizmo;

    Quaternion previousRotation;

    private bool staticChange;

    public void Start()
    {
        injuryAdding = injuryAddingObj.GetComponent<InjuryAdding>();
        gizmo = transformGizmoObj.GetComponent<RuntimeGizmos.TransformGizmo>();
    }

    public void Reset() {
        if (parent == null || InjuryManager.activeInjury == null || !InjuryManager.activeInjury.HasMarker()) return;
        parent.transform.rotation = previousRotation;
        InjuryManager.activeInjury.AddModel(parent);

        Destroy(parent);
        parent = null;
    }

    public void ToggleCurrentModel() {
        SetParentIfNonExisting();
        InjuryModelGizmos modelGizmo = parent.GetComponentInChildren<InjuryModelGizmos>();
        if (parent != null)
        {
            if(InjuryManager.activeInjury.HasMarker()){
                InjuryManager.activeInjury.Marker.activeInPresentation = !parent.activeSelf;
            }
            
            parent.SetActive(!parent.activeSelf);
            modelActive = parent.activeSelf;
            //gizmo.enabled = parent.activeSelf;
            if(!parent.activeSelf){
                RemoveGizmoFromModel();
                modelGizmo.gizmoActive = false;
            }
        }

    }

    public void ToggleGizmo() {
        SetParentIfNonExisting();
        if (parent == null || parent.activeSelf == false) return;
        InjuryModelGizmos modelGizmo = parent.GetComponentInChildren<InjuryModelGizmos>();
        if (modelGizmo.gizmoActive) {
            //SaveRotation();
            RemoveGizmoFromModel();
            modelGizmo.gizmoActive = false;
        }
        else {
            modelGizmo.gizmoActive = true;
            AddGizmoToModel();
        }
    }

    public void AddGizmoToModel()
    {
        SetParentIfNonExisting();
        GameObject curModel = parent;
        if (curModel == null || parent.activeSelf == false) return;
        InjuryModelGizmos comp = curModel.GetComponentInChildren<InjuryModelGizmos>();
        RuntimeGizmos.TransformGizmo gizmo = comp.gizmo;
        comp.AddGizmo(curModel);
        gizmo.bodyPartsMovement = false;
    }

    public void RemoveGizmoFromModel()
    {
        SetParentIfNonExisting();
        GameObject curModel = parent;
        if (curModel == null) return;
        InjuryModelGizmos comp = curModel.GetComponentInChildren<InjuryModelGizmos>();
        RuntimeGizmos.TransformGizmo gizmo = comp.gizmo;
        comp.RemoveGizmo(curModel);
        gizmo.bodyPartsMovement = true;
    }

    public void AddModel()
    {
        if (parent != null) parent.SetActive(true);
        modelActive = true;
    }

    public void RemoveModel()
    {
        if (parent != null) parent.SetActive(false);
        modelActive = false;
    }

    public void UpdateModel()
    {
        /*
        if (InjuryManager.activeInjury.HasMarker()) {
            parent = InjuryManager.activeInjury.Marker.parent;
            model = parent.transform.GetChild(0).gameObject;
            UpdateParentAndChild();
            Debug.Log(model == null);
            return;
        }*/

        if (parent != null)
        {
            Destroy(parent);
        }

        GameObject currentModel = GetCurrentModel();
        if (InjuryManager.activeInjury.HasMarker())
        {
            InjuryManager.activeInjury.Marker.GetParent().SetActive(false);
            //UpdateParentAndChild();
        }

        if (currentModel == null) return;
        parent = GameObject.Instantiate(currentModel);
        parent.GetComponent<InjuryModelGizmos>().gizmo = this.gizmo;

        //parent = new GameObject("parentToWeaponModel to " + InjuryManager.activeInjury.Name);
        UpdateParentAndChild();
        RotateModel();
    }

    public void UpdateParentAndChild() {
        parent.transform.parent = injuryAdding.hitPart;
        parent.transform.position = injuryAdding.markerPos;
        parent.SetActive(true);
        modelActive = true;
    }

    public void SetParentIfNonExisting() {
        if (parent == null && InjuryManager.activeInjury != null &&InjuryManager.activeInjury.HasMarker())
        {
            parent = InjuryManager.activeInjury.Marker.GetParent();

        }
    }

    public void RotateModel()
    {
        // Rotates the model towards the user

        Vector3 cameraPos = Camera.main.transform.position;

        parent.transform.rotation = Quaternion.FromToRotation(Vector3.left, cameraPos - injuryAdding.markerPos);
    }

    public void SaveRotation() {
        // Save the rotation from the gizmo to the temporary model.
        /*InjuryModelGizmos comp = parent.GetComponentInChildren<InjuryModelGizmos>();
        if (comp != null)
        {

            Quaternion rot = comp.GetRotation();
            if (rot != null) parent.transform.rotation = rot;
        }*/
        if (InjuryManager.activeInjury != null && InjuryManager.activeInjury.HasMarker())
        {
            InjuryManager.activeInjury.Marker.SetModelRotation(parent.transform.rotation);
            //InjuryManager.activeInjury.injuryMarkerObj.transform.rotation = parent.transform.rotation;
        }
    }


    public void SaveModel() {
        // Add the temporary model to the injury and reset variables
        SetParentIfNonExisting();
        if (parent == null) return;

        //SaveRotation();
        RemoveGizmoFromModel();
        InjuryManager.activeInjury.Marker.activeInPresentation = parent.activeSelf;
        InjuryManager.activeInjury.AddModel(parent);
        Destroy(parent);
        parent = null;
    }

    public GameObject GetModel(Injury injury)
    {
        switch (injury.Type)
        {
            case InjuryType.Cut:
                return injuryAdding.cutModel;
            case InjuryType.Crush:
                return injuryAdding.crushModel;
            case InjuryType.Shot:
                return injuryAdding.shotModel;
            case InjuryType.Stab:
                return injuryAdding.stabModel;
            default:
                return null;
        }
    }

    public GameObject GetCurrentModel()
    {
        return GetModel(InjuryManager.activeInjury);
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

        MeshRenderer mesh = injury.Marker.GetParent().transform.GetComponentInChildren<MeshRenderer>();
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
        parent = newParent;
    }

    public void ResetModel(){
        Destroy(parent);
        parent = null;
    }
}
