using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSonarReplacementMain : MonoBehaviour
{
    // Throwaway values to set position to at the start.
    private static readonly Vector4 GarbagePosition = new Vector4(-5000, -5000, -5000, -5000);
    private static readonly Vector4 White = new Vector4(1, 1, 1, 1);

    // The number of rings that can be rendered at once.
    // Must be the samve value as the array size in the shader.
    private static int QueueSize = 20;

    // Queue of start positions of sonar rings.
    // The xyz values hold the xyz of position.
    // The w value holds the time that position was started.
    private Queue<Vector4> positionsQueue = new Queue<Vector4>(QueueSize);

    // Queue of intensity values for each ring.
    // These are kept in the same order as the positionsQueue.
    private Queue<float> intensityQueue = new Queue<float>(QueueSize);

    // Queue of color values for each ring.
    // These are kept in the same order as the positionsQueue.
    private Queue<Vector4> colorQueue = new Queue<Vector4>(QueueSize);

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < QueueSize; i++)
        {
            positionsQueue.Enqueue(GarbagePosition);
            intensityQueue.Enqueue(-5000f);
            colorQueue.Enqueue(White);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Starts a sonar ring from this position with the given intensity.
    /// </summary>
    public void StartSonarRing(Vector4 position, float intensity, Vector4 color)
    {
        // Put values into the queue
        position.w = Time.timeSinceLevelLoad;
        positionsQueue.Dequeue();
        positionsQueue.Enqueue(position);

        intensityQueue.Dequeue();
        intensityQueue.Enqueue(intensity);

        colorQueue.Dequeue();
        colorQueue.Enqueue(color);

        List<Vector4> positions = new List<Vector4>();
        positions.Add(position);
        List<float> intensities = new List<float>();
        intensities.Add(intensity);
        List<Vector4> colors = new List<Vector4>();
        colors.Add(color);

        Shader.SetGlobalVector("_RingColor", new Vector4(1,1,1,1));
        Shader.SetGlobalFloat("_RingColorIntensity", 2f);
        Shader.SetGlobalFloat("_RingSpeed", 4f);
        Shader.SetGlobalFloat("_RingWidth", 1f);
        Shader.SetGlobalFloat("_RingGradiant", 0f);
        Shader.SetGlobalFloat("_RingIntensityScale", 1f);
        Shader.SetGlobalFloat("_Glossiness", 0.5f);
        Shader.SetGlobalFloat("_Metallic", 0f);
        Shader.SetGlobalFloat("_Darkness", 1f);
        Shader.SetGlobalVectorArray("_hitPts", positionsQueue.ToArray());
        Shader.SetGlobalFloatArray("_Intensity", intensityQueue.ToArray());
        Shader.SetGlobalVectorArray("_ringColors", colorQueue.ToArray());
    }
}
