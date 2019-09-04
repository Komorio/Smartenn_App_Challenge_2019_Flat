﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CubeMove : MonoBehaviour{


    [SerializeField]
    private GameObject gameCube;

    [SerializeField]
    private int movePower = 1;

    private Ray ray;

    protected bool isMove = false;
    private Vector3 rotateVector;
    private Vector3 moveVector;

    private IObserver CubeMoveObserver; 
    [SerializeField]
    private GameObject[] cinemachines;
    private int cameraNumber = 0;
    public int CameraNumber{
        get => cameraNumber;

        set {
            
            if(value > 3)
                cameraNumber = 0;
            
            else if(value < 0)
                cameraNumber = 3;
            else
                cameraNumber = value;
        }
    }

    private ICommand Up;
    private ICommand Down;
    private ICommand Left;
    private ICommand Right;


    public void Start(){
        ray.origin = gameObject.transform.position;
        
        Up = new MoveUp();
        Down = new MoveDown();
        Left = new MoveLeft();
        Right = new MoveRight();
    }

    public void CubeUp(){
        Up.Excute(out moveVector, out rotateVector);
        isMove = true;        
        StartCoroutine(MoveAndRotate());
    }

    public void CubeDown(){
        Down.Excute(out moveVector, out rotateVector);
        isMove = true;        
        StartCoroutine(MoveAndRotate());
    }

    public void CubeLeft(){
        Left.Excute(out moveVector, out rotateVector);
        isMove = true;        
        StartCoroutine(MoveAndRotate());
    }

    public void CubeRight(){
        Right.Excute(out moveVector,out rotateVector);
        isMove = true;        
        StartCoroutine(MoveAndRotate());
    }

    private bool DetectedWall()
    {

       ray.origin = gameObject.transform.position;
       ray.direction = moveVector;
       RaycastHit hit;
       
       if(Physics.Raycast(ray,out hit, movePower, LayerMask.GetMask("Wall")))
        {
            return true;
        }
        return false;
    }
    protected IEnumerator MoveAndRotate()
    {

        if (!DetectedWall()){
        // colorState.CurColor.ColorDebug();
            for(int i  = 0 ; i < 10; i++){
                gameCube.transform.Rotate(rotateVector * 9, Space.World);
                gameObject.transform.Translate((moveVector / 10) * movePower, Space.World);
                
                yield return null;
            }
            isMove = false;
            CubeMoveObserver?.Notify();
        }
        else {
            isMove = false;
        }
        moveVector = Vector3.zero;
        rotateVector = Vector3.zero;
    }

    public void CameraTurnLeft(){
        cameraNumber++;
        ChangeCamera();

        ICommand command = Up;
        Up =  Left;
        Left = Down;
        Down = Right;
        Right = command;
    }
    public void CameraTurnRight(){
        cameraNumber--;
        ChangeCamera();

        ICommand command = Up;
        Up = Right;
        Right = Down;
        Down = Left;
        Left = command;

    }

    public void SubscribeObserver(IObserver observer){
        CubeMoveObserver = observer;
    }

    private void ChangeCamera(){
        for(int i = 0; i < cinemachines.Length; i++){
            cinemachines[i].gameObject.SetActive(false);
        }

        cinemachines[cameraNumber].gameObject.SetActive(true);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(ray.origin,ray.direction);
    }
}

public class MoveUp : CubeMove, ICommand
{
    public void Excute(out Vector3 moveVector, out Vector3 rotateVector){
        moveVector = Vector3.forward;
        rotateVector = Vector3.right;
        Debug.Log("Up");
    }
}
public class MoveDown : CubeMove, ICommand
{
    public void Excute(out Vector3 moveVector, out Vector3 rotateVector){
        moveVector = Vector3.back;
        rotateVector = Vector3.left;
        Debug.Log("Down");
    }
}

public class MoveLeft : CubeMove, ICommand
{
    public void Excute(out Vector3 moveVector, out Vector3 rotateVector){
        moveVector = Vector3.left;
        rotateVector = Vector3.forward;
        Debug.Log("Left");
    }
}

public class MoveRight : CubeMove, ICommand
{
    public void Excute(out Vector3 moveVector, out Vector3 rotateVector){
        moveVector = Vector3.right;
        rotateVector = Vector3.back;
        Debug.Log("Right");
    }
}