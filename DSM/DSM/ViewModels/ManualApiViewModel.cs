using DSMData;
using DSMData.Model;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DSM.ViewModels
{
    public class ManualApiViewModel : BindableBase
    {
        DSMModelData objDSMModelData;
        public ICommand PostAPIKeyCommand { get; set; }

        public ICommand SearchCommand { get; set; }



        private ObservableCollection<InvoiceDisplayModel> _invoiceList = new ObservableCollection<InvoiceDisplayModel>();
        public ObservableCollection<InvoiceDisplayModel> InvoiceList
        {
            get { return _invoiceList; }
            set { SetProperty(ref _invoiceList, value); }
        }

        private InvoiceDisplayModel _selectedAPI;
        public InvoiceDisplayModel SelectedAPI
        {
            get { return _selectedAPI; }
            set { SetProperty(ref _selectedAPI, value); }
        }
        private string _SearchInvoiceNo;
        public string SearchInvoiceNo
        {
            get { return _SearchInvoiceNo; }
            set { SetProperty(ref _SearchInvoiceNo, value); }
        }
        private string _ProcessMsg;
        public string ProcessMsg
        {
            get { return _ProcessMsg; }
            set { SetProperty(ref _ProcessMsg, value); }
        }


        public ManualApiViewModel()
        {
            objDSMModelData = new DSMModelData();
            SelectedAPI = new InvoiceDisplayModel();
            PostAPIKeyCommand = new DelegateCommand(PostAPI);
            SearchCommand = new DelegateCommand(Search);
            //InvoiceList = new ObservableCollection<InvoiceDisplayModel>(objDSMModelData.GetAllManualInvoiceStatus().Where(
            //        x => ((x.ApiStatus == "MANUAL" || x.ApiStatus == "SKIPPEDAPI") && x.DSStatus == "TRUE") ||
            //        (x.DSStatus == "TRUE" && x.ApiStatus == "E" && x.ApiMsg != "I/F Success, ASN Not created,Create Manual ASN" && x.ApiMsg != "I/F Success, ASN Not cerated,Create Manual ASN")));
            InvoiceList = new ObservableCollection<InvoiceDisplayModel>(objDSMModelData.GetAllManualInvoiceStatus());
        }
      public void Search()
        {
            if (String.IsNullOrEmpty(SearchInvoiceNo))
            {
                MessageBox.Show("Enter Manual InvoiceNumber!!!!");
            }
            else 
            {
                RefreshSearchLists();

            }
        }
        //private void RefreshSearchLists()
        //{

        //    string sinvoiceNo = SearchInvoiceNo;

        //    var allInvoiceList = objDSMModelData.GetAllManualInvoiceSearch(sinvoiceNo);
        //    InvoiceList = new ObservableCollection<InvoiceDisplayModel>(allInvoiceList);
        //    ProcessMsg = "";
        //}
        private void RefreshSearchLists()
        {

            string sinvoiceNo = SearchInvoiceNo;

            var allInvoiceList = objDSMModelData.GetAllManualInvoiceSearch(sinvoiceNo).Where(x => x.InvNumber == sinvoiceNo);
            //var allInvoiceList = objDSMModelData.GetAllManualInvoiceSearch(sinvoiceNo);

            InvoiceList = new ObservableCollection<InvoiceDisplayModel>(allInvoiceList);
            ProcessMsg = "";


        }

        public void PostAPI()
        {

            SettingsDisplayModel objSettings = new SettingsDisplayModel();
            objSettings = objDSMModelData.GetSettingsData(System.Environment.MachineName);
            if (objSettings != null)
            {
                Global.SystemName = objSettings.SystemName;
                Global.InputPath = objSettings.InputPath;
                Global.InvoicePath = objSettings.InvoicePath;
                Global.OutputPath = objSettings.OutputPath;
                Global.APIInvoiceNumber = SelectedAPI.InvNumber;

                CustomerDisplayModel objCustomer = new CustomerDisplayModel();
                Global.CustomerName = SelectedAPI.CustomerName;
                objCustomer = objDSMModelData.GetCustomerByName(Global.CustomerName);

                //string[] invPath = Directory.GetFiles(Global.InvoicePath, "*.pdf");
                //Clientname(invPath[0]);
                
                if (Global.CustomerName == "HYUNDAI")
                {
                     
                    objSettings = objDSMModelData.GetSettingsData(System.Environment.MachineName);
                    if (objSettings != null)
                    {
                        Global.SystemName = objSettings.SystemName;
                        Global.InputPath = objSettings.InputPath;
                        Global.InvoicePath = objSettings.InvoicePath;
                        Global.OutputPath = objSettings.OutputPath;
                        Global.PrinterName = objSettings.PrinterName;

                    }

                    objCustomer = objDSMModelData.GetCustomerByName(Global.CustomerName);

                    if (objCustomer != null)
                    {
                        Global.VendorCode = objCustomer.VendorCode;
                        Global.CustomerId = objCustomer.CustomerId;
                        Global.PDFtype = objCustomer.PDFtype == true ? "SINGLE" : "MULTIPLE";
                        Global.PrintType = objCustomer.PrintType == true ? "ORIGINAL" : "SELECTED";
                        Global.PrintCopies = objCustomer.PrintCopies;
                        if (objCustomer.IsAPIpost)
                        {
                            Global.IsAPIpost = true;
                            Global.APIpostType = objCustomer.APItype == true ? "AUTO" : "MANUAL";
                            Global.APIurl = objCustomer.APIUrl;
                            if (Global.APIurl == "https://hmieai.hmil.net:6004/Service.asmx?WSDL")
                            {
                                Global.APIPosturl = "http://hmieai/DI/data";
                            }
                            else
                            {
                                Global.APIPosturl = "http://hieai/KIMIL1/data";
                            }
                            APIPost(Global.APIInvoiceNumber);
                        }
                        else
                        {
                            Global.IsAPIpost = false;
                        }
                    }
                        

                }
                else if (Global.CustomerName.ToUpper() == "KIA")
                {
 
                    objSettings = objDSMModelData.GetSettingsData(System.Environment.MachineName);
                    if (objSettings != null)
                    {
                        Global.SystemName = objSettings.SystemName;
                        Global.InputPath = objSettings.InputPath;
                        Global.InvoicePath = objSettings.InvoicePath;
                        Global.OutputPath = objSettings.OutputPath;
                        Global.PrinterName = objSettings.PrinterName;

                    }

                    objCustomer = objDSMModelData.GetCustomerByName(Global.CustomerName);

                    if (objCustomer != null)
                    {
                        Global.VendorCode = objCustomer.VendorCode;
                        Global.CustomerId = objCustomer.CustomerId;
                        Global.PDFtype = objCustomer.PDFtype == true ? "SINGLE" : "MULTIPLE";
                        Global.PrintType = objCustomer.PrintType == true ? "ORIGINAL" : "SELECTED";
                        Global.PrintCopies = objCustomer.PrintCopies;
                        if (objCustomer.IsAPIpost)
                        {
                            Global.IsAPIpost = true;
                            Global.APIpostType = objCustomer.APItype == true ? "AUTO" : "MANUAL";
                            Global.APIurl = objCustomer.APIUrl;
                            if (Global.APIurl == "https://hmieai.hmil.net:6004/Service.asmx?WSDL")
                            {
                                Global.APIPosturl = "http://Kmieai/DMDI/data";
                            }
                            else
                            {
                                Global.APIPosturl = "http://Kmieai/DMDI/data";
                            }
                            APIPost(Global.APIInvoiceNumber);
                        }
                        else
                        {
                            Global.IsAPIpost = false;
                        }
                    }
                       
                }
                else
                {
                    MessageBox.Show("Client Settings not provided");
                }
            }
            else
            {
                MessageBox.Show("Settings not provided for this pc");
            }
        }


        public void APIPost(string InvNumber)
        {
            Global.APIInvoiceNumber = InvNumber;
            var lstDS = objDSMModelData.GetSelectedInvoice(InvNumber).Where(x => x.CustomerName.ToUpper() == "HYUNDAI").FirstOrDefault();
            if (lstDS != null)
            {
                Global.APIInvoiceNumber = InvNumber;

                try
                {
                    ExampleAPIProxy exProxy = new ExampleAPIProxy();
                    WebService webService = new WebService(Global.APIurl, "getData");
                    webService.Invoke();
                }
                catch (Exception ex)
                {
                    //LogErrors(ex);
                    //timer.Start();
                    //timerAPIPost.Start();
                }

            }
            else
            {
                var lstDS1 = objDSMModelData.GetSelectedInvoice(InvNumber).Where(x => x.CustomerName.ToUpper() == "KIA").FirstOrDefault();
                if (lstDS1 != null)
                {
                    Global.CustomerName = "KIA";
                    CustomerDisplayModel objCustomer = new CustomerDisplayModel();
                    objCustomer = objDSMModelData.GetCustomerByName(Global.CustomerName);
                    if (objCustomer.IsAPIpost)
                    {
                        Global.IsAPIpost = true;
                        Global.APIpostType = objCustomer.APItype == true ? "AUTO" : "MANUAL";
                        Global.APIurl = objCustomer.APIUrl;
                        if (Global.APIurl == "https://hmieai.hmil.net:6004/Service.asmx?WSDL")
                        {
                            Global.APIPosturl = "http://Kmieai/DMDI/data";
                        }
                        else
                        {
                            Global.APIPosturl = "http://Kmieai/DMDI/data";
                        }
                    }
                    else
                    {
                        Global.IsAPIpost = false;
                    }



                    Global.APIInvoiceNumber = InvNumber;


                    try
                    {
                        ExampleAPIProxy exProxy = new ExampleAPIProxy();
                        Webservicekia Webservicekia = new Webservicekia(Global.APIurl, "getData");
                        Webservicekia.Invoke();
                    }
                    catch (Exception ex)
                    {
                        // LogErrors(ex);
                        // timer.Start();
                        //timerAPIPost.Start();
                    }
                }
                else
                { }
            }
        }

        public void BULKAPIPost()
        {
            try
            {

                SettingsDisplayModel objSettings = new SettingsDisplayModel();
                objSettings = objDSMModelData.GetSettingsData(System.Environment.MachineName);
                if (objSettings != null)
                {
                    Global.SystemName = objSettings.SystemName;
                    Global.InputPath = objSettings.InputPath;
                    Global.InvoicePath = objSettings.InvoicePath;
                    Global.OutputPath = objSettings.OutputPath;


                    CustomerDisplayModel objCustomer = new CustomerDisplayModel();
                    Global.CustomerName = "HYUNDAI";
                    objCustomer = objDSMModelData.GetCustomerByName(Global.CustomerName);

                    if (objCustomer != null)
                    {
                        Global.IsAPIpost = objCustomer.IsAPIpost;
                        Global.APIpostType = "MANUAL";
                        Global.APIurl = objCustomer.APIUrl;
                        if (Global.APIurl == "https://hmieai.hmil.net:6004/Service.asmx?WSDL")
                        {
                            Global.APIPosturl = "http://hmieai/DI/data";
                        }
                        else
                        {
                            Global.APIPosturl = "http://hieai/KIMIL1/data";
                        }

                        foreach (var Inv in InvoiceList)
                        {
                            Global.APIInvoiceNumber = Inv.InvNumber;
                            ExampleAPIProxy exProxy = new ExampleAPIProxy();
                            //WebService webService = new WebService("https://hmieai.hmil.net:94/KML.asmx?WSDL", "getData");
                            //WebService webService = new WebService("https://hmieai.hmil.net:6004/Service.asmx?WSDL", "getData");
                            WebService webService = new WebService(Global.APIurl, "getData");
                            webService.Invoke();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Client Settings not provided");
                    }
                }
                else
                {
                    MessageBox.Show("Settings not provided for this pc");
                }


                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invoice - " + Global.APIInvoiceNumber + " Error Posting - " + ex.ToString());
            }
        }
    }
}
