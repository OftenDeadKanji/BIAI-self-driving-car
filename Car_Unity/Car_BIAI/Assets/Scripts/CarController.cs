using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Neural Network's inputs")]
    [SerializeField] private float leftDist;
    [SerializeField] private float rightDist;
    [SerializeField] private float leftfwdDist;
    [SerializeField] private float rightfwdDist;
    [SerializeField] private float fwdDist;

    [Header("Neural Network's outputs")]
    [SerializeField] private float acc = .01f;
    [SerializeField] private float wheels = .0f;

    [Header("Other")]
    [SerializeField] private float spd = 0f;
    [SerializeField] private float passedTime = 0.0f;
    [SerializeField] private float maxPassedTimeForStupid = 10.0f;
    [SerializeField] private int checkStupid = 1;
    [SerializeField] private float prevScore = 0.0f;
    [SerializeField] private float score = 0.0f;
    public float Score { get => score; }

    [Header("Fitness")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private int waypointIter = 0;
    [SerializeField] private int waypointCount = 22;


    private float maxSpdFwd = 5f;
    private float maxSpdBck = -2f;
    private bool isAlive;
    public bool IsAlive { get => isAlive; }

    //NeuralNetwork brain;

    [SerializeField] NeuralNet net;

    SpriteRenderer render;

    // Start is called before the first frame update
    public void StartCar()
    {
        render = GetComponent<SpriteRenderer>();
        ResetCar();

        net = new NeuralNet();

        //setting waypionts for fitness
        waypoints = new Transform[waypointCount];
        for (int i = 0; i < waypointCount; i++)
        {
            Transform waypoint = GameObject.Find("Waypoint (" + (i + 1) + ")").transform;
            waypoints[i] = waypoint;
        }
    }

    // Update is called once per frame
    public void UpdateCar()
    {
        if (isAlive)
        {
            transform.localPosition += spd * transform.up * Time.deltaTime;
            spd += Time.deltaTime * acc;
            //fitness
            float waypointDist = (waypoints[waypointIter].position - transform.position).magnitude;
            score = (waypointIter + 1) * 1000 - (waypointDist * 100);

            passedTime += Time.deltaTime;
            if (passedTime >= maxPassedTimeForStupid)
            {
                if (score - prevScore < 500)
                {
                    isAlive = false;
                    render.color = Color.red;
                    return;
                }
                passedTime = 0;
                prevScore = score;
            }


            if (spd > maxSpdFwd)
                spd = maxSpdFwd;
            else if (spd != 0)
                transform.Rotate(transform.forward, wheels / 3 * spd / Mathf.Abs(spd));
            else if (spd < maxSpdBck)
                spd = maxSpdBck;

            UpdateDistance();

            //i tu mamy część z siecią neuronową

            //ustawienie inputów
            net.setInputs(leftDist, rightDist, leftfwdDist, rightfwdDist, fwdDist, spd);
            //przetworzenie danych
            net.Think();

            //uzyskanie wyników z outputów
            float[] result = net.getOutputs();

            //ustawienie wartości
            acc = 2 * result[0] - 1;
            wheels = 2 * result[1] - 1;
            //acc = result[0];
            //wheels = result[1];
        }
    }

    public void ResetCar()
    {
        transform.position = new Vector3(0, 0, 0);
        transform.rotation = Quaternion.identity;
        checkStupid = 1;
        isAlive = true;
        spd = acc = score = prevScore = waypointIter = 0;
        passedTime = .0f;
        render.color = Color.white;
        //brain = new NeuralNetwork();
    }

    //public void RandomizeBrain()
    //{
    //    brain.RandomizeWeights();
    //}
    //
    //public void SetBrain(float[,] newWeights)
    //{
    //    brain.Weights = newWeights;
    //}
    //
    //public float[,] GetWeights()
    //{
    //    return brain.Weights;
    //}

    public Genome GetGenes()
    {
        return net.getGenome();
    }

    public void setNet(NeuralNet brain)
    {
        net = brain;
    }

    public NeuralNet getNet()
    {
        return net;
    }

    private void UpdateDistance()
    {
        RaycastHit2D seen;
        if (seen = Physics2D.Raycast((Vector2)transform.position, (Vector2)transform.up, 3.0f))
        {
            fwdDist = (seen.point - (Vector2)transform.position).magnitude;
        }
        else
        {
            fwdDist = 3.0f;
        }
        if (seen = Physics2D.Raycast((Vector2)transform.position, (Vector2)transform.right, 3.0f))
        {
            rightDist = (seen.point - (Vector2)transform.position).magnitude;
        }
        else
        {
            rightDist = 3.0f;
        }
        if (seen = Physics2D.Raycast((Vector2)transform.position, (Vector2)(-transform.right), 3.0f))
        {
            leftDist = (seen.point - (Vector2)transform.position).magnitude;
        }
        else
        {
            leftDist = 3.0f;
        }
        if (seen = Physics2D.Raycast((Vector2)transform.position, (Vector2)(transform.right + transform.up), 3.0f))
        {
            rightfwdDist = (seen.point - (Vector2)transform.position).magnitude;
        }
        else
        {
            rightfwdDist = 3.0f;
        }
        if (seen = Physics2D.Raycast((Vector2)transform.position, (Vector2)(transform.up - transform.right), 3.0f))
        {
            leftfwdDist = (seen.point - (Vector2)transform.position).magnitude;
        }
        else
        {
            leftfwdDist = 3.0f;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Wall"))
        {
            render.color = Color.red;
            isAlive = false;
        }
        else if (col.CompareTag("Waypoint"))
        {
            if (waypointIter < waypoints.Length - 1) waypointIter++;
            else Debug.Log("FIN");
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Wall"))
        {
            render.color = Color.white;
        }
    }

    void OnDrawGizmosSelected()
    {
        Vector3 target = 3 * transform.right + transform.position;
        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position, target);
        target = -3 * transform.right + transform.position;
        Gizmos.DrawLine(transform.position, target);
        target = 3 * transform.up + transform.position;
        Gizmos.DrawLine(transform.position, target);
        target = 3 * Vector3.Normalize(transform.right + transform.up) + transform.position;
        Gizmos.DrawLine(transform.position, target);
        target = 3 * Vector3.Normalize(-transform.right + transform.up) + transform.position;
        Gizmos.DrawLine(transform.position, target);
    }

}

class NeuralNetwork
{
    private float[] inputs;

    public float[] Inputs
    {
        set
        {
            inputs = value;
        }
    }

    public float[] Outputs { get; }

    public float[,] Weights { get; set; }

    public NeuralNetwork()
    {
        inputs = new float[6];
        Outputs = new float[2];
        Weights = new float[2, 6];
    }

    public void RandomizeWeights()
    {
        for (int i = 0; i < 6; i++)
        {
            Weights[0, i] = Random.Range(-1.0f, 1.0f);
            Weights[1, i] = Random.Range(-1.0f, 1.0f);
        }
    }

    public void Think()
    {
        Outputs[0] = Outputs[1] = 0.0f;
        for (int i = 0; i < 6; i++)
        {
            Outputs[0] += inputs[i] * Weights[0, i];
            Outputs[1] += inputs[i] * Weights[1, i];
        }
        //TODO? add bias? Nope

        Outputs[0] = Sigmoid(Outputs[0]);
        Outputs[1] = Sigmoid(Outputs[1]);
    }

    private float Sigmoid(float inValue)
    {
        float i = Mathf.Exp(inValue);
        return i / (i + 1.0f);
    }
}

public class NeuralNet
{
    private float[][] neurons;
    private float[][][] weights;
    private int hidLayers;

    public NeuralNet(params int[] neurInLay)
    {
        hidLayers = neurInLay.Length;
        int layers = 2 + neurInLay.Length;
        neurons = new float[layers][];
        weights = new float[layers - 1][][];
        neurons[0] = new float[6];
        neurons[neurons.Length - 1] = new float[2];
        for (int i = 1; i < layers - 1; i++)
        {
            neurons[i] = new float[neurInLay[i - 1]];
        }
        for (int i = 0; i < neurons.Length - 1; i++)
        {
            weights[i] = new float[neurons[i + 1].Length][];
            for (int j = 0; j < weights[i].Length; j++)
            {
                weights[i][j] = new float[neurons[i].Length];
                for (int k = 0; k < neurons[i].Length; k++)
                {
                    float wgt = Random.Range(0.00001f, 0.9f);
                    if (Random.Range(0f, 1f) > 0.5f)
                    {
                        weights[i][j][k] = -wgt;
                    }
                    else
                    {
                        weights[i][j][k] = wgt;
                    }
                }
            }
        }
    }

    public NeuralNet(Genome genes)
    {
        int[] neurInLay = genes.getTopology();
        hidLayers = neurInLay.Length;
        int layers = 2 + neurInLay.Length;
        neurons = new float[layers][];
        weights = new float[layers - 1][][];
        neurons[0] = new float[6];
        neurons[neurons.Length - 1] = new float[2];
        for (int i = 1; i < layers - 1; i++)
        {
            neurons[i] = new float[neurInLay[i - 1]];
        }
        int extIter = 0;
        for (int i = 0; i < neurons.Length - 1; i++)
        {
            weights[i] = new float[neurons[i + 1].Length][];
            for (int j = 0; j < weights[i].Length; j++)
            {
                weights[i][j] = new float[neurons[i].Length];
                for (int k = 0; k < neurons[i].Length; k++)
                {
                    weights[i][j][k] = genes.getGene(extIter);
                    extIter++;
                }
            }
        }
    }

    public void Think()
    {
        for (int i = 1; i < neurons.Length; i++)
        {
            for (int j = 0; j < neurons[i].Length; j++)
            {
                float suma = 0f;
                for (int k = 0; k < neurons[i - 1].Length; k++)
                {
                    suma += neurons[i - 1][k] * weights[i - 1][j][k];
                }
                neurons[i][j] = Sigmoid(suma);
            }
        }
    }

    public void setInputs(float n1, float n2, float n3, float n4, float n5, float n6)
    {
        neurons[0][0] = n1;
        neurons[0][1] = n2;
        neurons[0][2] = n3;
        neurons[0][3] = n4;
        neurons[0][4] = n5;
        neurons[0][5] = n6;
    }

    public float[] getOutputs()
    {
        float[] ret = { neurons[neurons.Length - 1][0], neurons[neurons.Length - 1][1] };
        return ret;
    }

    private float Sigmoid(float inValue)
    {
        float i = Mathf.Exp(inValue);
        return i / (i + 1.0f);
    }

    public Genome getGenome()
    {
        int genomeSize = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                genomeSize += weights[i][j].Length;
            }
        }
        int[] topology = new int[hidLayers];
        for (int i = 0; i < hidLayers; i++)
        {
            topology[i] = neurons[i + 1].Length;
        }
        Genome genome = new Genome(genomeSize, topology);
        int extIter = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    genome.setGene(extIter, weights[i][j][k]);
                    extIter++;
                }
            }
        }
        return genome;
    }

    public NeuralNet[] cross(NeuralNet otherParent)
    {
        Genome parentOneGenes = this.getGenome();
        Genome parentTwoGenes = otherParent.getGenome();
        int genomeSize = parentOneGenes.getSize();
        if (genomeSize != parentTwoGenes.getSize()) return null;
        int slicePoint = Random.Range(1, genomeSize);
        int[] topology = new int[hidLayers];
        for (int i = 0; i < hidLayers; i++)
        {
            topology[i] = neurons[i + 1].Length;
        }
        Genome childOneGenes = new Genome(genomeSize, topology);
        Genome childTwoGenes = new Genome(genomeSize, topology);
        for (int i = 0; i < genomeSize; i++)
        {
            if (i < slicePoint)
            {
                childOneGenes.setGene(i, parentOneGenes.getGene(i));
                childTwoGenes.setGene(i, parentTwoGenes.getGene(i));
            }
            else
            {
                childOneGenes.setGene(i, parentTwoGenes.getGene(i));
                childTwoGenes.setGene(i, parentOneGenes.getGene(i));
            }
        }

        NeuralNet[] children = new NeuralNet[2];
        children[0] = new NeuralNet(childOneGenes);
        children[1] = new NeuralNet(childTwoGenes);

        return children;
    }

    public NeuralNet[] crossMutate(NeuralNet otherParent)
    {
        Genome parentOneGenes = this.getGenome();
        Genome parentTwoGenes = otherParent.getGenome();
        int genomeSize = parentOneGenes.getSize();
        if (genomeSize != parentTwoGenes.getSize()) return null;
        int slicePoint = Random.Range(1, genomeSize);
        int[] topology = new int[hidLayers];
        for (int i = 0; i < hidLayers; i++)
        {
            topology[i] = neurons[i + 1].Length;
        }
        Genome childOneGenes = new Genome(genomeSize, topology);
        Genome childTwoGenes = new Genome(genomeSize, topology);
        for (int i = 0; i < genomeSize; i++)
        {
            if (i < slicePoint)
            {
                childOneGenes.setGene(i, parentOneGenes.getGene(i));
                childTwoGenes.setGene(i, parentTwoGenes.getGene(i));
            }
            else
            {
                childOneGenes.setGene(i, parentTwoGenes.getGene(i));
                childTwoGenes.setGene(i, parentOneGenes.getGene(i));
            }
        }

        childOneGenes.mutate(3);
        childTwoGenes.mutate(3);

        NeuralNet[] children = new NeuralNet[2];
        children[0] = new NeuralNet(childOneGenes);
        children[1] = new NeuralNet(childTwoGenes);

        return children;
    }
}

public class Genome
{
    private float[] genes;
    private int[] topology;

    public Genome(int genomeSize, int[] neuronAm)
    {
        genes = new float[genomeSize];
        topology = neuronAm;
    }

    public void setGene(int i, float val)
    {
        if (i >= genes.Length) { Debug.Log("Excceded genome length!"); return; }
        genes[i] = val;
    }

    public float getGene(int i)
    {
        if (i >= genes.Length) return 0;
        return genes[i];
    }

    public int[] getTopology()
    {
        return topology;
    }

    public int getSize()
    {
        if (genes == null) return 0;
        return genes.Length;
    }

    public void mutate(int noMutation)
    {
        for (int i = 0; i < noMutation; i++)
        {
            int mutaPoint = Random.Range(0, genes.Length - 1);
            if (Random.Range(0.0f, 1.0f) > 0.5f)
            {
                genes[mutaPoint] *= -1;
            }
            else
            {
                genes[mutaPoint] *= Random.Range(0.7f, 1.2f);
                if (genes[mutaPoint] > 1f) genes[mutaPoint] = 1.0f;
                else if (genes[mutaPoint] < -1f) genes[mutaPoint] = -1.0f;
            }
        }
    }

    public string getStr()
    {
        string ret = "";
        for (int i = 0; i < genes.Length; i++)
        {
            ret += genes[i] + "\n";
        }
        return ret;
    }
}