using UnityEngine;

public class TestTriggerAutoSave : MonoBehaviour
{
    [SerializeField] private bool autoSave = true;
    [SerializeField] private float autoSaveTime_Max = 60.0f * 60.0f * 15.0f;//fifteen minutes
    [SerializeField] private float autoSaveTime = 60.0f * 60.0f * 15.0f;//fifteen minutes
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        if (!autoSave)
        {
            return;
        }

        autoSaveTime -= Time.deltaTime;

        if (autoSaveTime <= 0)
        {
            Debug.Log("Triggering Auto Save!");
            SaveManager.AutoSave(GameDataManager.GetGameData());
            autoSaveTime = autoSaveTime_Max;
        }
    }
}
