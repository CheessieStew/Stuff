using UnityEngine;
using System.Collections;

public class IngredientBehaviour : MonoBehaviour {
    public enum IngredientKind 
    {
        Egg,
        Flour,
        Dough,
        BasicDonut
    }

    public IngredientKind Kind;

	// Use this for initialization
	void Start () {
	}

    void OnTriggerEnter(Collider other)
    {
        MachineBehaviour machine = other.GetComponent<MachineBehaviour>();
        if (machine)
        {
            machine.NewIngredient(other, this);
        }
    }

    void OnTriggerExit(Collider other)
    {
        MachineBehaviour machine = other.GetComponent<MachineBehaviour>();
        if (machine)
        {
            machine.RemoveIngredient(other, this);
        }
    }

	// Update is called once per frame
	void Update () {
	
	}
}
