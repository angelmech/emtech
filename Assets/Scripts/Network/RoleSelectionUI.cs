using UnityEngine;
using UnityEngine.UI;

public class RoleSelectionUI : MonoBehaviour
{
    public Button therapistButton;
    public Button patientButton;
    public NetworkManager networkManager;

    void Start()
    {
        therapistButton.onClick.AddListener(OnTherapistClicked);
        patientButton.onClick.AddListener(OnPatientClicked);
    }

    void OnTherapistClicked()
    {
        Debug.Log("Therapist selected → Start Host");
        networkManager.StartHost();
        gameObject.SetActive(false);
    }

    void OnPatientClicked()
    {
        Debug.Log("Patient selected → Join Session");
        networkManager.JoinSession();
        gameObject.SetActive(false);
    }
}