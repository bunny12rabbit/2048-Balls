using UnityEngine;

public class ReporterGUI : MonoBehaviour
{
	private Reporter _reporter;

	private void Awake()
	{
		_reporter = gameObject.GetComponent<Reporter>();
	}

	private void OnGUI()
	{
		_reporter.OnGuiDraw();
	}
}
