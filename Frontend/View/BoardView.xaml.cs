using Frontend.Model;
using Frontend.Model.Board;
using Frontend.Model.User;
using Frontend.ViewModel;
using System;
using System.Windows;

namespace Frontend.View
{
    /// <summary>
    /// Interaction logic for BoardView.xaml
    /// </summary>
    public partial class BoardView : Window
    {
        /// <value>
        /// Field <c>boardViewModel</c> performs logic for the board view (see <see cref="BoardViewModel"/>)
        /// </value>
        private BoardViewModel boardViewModel;

        /// <summary>
        /// <c>BoardView</c> constructor
        /// </summary>
        /// <param name="backendController">The backend controller used to interact with the backend</param>
        /// <param name="board">The current board</param>
        /// <param name="user">The user logged in the system</param>
        public BoardView(BackendController backendController, BoardModel board, UserModel user)
        {
            InitializeComponent();
            try
            {
                this.boardViewModel = new BoardViewModel(backendController, board, user);
                this.DataContext = this.boardViewModel;
            }
            catch (Exception exception)
            {
                this.displayError(exception.Message);
                this.Close();
            }
        }

        /// <summary>
        /// This method is the event handler for the close button click. It closes the window
        /// </summary>
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
