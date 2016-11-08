#if UNITY_5 && !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 && !UNITY_5_3_0 && !UNITY_5_3_1 && !UNITY_5_3_2
#define UNITY_HAS_VRCLASS
#endif

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class SphereDemo : MonoBehaviour
{
    private static string savedDataPath = Application.persistentDataPath + "/savedData";
    private static string dictionaryName = "Rotations.txt";
    private static string dictionaryFullName = savedDataPath + "/" + dictionaryName;

    void Start ()
	{
#if UNITY_HAS_VRCLASS
		if (UnityEngine.VR.VRDevice.isPresent) {
			return;
		}
#endif
		if (SystemInfo.supportsGyroscope) {
			Input.gyro.enabled = true;
			this.transform.parent.Rotate (new Vector3 (90f, 0f, 0f));
		}
	}


	void OnDestroy ()
	{
		if (SystemInfo.supportsGyroscope) {
			Input.gyro.enabled = false;
		}
	}

	private float _spinX;
	private float _spinY;
    Dictionary<string, Quaternion> rotations = new Dictionary<string, Quaternion> ();

    void Update ()
	{
        Debug.Log(Application.persistentDataPath);
#if UNITY_HAS_VRCLASS
		if (UnityEngine.VR.VRDevice.isPresent) {
            var rotation = UnityEngine.VR.InputTracking.GetLocalRotation (UnityEngine.VR.VRNode.CenterEye);
            rotations.Add (Time.frameCount.ToString (), rotation);
            
//			print ("x: " + angles.eulerAngles.x + " y: " + angles.eulerAngles.y + " z: " + angles.eulerAngles.z);
			// Mouse click translates to gear VR touch to reset view
			if (Input.GetMouseButtonDown (0) || Input.GetKeyDown (KeyCode.Space)) {
				UnityEngine.VR.InputTracking.Recenter ();
			}

			if (Input.GetKeyDown (KeyCode.V)) {
				UnityEngine.VR.VRSettings.enabled = !UnityEngine.VR.VRSettings.enabled;
			}

			if (Input.GetKeyDown (KeyCode.Q)) {
				#if UNITY_EDITOR
				//foreach (KeyValuePair<string, Quaternion> r in rotations) {
				//	//display each product to console by using Display method in Farm Shop class
				//	print ("SWIS frame: " + r.Key + " rotation: " + r.Value.eulerAngles);

				//}

                // Save the dictionary to a file
                if (rotations != null)
                {
                    string fileContent = "";
                    foreach (var item in rotations)
                    {
                        fileContent += item.Key + "," + item.Value + "\n";
                    }

                    File.WriteAllText(dictionaryFullName, fileContent);
                }
				UnityEditor.EditorApplication.isPlaying = false;
				#else
				Application.Quit();
				#endif
			}
		} else
#endif
		{
			if (SystemInfo.supportsGyroscope) {
				// Invert the z and w of the gyro attitude
				this.transform.localRotation = new Quaternion (Input.gyro.attitude.x, Input.gyro.attitude.y, -Input.gyro.attitude.z, -Input.gyro.attitude.w);
			}
			// Also rotate from mouse / touch input
			else if (Input.GetMouseButton (0)) {
				float h = 40.0f * -Input.GetAxis ("Mouse X") * Time.deltaTime;
				float v = 40.0f * Input.GetAxis ("Mouse Y") * Time.deltaTime;
				h = Mathf.Clamp (h, -0.5f, 0.5f);
				v = Mathf.Clamp (v, -0.5f, 0.5f);
				_spinX += h;
				_spinY += v;
			}
			if (!Mathf.Approximately (_spinX, 0f) || !Mathf.Approximately (_spinY, 0f)) {
				this.transform.Rotate (Vector3.up, _spinX);
				this.transform.Rotate (Vector3.right, _spinY);

				_spinX = Mathf.MoveTowards (_spinX, 0f, 5f * Time.deltaTime);
				_spinY = Mathf.MoveTowards (_spinY, 0f, 5f * Time.deltaTime);
			}
		}
	}
}
