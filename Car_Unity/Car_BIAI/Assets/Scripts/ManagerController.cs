using UnityEngine;
using UnityEngine.UI;

public class ManagerController : MonoBehaviour
{
    private int generationNumber = 1;
    [SerializeField] private int maxPopulationCount = 10;
    private float[,,] currentGeneration;
    private float[] currentGenerationsScore;
    [SerializeField] private GameObject carPrefab;

    [Header("UI")]
    [SerializeField] private Text generationNumberText;

    private GameObject[] cars;
    public GameObject[] Cars { get => cars; }

    void Start()
    {
        currentGeneration = new float[maxPopulationCount, 2, 7];
        currentGenerationsScore = new float[maxPopulationCount];

        cars = new GameObject[maxPopulationCount];
        for (int i = 0; i < maxPopulationCount; i++)
        {
            cars[i] = Instantiate(carPrefab, carPrefab.transform.parent);
            cars[i].GetComponent<CarController>().StartCar();
            cars[i].GetComponent<CarController>().RandomizeBrain();
        }

        generationNumberText.text = "1";
    }

    void Update()
    {
        //checking if there's at least one car left
        bool ifAlive = false;
        foreach (GameObject car in cars)
        {
            if (car.GetComponent<CarController>().IsAlive)
            {
                ifAlive = true;
                break;
            }
        }
        //if there is then update
        if (ifAlive)
        {
            foreach (GameObject car in cars)
            {
                car.GetComponent<CarController>().UpdateCar();
            }
        }
        //if not then collect data
        else
        {
            for (int i = 0; i < maxPopulationCount; i++)
            {
                CarController car = cars[i].GetComponent<CarController>();

                float[,] weights = car.GetWeights();
                for (int j = 0; j < 6; j++)
                {
                    //save first output weights
                    currentGeneration[i, 0, j] = weights[0, j];
                    //save second output weights
                    currentGeneration[i, 1, j] = weights[1, j];
                }
                currentGenerationsScore[i] = car.Score;

                car.ResetCar();
            }
            MakeNextGeneration();
        }
    }

    private void MakeNextGeneration()
    {
        //TODO mutation and other things

        //increment generation number and change UI Text
        generationNumberText.text = (++generationNumber).ToString();
    }
}
