using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace DSMData.Model
{
    public class IRPSettingsDisplayModel
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName()] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int iD;
        public int ID
        {
            get { return iD; }
            set { iD = value; NotifyPropertyChanged(); }
        }

        private string client_ID;
        public string Client_ID
        {
            get { return client_ID; }
            set { client_ID = value; NotifyPropertyChanged(); }
        }

        private string client_Secret;
        public string Client_Secret
        {
            get { return client_Secret; }
            set { client_Secret = value; NotifyPropertyChanged(); }
        }

        private string  userName;
        public string UserName
        {
            get { return userName; }
            set { userName = value; NotifyPropertyChanged(); }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set { password = value; NotifyPropertyChanged(); }
        }

        private string publicKey;
        public string PublicKey
        {
            get { return publicKey; }
            set { publicKey = value; NotifyPropertyChanged(); }
        }

        private string gSTIN;
        public string GSTIN
        {
            get { return gSTIN; }
            set { gSTIN = value; NotifyPropertyChanged(); }
        }

        private bool isActive;
        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; NotifyPropertyChanged(); }
        }

        private string emailID;
        public string EmailID
        {
            get { return emailID; }
            set { emailID = value; NotifyPropertyChanged(); }
        }

        private string iP_Address;
        public string IP_Address
        {
            get { return iP_Address; }
            set { iP_Address = value; NotifyPropertyChanged(); }
        }
        private string auth;
        public string Auth
        {
            get { return auth; }
            set { auth = value; NotifyPropertyChanged(); }
        }

        private string asn;
        public string ASN
        {
            get { return asn; }
            set { asn = value; NotifyPropertyChanged(); }
        }
        private string client_secret;
        public string ClientSecret
        {
            get { return client_secret; }
            set { client_secret = value; NotifyPropertyChanged(); }
        }

        private string customer_code;
        public string Customercode
        {
            get { return customer_code; }
            set { customer_code = value; NotifyPropertyChanged(); }
        }

       

       
        private string customercode;
        public string Customer_Code
        {
            get { return customercode; }
            set { customercode = value; NotifyPropertyChanged(); }
        }
       



    }
}
