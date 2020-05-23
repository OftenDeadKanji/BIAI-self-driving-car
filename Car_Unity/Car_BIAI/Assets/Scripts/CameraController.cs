using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    private Transform carTransform;
    [SerializeField] private Vector3 cameraOffset;
    [SerializeField] private ManagerController SimulationManager;
    [SerializeField] private Text scoreTxt;

    private void Start()
    {
        cameraOffset = new Vector3(0, 0, -10);
    }

    // Update is called once per frame
    void Update()
    {
        //getting car with the highest score
        float maxScore = -10.0f;
        if (SimulationManager.Cars != null)
        {
            foreach (GameObject car in SimulationManager.Cars)
            {
                
                if (car.GetComponent<CarController>().IsAlive && car.GetComponent<CarController>().Score > maxScore)
                {
                    maxScore = car.GetComponent<CarController>().Score;
                    carTransform = car.transform;
                    scoreTxt.text = "" + maxScore;
                }
            }
        }
        //setting camera position
        transform.position = carTransform.position + cameraOffset;
        
    }

}
