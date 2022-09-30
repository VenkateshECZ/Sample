using DSMData;
using DSMData.Model;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Net;
using System.Data.SqlClient;

namespace DSM.ViewModels
{
    public class ASNViewModel : BindableBase, INotifyPropertyChanged
    {
        public ICommand CreateClientCommand { get; set; }
        public ICommand CreateASNCommand { get; set; }
        public ICommand DeleteCustomerKeyCommand { get; set; }
        public ICommand EditCustomerKeyCommand { get; set; }
        DSMModelData objDSMModelData;

        private IRPSettingsDisplayModel objIRPSettings;
        public IRPSettingsDisplayModel IRPSettings
        {
            get { return objIRPSettings; }
            set { SetProperty(ref objIRPSettings, value); }
        }
        private APImasterDisplayModel selected_Client;
        public APImasterDisplayModel Selected_Client
        {
            get { return selected_Client; }
            set { SetProperty(ref selected_Client, value); }
        }
        private APImasterDisplayModel asn_master;
        public APImasterDisplayModel ASNMASTER
        {
            get { return asn_master; }
            set { SetProperty(ref asn_master, value); }
        }
        private string password;
        public string Password
        {
            get { return password; }
            set { SetProperty(ref password, value); }
        }


        private ObservableCollection<APImasterDisplayModel> listIRPCredentials;
        public ObservableCollection<APImasterDisplayModel> ListIRPCredentials
        {
            get { return listIRPCredentials; }
            set { SetProperty(ref listIRPCredentials, value); }
        }
        public ASNViewModel()
        {
            ASNMASTER = new APImasterDisplayModel();
            //CreateClientCommand = new DelegateCommand(CreateClient);
            CreateClientCommand = new DelegateCommand<object>(CreateClient);
            CreateASNCommand = new DelegateCommand<object>(CreateASN);

            DeleteCustomerKeyCommand = new DelegateCommand(DeleteCustomer);
            //EditCustomerKeyCommand = new DelegateCommand(EditCustomer);
            EditCustomerKeyCommand = new DelegateCommand<object>(EditCustomer);
            objDSMModelData = new DSMModelData();
            //ListDS = new ObservableCollection<DSMasterDisplayModel>();
            //ListDS = new ObservableCollection<DigitalSignDetailDisplayModel>();
            // ListCustomer = new ObservableCollection<IRPSettingsDisplayModel>(objDSMModelData.GetAllCustomers());
            //ListIRPCredentials = new ObservableCollection<APImasterDisplayModel>(objDSMModelData.ASN_API());
            ListIRPCredentials = new ObservableCollection<APImasterDisplayModel>(objDSMModelData.ASN_API());
            //ASNMASTER.IP_Address = myIP;
        }
        private void CreateASN(object sender)
        {
            if (sender == null || sender.ToString().Trim() == "" || sender.ToString().Trim().Length <= 0) return;

            var passwordBox = sender as PasswordBox;
            Password = passwordBox.Password;

            if (ASNMASTER.Auth != null && ASNMASTER.ClientSecret != null && ASNMASTER.UserName != null)
            {
                if (ASNMASTER.Auth != "" && ASNMASTER.Auth.Length > 0)
                {

                    ASNMASTER.Password = Password;
                    objDSMModelData.SaveASNSettings(ASNMASTER);


                    ASNMASTER = new APImasterDisplayModel();
                    ListIRPCredentials = new ObservableCollection<APImasterDisplayModel>(objDSMModelData.ASN_API());


                }
                else
                {
                    ListIRPCredentials = new ObservableCollection<APImasterDisplayModel>(objDSMModelData.ASN_API());
                }
            }
            else
            {
                ListIRPCredentials = new ObservableCollection<APImasterDisplayModel>(objDSMModelData.ASN_API());
                MessageBox.Show("Please Enter Mandatory Fields");
            }
        }
        private void EditCustomer(object sender)
        {

            if (selected_Client != null)
            {
                //IRPSettingsDisplayModel IRPSettings = new IRPSettingsDisplayModel();
                ASNMASTER = selected_Client;

                ASNMASTER.Client_ID = selected_Client.Client_ID;
                ASNMASTER.Auth = selected_Client.Auth;
                ASNMASTER.Password = selected_Client.Password;
            }
        }
        private void DeleteCustomer()
        {
            if (selected_Client != null)
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure, Do you want to Delete it?", "Delete Confirmation", System.Windows.MessageBoxButton.OKCancel);
                if (messageBoxResult == MessageBoxResult.OK)
                {
                    string cl = selected_Client.Client_ID;
                    //DELETE_QUERY
                    string _sQuery = "";
                    _sQuery = "DELETE FROM ASN_API_MASTER WHERE client_id = ''"+cl+"";
                    SqlDataReader drr1 = Utilities.executeQuery(_sQuery);


                    //int cid= Convert.ToInt32(selected_Client.Client_ID);
                    //objDSMModelData.DeleteASNSettings(cid);
                    ListIRPCredentials.Remove(selected_Client);
                }
            }
        }
        
        private void CreateClient(object sender)
        { 
            
        }
    }
}
