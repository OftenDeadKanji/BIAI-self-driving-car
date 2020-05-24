using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

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

    //do danych statystycznych
    private const string FILE_NAME = "D:\\Users\\Kanjiklub\\Documents\\GitHub\\BIAI-self-driving-car\\Car_Unity\\Car_BIAI\\Excel\\data.xls";

    void Start()
    {
        currentGeneration = new float[maxPopulationCount, 2, 7];
        currentGenerationsScore = new float[maxPopulationCount];

        cars = new GameObject[maxPopulationCount];
        for (int i = 0; i < maxPopulationCount; i++)
        {
            cars[i] = Instantiate(carPrefab, carPrefab.transform.parent);
            cars[i].GetComponent<CarController>().StartCar();
            //cars[i].GetComponent<CarController>().RandomizeBrain();
        }

        generationNumberText.text = "1";
        System.IO.File.WriteAllText(FILE_NAME, "GENERATION\tMAX SCORE\tAVG SCORE\n");
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
            MakeNextGeneration();
            for (int i = 0; i < maxPopulationCount; i++)
            {
                CarController car = cars[i].GetComponent<CarController>();
                car.ResetCar();
            }
        }
    }

    private void MakeNextGeneration()
    {
        float maxScore = 0.0f, avgScore = 0.0f;
        foreach(GameObject car in cars)
        {
            float score = car.GetComponent<CarController>().Score;
            if(maxScore < score)
            {
                maxScore = score;
            }
            avgScore += score;
        }
        avgScore /= cars.Length;

        using (StreamWriter file = new StreamWriter(FILE_NAME, true))
        {
            file.WriteLine(generationNumber.ToString() + "\t" + maxScore.ToString() + "\t" + avgScore.ToString());
        }


        //TODO mutation and other things - no mutation for the moment
        NeuralNet[] brains = new NeuralNet[maxPopulationCount];
        float best = -5f;
        float sndBest = -10f;
        CarController bestCar = null;
        CarController sndBestCar = null;
        foreach (GameObject car in cars)
        {
            CarController tempCar = car.GetComponent<CarController>();
            if (tempCar.Score > best)
            {
                best = tempCar.Score;
                bestCar = car.GetComponent<CarController>();
            }
            else if (tempCar.Score > sndBest)
            {
                sndBest = tempCar.Score;
                sndBestCar = car.GetComponent<CarController>();
            }
        }

        NeuralNet parentOne = bestCar.getNet();
        NeuralNet parentTwo = sndBestCar.getNet();

        brains[0] = parentOne;
        brains[1] = parentTwo;
        NeuralNet[] pureChildren = parentOne.cross(parentTwo);
        brains[2] = pureChildren[0];
        brains[3] = pureChildren[1];

        for (int i = 4; i < maxPopulationCount - (maxPopulationCount * 0.1); i += 2)
        {
            NeuralNet[] children = parentOne.crossMutate(parentTwo);
            brains[i] = children[0];
            brains[i + 1] = children[1];
        }
        for (int i = (int)(maxPopulationCount - (maxPopulationCount * 0.1)); i < maxPopulationCount; i++)
        {
            brains[i] = new NeuralNet();
        }
        for (int i = 0; i < maxPopulationCount; i++)
        {
            CarController car = cars[i].GetComponent<CarController>();
            car.setNet(brains[i]);
        }
        //increment generation number and change UI Text
        generationNumberText.text = (++generationNumber).ToString();
    }

    private void OnDestroy()
    {
        
    }
}
