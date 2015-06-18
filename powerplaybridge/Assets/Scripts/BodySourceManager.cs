using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class BodySourceManager : MonoBehaviour {

    private KinectSensor sensor;
    private BodyFrameReader reader;
    private Body[] data = null;

    public Body[] GetData()
    {
        return data;
    }

	// Use this for initialization
	void Start () {
        sensor = KinectSensor.GetDefault();

        if (sensor != null)
        {
            reader = sensor.BodyFrameSource.OpenReader();

            if (!sensor.IsOpen)
            {
                sensor.Open();
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (reader != null)
        {
            var frame = reader.AcquireLatestFrame();
            if (frame != null)
            {
                if (data == null)
                {
                    data = new Body[sensor.BodyFrameSource.BodyCount];
                }

                frame.GetAndRefreshBodyData(data);

                frame.Dispose();
                frame = null;
            }
        }
	}

    void OnApplicationQuit()
    {
        if (reader != null)
        {
            reader.Dispose();
            reader = null;
        }

        if (sensor != null)
        {
            if (sensor.IsOpen)
            {
                sensor.Close();
            }

            sensor = null;
        }
    }
}
