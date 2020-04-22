using UnityEngine;

public class CarFollower : MonoBehaviour
{
    public Transform carTransform;
    public Vector3 cameraOffset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = carTransform.position + cameraOffset;
    }

    void changeCar()
    {
        //TODO - zmiana auta na te z najlepszym wynikiem
    }
}
