using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class CharacterModel : MonoBehaviour
{

    public static CharacterModel instance;
    private StickModel Stick;
    
    [SerializeField]
    private Vector3 _modelWalkingStatePosition, _modelClimbingStatePosition;
    private float _climbTopOffsetY = 0.065f;
    
    private Vector3 _climbBeginningPosition, _climbTargetPosition;
    
    
    public Animator modelAnimator;
    private bool _isClimbing;
    
    //Time elapsed kullanılarak dönme işleminin ilerlemesi elde edildi
    private float _timeElapsed = 0;
    private float _climbSpeed = 5f;

    private void Awake()
    {
        instance = this;
        modelAnimator = GetComponent<Animator>();
        Stick = StickModel.instance;
    }



    void Update()
    {
        print("Hang: " + modelAnimator.GetBool("hang"));
        //print("Climb: " + _modelAnimator.GetBool("climb"));
        //print("Push: " +_modelAnimator.GetBool("push"));
        //Karakterin dönme işlemi devam etmiyorsa
        if (!_isClimbing)
        {
            //Bir sonraki işlem için global değer sıfırlanıldı
            _timeElapsed = 0;
            return;
        }
        Lerp();
        
        
       
        
    }
    public void Lerp()
    {
        _climbBeginningPosition = new Vector3(transform.position.x, _climbBeginningPosition.y, transform.position.z);
        _climbTargetPosition = new Vector3(transform.position.x, Stick.GetTopPosition().y - _climbTopOffsetY, transform.position.z);
        transform.position = Vector3.Lerp(_climbBeginningPosition, _climbTargetPosition, _timeElapsed * _climbSpeed);
        if (transform.position.y >= Stick.GetTopHeight())
        {
            _timeElapsed = 1f;
        }
        //Tırmanma işlemi tamamlanıldığında
        if (_timeElapsed >= 1f)
        {
            //Tutma animasyonu çalıştırılır
            modelAnimator.SetBool("climb", false);
            modelAnimator.SetBool("hang", true);
        }
        _timeElapsed += Time.deltaTime;
    }

    public void WalkingState()
    {
        modelAnimator.SetBool("push", true);

        _isClimbing = false;
        transform.localPosition = _modelWalkingStatePosition;

    }

    public void ClimbingState()
    {
        modelAnimator.SetBool("climb", true);
        modelAnimator.SetBool("push", false);


        transform.localPosition = _modelClimbingStatePosition;
        _climbBeginningPosition.y = Stick.GetBottomHeight();
        _climbTargetPosition.y = Stick.GetTopHeight() - _climbTopOffsetY;
        _isClimbing = true;
    }
    
}