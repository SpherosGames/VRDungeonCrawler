using UnityEngine.Assertions;

namespace UnityEngine.XR.Interaction.Toolkit.Filtering
{
    public class XRSocketInteractorFilter : MonoBehaviour, IXRHoverFilter, IXRSelectFilter
    {
        #region Fields

        [SerializeField]
        private XRSocketInteractor m_interactor;

        [SerializeField]
        private bool m_locked = false;

        #endregion

        #region Properties

        public bool locked { get => m_locked; set => m_locked = value; }

        public bool canProcess => true;

        #endregion

        #region Methods

        private void Awake()
        {
            m_interactor = m_interactor ?? GetComponent<XRSocketInteractor>();
            Assert.IsNotNull(m_interactor);
        }

        private void OnEnable()
        {
            m_interactor.selectEntered.AddListener(Socket_SelectEntered);
            m_interactor.selectExited.AddListener(Socket_SelectExited);
        }

        private void OnDisable()
        {
            m_interactor.selectEntered.RemoveListener(Socket_SelectEntered);
            m_interactor.selectExited.RemoveListener(Socket_SelectExited);
        }

        private void Socket_SelectEntered(SelectEnterEventArgs e)
        {
            var interactable = e.interactableObject.transform.GetComponent<XRBaseInteractable>();
            if (interactable == null)
                return;

            // Add filter to interactable
            interactable.hoverFilters.Add(this);
            interactable.selectFilters.Add(this);
        }

        private void Socket_SelectExited(SelectExitEventArgs e)
        {
            var interactable = e.interactableObject.transform.GetComponent<XRBaseInteractable>();
            if (interactable == null)
                return;

            // Remove filter from interactable
            interactable.hoverFilters.Remove(this);
            interactable.selectFilters.Remove(this);
        }

        public bool Process(IXRHoverInteractor interactor, IXRHoverInteractable interactable)
        {
            return Process();
        }

        public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
        {
            return Process();
        }

        private bool Process()
        {
            if (m_interactor == null)
                return false;

            return !m_locked;
        }

        #endregion
    }
}