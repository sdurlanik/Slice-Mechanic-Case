using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    //Singleton
    public static PlayerController instance;
    
    //Alt objelerin tanımı
    private CharacterModel Character;
    private StickModel Stick;
    private Path Path;
    
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;

    [SerializeField]
    private float _forwardSpeed;

    public float GetForwardSpeed => _forwardSpeed;
    private bool isGameEnded ;


    public bool IsGameEnded
    {
        get => isGameEnded;
        set => isGameEnded = value;
    }



    private void Awake()
    {
        instance = this;
        Character = CharacterModel.instance;
        Stick = StickModel.instance;
        Path = Path.instance;
        Time.timeScale = 1;
        isGameEnded = false;
    }

    void Update()
    {
        if(!isGameEnded)
            MovePosition();

        
        if (transform.position.y < -1)
        {
            GameOver();
        }
        

        
        if (Input.GetMouseButtonDown(0))
        {
            ClimbingState();
        }

        if (Input.GetMouseButtonUp(0))
        {
            WalkingState();
        }

        if (Input.GetMouseButtonDown(1))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    //Alt objelere lokal pozisyonları atanır
    public void WalkingState()
    {
        Stick.HorizontalState();
        Character.WalkingState();
    }
    //Alt objelere lokal pozisyonları atanır
    public void ClimbingState()
    {
        Stick.VerticalState();
        Character.ClimbingState();
    }

    public void MovePosition()
    {
       
        transform.Translate(0, 0, _forwardSpeed * Time.deltaTime); 

    }

    public void TurnLeft(Collider other)
    {
        Bend bend = Path.GetBend(other);
        if (bend == null) return;
        StartCoroutine(RotateMe(new Vector3(0, -90, 0), bend));
    }
    public void TurnRight(Collider other)
    {
        Bend bend = Path.GetBend(other);
        if (bend == null) return;
        StartCoroutine(RotateMe(new Vector3(0, 90, 0), bend));
    }

    IEnumerator RotateMe(Vector3 toAngles, Bend bend)
    {
        var fromAngle = transform.rotation;
        var toAngle = Quaternion.Euler(transform.eulerAngles + toAngles);
        float bendTime = bend.CalculateBendTime();
        bool direction = transform.rotation.y > -15 && transform.rotation.y < 15;

        for (var t = 0f; t <= bendTime + Time.deltaTime; t += Time.deltaTime)
        {
            float progress = t / bendTime;
            transform.rotation = Quaternion.Slerp(fromAngle, toAngle, progress);
            yield return null;
        }
        Vector3 fixMove = Vector3.zero;
        if (direction){
            fixMove.z = bend.bendQuitPoint.position.z - transform.position.z;
        }
        else
        {
            fixMove.x = bend.bendQuitPoint.position.x - transform.position.x;
        }
        int fixStep = 20;
        Vector3 increment = fixMove / fixStep;

        for (int i = 0; i < fixStep; i++)
        {
            transform.position = transform.position + (increment);
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        WoodTrigger(other);
        DiamondTrigger(other);
        TurnRightTrigger(other);
        TurnLeftTrigger(other);
        ObstacleXTrigger(other);
        ObstacleYTrigger(other);
        EndTrigger(other);
        
    }

    private void OnCollisionEnter(Collision other)
    {
        print(other.contacts.Length);
    }

    private void WoodTrigger(Collider other)
    {
        if (!other.CompareTag("Wood")) return;
        other.gameObject.SetActive(false);
        Stick.transform.localScale += new Vector3(0, 0.01f, 0);
        UI.instance.AddScore(10);
    }
    private void EndTrigger(Collider other)
    {
        if (!other.CompareTag("EndPlane")) return;
        
        print(other.gameObject.transform.GetSiblingIndex()+1);
        GameWin(other.gameObject.transform.GetSiblingIndex()+1);

        Stick.transform.gameObject.SetActive(false);

        _virtualCamera.Follow = transform.GetChild(2);
        transform.rotation = new Quaternion(0, 170, 0, 0);
        isGameEnded = true;
        Character.modelAnimator.SetTrigger("dance");
        


    }
    private void DiamondTrigger(Collider other)
    {
        if (!other.CompareTag("Diamond")) return;
        other.gameObject.SetActive(false);
        UI.instance.AddScore(50);
    }
    private void TurnRightTrigger(Collider other)
    {
        if (!other.CompareTag("TurnRightTrigger")) return;
        
        TurnRight(other);
        other.gameObject.SetActive(false);
    }
    private void TurnLeftTrigger(Collider other)
    {
        if (!other.CompareTag("TurnLeftTrigger")) return;
        
        TurnLeft(other);
        other.gameObject.SetActive(false);
    }
    
    private void ObstacleXTrigger(Collider other)
    {
        if (!other.CompareTag("Obstacle")) return;
        Stick.SliceByX(other);
    }
    
    private void ObstacleYTrigger(Collider other)
    {
        if (!other.CompareTag("Obstacle2")) return;
        if (Character.transform.position.y <= other.transform.position.y)
        {
            GameOver();
            return;
        }
        Stick.SliceByY(other);
        other.GetComponent<Collider>().enabled = false;
    }

    public void GameOver()
    {
        Time.timeScale = 0; 
        isGameEnded = true;
        UI.instance.GameOverUI();
    }

    public void GameWin(int multiplyValue)
    {
        isGameEnded = true;
        UI.instance.GameWinUI(multiplyValue);

    }
}
