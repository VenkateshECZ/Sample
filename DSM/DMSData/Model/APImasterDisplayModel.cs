
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace DSMData.Model
{
    public class APImasterDisplayModel  
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName()] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

        private string username;
        public string UserName
        {
            get { return username; }
            set { username = value; NotifyPropertyChanged(); }
        }
        private string creatdt;
        public string CreateDT
        {
            get { return creatdt; }
            set { creatdt = value; NotifyPropertyChanged(); }
        }
        private string updatedt;
        public string UpdateDT
        {
            get { return updatedt; }
            set { updatedt = value; NotifyPropertyChanged(); }
        }
        private string status;
        public string STATUS
        {
            get { return status; }
            set { status = value; NotifyPropertyChanged(); }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set { password = value; NotifyPropertyChanged(); }
        }
        private string grant_type;
        public string Grant_Type
        {
            get { return grant_type; }
            set { grant_type = value; NotifyPropertyChanged(); }
        }
        private string customercode;
        public string Customer_Code
        {
            get { return customercode; }
            set { customercode = value; NotifyPropertyChanged(); }
        }
        private string clientid;
        public string Client_ID
        {
            get { return clientid; }
            set { clientid = value; NotifyPropertyChanged(); }
        }



    }
}

