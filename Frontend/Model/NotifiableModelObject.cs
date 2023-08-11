
namespace Frontend.Model
{
    /// <summary>
    /// Notifiable object, used to notify the view of changes in the model
    /// </summary>
    public class NotifiableModelObject : NotifiableObject
    {
        /// <summary>
        /// <c>Controller</c> represents the backend controller
        /// </summary>
        public BackendController Controller { get; private set; }

        /// <summary>
        /// Constructor for the <c>NotifiableModelObject</c> class, gets a backend controller
        /// </summary>
        /// <param name="controller"></param>
        protected NotifiableModelObject(BackendController controller)
        {
            this.Controller = controller;
        }
    }
}
