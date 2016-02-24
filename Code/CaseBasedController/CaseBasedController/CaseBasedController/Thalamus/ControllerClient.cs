using Thalamus;

namespace CaseBasedController.Thalamus
{
    public partial class ControllerClient : ThalamusClient
    {
        public const string CLIENT_NAME = "EnercitiesCaseController";
        public const string CASE_POOL_FILE = "casepool.json";
        public const string DEFAULT_CHARACTER_NAME = "";

        static ControllerClient _instance = null;

        public ControllerClient(string character = DEFAULT_CHARACTER_NAME, string clientName = CLIENT_NAME)
            : base(clientName, character)
        {
            this.SetPublisher<IAllActionPublisher>();
            this.ControllerPublisher = new ControllerPublisher(this.Publisher);

            this.showDebugErrors = false;
            _instance = this;
        }

        public static ControllerClient GetInstance()
        {
            return _instance;
        }

        /// <summary>
        ///     The <see cref="IThalamusPublisher" /> used by this client to publish actions to other modules.
        /// </summary>
        public IAllActionPublisher ControllerPublisher { get; private set; }

    }
}