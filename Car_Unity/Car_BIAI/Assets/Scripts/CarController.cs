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
    [SerializeField] private float maxSpdFwd = 5f;
    [SerializeField] private float maxSpdBck = -2f;
    [SerializeField] private float score = 0.0f;
    public float Score { get => score; }

    private bool isAlive;
    public bool IsAlive { get => isAlive; }

    NeuralNetwork brain;

    SpriteRenderer render;

    // Start is called before the first frame update
    public void StartCar()
    {
        render = GetComponent<SpriteRenderer>();
        ResetCar();
    }

    // Update is called once per frame
    public void UpdateCar()
    {
        transform.localPosition += spd * transform.up * Time.deltaTime;
        if (isAlive)
        {
            spd += Time.deltaTime * acc;
            score = spd * (1 + Time.deltaTime) / 100.0f;

            if (spd > maxSpdFwd)
                spd = maxSpdFwd;
            else if (spd != 0)
                transform.Rotate(transform.forward, wheels / 3 * spd / Mathf.Abs(spd));
            else if (spd < maxSpdBck)
                spd = maxSpdBck;

            UpdateDistance();

            //i tu mamy część z siecią neuronową

            //ustawienie inputów
            brain.Inputs = new float[] { leftDist, leftfwdDist, fwdDist, rightfwdDist, rightDist, spd };

            //przetworzenie danych
            brain.Think();

            //uzyskanie wyników z outputów
            float[] result = brain.Outputs;

            //ustawienie wartości
            acc = result[0];
            wheels = result[1];
        }
    }

    public void ResetCar()
    {
        transform.position = new Vector3(0, 0, 0);
        transform.rotation = Quaternion.identity;
        isAlive = true;
        spd = acc = score = 0;

        brain = new NeuralNetwork();
    }

    public void RandomizeBrain()
    {
        brain.RandomizeWeights();
    }

    public float[,] GetWeights()
    {
        return brain.Weights;
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
        render.color = Color.red;
        isAlive = false;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        render.color = Color.white;
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
        //TODO? dodać bias

        Outputs[0] = Sigmoid(Outputs[0]);
        Outputs[1] = Sigmoid(Outputs[1]);
    }

    private float Sigmoid(float inValue)
    {
        float i = Mathf.Exp(inValue);
        return i / (i + 1.0f);
    }
}