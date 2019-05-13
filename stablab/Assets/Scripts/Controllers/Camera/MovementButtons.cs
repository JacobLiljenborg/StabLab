﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


enum MOVEMENTTYPE {
    PAN,
    ROTATE,
    ZOOM,
    RESET
};

public class MovementButtons : MonoBehaviour,IPointerDownHandler, IPointerUpHandler
{
    private Transform target;
    public int directionNumber;
    public int typeNumber;
    private Vector3 direction;
    private int speed = 5;
    private bool move = false;
    private MOVEMENTTYPE MoveType = MOVEMENTTYPE.PAN;
    // Start is called before the first frame update
    void Start()
    {
        target = Camera.main.transform;
        switch (typeNumber) {
            case 0: //pan
                MoveType = MOVEMENTTYPE.PAN;
                speed = 5;
                switch (directionNumber) {
                    case 1:
                        direction = Vector3.left;
                        break;
                    case 2:
                        direction = Vector3.right;
                        break;
                    case 3:
                        direction = Vector3.down;
                        break;
                    case 4:
                        direction = Vector3.up;
                        break;
                    default:
                        direction = Vector3.zero;
                        break;
                }
                break;
            case 1: //rotation
                MoveType = MOVEMENTTYPE.ROTATE;
                speed = 20;
                switch (directionNumber)
                {
                    case 1:
                        direction = Vector3.up;
                        break;
                    case 2:
                        direction = Vector3.down;
                        break;
                    case 3:
                        direction = Vector3.left;
                        break;
                    case 4:
                        direction = Vector3.right;
                        break;
                    default:
                        direction = Vector3.zero;
                        break;
                }
                break;
            case 2: // zoom
                MoveType = MOVEMENTTYPE.ZOOM;
                speed = 20;
                break;
            case 3:
                MoveType = MOVEMENTTYPE.RESET;
                break;

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (move)
        {
            if ((MoveType == MOVEMENTTYPE.PAN))
            {
                target.Translate(direction * Time.deltaTime * speed * Camera.main.gameObject.GetComponent<CameraController>().fov / 10);
            }
            else if (MoveType == MOVEMENTTYPE.ROTATE)
            {
                target.RotateAround(Vector3.zero,direction,Time.deltaTime * speed * Camera.main.gameObject.GetComponent<CameraController>().fov / 10);
            }
            else if (MoveType == MOVEMENTTYPE.ZOOM)
            {
                float fov;
                fov = Camera.main.fieldOfView;

                fov -= directionNumber * Time.deltaTime * speed;
                fov = Mathf.Clamp(fov, 0, 100);
                Camera.main.fieldOfView = fov;
            }
        }
    }

    public void OnPointerDown(PointerEventData e)
    {
        if (MoveType == MOVEMENTTYPE.RESET) ResetView();
        move = true;
    }

    public void OnPointerUp(PointerEventData e) {
        move = false;
    }

    private void ResetView() {
        target.position = Camera.main.gameObject.GetComponent<CameraController>().stdPos;
        target.rotation = Camera.main.gameObject.GetComponent<CameraController>().stdRot;
        Camera.main.fieldOfView = Camera.main.gameObject.GetComponent<CameraController>().stdFov;
    }
}