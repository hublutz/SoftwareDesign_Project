using Frontend.Model;
using Frontend.Model.User;
using Frontend.ViewModel;
using System;
using System.Windows;



namespace Frontend.View
{
    /// <summary>
    /// The login window xaml cs file
    /// </summary>
    public partial class LoginView : Window
    {
        /// <summary>
        /// the UserViewModel we use to forword the actions to
        /// </summary>
        private UserViewModel viewModel;
        /// <summary>
        /// The constractor of LoginWindow
        /// </summary>
        /// <param name="backendController">the backend controller we forrword to the backendController</param>
        public LoginView()
        {
            InitializeComponent();
            try
            {
                this.viewModel = new UserViewModel(new BackendController());
                this.DataContext = viewModel;
            }
            catch (Exception exception)
            {
                this.displayError(exception.Message);
                this.Close();
            }

        }

        /// <summary>
        /// <c>LoginView</c> constructor which receives the backend controller
        /// </summary>
        /// <param name="backendController">Backend communicates with the backend to make operations</param>
        public LoginView(BackendController backendController)
        {
            InitializeComponent();
            this.viewModel = new UserViewModel(backendController);
            this.DataContext = this.viewModel;
        }

        /// <summary>
        /// The function that hapends when the loggin button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Login_Click(object sender, RoutedEventArgs e)
        {
           var user = viewModel.Login();
           if (user != null)
           {
               this.openDashboardWindow(user);
           }  
          
        }
        /// <summary>
        /// he function that hapends when the register button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Register_Click(object sender, RoutedEventArgs e)
        {
           var user = viewModel.Register();
            if (user != null)
            {
                this.openDashboardWindow(user);
            }
        }

        /// <summary>
        /// This method opens the Dashboard window for the given user
        /// </summary>
        /// <param name="user">The user logged in the system</param>
        private void openDashboardWindow(UserModel user)
        {
            DashboardView nextWindow = new DashboardView(this.viewModel.BackendController, user);
            nextWindow.Show();
            this.Close();
        }
       
        /// <summary>
        /// A function that displays a MessageBoxResult in case of error
        /// </summary>
        /// <param name="errorMessage">the error massege to display</param>
        /// <returns>The MessageBoxResult</returns>
        private MessageBoxResult displayError(string errorMessage)
        {
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage image = MessageBoxImage.Error;
            MessageBoxResult result = MessageBox.Show(errorMessage, "Operation Failed",
                button, image);
            return result;
        }
    }
}
