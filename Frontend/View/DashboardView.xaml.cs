using Frontend.Model.Board;
using Frontend.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Frontend.Model.User;
using Frontend.Model;
using System.ComponentModel;
using System;

namespace Frontend.View
{
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView : Window
    {
        private DashboardViewModel dashboardViewModel;

        /// <summary>
        /// Constructor for the dashboard view, initalizes the components and sets the data context
        /// </summary>
        /// <param name="backendController">The controller</param>
        /// <param name="userModel">The user whos logged in and whos board we will show</param>
        public DashboardView(BackendController backendController, UserModel userModel)
        {
            InitializeComponent();
            try
            {
                this.dashboardViewModel = new DashboardViewModel(backendController, userModel);
            }
            catch (Exception exc)
            {
                this.displayError(exc.Message);
                this.Close();
            }
            this.DataContext = dashboardViewModel;
        }

        /// <summary>
        /// This method opens the board view when a board is clicked
        /// </summary>
        /// <param name="sender">The clicked item</param>
        /// <param name="e">State of the mouse button</param>
        private void ListViewItem_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var board = sender as ListViewItem;
            if (board != null && board.IsSelected)
            {
                BoardView boardView = new BoardView(
                    dashboardViewModel.BackendController,
                    board.DataContext as BoardModel,
                    dashboardViewModel.UserModel);
                this.Hide();
                boardView.ShowDialog();
                this.Show();
            }
        }

        /// <summary>
        /// Logs out the user and closes the window.
        /// </summary>
        /// <param name="sender">The clicked item</param>
        /// <param name="e">The event arguements</param>
        private void Logout_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Handles the closing event of the window.
        /// </summary>
        /// <param name="sender">The clicked item</param>
        /// <param name="e">The even arguements</param>
        private void DashboardView_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                dashboardViewModel.Logout();
                LoginView loginView = new LoginView(this.dashboardViewModel.BackendController);
                loginView.Show();
            }
            catch (Exception exc)
            {
                displayError(exc.Message);
            }
        }


        /// <summary>
        /// This method displays an error message box to the user
        /// </summary>
        /// <param name="errorMessage">The error to present</param>
        /// <returns>Returns the result of the message box</returns>
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
