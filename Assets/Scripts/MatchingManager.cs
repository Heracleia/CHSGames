using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchingManager : MonoBehaviour {

    public Transform matchTo;
    public Transform matchFrom;
    public GameObject[] shapes;
    public Color[] colors;

    List<GameObject> currentSet;
    bool active;

    private void Start() {
        currentSet = new List<GameObject>();
        CreateSet();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space))
            CreateSet();
    }

    //Create the set of shapes
    void CreateSet() {
        while(currentSet.Count > 0) {
            GameObject temp = currentSet[0];
            currentSet.Remove(temp);
            Destroy(temp);
        }

        List<GameObject> tempShapes = new List<GameObject>();
        List<Color> tempColors = new List<Color>();

        tempShapes.AddRange(shapes);
        tempColors.AddRange(colors);

        System.Random rng = new System.Random(Random.Range(0, 100000));
        GameObject correctShape = tempShapes[rng.Next(tempShapes.Count)];
        Color correctColor = tempColors[rng.Next(tempColors.Count)];
        tempShapes.Remove(correctShape);
        tempColors.Remove(correctColor);

        for(float x = -6f; x <= 2f; x += 4f) {
            GameObject temp = Instantiate(correctShape, matchTo);
            temp.transform.localPosition += Vector3.right * x;
            foreach (ParticleSystem particle in temp.GetComponentsInChildren<ParticleSystem>()) {
                ParticleSystem.MainModule main = particle.main;
                main.startColor = correctColor;
            }
            currentSet.Add(temp);
        }

        float correctX = 4f * rng.Next(0, 2) - 4f;
        for(float x = -4f; x <= 4f; x += 4f) {
            if (x == correctX) {
                GameObject temp = Instantiate(correctShape, matchFrom);
                temp.transform.localPosition += Vector3.right * x;
                foreach (ParticleSystem particle in temp.GetComponentsInChildren<ParticleSystem>()) {
                    ParticleSystem.MainModule main = particle.main;
                    main.startColor = correctColor;
                }
                GameShape gameShape = temp.GetComponent<GameShape>();
                gameShape.OnClicked += CorrectClick;
                gameShape.OnHoverEnter += ShapeHoverEnter;
                gameShape.OnHoverExit += ShapeHoverExit;
                currentSet.Add(temp);
            }
            else {
                GameObject temp = Instantiate(tempShapes[rng.Next(tempShapes.Count)], matchFrom);
                temp.transform.localPosition += Vector3.right * x;
                Color color = tempColors[rng.Next(tempColors.Count)];
                foreach (ParticleSystem particle in temp.GetComponentsInChildren<ParticleSystem>()) {
                    ParticleSystem.MainModule main = particle.main;
                    main.startColor = color;
                }
                GameShape gameShape = temp.GetComponent<GameShape>();
                gameShape.OnClicked += IncorrectClick;
                gameShape.OnHoverEnter += ShapeHoverEnter;
                gameShape.OnHoverExit += ShapeHoverExit;
                currentSet.Add(temp);
            }
        }

        active = true;
    }

    void CorrectClick(GameObject sender) {
        if (!active) return;
        StartCoroutine(CorrectSequence(sender));
    }

    void IncorrectClick(GameObject sender) {
        if (!active) return;
        Debug.Log("Incorrect");
    }

    void ShapeHoverEnter(GameObject sender) {
        if (!active) return;
        foreach(ParticleSystem particle in sender.GetComponentsInChildren<ParticleSystem>()) {
            particle.Emit((int)particle.emission.rateOverTimeMultiplier * 2);
        }
    }

    void ShapeHoverExit(GameObject sender) {
        
    }

    IEnumerator CorrectSequence(GameObject sender) {
        active = false;
        sender.transform.parent = matchTo;
        Vector2 target = new Vector2(6, 0);
        while((Vector2)sender.transform.localPosition != target) {
            sender.transform.localPosition = Vector3.MoveTowards(sender.transform.localPosition, target, 0.25f);
            yield return new WaitForSeconds(.01f);
        }
    }
}
