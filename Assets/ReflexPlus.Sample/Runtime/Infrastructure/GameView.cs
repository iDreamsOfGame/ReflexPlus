using ReflexPlus.Attributes;
using ReflexPlus.Sample.Application;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ReflexPlus.Sample.Infrastructure
{
    internal class GameView : MonoBehaviour
    {
        [FormerlySerializedAs("_progressText")]
        [SerializeField]
        private Text progressText;

        [FormerlySerializedAs("_resetButton")]
        [SerializeField]
        private Button resetButton;

        [Inject]
        private readonly ICollectionStorage collectionStorage;

        private void Start()
        {
            resetButton.onClick.AddListener(Reset);
        }

        private void Update()
        {
            progressText.text = $"{collectionStorage.Count()} / 4";
        }

        private void Reset()
        {
            collectionStorage.Clear();
            SceneManager.LoadScene("ReflexPlus.Sample");
        }
    }
}