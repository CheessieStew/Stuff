using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class MachineBehaviour : MonoBehaviour {
    enum Light { Red, Yellow, Green };

    [Tooltip("Array of size 3 with references to this machines' lamp, in order: red, yellow, green.")]
    public LampBehaviour[] Lights;

    [Tooltip("An array of triggers that indicate where this machines' inputs are placed")]
    public Collider[] Inputs; // idealnie by było mieć listę par (Collider,DonutKind)

    [Tooltip("An array of Donut Kinds accepted on each input, length should be the same as Inputs.")]
    public IngredientBehaviour.IngredientKind[] AcceptedInputs;

    [Tooltip("The time it takes for this machine to produce an output after consuming its' inputs.")]
    public float ProcessingTime;

    [Tooltip("Prefab to be produced by this machine")]
    public GameObject Output;

    [Tooltip("Size of the area where the new product should be spawned")]
    public Vector3 OutputAreaSize;

    [Tooltip("Center of the area where the new product should be spawned")]
    public Vector3 OutputAreaCenter;
    
    // Ingredients that are placed in each of this machines' inputs
    List<IngredientBehaviour>[] WaitingIngredients;

    // the number of donuts waiting to be produced
    int Queue;
    // time until next donut is produced
    float Timer;

    #region Technical
    // initialization
    void Start ()
    {
        WaitingIngredients = new List<IngredientBehaviour>[Inputs.Length];
        for(int i=0; i< WaitingIngredients.Length; i++)
            WaitingIngredients[i] = new List<IngredientBehaviour>();
        Timer = ProcessingTime;
        Queue = 0;
        
    }

    // validation
    void OnValidate()
    {
        UnityEngine.Assertions.Assert.AreEqual(Inputs.Length,
            AcceptedInputs.Length,
            name + ": Inputs and AcceptedInputs need to have the same length");
    }

    // makes the output area visible for easier editting
    void OnDrawGizmosSelected()
    {

        Gizmos.color = Color.yellow;
        Matrix4x4 cubeTransform = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.matrix = cubeTransform;
        Gizmos.DrawWireCube(Vector3.zero + OutputAreaCenter, OutputAreaSize);
    }

    #endregion

    #region Gameplay
    // try to eat the ingredients and produce a new product
    public void ProcessIngredients()
    {
        bool CanProduce = true;
        for (int i = 0; i < WaitingIngredients.Length; i++)
        {
            if (WaitingIngredients[i].Count == 0)
            {   // pusty input - żółta lampka
                FlashLights(Light.Yellow);
                CanProduce = false;
            }
            else if (!WaitingIngredients[i].Any(donut => donut.Kind == AcceptedInputs[i]))
            {
                // niepusty input, ale brak dobrych składników - czerwona lampka
                FlashLights(Light.Red);
                CanProduce = false;
            }
        }
        if (!CanProduce)
            return;

        //na każdym inpucie zjedz jeden dobry składnik
        for (int i = 0; i < WaitingIngredients.Length; i++)
        {
            IngredientBehaviour input = WaitingIngredients[i].Find(donut => donut.Kind == AcceptedInputs[i]);
            EatInput(input);
            WaitingIngredients[i].Remove(input);
        }
        FlashLights(Light.Green);
        //make a new product
        Queue++;
    }

    // add a new part to correct waiting list
    public void NewIngredient(Collider col, IngredientBehaviour ingredient)
    {
        for (int i=0; i<Inputs.Length; i++)
        {
            if (col == Inputs[i])
            {
                print(name + ": new ingredient on input " + i);
                WaitingIngredients[i].Add(ingredient);
            }
        }
    }
	
    // remove an ingredient that leaves input area
    public void RemoveIngredient(Collider col, IngredientBehaviour ingredient)
    {
        for (int i = 0; i < Inputs.Length; i++)
        {
            if (col == Inputs[i])
            {
                print(name + ": lost ingredient on input " + i);
                WaitingIngredients[i].Remove(ingredient);
            }
        }
    }

    // the machines' way to communicate with the player
    void FlashLights(Light light)
    {
        print(name + ": " + light + " light");
        if (Lights.Length > (int)light && Lights[(int)light]!= null)
        {
            Lights[(int)light].Flash(1);
        }
    }

    // destroy an ingredient
    void EatInput(IngredientBehaviour input)
    {
        // może będziemy chcieli jakieś particle, przemieszczenie składnika,
        // czy coś, zanim nastąpi:
        Destroy(input.gameObject);
    }

    // instantiate the Output prefab
    void Produce()
    {
        print(name + ": producing donut");
        Vector3 newProductPosition = transform.position + transform.rotation * OutputAreaCenter;
        Vector3 displacement = new Vector3(
            Random.Range(-0.5f, 0.5f),
            Random.Range(-0.5f, 0.5f),
            Random.Range(-0.5f, 0.5f));
        displacement.Scale(OutputAreaSize);
        newProductPosition += transform.rotation * displacement;

        GameObject newProduct = Instantiate<GameObject>(Output);
        newProduct.transform.position = newProductPosition;
        Rigidbody body = newProduct.GetComponent<Rigidbody>();

        // TODO: to takie placeholderowe trochu - spawnimy nowy produkt
        // w obszarze zadanym i nadajemy mu prędkość, by wyleciał z maszyny.
        // wygląda ładnie nawet
        if (body)
            body.velocity = transform.forward * 2f;

    }

    #endregion
    void Update () {
        
        //if there are things to produce, produce every ProcessingTime seconds
        if (Queue > 0)
        {
            Timer -= Time.deltaTime;
            if (Timer <= 0f)
            {
                Timer = ProcessingTime;
                Queue--;
                Produce();
            }
        }
	}
}
