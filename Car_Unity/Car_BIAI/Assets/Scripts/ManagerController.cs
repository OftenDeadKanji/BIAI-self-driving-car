using UnityEngine;
using System;
public class ManagerController : MonoBehaviour
{
    [SerializeField] private int generationNumber = 1;
    [SerializeField] private int maxPopulationCount = 10;
    private int currentCarNumber = 0;
    private float[,,] currentGeneration;
    private float[] currentGenerationsScore;

    [SerializeField] private CarController car;

    void Start()
    {
        currentGeneration = new float[maxPopulationCount, 2, 7];
        currentGenerationsScore = new float[maxPopulationCount];

        //car = GetComponent<CarController>();
        car.StartCar();

        Debug.Log("Simulation started.");
    }

    void Update()
    {
        if (car.IsAlive)
        {
            car.UpdateCar();
            Debug.Log("Car uptaded.");
        }
        else
        {
            if (++currentCarNumber < 10)
            {
                float[,] weights = car.GetWeights();
                for (int i = 0; i < 6; i++)
                {
                    currentGeneration[currentCarNumber, 0, i] = weights[0, i];
                    currentGeneration[currentCarNumber, 1, i] = weights[1, i];
                }
                currentGenerationsScore[currentCarNumber] = car.Score;
            }
            else
            {
                MakeNextGeneration();
                generationNumber++;
            }
        }
    }
    private void MakeNextGeneration()
    {
        //TODO
    }
}
