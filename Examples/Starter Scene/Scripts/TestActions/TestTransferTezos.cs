using System.Collections;
using System.Text.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tezos.StarterScene
{
    public class TestTransferTezos : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private Button _button;
        [SerializeField] private Button _hyperlinkButton;
        [SerializeField] private TextMeshProUGUI _resultText;
        [SerializeField] private TMP_InputField _inputFieldAddress;
        [SerializeField] private TMP_InputField _inputFieldAmount;

        private void Start()
        {
            _button.onClick.AddListener(OnButtonClicked);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            _resultText.text = "Requested.";
            _hyperlinkButton.interactable = false;

            string toAddress = _inputFieldAddress.text;
            ulong amount = ulong.Parse(_inputFieldAmount.text);

            TezosManager.Instance.MessageReceiver.ContractCallInjected += OnContractCallInjected;
            TezosManager.Instance.RequestTransferTezos(toAddress, amount);
        }
        
        private void OnContractCallInjected(string transaction)
        {
            TezosManager.Instance.MessageReceiver.ContractCallInjected -= OnContractCallInjected;
            var json = JsonSerializer.Deserialize<JsonElement>(transaction);
            var transactionHash = json.GetProperty("transactionHash").GetString();
            IEnumerator routine = TezosManager.Instance.TrackTransaction(transactionHash, result =>
            {
                if (result.success)
                {
                    _resultText.text = result.transactionHash;
                    _hyperlinkButton.interactable = true;
                }
                else
                {
                    _resultText.text = "Failed.";
                }
            });
            TezosManager.Instance.StartCoroutine(routine);
            _resultText.text = "Pending...";
        }
    }
}
