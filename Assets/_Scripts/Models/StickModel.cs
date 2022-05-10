using UnityEngine;

public class StickModel : MonoBehaviour
{
    public static StickModel instance;
    private PlayerController Player;
    private CharacterModel Character;
    
    [SerializeField] private Vector3 _stickHorizontalStatePosition, _stickVerticalStatePosition;
    [SerializeField] private Vector3 _stickHorizontalStateAngle, _stickVerticalStateAngle;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Player = PlayerController.instance;
        Character = CharacterModel.instance;
    }

    public void HorizontalState()
    {
        transform.localPosition = _stickHorizontalStatePosition;
        transform.localRotation = Quaternion.Euler(_stickHorizontalStateAngle);
    }

    public void VerticalState()
    {
        Vector3 calculatedPosition = _stickVerticalStatePosition;
        calculatedPosition.y += GetStickScale().y;
        transform.localPosition = calculatedPosition;
        transform.localRotation = Quaternion.Euler(_stickVerticalStateAngle);
    }
    public Vector3 GetStickScale()
    {
        return transform.localScale;
    }

    public Vector3 GetTopPosition()
    {
        Vector3 position = transform.position;
        position.y += GetStickScale().y;
        return position;
    }

    public float GetTopHeight()
    {
        return transform.position.y + GetStickScale().y;
    }

    public float GetBottomHeight()
    {
        return transform.position.y - GetStickScale().y;
    }

    public void SliceByX(Collider cutter)
    {
        Transform cutterTransform = cutter.transform;
        Vector3 cutterPosition = cutterTransform.position;

        float overageDistance;
        Vector3 newScale, newPosition, overageDistanceVector, leftOverScale, leftOverPosition;
        GameObject leftoverPart = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        
        //Kesici obje çubuğun hangi konumunda bulunduğu hesaplanıldı.
        //Buna göre artık parçanın hangi yönde hesaplanacağı belirlenildi
        float cutterOnTheXLeft = cutterPosition.x < Player.transform.position.x ? -1 : 1;
        float cutterOnTheZLeft = cutterPosition.z < Player.transform.position.z ? -1 : 1;

        //Çubuğun kesiciye giren uç noktasının konumunun açıya bağlı olarak hesaplanıldı
        Vector3 endPointVector = new Vector3(
            transform.position.x + cutterOnTheXLeft * (Helper.Cos(transform.eulerAngles.y) * GetStickScale().y), 
            transform.position.y, 
            transform.position.z + cutterOnTheZLeft * (Helper.Sin(transform.eulerAngles.y)  * GetStickScale().y)
            );
        
        //Kesişimde kırpılacak kısmın uzunluğunu hesaplanıldı
        overageDistanceVector = endPointVector - cutterPosition;
        overageDistanceVector.y = 0;
        overageDistance = Vector3.Magnitude(overageDistanceVector);
        
        //Çubuğun boyunun iki katı kadar kısalmaması için kesişim/2 kadar kısaltıldı
        newScale = new Vector3(transform.localScale.x, transform.localScale.y - overageDistance / 2,
            transform.localScale.z);
       
        //Artık parçanın değerleri belirlenir
        leftOverScale = new Vector3(transform.localScale.x, overageDistance / 2, transform.localScale.z);
        leftOverPosition = endPointVector;
        //Artık parçanın konumu, kesim öncesi uç nokta, artık parçanın genişliği ve cismin açısı kullanılarak bulundu
        leftOverPosition.x += -cutterOnTheXLeft * (Helper.Cos(transform.eulerAngles.y) * leftOverScale.y);
        leftOverPosition.z += -cutterOnTheZLeft * (Helper.Sin(transform.eulerAngles.y) * leftOverScale.y);
        
        //Ana çubuk yeniden konumlandırıldı
        newPosition = transform.position;
        newPosition.x += -cutterOnTheXLeft * (Helper.Cos(transform.eulerAngles.y) * overageDistance / 2);
        newPosition.z += -cutterOnTheZLeft * (Helper.Sin(transform.eulerAngles.y) * overageDistance / 2);
        //**Çubuk her seferinde ortalansın istenirse bu satır yorum satırına alınabilir**
        transform.position = newPosition;
        
        transform.localScale = newScale;
        
        //Artık parçanın değer ataması yapıldı
        leftoverPart.transform.rotation = transform.rotation;
        leftoverPart.transform.localScale = leftOverScale;
        leftoverPart.transform.position = leftOverPosition;
        leftoverPart.AddComponent<Rigidbody>();
    }

    public void SliceByY(Collider cutter)
    {
        //Çubuğun zemin noktasından kesicinin merkez noktası çıkarılarak kesim miktarı hesaplanıldı
        float stickBottomY = GetBottomHeight();
        float dist = Mathf.Abs(cutter.transform.position.y - stickBottomY);

        
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y - dist / 2, transform.localScale.z);
        transform.position = new Vector3(transform.position.x, transform.position.y + dist / 2, transform.position.z);
        
        GameObject leftoverPart = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        leftoverPart.transform.localScale = new Vector3(transform.localScale.x, dist / 2, transform.localScale.z);
        leftoverPart.transform.rotation = transform.rotation;
        leftoverPart.transform.position = new Vector3(transform.position.x, stickBottomY + leftoverPart.transform.localScale.y, transform.position.z);
        leftoverPart.AddComponent<Rigidbody>();
        //Hata olmasını önelemek amaçlı lokal konumlara geçilmesi için gerekli komutlar yürütüldü
        VerticalState();
        Character.ClimbingState();
        //Birbirinden uzaklaşan alt objelerin, ana objenin merkezini bozmaması için ana obje
        //Yeniden konumlandırıldı
        Vector3 playerPosition = Player.transform.position;
        playerPosition.y = cutter.transform.position.y;
        Player.transform.position = playerPosition;
    }
}
