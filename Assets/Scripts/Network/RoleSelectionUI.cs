/*using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

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

    private async void OnTherapistClicked()
    {
        Debug.Log("Therapist selected → Start Host");
        await networkManager.StartHost();
        gameObject.SetActive(false);
    }

    private async void OnPatientClicked()
    {
        Debug.Log("Patient selected → Join Session");
        await networkManager.JoinSession();
        gameObject.SetActive(false);
    }
}*/