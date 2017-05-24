using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchingManager : MonoBehaviour {

    public Transform matchTo;
    public Transform matchFrom;
    public GameObject[] shapes;
    public Color[] colors;
    public Image starPowerImage;
    public CanvasGroup helpText;

    List<GameObject> currentSet;
    float starPower;
    bool active;

    private void Start() {
        currentSet = new List<GameObject>();
        CreateSet();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space))
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    private void OnGUI() {
        starPowerImage.fillAmount = starPower;
    }

    //Create the set of shapes
    void CreateSet() {
        StartCoroutine(FadeHelpText(1));

        while (currentSet.Count > 0) {
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
                int matchWhich = rng.Next(0, 1);
                GameObject temp = Instantiate(matchWhich == 0 ? correctShape : shapes[rng.Next(tempShapes.Count)], matchFrom);
                temp.transform.localPosition += Vector3.right * x;
                Color color = matchWhich == 1 ? correctColor : colors[rng.Next(tempColors.Count)];
                foreach (ParticleSystem particle in temp.GetComponentsInChildren<ParticleSystem>()) {
                    ParticleSystem.MainModule main = particle.main;
                    main.startColor = color;
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
        StartCoroutine(IncorrectSequence(sender));
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
        StartCoroutine(FadeHelpText(0));
        sender.transform.parent = matchTo;
        Vector2 target = new Vector2(6, 0);
        while((Vector2)sender.transform.localPosition != target) {
            sender.transform.localPosition = Vector3.MoveTowards(sender.transform.localPosition, target, 0.25f);
            yield return new WaitForSeconds(.01f);
        }
        yield return new WaitForSeconds(0.5f);
        foreach (GameObject obj in currentSet) {
            foreach (ParticleSystem particle in obj.GetComponentsInChildren<ParticleSystem>()) {
                particle.Emit(50);
                particle.Stop();
            }
        }
        float targetStarPower = Mathf.Clamp01(starPower + 0.1f);
        while(starPower < targetStarPower) {
            starPower += 0.001f;
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(1f);
        if (starPower >= 1) {

        }
        else {
            CreateSet();
        }
    }

    IEnumerator IncorrectSequence(GameObject sender) {
        active = false;
        StartCoroutine(FadeHelpText(0));
        float targetStarPower = Mathf.Clamp01(starPower - .05f);
        foreach (GameObject obj in currentSet) {
            foreach (ParticleSystem particle in obj.GetComponentsInChildren<ParticleSystem>()) {
                particle.Stop();
            }
        }
        while (starPower > targetStarPower) {
            starPower -= 0.001f;
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(1f);
        CreateSet();
    }

    IEnumerator FadeHelpText(float alpha) {
        while(helpText.alpha < alpha) {
            helpText.alpha += .01f;
            yield return new WaitForSeconds(.01f);
        }
        while(helpText.alpha > alpha) {
            helpText.alpha -= .01f;
            yield return new WaitForSeconds(.01f);
        }
    }
}
