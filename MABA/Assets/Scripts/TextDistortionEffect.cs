using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextDistortionEffect : MonoBehaviour
{
    public float maxDistortionStrength = 5.0f; 
    public float transitionDuration = 3.0f; 
    public float frequency = 5.0f;
    public float amplitude = 0.5f; 
    public float delay = 2.0f; 

    private TextMeshProUGUI textMeshPro;
    private Mesh mesh;
    private Vector3[] vertices;
    private float distortionStrength = 1.0f; 

    void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        textMeshPro.ForceMeshUpdate(); 
        mesh = textMeshPro.mesh;
        vertices = mesh.vertices;

        StartCoroutine(StartDistortionAfterDelay());
    }

    void Update()
    {
            DistortText();
    }

    IEnumerator StartDistortionAfterDelay()
    {
        yield return new WaitForSeconds(delay);

        StartCoroutine(IncreaseDistortionStrength());
    }

    IEnumerator IncreaseDistortionStrength()
    {
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            distortionStrength = Mathf.Lerp(1.0f, maxDistortionStrength, elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        distortionStrength = maxDistortionStrength;
    }

    void DistortText()
    {
        textMeshPro.ForceMeshUpdate();
        mesh = textMeshPro.mesh;
        vertices = mesh.vertices;

        for (int i = 0; i < textMeshPro.textInfo.characterCount; i++)
        {
            if (!textMeshPro.textInfo.characterInfo[i].isVisible)
                continue;

            int vertexIndex = textMeshPro.textInfo.characterInfo[i].vertexIndex;

            Vector3 offset = WavyOffset(Time.time + i);

            vertices[vertexIndex + 0] += offset;
            vertices[vertexIndex + 1] += offset;
            vertices[vertexIndex + 2] += offset;
            vertices[vertexIndex + 3] += offset;
        }

        mesh.vertices = vertices;
        textMeshPro.canvasRenderer.SetMesh(mesh);
    }

    Vector3 WavyOffset(float time)
    {
        float xOffset = Mathf.Sin(time * frequency) * amplitude * distortionStrength;
        float yOffset = Mathf.Cos(time * frequency) * amplitude * distortionStrength;
        return new Vector3(xOffset, yOffset, 0);
    }
}
