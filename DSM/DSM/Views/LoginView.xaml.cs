using DSMData;
using DSMData.Model;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DSM.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public ICommand LoginCommand { get; set; }

        public DSMModelData objDSMModelData;
        public UserDisplayModel objUser;

        private string errorMsg;
        public string ErrorMsg
        {
            get { return errorMsg; }
            set { errorMsg = value; }
        }
        public LoginView()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //if (System.Environment.MachineName == "SERVER")
            //{
            //    if (string.IsNullOrEmpty(txtUserName.Text) || string.IsNullOrEmpty(txtPassword.ToString()))
            //    {
            //        //ErrorMsg = "UserName or Password is required.";
            //        //return;
            //        txtUserName.Text = "apipost";
            //        //PasswordBox passwordBox = new PasswordBox();
            //        ////Password = passwordBox.Password;
            //        //passwordBox.Password = "HI";
            //        //txtPassword = "API@123";
            //        txtPassword.Password = "API@123";
            //        //var passwordBox = sender as PasswordBox;
            //        //passwordBox.Password = "API@123";
            //        //Password = "API@123";
            //        LoginAction();
            //    }
            //}
        }

        public void LoginAction()
        {

            // If no operation specified, return
            //if (sender == null) return;

            //var passwordBox = sender as PasswordBox;
            //Password = passwordBox.Password;

            //if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
            if (string.IsNullOrEmpty(txtUserName.Text) || string.IsNullOrEmpty(txtPassword.ToString()))
            {
                ErrorMsg = "UserName or Password is required.";
                return;
            }
            else
            {
                try
                {
                    objDSMModelData = new DSMModelData();
                    objUser = new UserDisplayModel();
                    objUser = objDSMModelData.GetUserType(txtUserName.Text, txtPassword.Password.ToString());
                    if (!string.IsNullOrEmpty(objUser.UserType))
                    {
                        //Assign value to global variables
                        Global.UserId = objUser.UserId;
                        Global.UserName = objUser.UserName;
                        Global.UserType = objUser.UserType;
                        //Global.UserIsActive = objUser.UserIsActive;

                        var loginWondow = App.Current.MainWindow;
                        Bootstrapper bootstrapper = new Bootstrapper();
                        bootstrapper.Run();
                        loginWondow.Close();
                    }
                    else
                    {
                        ErrorMsg = "Invalid UserName or Password.";
                        return;
                    }
                }
                catch (Exception ex)
                {
                    ErrorMsg = "Invalid credential." + ex.Message;
                }
            }
        }
    }
}
