using System.ComponentModel;

namespace Frontend
{
    /// <summary>
    /// Abstract class <c>NotifiableObject</c> represents objects whose
    /// fields and properties are bound, thus enabling notifing on changes
    /// </summary>
    public abstract class NotifiableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// This method invokes a property of the class has been changed
        /// </summary>
        /// <param name="property">The name of the changed property</param>
        protected void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this,
                new PropertyChangedEventArgs(property));
        }
    }
}
