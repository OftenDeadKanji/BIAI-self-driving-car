using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform carTransform;
    [SerializeField] private Vector3 cameraOffset;

    private void Start()
    {
        //carTransform = GetComponent<CarController>().transform;
        cameraOffset = new Vector3(0, 0, -10);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = carTransform.position + cameraOffset;
    }

    public void ChangeCar()
    {
        //TODO - zmiana auta na te z najlepszym wynikiem
    }
}
