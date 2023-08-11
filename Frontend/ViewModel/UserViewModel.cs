using Frontend.Model;
using Frontend.Model.User;
using System;

namespace Frontend.ViewModel
{
    public class UserViewModel : NotifiableObject
    {
        private readonly string ERROR_MESSAGE_CONST = "OperationMessage";

        /// <summary>
        /// The UserViewModel constractor
        /// </summary>
        /// <param name="backendController"></param>
        public UserViewModel(BackendController backendController) {
            this.BackendController = backendController;
            this.email = string.Empty; 
            this.password = string.Empty;
            operationMassage = string.Empty;
        }

        /// <summary>
        /// The backendController used to forward  
        /// </summary>
        public BackendController BackendController { get; set; }
        private string email;
        public string Email { get => email; set 
            { 
                this.email = value;
            }
        }
        /// <summary>
        /// the password field
        /// </summary>
        private string password;
        /// <summary>
        /// pasword getters and setters
        /// </summary>
        public string Password { get=> password; 
            set {
                this.password = value;
            } 
        }

        private string operationMassage;

        /// <summary>
        /// OperationMessage getter and setters
        /// </summary>
        public string OperationMessage
        {
            get => operationMassage; set
            {
                operationMassage = value;
                RaisePropertyChanged(ERROR_MESSAGE_CONST);
            }
        }

        /// <summary>
        /// Register function forwards to ModelControler
        /// </summary>
        /// <returns> UserModel if operation was successful and null if not</returns>
        public UserModel Register()
        {
            try{
                UserModel ret = BackendController.UserController.Register(Email, Password);
                OperationMessage = string.Empty;
                return ret;
            }catch (Exception e) {
                OperationMessage = e.Message;
                return null;
            }

            
        }
        /// <summary>
        /// Register function forwards to ModelControler
        /// </summary>
        /// <returns>UserModel if operation was successful and null if not</returns>
        public UserModel Login() {
            try
            {
                UserModel ret = BackendController.UserController.Login(Email, Password);
                OperationMessage = string.Empty;
                return ret;
            }
            catch (Exception e) {
                OperationMessage = e.Message;
                return null;
            }
        }
    }
}
