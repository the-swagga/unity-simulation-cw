using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] private Light source;
    [SerializeField] private float[] startTimeRange = new float[2];
    [SerializeField] private float cycleTime;
    private float timeNow;

    private void Start()
    {
        timeNow = Random.Range(startTimeRange[0], startTimeRange[1]);
    }

    void Update()
    {
        timeNow += (24.0f / cycleTime) * Time.deltaTime;

        if (timeNow >= 24.0f) timeNow = 0.0f;

        float sourceAngle = (timeNow / 24.0f) * 360.0f - 90.0f;
        source.transform.rotation = Quaternion.Euler(sourceAngle, 180.0f, 0.0f);
    }
}
