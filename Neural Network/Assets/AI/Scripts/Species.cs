using UnityEngine;

public class Species {
    string name;
    Color color;

    public Species() {
        color = Random.ColorHSV();
        name = Random.Range(0, 10000000).ToString("X"); ;
    }

    //get color of the species
    public Color GetColor() {
        return color;
    }

    //get name of the species
    public string GetName() {
        return name;
    }
}
