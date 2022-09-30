using DSMData;
using DSMData.Model;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using KeepDynamic.BarcodeReader;
using Microsoft.Practices.Unity;
using OnBarcode.Barcode.BarcodeScanner;
using PdfToImage;
using PQScan.BarcodeScanner;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Spire.Barcode;
using Spire.Pdf;
using Spire.Pdf.Exporting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;
using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Data;

namespace DSM.ViewModels
{
    public class DashBoardViewModel : BindableBase, INotifyPropertyChanged
    {

        //[DllImport("user32")]
        //public static extern int SetCursorPos(int x, int y);

        //[DllImport("gsdll32.dll", EntryPoint = "gsapi_new_instance")]
        //public static void ReadPDF();

        public event PropertyChangedEventHandler PropertyChangeds;
         
        int mailcount = 0;
        string[] invPath = new string[1000];
        private void NotifyPropertyChanged([CallerMemberName()] string propertyName = null)
        {
            PropertyChangeds?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public DSMModelData objDSMModelData;
        public InvoiceDisplayModel objInvoice;
        public UserDisplayModel objUser;
        public List<InvoiceDisplayModel> lstInvoice;
        private IRegionManager regionManager;
        private IUnityContainer container;
        public ICommand GoToAdminCommand { get; set; }
        public ICommand ReprintCommand { get; set; }

        public ICommand SearchCommand { get; set; }

        public string cur_pos = "";
        public static string Auth_URL = "";
        public static string ContentType = "";
        public static string Username = "";
        public static string Password = "";
        public static string CLIENTID = "";
        // public static string client_code = "";
        public static string grant_type = "";
        public static string client_secret = "";
        public static string Auth_Token = "";
        public static string ASN_URL = "";
        public static string Gstin = "";
        public static string Client_Code = "";
        public static string PORefr = "";
        public static string SINo = "";
        public static string Qty = "";
        public static string Inv_No = "";
        public static string Inv_Dt = "";
        public static string UnitPrice = "";
        public static string NetUnitPrice = "";
        public static string TotAmt = "";
        public static string AssAmt = "";
        public static string SgstVal = "";
        public static string CgstVal = "";
        public static string IgstVal = "";
        public static string SGstRt = "";
        public static string CGstRt = "";
        public static string IGstRt = "";
        public static string TotItemVal = "";
        public static string ForCur = "";
        public static string Sup_Gstin = "";
        public static string VehNo = "";
        public static string Part_Rev = "";
        public static string VendorCode = "";
        public static string Buy_Gstin = "";
        public static string Irn = "";
        public static string ASN_Number = "";
        public static string user = "";
        public static string pwrd = "";

        DigitalSign objDigitalSign = new DigitalSign();
        DispatcherTimer timer = new DispatcherTimer();
        DispatcherTimer timerPick = new DispatcherTimer();
        DispatcherTimer timerSign = new DispatcherTimer();
        DispatcherTimer timerPrintPost = new DispatcherTimer();
        DispatcherTimer timerAPIPost = new DispatcherTimer();
        DispatcherTimer timerRefresh = new DispatcherTimer();
        DispatcherTimer timerMail = new DispatcherTimer();

        private ObservableCollection<InvoiceDisplayModel> _invoiceList = new ObservableCollection<InvoiceDisplayModel>();
        public ObservableCollection<InvoiceDisplayModel> InvoiceList
        {
            get { return _invoiceList; }
            set { SetProperty(ref _invoiceList, value); }
        }

        private ObservableCollection<InvoiceDisplayModel> _currentinvoiceList = new ObservableCollection<InvoiceDisplayModel>();
        public ObservableCollection<InvoiceDisplayModel> CurrentInvoiceList
        {
            get { return _currentinvoiceList; }
            set { SetProperty(ref _currentinvoiceList, value); }
            //get { return _currentinvoiceList; }
            //set { _currentinvoiceList = value; NotifyPropertyChanged("CurrentInvoiceList"); }
        }

        private string _isData;
        public string IsData
        {
            get { return _isData; }
            set { SetProperty(ref _isData, value); }
        }

        private InvoiceDisplayModel _selectedReprint;
        public InvoiceDisplayModel SelectedReprint
        {
            get { return _selectedReprint; }
            set { SetProperty(ref _selectedReprint, value); }
        }

        private ObservableCollection<QuarantineDisplayModel> _QList = new ObservableCollection<QuarantineDisplayModel>();
        public ObservableCollection<QuarantineDisplayModel> QtList
        {
            get { return _QList; }
            set { SetProperty(ref _QList, value); }
        }

        private ObservableCollection<InvoiceDisplayModel> _SignedInvoiceList = new ObservableCollection<InvoiceDisplayModel>();
        public ObservableCollection<InvoiceDisplayModel> SignedInvoiceList
        {
            get { return _SignedInvoiceList; }
            set { SetProperty(ref _SignedInvoiceList, value); }
        }

        private ObservableCollection<InvoiceDisplayModel> _PrintedInvoiceList = new ObservableCollection<InvoiceDisplayModel>();
        public ObservableCollection<InvoiceDisplayModel> PrintedInvoiceList
        {
            get { return _PrintedInvoiceList; }
            set { SetProperty(ref _PrintedInvoiceList, value); }
        }

        private ObservableCollection<InvoiceDisplayModel> _APIInvoiceList = new ObservableCollection<InvoiceDisplayModel>();
        public ObservableCollection<InvoiceDisplayModel> APIInvoiceList
        {
            get { return _APIInvoiceList; }
            set { SetProperty(ref _APIInvoiceList, value); }
        }

        private ObservableCollection<InvoiceDisplayModel> _pendingInv = new ObservableCollection<InvoiceDisplayModel>();
        public ObservableCollection<InvoiceDisplayModel> PendingInvList
        {
            get { return _pendingInv; }
            set { SetProperty(ref _pendingInv, value); }
        }

        private string _currentInvoice;
        public string CurrentInvoice
        {
            get { return _currentInvoice; }
            set { _currentInvoice = value; NotifyPropertyChanged("CurrentInvoice"); }
        }

        private bool _splitStatus;
        public bool SplitStatus
        {
            get { return _splitStatus; }
            set { SetProperty(ref _splitStatus, value); }
        }

        private string _DataMsg;
        public string DataMsg
        {
            get { return _DataMsg; }
            set { SetProperty(ref _DataMsg, value); }
        }

        private string _ProcessMsg;
        public string ProcessMsg
        {
            get { return _ProcessMsg; }
            set { SetProperty(ref _ProcessMsg, value); }
        }

        private double _ScreenHeight;
        public double ScreenHeight
        {
            get { return _ScreenHeight; }
            set { SetProperty(ref _ScreenHeight, value); }
        }

        private double _ScreenWidth;
        public double ScreenWidth
        {
            get { return _ScreenWidth; }
            set { SetProperty(ref _ScreenWidth, value); }
        }

        private double _DashboardWidth;
        public double DashboardWidth
        {
            get { return _DashboardWidth; }
            set { SetProperty(ref _DashboardWidth, value); }
        }

        private double _DashboardHeight;
        public double DashboardHeight
        {
            get { return _DashboardHeight; }
            set { SetProperty(ref _DashboardHeight, value); }
        }

        private double _CurDashboardWidth;
        public double CurDashboardWidth
        {
            get { return _CurDashboardWidth; }
            set { SetProperty(ref _CurDashboardWidth, value); }
        }

        private double _CurDashboardHeight;
        public double CurDashboardHeight
        {
            get { return _CurDashboardHeight; }
            set { SetProperty(ref _CurDashboardHeight, value); }
        }

        private ObservableCollection<CustomerDisplayModel> _Customers;
        public ObservableCollection<CustomerDisplayModel> Customers
        {
            get { return _Customers; }
            set { SetProperty(ref _Customers, value); }
        }

        private double _QDashboardHeight;
        public double QDashboardHeight
        {
            get { return _QDashboardHeight; }
            set { SetProperty(ref _QDashboardHeight, value); }
        }

        private string _CurrentInvDetEnable;
        public string CurrentInvDetEnable
        {
            get { return _CurrentInvDetEnable; }
            set { SetProperty(ref _CurrentInvDetEnable, value); }
        }

        private string _HappyImgVisible;
        public string HappyImgVisible
        {
            get { return _HappyImgVisible; }
            set { SetProperty(ref _HappyImgVisible, value); }
        }

        private string _SadImgVisible;
        public string SadImgVisible
        {
            get { return _SadImgVisible; }
            set { SetProperty(ref _SadImgVisible, value); }
        }

        private string _SearchInvoiceNo;
        public string SearchInvoiceNo
        {
            get { return _SearchInvoiceNo; }
            set { SetProperty(ref _SearchInvoiceNo, value); }
        }

        string QRCodeString="";
        bool isAssignedcoord = true;
        bool iskeycheck = true;
        bool isserverconnected = true;
        public DashBoardViewModel(IRegionManager regionManager, IUnityContainer uContainer)
        {
            //ProcessMsg = "Scanning under progress...";
            this.regionManager = regionManager;
            this.container = uContainer;

            ScreenWidth = SystemParameters.PrimaryScreenWidth;
            ScreenHeight = SystemParameters.PrimaryScreenHeight;
            DashboardWidth = SystemParameters.PrimaryScreenWidth - 200;
            //DashboardHeight = SystemParameters.PrimaryScreenHeight - 200;
            //DashboardHeight = (SystemParameters.PrimaryScreenHeight - 300)/2;
            //CurDashboardHeight= (SystemParameters.PrimaryScreenHeight - 300) / 2;

            //DashboardHeight = (SystemParameters.PrimaryScreenHeight - 200) / 2;

            //DashboardHeight = SystemParameters.PrimaryScreenHeight - 280;
            //DashboardHeight = SystemParameters.PrimaryScreenHeight - 320;
            //DashboardHeight = SystemParameters.PrimaryScreenHeight - 370;
            //DashboardHeight = SystemParameters.PrimaryScreenHeight - 370;
            DashboardHeight = SystemParameters.PrimaryScreenHeight - 470;

            //CurDashboardHeight = (SystemParameters.PrimaryScreenHeight - 300) / 2;

            //CurDashboardHeight = (SystemParameters.PrimaryScreenHeight - 830);
            //CurDashboardHeight = (SystemParameters.PrimaryScreenHeight - 630);
            //CurDashboardHeight = (SystemParameters.PrimaryScreenHeight - 660);
            //CurDashboardHeight = (SystemParameters.PrimaryScreenHeight - 560);
            //CurDashboardWidth = SystemParameters.PrimaryScreenWidth - 550;
            CurDashboardHeight = (SystemParameters.PrimaryScreenHeight - 900);
            CurDashboardWidth = SystemParameters.PrimaryScreenWidth - 200;

            QDashboardHeight = SystemParameters.PrimaryScreenHeight - 300;
            GoToAdminCommand = new DelegateCommand(GoToAdmin);

            objDSMModelData = new DSMModelData();
            objInvoice = new InvoiceDisplayModel();
            lstInvoice = new List<InvoiceDisplayModel>();

            SelectedReprint = new InvoiceDisplayModel();

            CurrentInvoice = "";

            SearchCommand = new DelegateCommand(SearchInvNo);

            objUser = new UserDisplayModel();
            
               CheckApiExeRunOrNot();
            if (Global.UserType == "User")
            {
                //EthernetConnectivityCheck();
                AssignSettings();

                if (iskeycheck && isserverconnected && isAssignedcoord)
                {
                    RefreshLists();
                    CurrentInvDetEnable = "Visible";
                    HappyImgVisible = "Hidden";
                    SadImgVisible = "Hidden";
                    CheckApiExeRunOrNot();
                    timer = new DispatcherTimer();
                    //timer.Interval = new TimeSpan(0, 0, int.Parse(ConfigurationManager.AppSettings["timerSeconds"].ToString()));
                    timer.Interval = TimeSpan.FromSeconds(5);
                    timer.Tick += new EventHandler(StartProcess);
                    timer.Start();
                  
                    ReprintCommand = new DelegateCommand(Reprint);
                    //EthernetConnectivityCheck();
                    objDSMModelData.UpdateUserEntryDetails(Global.UserName.ToString().Trim());
                    Application.Current.MainWindow.Closing += new CancelEventHandler(MainWindow_Closing);
                }
                else
                {
                    //EthernetConnectivityCheck();
                    objDSMModelData.UpdateUserExitDetails(Global.UserName.ToString().Trim());
                }
            }
            else if (Global.UserType == "Api")
            {
                //EthernetConnectivityCheck();
                AssignServerSettings();
                //RefreshLists();
                RefreshServerLists();
                //CurrentRefreshLists();
                //dgCurrentInv.
                CurrentInvDetEnable = "Hidden";
                HappyImgVisible = "Hidden";
                SadImgVisible = "Hidden";
                timerAPIPost = new DispatcherTimer();
                timerAPIPost.Interval = TimeSpan.FromSeconds(5);
                timerAPIPost.Tick += new EventHandler(APIPostFiles);
                timerAPIPost.Start();
                //EthernetConnectivityCheck();
                objDSMModelData.UpdateUserEntryDetails(Global.UserName.ToString().Trim());
                Application.Current.MainWindow.Closing += new CancelEventHandler(MainWindow_Closing);
            }
        }

        public void Closing(CancelEventArgs e)
        {
            e.Cancel = true;
        }

        public void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Your code to handle the event
            //App.Current.MainWindow.Close();
            //Application.Current.MainWindow.Close();
            //e.Cancel = true;
            //this.Visibility = Visibility.Hidden;
            //int loginvalue = 1;
            //EthernetConnectivityCheck();
            objDSMModelData.UpdateUserExitDetails(Global.UserName.ToString().Trim());
            foreach (Process proc in Process.GetProcessesByName("DSM"))
            {
                proc.Kill();
            }
        }
        public void Reprint()
        {
           
            CustomerDisplayModel objCustomer = new CustomerDisplayModel();
           
           
            if (Global.UserType == "User")
            {
                timer.Stop();

                string PrintInvNo = SelectedReprint.InvNumber;
                objCustomer = objDSMModelData.GetCustomerByInvNumber(PrintInvNo);
               
                Global.CustomerName = objCustomer.CustomerName;
                objCustomer = objDSMModelData.GetCustomerByName(Global.CustomerName);
                Global.PrintCopies = objCustomer.PrintCopies;
                Print objPrint = new Print();
                //var printStatus = objPrint.PrintInvoicePDFs(Global.InvoiceNumber, "invoice");
                var printStatus = objPrint.ReprintInvoicePDFs(PrintInvNo, "invoice");
                timer.Start();
            }
            else if (Global.UserType == "Api")
            {
                timerAPIPost.Stop();

                string PrintInvNo = SelectedReprint.InvNumber;

                Print objPrint = new Print();
                //var printStatus = objPrint.PrintInvoicePDFs(Global.InvoiceNumber, "invoice");
                var printStatus = objPrint.ReprintInvoicePDFs(PrintInvNo, "invoice");
                timerAPIPost.Start();
            }
        }

        private void SearchInvNo()
        {
           
            if (Global.UserType == "User")
            {
                timer.Stop();
                RefreshSearchLists();
                timer.Start();
            }
            else if (Global.UserType == "Api")
            {
                timerAPIPost.Stop();
                RefreshSearchLists();
                timerAPIPost.Start();
            }
        }

        public void GoToAdmin()
        {
            //App.Current.MainWindow.Close();
        }
        
        public void CheckApiExeRunOrNot()
        {
            //var lstApiInv = objDSMModelData.GetApiUserType("apipost");
            //objUser = null;
            //EthernetConnectivityCheck();
            var lstusers = objDSMModelData.GetApiUserType("Api");
            if (lstusers.Count>0)
            {
                for (int k = 0; k < lstusers.Count; k++)
                {
                    if (lstusers[k].LoginStatus == "TRUE")
                    {
                        HappyImgVisible = "Visible";
                        SadImgVisible = "Hidden";
                        isserverconnected = true;
                        mailcount = 0;
                    }
                    else
                    {
                        HappyImgVisible = "Hidden";
                        SadImgVisible = "Visible";

                        //Mail alert
                        int Finyear = 0;
                        string prefix_invoice_no = "";
                        int fin_mon = 0;
                        //string _sQry_Prefix_invoice_no = "select month(getdate()) as cur_month, FORMAT(getdate(), 'yy') as cur_year";
                        string _sQry_Prefix_invoice_no = "select month(getdate()) as cur_month, year(getdate()) as cur_year";
                        SqlDataReader _rdr_Prefix_invoice_no = Global.executeQuery(_sQry_Prefix_invoice_no);
                        if (_rdr_Prefix_invoice_no.Read())
                        {
                            string year = _rdr_Prefix_invoice_no["cur_year"].ToString().Substring(2, 2);
                            fin_mon = 3;
                            if (int.Parse(_rdr_Prefix_invoice_no["cur_month"].ToString()) > fin_mon)
                                prefix_invoice_no = year + (int.Parse(year) + 1);
                            else
                                prefix_invoice_no = (int.Parse(year) - 1) + (year);
                        }
                        //_inv.Finyear = Convert.ToInt32(prefix_invoice_no.ToString().Trim());
                        Finyear = Convert.ToInt32(prefix_invoice_no.ToString().Trim());

                        Global.APIFinYear = Finyear;

                        int lstApiInv = objDSMModelData.GetAllAPIInvoiceStatusAll(Finyear).Count();
                        if (lstApiInv >= 1)
                        {
                            if (mailcount == 0)
                            {
                                Global.MailAlert("API Application was not running. Please connect the Server","","");
                                mailcount++;
                            }
                        }
                        //MessageBox.Show("Server Disconnected");

                    }
                }
            }
            else
            {
                HappyImgVisible = "Hidden";
                SadImgVisible = "Visible";
            }
        }
        //public void AssignSettings()
        //{
        //    try
        //    {
        //        //EthernetConnectivityCheck();
        //        SplitStatus = true;
        //        SettingsDisplayModel objSettings = new SettingsDisplayModel();
        //        CustomerDisplayModel objCustomer = new CustomerDisplayModel();

        //        Global.CustomerName = "HYUNDAI";

        //        objDigitalSign = new DigitalSign();
        //        var keyConnected = objDigitalSign.CheckKeyConnected(Global.CustomerName);
        //        //var keyConnected = true;

        //        if (keyConnected == false)
        //        {
        //            MessageBox.Show("Check whether the key is connected or not!");
        //            App.Current.MainWindow.Close();
        //            for (int intCounter = App.Current.Windows.Count - 1; intCounter > 0; intCounter--)
        //                App.Current.Windows[intCounter].Close();
        //            //App.Current.Windows[1].Close();
        //            iskeycheck = false;
        //        }

        //        else
        //        {
        //            //EthernetConnectivityCheck();
        //            if (isAssignedcoord && iskeycheck)
        //            {
        //                objSettings = objDSMModelData.GetSettingsData(System.Environment.MachineName);
        //                if (objSettings != null)
        //                {
        //                    Global.SystemName = objSettings.SystemName;
        //                    Global.InputPath = objSettings.InputPath;
        //                    Global.InvoicePath = objSettings.InvoicePath;
        //                    Global.OutputPath = objSettings.OutputPath;
        //                    Global.PrinterName = objSettings.PrinterName;

        //                }

        //                objCustomer = objDSMModelData.GetCustomerByName(Global.CustomerName);

        //            if (objCustomer != null)
        //            {
        //                Global.VendorCode = objCustomer.VendorCode;
        //                Global.CustomerId = objCustomer.CustomerId;
        //                Global.PDFtype = objCustomer.PDFtype == true ? "SINGLE" : "MULTIPLE";
        //                Global.PrintType = objCustomer.PrintType == true ? "ORIGINAL" : "SELECTED";
        //                Global.PrintCopies = objCustomer.PrintCopies;
        //                if (objCustomer.IsAPIpost)
        //                {
        //                    Global.IsAPIpost = true;
        //                    Global.APIpostType = objCustomer.APItype == true ? "AUTO" : "MANUAL";
        //                    Global.APIurl = objCustomer.APIUrl;
        //                    if (Global.APIurl == "https://hmieai.hmil.net:6004/Service.asmx?WSDL")
        //                    {
        //                        Global.APIPosturl = "http://hmieai/DI/data";
        //                    }
        //                    else
        //                    {
        //                        Global.APIPosturl = "http://hieai/KIMIL1/data";
        //                    }
        //                }
        //                else
        //                {
        //                    Global.IsAPIpost = false;
        //                }
        //            }
        //        }
        //    }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogErrors(ex);
        //    }
        //}
        public void AssignSettings()
        {
            try
            {
                //EthernetConnectivityCheck();
                SplitStatus = true;
                SettingsDisplayModel objSettings = new SettingsDisplayModel();
                CustomerDisplayModel objCustomer = new CustomerDisplayModel();

                objSettings = objDSMModelData.GetSettingsData(System.Environment.MachineName);
                if (objSettings != null)
                {
                    Global.SystemName = objSettings.SystemName;
                    Global.InputPath = objSettings.InputPath;
                    Global.InvoicePath = objSettings.InvoicePath;
                    Global.OutputPath = objSettings.OutputPath;
                    Global.PrinterName = objSettings.PrinterName;

                }

               
            }
            catch (Exception ex)
            {
                LogErrors(ex);
            }
        }
        public void Clientname(string src)
        {
            int i = 0;
            string[] cust_name = { "HYUNDAI", "KIA" };
            string[] cust_id = { "HMI1", "KIA" };
            for (i = 0; i < 2; i++)
            {
                var coordinates = objDSMModelData.GetTemplatesCord(cust_name[i], cust_id[i]);
                string invnocoord = "";

                invnocoord = coordinates[0].Clientnamecoord.ToString();

                string pathToPdf = src;
                PdfReader reader = new PdfReader(pathToPdf);

                if (invnocoord.Length > 0 && invnocoord.ToString().Trim() != "")
                {
                    RenderFilter[] filters = new RenderFilter[1];
                    LocationTextExtractionStrategy regionFilter = new LocationTextExtractionStrategy();
                    string[] arrinvcoord = invnocoord.Split(',');
                    filters[0] = new RegionTextRenderFilter(new iTextSharp.text.Rectangle(int.Parse(arrinvcoord[0]), int.Parse(arrinvcoord[1]), int.Parse(arrinvcoord[2]), int.Parse(arrinvcoord[3])));
                    FilteredTextRenderListener strategy = new FilteredTextRenderListener(regionFilter, filters);

                    String invresult = PdfTextExtractor.GetTextFromPage(reader, 1, strategy);
                    string[] invno = new string[3];
                    if (invresult.ToUpper().Contains("HYUNDAI"))
                    {


                        Global.CustomerName = "HYUNDAI";
                    }
                    else
                    {
                        Global.CustomerName = "KIA";
                    }
                    if(Global.CustomerName == null || Global.CustomerName == "")
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        public void EthernetConnectivityCheck()
        {
            try
            {
                // IPHostEntry obj = Dns.GetHostByName("www.home.live.com");
                TcpClient clnt = new TcpClient("www.google.com", 80);
                clnt.Close();
                //MessageBox.Show("Connected");
                // Console.WriteLine("Ethernet Connected");
                //Console.ReadLine();
            }
            catch
            {
                MessageBox.Show("Server Disconnected");
                //for (int intCounter = App.Current.Windows.Count - 1; intCounter > 0; intCounter--)
                //    App.Current.Windows[intCounter].Close();
                //App.Current.MainWindow.Close();
                foreach (Process proc in Process.GetProcessesByName("DSM"))
                {
                    proc.Kill();
                }
                timer.Stop();
                //timerAPIPost.Stop();
                //for (int intCounter = App.Current.Windows.Count - 1; intCounter > 0; intCounter--)
                //    App.Current.Windows[intCounter].Close();

                //Console.WriteLine("Ehternet Not Connected");
                //Console.ReadLine();
            }
        }

        public void AssignServerSettings()
        {
            try
            {
                //SplitStatus = true;
                SettingsDisplayModel objSettings = new SettingsDisplayModel();
                CustomerDisplayModel objCustomer = new CustomerDisplayModel();

                Global.CustomerName = "HYUNDAI";
                //EthernetConnectivityCheck();
                objSettings = objDSMModelData.GetSettingsData(System.Environment.MachineName);
                if (objSettings != null)
                {
                    Global.SystemName = objSettings.SystemName;
                    Global.InputPath = objSettings.InputPath;
                    Global.InvoicePath = objSettings.InvoicePath;
                    Global.OutputPath = objSettings.OutputPath;
                    Global.PrinterName = objSettings.PrinterName;

                }
                //EthernetConnectivityCheck();
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
                    }
                    else
                    {
                        Global.IsAPIpost = false;
                    }
                }
            }
            catch (Exception ex)
            {
                LogErrors(ex);
            }
        }

        private void RefreshList(object sender, EventArgs e)
        {
            timerRefresh.Stop();
            //EthernetConnectivityCheck();
            //var allInvoiceList = objDSMModelData.GetAllInvoiceStatus();
            var allInvoiceList = objDSMModelData.GetAllSYSInvoiceStatus();
            
            for (int i = 0; i < allInvoiceList.Count; i++)
            {
                if (allInvoiceList[i].DSStatus == "TRUE")
                    allInvoiceList[i].DSStatus = "SIGNED";
                else
                    allInvoiceList[i].DSStatus = "PENDING";

                if (allInvoiceList[i].PrintStatus == "TRUE")
                    allInvoiceList[i].PrintStatus = "PRINTED";
                else
                    allInvoiceList[i].PrintStatus = "PENDING";

                if (allInvoiceList[i].ApiStatus == "FALSE")
                    allInvoiceList[i].ApiStatus = "PENDING";
            }
            if (allInvoiceList.Count() > 0)
            {
                InvoiceList = new ObservableCollection<InvoiceDisplayModel>(allInvoiceList);
            }
            timerRefresh.Start();
        }

        private void RefreshLists()
        {
            //EthernetConnectivityCheck();
            SearchInvoiceNo = "";
            //var allInvoiceList = objDSMModelData.GetAllInvoiceStatus();
            var allInvoiceList = objDSMModelData.GetAllSYSInvoiceStatus();
            QtList = new ObservableCollection<QuarantineDisplayModel>(objDSMModelData.GetQuarantineList());
            for (int i = 0; i < allInvoiceList.Count; i++)
            {
                if (allInvoiceList[i].DSStatus == "TRUE")
                    allInvoiceList[i].DSStatus = "SIGNED";
                else
                    allInvoiceList[i].DSStatus = "PENDING";

                if (allInvoiceList[i].PrintStatus == "TRUE")
                    allInvoiceList[i].PrintStatus = "PRINTED";
                else
                    allInvoiceList[i].PrintStatus = "PENDING";

                if (allInvoiceList[i].ApiStatus == "FALSE")
                    allInvoiceList[i].ApiStatus = "PENDING";
            }
            if (allInvoiceList.Count() > 0)
            {
                InvoiceList = new ObservableCollection<InvoiceDisplayModel>(allInvoiceList);
                ProcessMsg = "";
                DataMsg = "";
            }
            else
            {
                //allInvoiceList[0].DSStatus = "No Records Found";
                //allInvoiceList[0].PrintStatus = "No Records Found";
                //allInvoiceList[0].ApiStatus = "No Records Found";
                //List<InvoiceDisplayModel> inotfoundInvoiceList = new List<InvoiceDisplayModel>()
                //{
                //new InvoiceDisplayModel()
                //{
                //    DSStatus = "No Records Found",
                //    PrintStatus = "No Records Found",
                //    ApiStatus ="No Records Found"


                //}
                //};


                InvoiceList = new ObservableCollection<InvoiceDisplayModel>(allInvoiceList);
                DataMsg = "No Records Found";
                //ProcessMsg = "Scanning under progress...";
            }
        }

        private void RefreshServerLists()
        {
            try
            {
                //EthernetConnectivityCheck();
                var allInvoiceList = objDSMModelData.GetAllInvoiceStatus();
                //var allInvoiceList = objDSMModelData.GetAllSYSInvoiceStatus();
                QtList = new ObservableCollection<QuarantineDisplayModel>(objDSMModelData.GetQuarantineList());
                for (int i = 0; i < allInvoiceList.Count; i++)
                {
                    if (allInvoiceList[i].DSStatus == "TRUE")
                        allInvoiceList[i].DSStatus = "SIGNED";
                    else
                        allInvoiceList[i].DSStatus = "PENDING";

                    if (allInvoiceList[i].PrintStatus == "TRUE")
                        allInvoiceList[i].PrintStatus = "PRINTED";
                    else
                        allInvoiceList[i].PrintStatus = "PENDING";

                    if (allInvoiceList[i].ApiStatus == "FALSE")
                        allInvoiceList[i].ApiStatus = "PENDING";
                }
                if (allInvoiceList.Count() > 0)
                {
                    InvoiceList = new ObservableCollection<InvoiceDisplayModel>(allInvoiceList);
                    ProcessMsg = "";
                }
                else
                {
                    InvoiceList = new ObservableCollection<InvoiceDisplayModel>(allInvoiceList);
                    DataMsg = "No Records Found";
                    //ProcessMsg = "Scanning under progress...";
                }
            }
            catch(Exception ex)
            {
                LogErrors(ex);
            }
        }

        private void RefreshSearchLists()
        {
            //EthernetConnectivityCheck();
            //var allInvoiceList = objDSMModelData.GetAllInvoiceStatus();
            //string sinvoiceNo = "52173";
            string sinvoiceNo = SearchInvoiceNo;
            //InvoiceList = new ObservableCollection<InvoiceReportModel>(objDSMModelData.FilterInvoiceData(InvNumber == null ? "" : InvNumber, FromDate.ToString(), ToDate.ToString()));
            var allInvoiceList = objDSMModelData.GetAllSearchInvoiceStatus(sinvoiceNo);
            //QtList = new ObservableCollection<QuarantineDisplayModel>(objDSMModelData.GetQuarantineList());
            for (int i = 0; i < allInvoiceList.Count; i++)
            {
                if (allInvoiceList[i].DSStatus == "TRUE")
                    allInvoiceList[i].DSStatus = "SIGNED";
                else
                    allInvoiceList[i].DSStatus = "PENDING";

                if (allInvoiceList[i].PrintStatus == "TRUE")
                    allInvoiceList[i].PrintStatus = "PRINTED";
                else
                    allInvoiceList[i].PrintStatus = "PENDING";

                if (allInvoiceList[i].ApiStatus == "FALSE")
                    allInvoiceList[i].ApiStatus = "PENDING";
            }
            if (allInvoiceList.Count() > 0)
            {
                InvoiceList = new ObservableCollection<InvoiceDisplayModel>(allInvoiceList);
                ProcessMsg = "";
            }
            else
            {
                InvoiceList = new ObservableCollection<InvoiceDisplayModel>(allInvoiceList);
                DataMsg = "No Records Found";
                //ProcessMsg = "Scanning under progress...";
            }
        }

        private void CurrentRefreshLists()
        {
            //var allInvoiceList = objDSMModelData.GetAllInvoiceStatus();
            //var allInvoiceList = "1101";
            //QtList = new ObservableCollection<QuarantineDisplayModel>(objDSMModelData.GetQuarantineList());
            //for (int i = 0; i < allInvoiceList.Count; i++)
            //{
            //    if (allInvoiceList[i].DSStatus == "TRUE")
            //        allInvoiceList[i].DSStatus = "SIGNED";
            //    else
            //        allInvoiceList[i].DSStatus = "PENDING";

            //    if (allInvoiceList[i].PrintStatus == "TRUE")
            //        allInvoiceList[i].PrintStatus = "PRINTED";
            //    else
            //        allInvoiceList[i].PrintStatus = "PENDING";

            //    if (allInvoiceList[i].ApiStatus == "FALSE")
            //        allInvoiceList[i].ApiStatus = "PENDING";
            //}

            //InvNumber = x.InvNumber,
            //        InvDate = x.InvDate.Value,
            //        DSStatus = x.DSStatus,
            //        PrintStatus = x.PrintStatus,
            //        ApiStatus = x.APIStatus,
            //        ApiMsg = x.APIMessage,
            //        CreatedDate = x.UpdatedDate == null ? x.CreatedDate.Value : x.UpdatedDate.Value

            //List<InvoiceDisplayModel> CurInvList = new List<InvoiceDisplayModel>()
            //{
            //    //new InvoiceDisplayModel(){InvNumber = "001", DSStatus = "PENDING",PrintStatus="PENDING",ApiStatus="PENDING"}
            //    new InvoiceDisplayModel(){InvNumber = Global.InvoiceNumber,
            //        InvDate = Convert.ToDateTime(Global.InvoiceDate.ToString()),
            //        DSStatus = "PENDING",
            //        PrintStatus ="PENDING",
            //        ApiStatus ="PENDING"}

            //};

            //DateTime invdate= DateTime.ParseExact(Global.InvoiceDate.ToString().Replace(".", "/").Trim(), "d/M/yyyy", CultureInfo.InvariantCulture);
            //timer.Stop();
            List<InvoiceDisplayModel> CurInvList = new List<InvoiceDisplayModel>()
            {
                new InvoiceDisplayModel(){
                    CustomerName = "",
                    InvNumber ="",
                    PartNumber ="",
                    DSStatus = "",
                    PrintStatus =""
                    
                }

            };
            //List<InvoiceDisplayModel> CurInvList = new List<InvoiceDisplayModel>()
            //{
               

            //};


            var allInvoiceList = CurInvList;

            if (allInvoiceList.Count() > 0)
            {
                CurrentInvoiceList = new ObservableCollection<InvoiceDisplayModel>(allInvoiceList);
                ProcessMsg = "";
            }
            else
            {
                CurrentInvoiceList = new ObservableCollection<InvoiceDisplayModel>(allInvoiceList);
                //ProcessMsg = "Scanning under progress...";
            }
            //timer.Start();

        }

        private void CurrentRefreshInvoiceLists()
        {
            //timer.Stop();
            DateTime invdate = DateTime.Now;
            //timer.Stop();
            if (Global.InvoiceDate.Contains("."))
            {
                invdate = DateTime.ParseExact(Global.InvoiceDate.ToString().Replace(".", "/").Trim(), "d/M/yyyy", CultureInfo.InvariantCulture);
            }
            else if (Global.InvoiceDate.Contains("-"))
            {
                invdate = DateTime.ParseExact(Global.InvoiceDate.ToString().Replace("-", "/").Trim(), "d/M/yyyy", CultureInfo.InvariantCulture);
            }
            //DateTime invdate = DateTime.ParseExact(Global.InvoiceDate.ToString().Replace(".", "/").Trim(), "d/M/yyyy", CultureInfo.InvariantCulture);
            string invoicedate = invdate.ToString().Trim();
            List<InvoiceDisplayModel> CurInvList = new List<InvoiceDisplayModel>()
            {
                new InvoiceDisplayModel(){
                    CustomerName = Global.CustomerName,
                    InvNumber = Global.InvoiceNumber,
                    PartNumber = invoicedate,
                    DSStatus = "PENDING",
                    PrintStatus ="PENDING"
                   
                }

            };
          

            var allInvoiceList = CurInvList;

            if (allInvoiceList.Count() > 0)
            {
                CurrentInvoiceList = new ObservableCollection<InvoiceDisplayModel>(allInvoiceList);
                //ProcessMsg = "";
                ProcessMsg = "Scanning under progress...";
            }
            else
            {
                CurrentInvoiceList = new ObservableCollection<InvoiceDisplayModel>(allInvoiceList);
                ProcessMsg = "";
            }
            //timer.Start();
        }

        private void CurrentRefreshAlreadyInvoiceLists()
        {
            //timer.Stop();

            DateTime invdate = DateTime.ParseExact(Global.InvoiceDate.ToString().Replace(".", "/").Trim(), "d/M/yyyy", CultureInfo.InvariantCulture);
            string invoicedate = invdate.ToString().Trim();
            List<InvoiceDisplayModel> CurInvList = new List<InvoiceDisplayModel>()
            {
                new InvoiceDisplayModel(){
                    CustomerName = Global.CustomerName,
                    InvNumber = Global.InvoiceNumber,
                    PartNumber = invoicedate,
                    DSStatus = "Already Exist",
                    PrintStatus ="Already Exist"

                }

            };


            var allInvoiceList = CurInvList;

            if (allInvoiceList.Count() > 0)
            {
                CurrentInvoiceList = new ObservableCollection<InvoiceDisplayModel>(allInvoiceList);
                //ProcessMsg = "";
                ProcessMsg = "Scanning under progress...";
            }
            else
            {
                CurrentInvoiceList = new ObservableCollection<InvoiceDisplayModel>(allInvoiceList);
                ProcessMsg = "";
            }
            //timer.Start();
        }

        private void CurrentRefreshSignedLists()
        {
            //timer.Stop();
            DateTime invdate = DateTime.Now;
            //timer.Stop();
            if (Global.InvoiceDate.Contains("."))
            {
                invdate = DateTime.ParseExact(Global.InvoiceDate.ToString().Replace(".", "/").Trim(), "d/M/yyyy", CultureInfo.InvariantCulture);
            }
            else if (Global.InvoiceDate.Contains("-"))
            {
                invdate = DateTime.ParseExact(Global.InvoiceDate.ToString().Replace("-", "/").Trim(), "d/M/yyyy", CultureInfo.InvariantCulture);
            }
            //DateTime invdate = DateTime.ParseExact(Global.InvoiceDate.ToString().Replace(".", "/").Trim(), "d/M/yyyy", CultureInfo.InvariantCulture);
            string invoicedate = invdate.ToString().Trim();
            List<InvoiceDisplayModel> CurInvList = new List<InvoiceDisplayModel>()
            {
                new InvoiceDisplayModel(){
                    CustomerName = Global.CustomerName,
                    InvNumber = Global.InvoiceNumber,
                    PartNumber = invoicedate,
                    DSStatus = "SIGNED",
                    PrintStatus ="PENDING"
                    }

            };
           

            var allInvoiceList = CurInvList;

            if (allInvoiceList.Count() > 0)
            {
                CurrentInvoiceList = new ObservableCollection<InvoiceDisplayModel>(allInvoiceList);
                //ProcessMsg = "";
                ProcessMsg = "Scanning under progress...";
            }
            else
            {
                CurrentInvoiceList = new ObservableCollection<InvoiceDisplayModel>(allInvoiceList);
                ProcessMsg = "";
            }
            //timer.Start();
        }

        private void CurrentRefreshPrintedLists()
        {

            //timer.Stop();
            DateTime invdate = DateTime.Now;
            //timer.Stop();
            if (Global.InvoiceDate.Contains("."))
            {
                invdate = DateTime.ParseExact(Global.InvoiceDate.ToString().Replace(".", "/").Trim(), "d/M/yyyy", CultureInfo.InvariantCulture);
            }
            else if (Global.InvoiceDate.Contains("-"))
            {
                invdate = DateTime.ParseExact(Global.InvoiceDate.ToString().Replace("-", "/").Trim(), "d/M/yyyy", CultureInfo.InvariantCulture);
            }
            //DateTime invdate = DateTime.ParseExact(Global.InvoiceDate.ToString().Replace(".", "/").Trim(), "d/M/yyyy", CultureInfo.InvariantCulture);
            string invoicedate = invdate.ToString().Trim();
            //EthernetConnectivityCheck();
            List<InvoiceDisplayModel> CurInvList = new List<InvoiceDisplayModel>()
            {
                new InvoiceDisplayModel(){
                    CustomerName = Global.CustomerName,
                    InvNumber = Global.InvoiceNumber,
                    PartNumber = invoicedate,
                    DSStatus = "SIGNED",
                    PrintStatus ="PRINTED"
                    }

            };
           

            var allInvoiceList = CurInvList;

            if (allInvoiceList.Count() > 0)
            {
                CurrentInvoiceList = new ObservableCollection<InvoiceDisplayModel>(allInvoiceList);
                //ProcessMsg = "";
                ProcessMsg = "Scanning under progress...";
            }
            else
            {
                CurrentInvoiceList = new ObservableCollection<InvoiceDisplayModel>(allInvoiceList);
                ProcessMsg = "";
            }
            //timer.Start();
        }

        public void StartProcess(object sender, EventArgs e)
        {
            timer.Stop();
            CurrentRefreshLists();
            RefreshLists();
            CheckApiExeRunOrNot();
            //TML
            ReadPDFTml();

            //End TML
            //Normal Function
            //ReadPDF();
            //SignPDF();
            //End
            CurrentRefreshLists();
            timer.Start();
        }
        public void read()
        {
           
        }
        public void TML()
        {
            API_Authentication();
            ASN_Generation();
        }
          public void ASN_Generation()
        {
            try
            {


                Get_Auth_ASN_Details();
                //MessageBox.Show("ASN_Generation");
                SqlDataReader rdr;
                string ASNTokRes = "";
                Gstin = "33AAACS4920J1ZJ";

                string CurrentDt = DateTime.Now.ToString("yyyy-MM-dd");
                SqlConnection conn = new SqlConnection(Utilities._sConString);
                // string query = "select * from INVOICE where CONVERT(varchar(10),TIME_STAMP, 126) = '" + CurrentDt + "' AND API_STATUS ='PENDING' ";
                // string query = "select EINV_VIEW_HMI.CustomerPO,EINV_VIEW_HMI.qty,EINV_VIEW_HMI.No,EINV_VIEW_HMI.unit,EINV_VIEW_HMI.ass_val,EINV_VIEW_HMI.VendorCode,EINV_VIEW_HMI.sgst_amt,EINV_VIEW_HMI.cgst_amt,EINV_VIEW_HMI.igst_amt,EINV_VIEW_HMI.Dt,EINV_VIEW_HMI.Ship_Gstin,EINV_VIEW_HMI.GstRt,Output_Table.VEHNO,Output_Table.IRN_NO,Output_Table.APIStatus,Output_Table.Id from Output_Table INNER JOIN EINV_VIEW_HMI on Output_Table.DOCNO = EINV_VIEW_HMI.No where Output_Table.APIStatus = 'FALSE'";
                string query = "select * from INVOICE where CONVERT(varchar(10),CreatedDate, 126) = '" + CurrentDt + "' AND APIStatus ='FALSE'";

                rdr = Utilities.executeQuery(query);
                Utilities.LogFile(query);
                if (rdr.HasRows)
                {
                    //MessageBox.Show("ASN Data Has Rows");
                    while (rdr.Read())
                    {
                        //MessageBox.Show("Reader read");
                        PORefr = rdr["PONumber"].ToString();
                        SINo = rdr["InvoiceId"].ToString();
                        Qty = rdr["InvQuantity"].ToString();
                        Inv_No = rdr["InvNumber"].ToString();
                        Inv_Dt = Convert.ToDateTime(rdr["InvDate"]).ToString("dd.MM.yyyy");
                        UnitPrice = rdr["UnitPrice"].ToString();
                        NetUnitPrice = rdr["UnitPrice"].ToString();
                        TotAmt = rdr["AssessableValue"].ToString();
                        AssAmt = rdr["AssessableValue"].ToString();
                        SgstVal = rdr["SGSTrate"].ToString();
                        CgstVal = rdr["CGSTrate"].ToString();
                        IgstVal = rdr["IGSTrate"].ToString();
                        if (SgstVal == "") { SgstVal = "0"; }
                        if (CgstVal == "") { CgstVal = "0"; }
                        if (IgstVal == "") { IgstVal = "0"; }
                        decimal tottaxval = Convert.ToDecimal(SgstVal) + Convert.ToDecimal(CgstVal) + Convert.ToDecimal(IgstVal);

                        if ((rdr["SGSTrate"].ToString() != "0.00") || (rdr["CGSTrate"].ToString() != "0.00"))
                        {
                            decimal gst = Convert.ToDecimal(rdr["IGSTrate"].ToString());
                            gst = gst / 2;
                            SGstRt = gst.ToString();
                            CGstRt = gst.ToString();
                            IGstRt = "0";
                        }
                        else
                        {
                            decimal gst = Convert.ToDecimal(rdr["IGSTrate"].ToString());
                            SGstRt = "0";
                            CGstRt = "0";
                            IGstRt = gst.ToString();
                        }


                        decimal totIT = tottaxval + Convert.ToDecimal(AssAmt.ToString());
                        TotItemVal = totIT.ToString();
                        ForCur = "INR";
                        Sup_Gstin = rdr["GSTN"].ToString();
                        VehNo = rdr["VehicleNumber"].ToString();
                        Part_Rev = "A";
                        VendorCode = rdr["VendorCode"].ToString();
                        Buy_Gstin = "33AAACS4920J1ZJ";
                        Irn = rdr["E_InvNo"].ToString();

                        string Qrycount_auth = "select Auth_Token from TEMP_AUTH_TOKEN";
                        SqlDataAdapter sda1 = new SqlDataAdapter(Qrycount_auth, Utilities._sConString);
                        DataTable dt1 = new DataTable();
                        sda1.Fill(dt1);
                        if (dt1.Rows.Count > 0)
                        {
                            Auth_Token = dt1.Rows[0]["Auth_Token"].ToString();
                        }

                        Utilities.LogFile(ASN_URL);
                        var httpWebRequest = (HttpWebRequest)WebRequest.Create(ASN_URL);
                        httpWebRequest.ContentType = "application/json";
                        httpWebRequest.Method = "POST";
                        httpWebRequest.AllowAutoRedirect = false;
                        httpWebRequest.KeepAlive = false;
                        httpWebRequest.Timeout = 60 * 10000;
                        httpWebRequest.ReadWriteTimeout = 30000;

                        httpWebRequest.Headers["gstin"] = Gstin.ToString().Trim();
                        httpWebRequest.Headers["clientcode"] = Client_Code.ToString().Trim();
                        httpWebRequest.Headers["Authorization"] = "Bearer " + Auth_Token;
                        //httpWebRequest.UseDefaultCredentials = true;
                        //httpWebRequest.PreAuthenticate = true;
                        //httpWebRequest.Credentials = CredentialCache.DefaultCredentials;

                        using (var streamWriter = new

                        StreamWriter(httpWebRequest.GetRequestStream()))
                        {
                            string quote = "\"";
                            string ASNjsonSchema = @"{
                                                " + "\n" +
                                                    @"""PORefr"": " + quote + PORefr.ToString().Trim() + quote + ","
                                                     + "\n" +
                                                    @"""SlNo"": " + quote + SINo.ToString().Trim() + quote + ","
                                                     + "\n" +
                                                    @"""Qty"": " + Qty.ToString().Trim() + ","
                                                     + "\n" +
                                                    @"""Inv_No"": " + quote + Inv_No.ToString().Trim() + quote + ","
                                                     + "\n" +
                                                    @"""Inv_Dt"": " + quote + Inv_Dt + quote + ","
                                                     + "\n" +
                                                    @"""UnitPrice"": " + quote + UnitPrice.ToString().Trim() + quote + ","
                                                     + "\n" +
                                                    @"""NetUnitPrice"": " + quote + NetUnitPrice.ToString().Trim() + quote + ","
                                                     + "\n" +
                                                    @"""TotAmt"": " + quote + TotAmt.ToString().Trim() + quote + ","
                                                     + "\n" +
                                                    @"""AssAmt"": " + quote + AssAmt.ToString().Trim() + quote + ","
                                                     + "\n" +
                                                    @"""SgstVal"": " + quote + SgstVal.ToString().Trim() + quote + ","
                                                     + "\n" +
                                                    @"""CgstVal"": " + quote + CgstVal.ToString().Trim() + quote + ","
                                                     + "\n" +
                                                    @"""IgstVal"": " + quote + IgstVal.ToString().Trim() + quote + ","
                                                     + "\n" +
                                                    @"""CGstRt"": " + quote + CGstRt.ToString().Trim() + quote + ","
                                                     + "\n" +
                                                    @"""SGstRt"": " + quote + SGstRt.ToString().Trim() + quote + ","
                                                     + "\n" +
                                                    @"""IGstRt"": " + quote + IGstRt.ToString().Trim() + quote + ","
                                                     + "\n" +
                                                    @"""TotItemVal"": " + quote + TotItemVal.ToString().Trim() + quote + ","
                                                     + "\n" +
                                                    @"""ForCur"": " + quote + ForCur.ToString().Trim() + quote + ","
                                                     + "\n" +
                                                    @"""Sup_Gstin"": " + quote + Sup_Gstin.ToString().Trim() + quote + ","
                                                     + "\n" +
                                                    @"""VehNo"": " + quote + VehNo.ToString().Trim() + quote + ","
                                                     + "\n" +
                                                    @"""Part_Rev"": " + quote + Part_Rev.ToString().Trim() + quote + ","
                                                     + "\n" +
                                                    @"""Buy_Gstin"": " + quote + Buy_Gstin.ToString().Trim() + quote + ","
                                                     + "\n" +
                                                    @"""Irn"": " + quote + Irn.ToString().Trim() + quote + ","
                                                     + "\n" +
                                                    @"""VendorCode"": " + quote + VendorCode.ToString().Trim() + quote + ""
                                                     + "\n" +
                                                    @"}
                                                " + "\n" +
                                                    @"";

                            string json = ASNjsonSchema.ToString().Trim();
                            streamWriter.Write(json);
                            Utilities.LogFile(json);
                        }

                        ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                        System.Net.ServicePointManager.Expect100Continue = false;

                        using (HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                        {
                            // Do your processings here....
                            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                            {
                                var asnresult = streamReader.ReadToEnd();
                                ASNTokRes = asnresult.ToString().Trim();
                                Utilities.LogFile(ASNTokRes);
                                string[] asn_split = ASNTokRes.Split(',');
                                string split_val = asn_split[1].ToString();
                                string[] split_asn_val = split_val.Split(':');
                                ASN_Number = split_asn_val[2].Replace('\"', ' ').Trim();
                                Utilities.LogFile(ASNTokRes);
                            }

                            if (ASN_Number.Length == 10)
                            {
                                string update_success_qry = "update Invoice set  APIStatus='SUCCESS', ASN_NUMBER='" + ASN_Number + "' where InvNumber='" + Inv_No + "'";
                                bool getres = Utilities.executeNonQuery(update_success_qry);
                                if (getres)
                                {
                                    string msg_asnsuccess = "ASN Data Posted Successfully!!!";
                                    Utilities.LogFile(msg_asnsuccess);
                                }
                            }
                            else
                            {
                                string update_fail_qry = "update Invoice set APIStatus='FAILED' where InvNumber='" + Inv_No + "'";
                                bool getres = Utilities.executeNonQuery(update_fail_qry);
                                if (getres)
                                {
                                    string msg_asnfailed = "ASN Data Posted Failed!!!";
                                    Utilities.LogFile(msg_asnfailed);
                                }
                                //string delete_fail_inv = "delete from INVOICE where H_INVOICE_NO='" + Inv_No + "'";
                                //bool del_res = Utilities.executeNonQuery(delete_fail_inv);
                                //if (getres)
                                //{
                                //    string msg_asnfailed = "ASN Failed Data Deleted Successfully!!!";
                                //    Utilities.LogFile(msg_asnfailed);
                                //}
                            }
                        }

                    }
                }
            }
            catch (WebException ex)
            {
                Utilities.LogError(ex);
            }
            catch (Exception ex)
            {
                Utilities.LogError(ex);
            }
        }
        public void Get_Auth_ASN_Details()
        {
            try
            {
                //MessageBox.Show("FETCH ASN_API_MASTER");
                string QryAuth_ASN_URL = "select * from ASN_API_MASTER where STATUS=1";
                SqlDataAdapter sda = new SqlDataAdapter(QryAuth_ASN_URL, Utilities._sConString);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    Auth_URL = dt.Rows[0]["AUTH_URL"].ToString();
                    ASN_URL = dt.Rows[0]["ASN_URL"].ToString();
                    Username = dt.Rows[0]["USER_NAME"].ToString();
                    Password = dt.Rows[0]["PASSWORD"].ToString();
                    CLIENTID = dt.Rows[0]["client_id"].ToString();
                    grant_type = dt.Rows[0]["grant_type"].ToString();
                    client_secret = dt.Rows[0]["client_secret"].ToString();
                    Client_Code = dt.Rows[0]["Customer_Code"].ToString();
                    //MessageBox.Show("GOT ASN_API_MASTER");
                }
            }
            catch (Exception ex)
            {

            }

            //Auth_URL = "https://g11.tcsgsp.in/Tax-Tool-Core/services/accessMgmt/generateToken";
            //ASN_URL = "https://g11.tcsgsp.in/Tax-Tool-Core/services/auth/einvapi/generateasn";
            //Username = "api_user@mando.com";
            //Password = "Tcs@1234";
        }
        public void API_Authentication()
        {
            try
            {


                Get_Auth_ASN_Details();
                string grant_type = "password";
                string JRes = "";
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(Auth_URL);
                httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                httpWebRequest.Method = "POST";
                httpWebRequest.AllowAutoRedirect = false;
                httpWebRequest.KeepAlive = false;
                httpWebRequest.Timeout = 60 * 10000;
                httpWebRequest.ReadWriteTimeout = 30000;

                string url = "application/x-www-form-urlencoded";
                //httpWebRequest.Headers["client_id"] = CLIENTID;
                //httpWebRequest.Headers["grant_type"] = grant_type;
                //httpWebRequest.Headers["Username"] = Username.ToString().Trim();
                //httpWebRequest.Headers["Password"] = Password.ToString().Trim();
                //httpWebRequest.Headers["client_secret"] = client_secret;

                //WebRequest request = WebRequest.Create(Auth_URL);
                //request.Method = "POST";
                //string postData = "client_id='"+ CLIENTID.ToString().Trim() + "' & grant_type= '" + grant_type.ToString().Trim() + "' & Username='"+ Username.ToString().Trim() + "' & Password='" + Password.ToString().Trim() + "' & client_secret='" + client_secret.ToString().Trim() + "' & Content-Type = application/x-www-form-urlencoded";
                //byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                //request.ContentType = "application/x-www-form-urlencoded";
                //request.ContentLength = byteArray.Length;
                //Stream dataStream = request.GetRequestStream();
                //dataStream.Write(byteArray, 0, byteArray.Length);
                //dataStream.Close();
                //var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
                ////WebResponse response = request.GetResponse();
                ////Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                //dataStream = response.GetResponseStream();
                //StreamReader reader = new StreamReader(dataStream);
                //string responseFromServer = reader.ReadToEnd();
                //Console.WriteLine(responseFromServer);
                //reader.Close();
                //dataStream.Close();
                //response.Close();


                using (var streamWriter = new

                        StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string quote = "\"";
                    string ASNjsonSchema = @"{"  + "\n" +
                                                 @"""Content-Type"": " + quote + url.ToString().Trim() + quote + ","
                                                 + "\n" +
                                                 @"""client_id"": " + quote + CLIENTID.ToString().Trim() + quote + ","
                                                 + "\n" +
                                                 @"""grant_type"": " + quote + grant_type.ToString().Trim() + quote + ","
                                                 + "\n" +
                                                 @"""Username"": " + quote + Username.ToString().Trim() + quote + ","
                                                 + "\n" +
                                                 @"""Password"": " + quote + Password.ToString().Trim() + quote + ","
                                                 + "\n" +
                                                 @"""client_secret"": " + quote + client_secret.ToString().Trim() + quote + ""
                                                     + "\n" +
                                                    @"}
                                                " + "\n" +
                                                    @"";
                    

                    string json = ASNjsonSchema.ToString().Trim();
                    streamWriter.Write(json);
                    Utilities.LogFile(json);
                }

                //using (var streamWriter = new

                //    StreamWriter(httpWebRequest.GetRequestStream()))
                //{
                //    string s = reader.ReadToEnd();
                //    //string quote = "\"";
                //    //string AUTH_jsonSchema = @"{
                //    //                            " + "\n" +
                //    //                             @"""client_id"": " + quote + url.ToString().Trim() + quote + ","
                //    //                             + "\n" +
                //    //                             @"""client_id"": " + quote + CLIENTID.ToString().Trim() + quote + ","
                //    //                             + "\n" +
                //    //                             @"""grant_type"": " + quote + grant_type.ToString().Trim() + quote + ","
                //    //                             + "\n" +
                //    //                             @"""Username"": " + quote + Username.ToString().Trim() + quote + ","
                //    //                             + "\n" +
                //    //                             @"""Password"": " + quote + Password.ToString().Trim() + quote + ","
                //    //                             + "\n" +
                //    //                             @"""client_secret"": " + quote + client_secret.ToString().Trim() + quote + ","
                //    //                             + "\n" +
                //    //                             @"}
                //    //                             " + "\n" +
                //    //                             @"";

                //    string json = s.ToString().Trim();
                //    streamWriter.Write(json);
                //    Utilities.LogFile(json);
                //}



                ServicePointManager.SecurityProtocol = (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                System.Net.ServicePointManager.Expect100Continue = false;

                using (HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        JRes = result.ToString().Trim();
                        //MessageBox.Show(result.ToString().Trim());
                        string[] token_split = JRes.Split(',');
                        string[] token_val = token_split[0].Split(':');
                        string bearer_token = token_val[1].Replace("\"", " ").Trim();

                        string Qrycount_auth = "select * from TEMP_AUTH_TOKEN";
                        SqlDataAdapter sda = new SqlDataAdapter(Qrycount_auth, Utilities._sConString);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            string upd_auth_qry = "update TEMP_AUTH_TOKEN set Auth_Token='" + bearer_token + "'";
                            bool getres = Utilities.executeNonQuery(upd_auth_qry);
                        }
                        else
                        {
                            string insert_auth_qry = "insert into TEMP_AUTH_TOKEN values('" + bearer_token + "')";
                            bool getres = Utilities.executeNonQuery(insert_auth_qry);
                        }
                        if (JRes.Contains("SUCCESS"))
                        {
                            string msg_authsuccess = "Authenticated Success!!!";
                            Utilities.LogFile(msg_authsuccess);
                            //MessageBox.Show(msg_authsuccess);
                        }
                        else
                        {
                            string msg_authfailed = "Authenticated Failed!!!";
                            Utilities.LogFile(msg_authfailed);
                            //MessageBox.Show("Authentication Failed!!!");

                        }
                    }


                }

            }

            catch (WebException ex)
            {
                Utilities.LogError(ex);
            }

            catch (Exception ex)
            {
                Utilities.LogError(ex);
            }
        }

        private void ReadPDFTml()
        {
            try
            {
                //string[] inputPath = Directory.GetFiles(Global.InputPath, "*.pdf");
                //string[] outputPath = Directory.GetFiles(Global.OutputPath, "*.pdf");
                string[] invPath = Directory.GetFiles(Global.InvoicePath, "*.pdf");
                //for (int j = 0; j < invPath.Length; j++)
                //{

                if (invPath.Length > 0)
                {
                    String fileName = invPath[0];
                    AssignInvoiceNo_TruckNo(invPath[0], "INV");

                    if (!string.IsNullOrEmpty(Global.InvoiceDate))
                    {
                        string FinYear = GetFinancialYear(Global.InvoiceDate.ToString().Trim());
                        Global.FinYear = Convert.ToInt32(FinYear.ToString().Trim());
                        CurrentRefreshInvoiceLists();
                        //CurrentInvoiceList.Items.Refresh();
                        //EthernetConnectivityCheck();
                        objDSMModelData = new DSMModelData();
                        int inv_count_rec = objDSMModelData.GetInvoiceDetailsByInvoiceNumberFinYear(Global.InvoiceNumber, Convert.ToInt32(FinYear)).Count;
                        if (inv_count_rec == 0)
                        {
                            if (Global.CustomerName =="TATA")
                            {

                                CreateInvoice();
                                TML();

                            }
                            else
                            {
                                try
                                {
                                   MessageBox.Show("Other Customer Not a Tata");
                                }
                                catch (Exception i)
                                {
                                    LogErrors(i);
                                    timer.Start();
                                }
                            }

                        }
                        else
                        {
                            MoveFile(invPath[0], "AlreadyExist");
                            CurrentRefreshAlreadyInvoiceLists();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogErrors(ex);
                timer.Start();
            }
        }

        private void APIPostFiles(object sender, EventArgs e)
        {
            try
            {
                //EthernetConnectivityCheck();
                timer.Stop();
                //timerAPIPost.Stop();
                RefreshServerLists();
                DSMModelData objDSMModelData = new DSMModelData();
                

                ReprintCommand = new DelegateCommand(Reprint);

                int Finyear = 0;
                string prefix_invoice_no = "";
                int fin_mon = 0;
                string _sQry_Prefix_invoice_no = "select month(getdate()) as cur_month, year(getdate()) as cur_year";
                SqlDataReader _rdr_Prefix_invoice_no = Global.executeQuery(_sQry_Prefix_invoice_no);
                if (_rdr_Prefix_invoice_no.Read())
                {
                    string year = _rdr_Prefix_invoice_no["cur_year"].ToString().Substring(2, 2);
                    fin_mon = 3;
                    if (int.Parse(_rdr_Prefix_invoice_no["cur_month"].ToString()) > fin_mon)
                        prefix_invoice_no = year + (int.Parse(year) + 1);
                    else
                        prefix_invoice_no = (int.Parse(year) - 1) + (year);
                }
                
                Finyear = Convert.ToInt32(prefix_invoice_no.ToString().Trim());

                Global.APIFinYear = Convert.ToInt32(prefix_invoice_no.ToString().Trim());

                var lstApiInv = objDSMModelData.GetAllAPIInvoiceStatusAll(Finyear).ToList();

               
                for (int i = 0; i < lstApiInv.Count; i++)

                {
                    if (lstApiInv != null)
                    {
                       
                        if (Global.IsAPIpost)
                        {
                            if (Global.APIpostType == "AUTO")
                            {
                                APIPost(lstApiInv[i].InvNumber.ToString().Trim());
                            }
                            else
                            {
                               
                                objDSMModelData.UpdateInvoiceAPIstatusByInvoiceNoFinYear(lstApiInv[i].InvNumber.ToString().Trim(), Global.APIpostType, "Manual Posting", Finyear);
                            }
                        }
                        RefreshServerLists();
                    }
                }
              
            }
            catch (Exception ex)
            {
                LogErrors(ex);
                timer.Start();
                //timerAPIPost.Start();
            }
        }
        private void APIPostFiles()
        {
            try
            {
                //EthernetConnectivityCheck();
                //timerAPIPost.Stop();
                //efreshServerLists();
                DSMModelData objDSMModelData = new DSMModelData();
                //var lstApiInv = objDSMModelData.GetAllInvoiceStatusAll().Where(x => x.DSStatus == "TRUE" && x.PrintStatus == "FALSE").FirstOrDefault();
                //var lstApiInv = objDSMModelData.GetAllAPIInvoiceStatusAll().Where(x => x.DSStatus == "TRUE" && x.PrintStatus == "FALSE").FirstOrDefault();
                //var lstApiInv = objDSMModelData.GetAllAPIInvoiceStatusAll().ToList();

                ReprintCommand = new DelegateCommand(Reprint);

                int Finyear = 0;
                string prefix_invoice_no = "";
                int fin_mon = 0;
                string _sQry_Prefix_invoice_no = "select month(getdate()) as cur_month, year(getdate()) as cur_year";
                SqlDataReader _rdr_Prefix_invoice_no = Global.executeQuery(_sQry_Prefix_invoice_no);
                if (_rdr_Prefix_invoice_no.Read())
                {
                    string year = _rdr_Prefix_invoice_no["cur_year"].ToString().Substring(2, 2);
                    fin_mon = 3;
                    if (int.Parse(_rdr_Prefix_invoice_no["cur_month"].ToString()) > fin_mon)
                        prefix_invoice_no = year + (int.Parse(year) + 1);
                    else
                        prefix_invoice_no = (int.Parse(year) - 1) + (year);
                }
                //string _sQry_Prefix_invoice_no = "select month(getdate()) as cur_month, FORMAT(getdate(), 'yy') as cur_year";
                //SqlDataReader _rdr_Prefix_invoice_no = Global.executeQuery(_sQry_Prefix_invoice_no);
                //if (_rdr_Prefix_invoice_no.Read())
                //{
                //    fin_mon = 3;
                //    if (int.Parse(_rdr_Prefix_invoice_no["cur_month"].ToString()) > fin_mon)
                //        prefix_invoice_no = _rdr_Prefix_invoice_no["cur_year"].ToString() + (int.Parse(_rdr_Prefix_invoice_no["cur_year"].ToString()) + 1);
                //    else
                //        prefix_invoice_no = (int.Parse(_rdr_Prefix_invoice_no["cur_year"].ToString()) - 1) + (_rdr_Prefix_invoice_no["cur_year"].ToString());
                //}
                //_inv.Finyear = Convert.ToInt32(prefix_invoice_no.ToString().Trim());
                Finyear = Convert.ToInt32(prefix_invoice_no.ToString().Trim());

                Global.APIFinYear = Convert.ToInt32(prefix_invoice_no.ToString().Trim());

                var lstApiInv = objDSMModelData.GetAllAPIInvoiceStatusAll(Finyear).ToList();

                //for (var i = 0; i < lstPrint.Count; i++)
                //{
                for (int i = 0; i < lstApiInv.Count; i++)

                {
                    if (lstApiInv != null)
                    {
                        //Print objPrint = new Print();
                        //var printStatus = objPrint.PrintInvoicePDFs(lstApiInv.InvNumber, "invoice");
                        //objDSMModelData.UpdateInvoicePrintstatusByInvoiceNo(lstApiInv.InvNumber, printStatus == true ? "TRUE" : "FALSE");

                        //var lstapiinvno= objDSMModelData.get
                        if (Global.IsAPIpost)
                        {
                            if (Global.APIpostType == "AUTO")
                            {
                                APIPost(lstApiInv[i].InvNumber.ToString().Trim());
                            }
                            else
                            {
                                //objDSMModelData.UpdateInvoiceAPIstatusByInvoiceNo(lstApiInv[i].InvNumber.ToString().Trim(), Global.APIpostType, "Manual Posting");
                                objDSMModelData.UpdateInvoiceAPIstatusByInvoiceNoFinYear(lstApiInv[i].InvNumber.ToString().Trim(), Global.APIpostType, "Manual Posting", Finyear);
                            }
                        }
                        RefreshServerLists();
                    }
                }
                //}
                timer.Start();
                //timerAPIPost.Start();
            }
            catch (Exception ex)
            {
                LogErrors(ex);
                timer.Start();
                //timerAPIPost.Start();
            }
        }
        public void APIPost(string InvNumber)
        {
            try
            {
                Global.APIInvoiceNumber = InvNumber;
                var lstDS = objDSMModelData.GetSelectedInvoice(InvNumber).Where(x => x.CustomerName.ToUpper() == "HYUNDAI").FirstOrDefault();
                if (lstDS != null)
                {
                    Global.CustomerName = "HYUNDAI";
                    Global.APIInvoiceNumber = InvNumber;
                    CurrentInvoice = "Invoice - " + Global.APIInvoiceNumber + " is Posting...";
                    CustomerDisplayModel objCustomer = new CustomerDisplayModel();
                    objCustomer = objDSMModelData.GetCustomerByName(Global.CustomerName);

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
                    }
                    else
                    {
                        Global.IsAPIpost = false;
                    }


                    Global.APIInvoiceNumber = InvNumber;

                    try
                    {


                        ExampleAPIProxy exProxy = new ExampleAPIProxy();

                        WebService webService = new WebService(Global.APIurl, "getData");


                        webService.Invoke();
                    }
                    catch (Exception ex)
                    {
                        LogErrors(ex);
                        timer.Start();
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
                            if (Global.APIurl == "https://kininvoice.kiaindia.net:91/Service.asmx?WSDL")
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
                            LogErrors(ex);

                            //timerAPIPost.Start();
                        }
                    }
                    else
                    { }
                }
            }
            catch (Exception ex)
            {
                LogErrors(ex);
            }
        }


        private void PickPDF(object sender, EventArgs e)
        {
            try
            {
                //timerPick.Stop();
                //RefreshLists();
                string[] invoicePathList = Directory.GetFiles(Global.InvoicePath, "*.pdf");
                if (invoicePathList.Count() == 0)
                {
                    ProcessMsg = "";
                }
                PendingInvList = new ObservableCollection<InvoiceDisplayModel>();
                for (int i = 0; i < invoicePathList.Length; i++)
                {
                    //AssignInvoiceNo_TruckNo(invoicePathList[i], "INV");
                    SplitAndSave(invoicePathList[i], Global.InputPath, Global.InvoiceNumber, "INVOICE");
                }
                //timerPick.Start();
            }
            catch (Exception ex)
            {
                LogErrors(ex);
                timerPick.Start();
            }
        }

        //[DllImport("gsdll32.dll", EntryPoint = "gsapi_new_instance")]
        // [DllImport("gsdll32.dll", EntryPoint = "gsapi_new_instance")] 
        //private void ReadPDF()
        //{
        //    try
        //    {
        //        string[] inputPath = Directory.GetFiles(Global.InvoicePath, "*.pdf");
        //        for (int j = 0; j < inputPath.Length; j++)
        //        {
        //            String fileName = inputPath[j];
        //            System.IO.FileInfo input = new FileInfo(fileName);
        //            AssignInvoiceNo_TruckNo(inputPath[j], "INV");
        //            AssignInvoiceNo_TruckNo(inputPath[j], "TRUCK");
        //            Global.TruckNumber = Global.TruckNumber == null ? "" : Global.TruckNumber;
        //            string directoryPath = System.IO.Path.GetDirectoryName(fileName) + "\\" + Path.GetFileNameWithoutExtension(input.Name);
        //            if (!Directory.Exists(directoryPath))
        //                Directory.CreateDirectory(directoryPath);
        //            Spire.Pdf.PdfDocument doc = new Spire.Pdf.PdfDocument();
        //            doc.LoadFromFile(inputPath[j]);
        //            Spire.Pdf.PdfPageBase page = doc.Pages[0];
        //            System.Drawing.Image[] images = page.ExtractImages();
        //            string output = string.Format("{0}\\{1}{2}", directoryPath, Path.GetFileNameWithoutExtension(input.Name), ".jpg");
        //            foreach (System.Drawing.Image image in images)
        //            {
        //                try
        //                {
        //                    string QRCodeString = GetQRCodeString(image, output, directoryPath, inputPath[j]);
        //                    if (string.IsNullOrEmpty(QRCodeString))
        //                    {
        //                        continue;
        //                    }
        //                    CreateInvoice(QRCodeString);
        //                    SplitAndSave(inputPath[j], Global.InputPath, Global.InvoiceNumber, "INVOICE");
        //                    if (Directory.Exists(directoryPath))
        //                        Directory.Delete(directoryPath, true);
        //                }
        //                catch (Exception i)
        //                {
        //                    LogErrors(i);

        //                    if (Directory.Exists(directoryPath))
        //                        Directory.Delete(directoryPath, true);

        //                    MoveFile(inputPath[j], "INVALID_INVOICE");
        //                    objDSMModelData = new DSMModelData();
        //                    objDSMModelData.SaveQuarantine(Global.InvoiceNumber);
        //                    timer.Start();
        //                }
        //                break;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogErrors(ex);
        //        timer.Start();
        //    }
        //}

        private void ReadPDF()
        {
            try
            { 
                CustomerDisplayModel objCustomer = new CustomerDisplayModel();
                if (invPath.Length > 0)
                {
                    string[] invPath = Directory.GetFiles(Global.InvoicePath, "*.pdf");
                    Clientname(invPath[0]);
                    if (Global.CustomerName == "HYUNDAI")
                    {
                        objDigitalSign = new DigitalSign();
                        var keyConnected = objDigitalSign.CheckKeyConnected(Global.CustomerName);
                        //var keyConnected = true;

                        if (keyConnected == false)
                        {
                            MessageBox.Show("Check whether the key is connected or not!");
                            App.Current.MainWindow.Close();
                            for (int intCounter = App.Current.Windows.Count - 1; intCounter > 0; intCounter--)
                                App.Current.Windows[intCounter].Close();
                            //App.Current.Windows[1].Close();
                            iskeycheck = false;
                        }

                        else
                        {
                            //EthernetConnectivityCheck();
                            if (isAssignedcoord && iskeycheck)
                            {

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
                                    }
                                    else
                                    {
                                        Global.IsAPIpost = false;
                                    }
                                }
                            }
                        }

                    }
                    else if (Global.CustomerName.ToUpper() == "KIA")
                    {

                        objDigitalSign = new DigitalSign();
                        var keyConnected = objDigitalSign.CheckKeyConnected(Global.CustomerName);
                        //var keyConnected = true;

                        if (keyConnected == false)
                        {
                            MessageBox.Show("Check whether the key is connected or not!");
                            App.Current.MainWindow.Close();
                            for (int intCounter = App.Current.Windows.Count - 1; intCounter > 0; intCounter--)
                                App.Current.Windows[intCounter].Close();
                            //App.Current.Windows[1].Close();
                            iskeycheck = false;
                        }

                        else
                        {
                            //EthernetConnectivityCheck();
                            if (isAssignedcoord && iskeycheck)
                            {

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
                                    }
                                    else
                                    {
                                        Global.IsAPIpost = false;
                                    }
                                }
                            }
                        }
                    }
                    String fileName = invPath[0];
                    //Clientname(invPath[0]);

                    AssignInvoiceNo_TruckNo(invPath[0], "INV");
                    if (!string.IsNullOrEmpty(Global.InvoiceDate))
                    {
                        string FinYear = GetFinancialYear(Global.InvoiceDate.ToString().Trim());
                        Global.FinYear = Convert.ToInt32(FinYear.ToString().Trim());
                        CurrentRefreshInvoiceLists();
                        //CurrentInvoiceList.Items.Refresh();
                        //EthernetConnectivityCheck();
                        objDSMModelData = new DSMModelData();
                        int inv_count_rec = objDSMModelData.GetInvoiceDetailsByInvoiceNumberFinYear(Global.InvoiceNumber, Convert.ToInt32(FinYear)).Count;
                        if (inv_count_rec == 0)
                        {
                            if (Global.CustomerName == "KIA" || Global.CustomerName == "HYUNDAI")
                            {
                                ////System.IO.FileInfo input = new FileInfo(fileName);
                                ////string directoryPath = System.IO.Path.GetDirectoryName(fileName) + "\\" + System.IO.Path.GetFileNameWithoutExtension(input.Name);
                                ////if (!Directory.Exists(directoryPath))
                                ////    Directory.CreateDirectory(directoryPath);
                                ////Spire.Pdf.PdfDocument doc = new Spire.Pdf.PdfDocument();
                                ////doc.LoadFromFile(fileName);
                                ////System.Drawing.Image bmp = doc.SaveAsImage(0);
                                ////System.Drawing.Image emf = doc.SaveAsImage(0, Spire.Pdf.Graphics.PdfImageType.Metafile);
                                ////System.Drawing.Image zoomImg = new Bitmap((int)(emf.Size.Width * 2), (int)(emf.Size.Height * 2));
                                ////using (Graphics g = Graphics.FromImage(zoomImg))
                                ////{
                                ////    g.ScaleTransform(2.0f, 2.0f);
                                ////    g.DrawImage(emf, new System.Drawing.Rectangle(new System.Drawing.Point(0, 0), emf.Size), new System.Drawing.Rectangle(new System.Drawing.Point(0, 0), emf.Size), GraphicsUnit.Pixel);
                                ////}
                                ////zoomImg.Save(directoryPath + "\\" + input.Name + ".png", ImageFormat.Png);

                                ////string[] datas = OnBarcode.Barcode.BarcodeScanner.BarcodeScanner.Scan(directoryPath + "\\" + input.Name + ".png", BarcodeType.QRCode);
                                ////if(datas.Length>0 && datas!=null)
                                ////QRCodeString = datas[0];

                                ////if (QRCodeString != "")
                                ////{
                                SplitAndSave(invPath[0], Global.InputPath, Global.InvoiceNumber, "INVOICE");
                                //}
                                //else
                                //{
                                //    MoveFile(invPath[0], "INVALID_INVOICE");
                                //    timer.Start();
                                //}
                                //if (Directory.Exists(directoryPath))
                                //    Directory.Delete(directoryPath, true);


                            }
                            else
                            {
                                try
                                {
                                    if (!string.IsNullOrEmpty(Global.InvoiceNumber))
                                    {
                                        SplitAndSave(invPath[0], Global.InputPath, Global.InvoiceNumber, "INVOICE");
                                    }
                                }
                                catch (Exception i)
                                {
                                    LogErrors(i);
                                    timer.Start();
                                }
                            }

                        }
                        else
                        {
                            MoveFile(invPath[0], "AlreadyExist");
                            CurrentRefreshAlreadyInvoiceLists();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogErrors(ex);
                timer.Start();
            }
        }

        private void SignPDF()
        {
            try
            {
                //timerSign.Stop();
                //var lstSign = objDSMModelData.GetAllInvoiceStatusAll().Where(x => x.DSStatus == "FALSE").FirstOrDefault();
                //var lstSign = objDSMModelData.GetAllInvoiceStatusAll().Where(x => x.DSStatus == "FALSE").FirstOrDefault();
                CurrentRefreshSignedLists();
                string[] inputPathList = Directory.GetFiles(Global.InputPath, "*.pdf");
                if (inputPathList.Count() == 0)
                {
                    ProcessMsg = "";
                }


                //PendingInvList = new ObservableCollection<InvoiceDisplayModel>();
                for (int i = 0; i < inputPathList.Length; i++)
                {
                    //AssignInvoiceNo_TruckNo(invoicePathList[i], "INV");
                    //SplitAndSave(inputPathList[i], Global.InputPath, Global.InvoiceNumber, "INVOICE");
                    CurrentInvoice = "Invoice - " + Global.InvoiceNumber + " is Signing...";
                    //EthernetConnectivityCheck();
                    objDigitalSign = new DigitalSign();
                    bool sign = objDigitalSign.FlatDigitalSignatureInitialize("Invoice", Global.InvoiceNumber, Global.CustomerName, Global.InvoiceNumber);
                    if (sign)
                    {
                        CreateInvoice();
                    }
                    CurrentRefreshSignedLists();
                }
                //timerSign.Start();
            }
            catch (Exception ex)
            {
                LogErrors(ex);
                //RefreshLists();
                //timerSign.Start();
                timer.Start();
            }
        }
        private void MailPDF()
        {
            try
            {

                string Fordpdffilename = Global.OutputPath + @"\" + Global.InvoiceNumber + "ORIGINALInvoice.pdf";
                if (Global.CustomerName == "FORD")
                {
                    string vendorcode = "FA9NA";
                    string newfilename = "E6_1_" + vendorcode.ToString().Trim() + "_" + Global.InvoiceNumber.ToString().Trim() + ".pdf";
                    if (System.IO.File.Exists(Fordpdffilename))
                    {
                        if (Fordpdffilename.ToString().Trim().Contains("ORIGINAL"))
                        {
                            if (Fordpdffilename.ToString().Trim().Contains("ORIGINAL"))
                            {
                                if ((System.IO.File.Exists(Global.OutputPath + @"\\" + newfilename.ToString().Trim())))
                                    File.Delete(Global.OutputPath + @"\\" + newfilename.ToString().Trim());
                                //System.IO.File.Move(pdffilename, SalesOrderManagementSystem.Utilities._sDSOutput_File_Path + @"\\" + newfilename.ToString().Trim());
                                System.IO.File.Move(Global.OutputPath + @"\\" + System.IO.Path.GetFileName(Fordpdffilename.ToString().Trim()), Global.OutputPath + @"\\" + newfilename.ToString().Trim());

                            }

                        }


                        //var lstSign = objDSMModelData.GetAllInvoiceStatusAll().Where(x => x.DSStatus == "TRUE" && x.IsEmailSend == false).FirstOrDefault();

                        //for (int i = 0; i < lstSign.Count; i++)
                        //{

                        //if (lstSign != null)
                        //{
                        //CurrentInvoice = "Invoice - " + lstSign.InvNumber + " is Signing...";
                        objDigitalSign = new DigitalSign();
                        //bool mailSent = objDigitalSign.SendEmail(lstSign.InvNumber);
                        bool mailSent = objDigitalSign.FordSendEmail(Global.OutputPath + @"\\" + newfilename.ToString().Trim());
                        if (mailSent == true)
                        {
                            //objDSMModelData.UpdateEmailstatusByInvoiceNo(lstSign.InvNumber, true);
                            Global.IsMailSend = true;
                        }
                        else
                        {
                            Global.IsMailSend = false;
                        }

                        //RefreshLists();
                        //}
                    }
                }
                else if (Global.CustomerName == "HYUNDAI")
                {
                   
                    if (System.IO.File.Exists(Global.OutputPath + @"\\" + Global.InvoiceNumber + "ORIGINALInvoice.pdf"))
                    {
                        objDigitalSign = new DigitalSign();
                        //bool mailSent = objDigitalSign.SendEmail(lstSign.InvNumber);
                        bool mailSent = objDigitalSign.FordSendEmail(Global.OutputPath + @"\\" + Global.InvoiceNumber + "ORIGINALInvoice.pdf");

                        if (mailSent == true)
                        {
                            //objDSMModelData.UpdateEmailstatusByInvoiceNo(lstSign.InvNumber, true);
                            Global.IsMailSend = true;
                        }
                        else
                        {
                            Global.IsMailSend = false;
                        }
                    }

                    //RefreshLists();
                    //}


                }
                //}
                //timerMail.Start();
            }
            catch (Exception ex)
            {

                LogErrors(ex);
                //RefreshLists();
                timer.Start();
            }
        }
       
        private void PrintFiles()
        {
            try
            { 
                string[] printPathList = Directory.GetFiles(Global.OutputPath, "*.pdf");
                if (printPathList.Count() == 0)
                {
                    ProcessMsg = "";
                }
                if (printPathList.ToString().Trim() != null)
                {
                    //PendingInvList = new ObservableCollection<InvoiceDisplayModel>();
                    //for (int i = 0; i < printPathList.Length; i++)
                    for (int i = 0; i < 1; i++)
                    {

                        CurrentInvoice = "Invoice - " + Global.InvoiceNumber + " is Printing...";
                        //EthernetConnectivityCheck();
                        Print objPrint = new Print();
                        var printStatus = objPrint.PrintInvoicePDFs(Global.InvoiceNumber, "invoice");
                        CurrentRefreshPrintedLists();
                         
                        if (Global.CustomerName == "HYUNDAI")
                        //if (Global.CustomerName != "")
                        {
                            string PdfFilename = Global.OutputPath + "PRINT" + @"\" + Global.InvoiceNumber + "ORIGINALinvoice.pdf";
                            //if (PdfFilename != null && File.Exists(PdfFilename))
                            if (PdfFilename != null && File.Exists(PdfFilename))
                            {
                                
                                if (!string.IsNullOrEmpty(QRCodeString))
                                {

                                   
                                    CreateInvoice();
                                    
                                }
                            }
                            //Global.CustomerName = "";
                        }
                        else if (Global.CustomerName != "")
                        //if (Global.CustomerName != "")
                        {

                            string PdfFilename = Global.OutputPath + "PRINT" + @"\" + Global.InvoiceNumber + "ORIGINALinvoice.pdf";
                            //if (PdfFilename != null && File.Exists(PdfFilename))
                            if (PdfFilename != null && File.Exists(PdfFilename))
                            {
                                OthercustInv();

                            }
                            Global.InvoiceNumber = "";
                            //Global.CustomerName = "";
                        }
                        else
                        {
                            return;
                        }
                        //}
                    } 
                }
                //}
                //timerPrintPost.Start();
            }
            catch (Exception ex)
            {
                LogErrors(ex);
                //timerPrintPost.Start();
                timer.Start();
            }
        }

        private string GetQRCodeString(System.Drawing.Image img, string outPutPath, string directory, string pdfPath,int Count)
        {
            string scaningResult = string.Empty;
            img.Save(outPutPath, System.Drawing.Imaging.ImageFormat.Png);
            System.Drawing.Image imgbmp = System.Drawing.Image.FromFile(outPutPath);
            Bitmap bmp = new Bitmap(imgbmp);

            try
            {
                BarcodeResult[] results = BarCodeScanner.Scan(bmp);
                foreach (BarcodeResult result in results)
                {
                    Console.WriteLine(result.Data);
                    scaningResult = result.Data;
                }

                //bmp = ResizeImage(bmp, 500, 500);
                //scaningResult = Spire.Barcode.BarcodeScanner.ScanOne(bmp);
                //bmp.Save(directory + "\\aaa.png");
                //string data = ScanQRPdf(directory + "\\aaa.png"); 

                bmp.Dispose();
                imgbmp.Dispose();
                img.Dispose();

            }
            catch (Exception ex)
            {
                LogErrors(ex);
                bmp.Dispose();
                imgbmp.Dispose();
                img.Dispose();
                if (Directory.Exists(directory))
                    Directory.Delete(directory, true);

                MoveFile(pdfPath, "INVALID_INVOICE");
                objDSMModelData = new DSMModelData();
                objDSMModelData.SaveQuarantine(Global.InvoiceNumber);
                timer.Start();
            }
            return scaningResult;
        }
        public static Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
        {
            var destRect = new System.Drawing.Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                    //wrapMode.Dispose();
                }
                //graphics.Dispose();
            }

            return destImage;
        }
        private string ScanQRPdf(string imagePath)
        {
            //EthernetConnectivityCheck();
            ArrayList barcodes = new ArrayList();
            DateTime dtStart = DateTime.Now;
            String decodedString = string.Empty;
            var img = (imagePath);
            if (System.IO.Path.GetExtension(img).ToLower() == ".bmp")
            {
                Bitmap b = new Bitmap(img);
                try
                {
                    QRCodeDecoder decoder = new QRCodeDecoder();
                    decodedString = decoder.decode(new QRCodeBitmapImage(b));
                }
                catch (Exception ex)
                {
                    LogErrors(ex);
                }
                b.Dispose();
            }
            return decodedString;
        }
        public void OthercustInv()
        {
             
            lstInvoice = new List<InvoiceDisplayModel>();
            objDSMModelData = new DSMModelData();
            for (int i = 0; i < 1; i++)
            {
                string dateString = Global.InvoiceDate;
                CultureInfo provider = CultureInfo.InvariantCulture;
                DateTime dateInv = DateTime.ParseExact(dateString, new string[] { "MM.dd.yyyy", "MM-dd-yyyy", "MM/dd/yyyy" }, provider, DateTimeStyles.None);

                objInvoice.InvNumber = Global.InvoiceNumber;
                objInvoice.InvDate = dateInv;

                objInvoice.CreatedBy = Global.UserId;
                objInvoice.CustomerName = Global.CustomerName;
                objInvoice.VehicleNumber = Global.TruckNumber;
                objInvoice.VendorCode = Global.VendorCode;
                objInvoice.IsEmailSend = Global.IsMailSend;
                objInvoice.OriginalPDF = Global.OriginalPDFFile;

                lstInvoice.Add(objInvoice);
            }
            
            objDSMModelData.saveOtherInvoice(lstInvoice);
            return;
        }
        public void CreateInvoice()
        {
            //EthernetConnectivityCheck();
          
            lstInvoice = new List<InvoiceDisplayModel>();
            objDSMModelData = new DSMModelData();

            string dateString = Global.InvoiceDate.Substring(3, 2) + '/' + Global.InvoiceDate.Substring(0, 2) + '/' + Global.InvoiceDate.Substring(6, 4);
            CultureInfo provider = CultureInfo.InvariantCulture;
            DateTime dateInv = DateTime.ParseExact(dateString, new string[] { "MM.dd.yyyy", "MM-dd-yyyy", "MM/dd/yyyy" }, provider, DateTimeStyles.None);

            objInvoice.InvNumber = Global.InvoiceNumber;
            Global.InvoiceNumber = Global.InvoiceNumber == null ? objInvoice.InvNumber : Global.InvoiceNumber;

            objInvoice.InvDate = dateInv;
            objInvoice.InvQuantity = Global.Qty.Trim();
            objInvoice.InvValue = Global.Totval.Replace(",", "").Trim();
            objInvoice.TarrifNumber = Global.HSN.Trim();
            objInvoice.BedAmount = "0.00";
            objInvoice.SGST = Global.Sgst.Replace(",","").Trim();
            objInvoice.IGST = Global.Igst.Replace(",", "").Trim(); ;
            objInvoice.VatAmount = "0.00";
            objInvoice.UnitPrice = Global.Rate.Replace(",", "").Trim();
            objInvoice.MaterialCost = Global.Ass.Replace(",", "").Trim();
            objInvoice.CGST = Global.Cgst.Replace(",", "").Trim();
            objInvoice.ConsigneePartCost = "0.00";
            objInvoice.ExciseDutyCost = "0.00";
            objInvoice.AssessableValue = Global.Ass.Replace(",", "").Trim();
            objInvoice.CSTAmount = "0.00";
            objInvoice.ToolCost = "0.00";
            objInvoice.ConsigneeMatlCost = "0.00";
            objInvoice.GSTN = "33AAACS4920J1ZJ";
            objInvoice.CreatedBy = Global.UserId;
            objInvoice.CustomerName = Global.CustomerName;
            objInvoice.VehicleNumber = Global.TruckNumber.Trim();
            if (Global.CustomerName.ToUpper() == "KIA") 
            {
                Global.VendorCode = "T010";

            }
            else
            {
                Global.VendorCode = "T010";

            }

            objInvoice.VendorCode = Global.VendorCode.Trim();
            objInvoice.TCS = Global.Tcs.Replace(",", "").Trim();
            objInvoice.E_InvNo = Global.Irn.Trim();
            objInvoice.Ewaybill_no = Global.Ewaybill.Trim();
            objInvoice.Lot_Code = "0";
            objInvoice.EXTRA_NUM_1 = "0";
            objInvoice.EXTRA_NUM_2 = "0";
            objInvoice.EXTRA_NUM_3 = "0";
            objInvoice.EXTRA_NUM_4 = "0";
            objInvoice.EXTRA_NUM_5 = "0";
            objInvoice.EXTRA_NUM_6 = "0";
            objInvoice.EXTRA_NUM_7 = "0";
            objInvoice.EXTRA_CHAR_1 = "0";
            objInvoice.EXTRA_CHAR_2 = "0";
            objInvoice.EXTRA_CHAR_3 = "0";
            objInvoice.EXTRA_CHAR_4 = "0";
            objInvoice.EXTRA_CHAR_5 = "0";
            objInvoice.Date1 = dateInv;
            objInvoice.Date2 = dateInv;
            objInvoice.Date3 = dateInv;
            objInvoice.Date4 = dateInv;
            objInvoice.Date5 = dateInv;
            objInvoice.Manfac_Date = dateInv;
            objInvoice.PdfGeneration = "";
            objInvoice.ShopCode = Global.Shopcode.Trim();
            objInvoice.PONumber = Global.Po.Trim();
            objInvoice.PartNumber = Global.Part.Replace(",", "").Trim();
            objInvoice.Finyear = Global.FinYear;
            objInvoice.SystemName = System.Environment.MachineName;
            objInvoice.OriginalPDF = Global.OriginalPDFFile;
            lstInvoice.Add(objInvoice);
            objDSMModelData.SaveInvoice(lstInvoice);
        }

        public void AssignInvoiceNo_TruckNo(string src, string type)
        {
            string truckNumber = string.Empty;
            string pathToPdf = src;

            Customers = new ObservableCollection<CustomerDisplayModel>(objDSMModelData.GetAllCustomers());

            using (PdfReader Custreader = new PdfReader(pathToPdf))
            {

                var custparser = new PdfReaderContentParser(Custreader);

                var custstrategy = custparser.ProcessContent(1, new LocationTextExtractionStrategyWithPosition());

                var res = custstrategy.GetLocations();

                Custreader.Close();

                //var custid = Customers;
                Global.CustomerName = "";
                Global.CustomerId = "";
                Global.Templatename = "";
                int custcount = 0;
                for (int k = 0; k < Customers.Count; k++)
                {

                    var custsearchResult = res.Where(p => p.Text.ToUpper().Contains(Customers[k].CustomerName.ToUpper())).OrderBy(p => p.Y).Reverse().ToList();
                    //var custsearchResult = res.Where(p => p.Text.Contains("Mobis")).OrderBy(p => p.Y).Reverse().ToList();
                    if (custsearchResult.Count > 0)
                    {
                        custcount++;
                        Global.CustomerName = Customers[k].CustomerName.ToString().Trim();
                        Global.CustomerId = Customers[k].CustomerId.ToString().Trim();
                        break;
                    }
                    var supplimentaryResult = res.Where(p => p.Text.ToUpper().Contains("KIA")).OrderBy(p => p.Y).Reverse().ToList();
                    //var custsearchResult = res.Where(p => p.Text.Contains("Mobis")).OrderBy(p => p.Y).Reverse().ToList();
                    if (supplimentaryResult.Count > 0)
                    {
                        custcount++;
                        Global.CustomerName = Customers[1].CustomerName.ToString().Trim();
                        Global.CustomerId = Customers[1].CustomerId.ToString().Trim();
                        break;

                    }

                }
                if (custcount == 0)
                {
                    MoveFile(pathToPdf, "OTHER INVOICE");
                    othercustomer("Other Customer Invoice");
                    return;
                }
            }
            SettingsDisplayModel objSettings = new SettingsDisplayModel();
            CustomerDisplayModel objCustomer = new CustomerDisplayModel();
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
                }
                else
                {
                    Global.IsAPIpost = false;
                }
            }
            PdfReader reader = new PdfReader(pathToPdf);
            //if(reader.)
            if (Global.CustomerId != "" && Global.CustomerId.Length > 0)
            {
                //var dsDataDetail = objDSMModelData.GetDSdetailbyClientId("HMI1");
                var dsDataDetail = objDSMModelData.GetDSdetailbyClientId(Global.CustomerId);

                string invnocoord = "";
                string invdatecoord = "";
                string ewaycodecoord = "";
                string trucknocoord = "";
                //string clientnamecoord = "";
                string ponocoord = "";
                string partnocoord = "";
                string qtycoord = "";
                string ratecoord = "";
                string asscoord = "";
                string cgstcoord = "";
                string cgstrtcoord = "";
                string sgstrtcoord = "";
                string igstrtcoord = "";

                string sgstcoord = "";
                string totvalcoord = "";
                string irncoord = "";
                string tcscoord = "";
                string shopcdcoord = "";
                string Hsncoord = "";
                string Igstcoord = "";

                //for (var i = 0; i < dsDataDetail.Count; i++)
                //{
                //    invnocoord= dsDataDetail[i].i
                //}

                //var coordinates = objDSMModelData.GetInputTemplates(Global.CustomerName);
                var template = objDSMModelData.GetTemplates(Global.CustomerId);
                Global.Templatename = template[0].Tempname.ToString();
                var coordinates = objDSMModelData.GetTemplatesCord(Global.Templatename, Global.CustomerId);
                //string coord = "";
                for (int i = 0; i < coordinates.Count; i++)
                {
                    //string DSSignName = dsMasterData[i].KeyName;
                    invnocoord = coordinates[i].Invnocoord.ToString().Trim();
                    invdatecoord = coordinates[i].Invdatecoord.ToString().Trim();
                    ewaycodecoord =  coordinates[i].Ewaycoord.ToString().Trim();
                    trucknocoord = coordinates[i].Trucknocoord.ToString().Trim();
                    //clientnamecoord = coordinates[i].Clientnamecoord.ToString().Trim();
                    ponocoord = coordinates[i].Pocoord.ToString().Trim();
                    partnocoord = coordinates[i].Partcoord.ToString().Trim();
                    qtycoord = coordinates[i].Qtycoord.ToString().Trim();
                    ratecoord = coordinates[i].Ratecoord.ToString().Trim();
                    asscoord = coordinates[i].Asscoord.ToString().Trim();
                    cgstrtcoord= coordinates[i].Cgstrtcoord.ToString().Trim();
                    sgstrtcoord = coordinates[i].Sgstrtcoord.ToString().Trim();
                    igstrtcoord = coordinates[i].Igstrtcoord.ToString().Trim();
                    cgstcoord = coordinates[i].Cgstcoord.ToString().Trim();
                    sgstcoord = coordinates[i].Sgstcoord.ToString().Trim();
                    totvalcoord = coordinates[i].Totvalcoord.ToString().Trim();
                    irncoord = coordinates[i].Irncoord.ToString().Trim();
                    tcscoord = coordinates[i].Tcscoord.ToString().Trim();
                    shopcdcoord = coordinates[i].Shopcdcoord.ToString().Trim();
                    Hsncoord = coordinates[i].Hsncoord.ToString().Trim();
                    Igstcoord= coordinates[i].Igstcoord.ToString().Trim();
                }
                if (invnocoord.Length > 0 && invnocoord.ToString().Trim() != "")
                {
                    RenderFilter[] filters = new RenderFilter[1];
                    LocationTextExtractionStrategy regionFilter = new LocationTextExtractionStrategy();
                    string[] arrinvcoord = invnocoord.Split(',');
                    filters[0] = new RegionTextRenderFilter(new iTextSharp.text.Rectangle(int.Parse(arrinvcoord[0]), int.Parse(arrinvcoord[1]), int.Parse(arrinvcoord[2]), int.Parse(arrinvcoord[3])));
                    FilteredTextRenderListener strategy = new FilteredTextRenderListener(regionFilter, filters);

                    String invresult = PdfTextExtractor.GetTextFromPage(reader, 1, strategy);
                    string[] invno = new string[3];
                    if (invresult.Contains(':'))
                    {
                        invno = invresult.Split(':');
                        int invind = invno.GetLength(0);
                        Global.InvoiceNumber = invno[invind - 1].ToString().Trim();
                    }
                    else if (invresult.Contains('\t'))
                    {
                        invno = invresult.Split('\t');
                        int invind = invno.GetLength(0);
                        Global.InvoiceNumber = invno[invind - 1].ToString().Trim();
                    }
                    else if (invresult.Contains('-'))
                    {
                        invno = invresult.Split('-');
                        int invind = invno.GetLength(0);
                        Global.InvoiceNumber = invno[invind - 1].ToString().Trim();
                    }
                    //int invind = invno.GetLength(0);
                    //Global.InvoiceNumber = invno[invind-1].ToString().Trim();
                    else
                    {
                        Global.InvoiceNumber = invresult.Trim();
                    }
                }


                if (invdatecoord.Length > 0 && invdatecoord.ToString().Trim() != "")
                {
                    RenderFilter[] filters = new RenderFilter[1];
                    LocationTextExtractionStrategy regionFilter = new LocationTextExtractionStrategy();
                    string[] arrinvdatecoord = invdatecoord.Split(',');
                    filters[0] = new RegionTextRenderFilter(new iTextSharp.text.Rectangle(int.Parse(arrinvdatecoord[0]), int.Parse(arrinvdatecoord[1]), int.Parse(arrinvdatecoord[2]), int.Parse(arrinvdatecoord[3])));
                    FilteredTextRenderListener strategy = new FilteredTextRenderListener(regionFilter, filters);

                    String invdateresult = PdfTextExtractor.GetTextFromPage(reader, 1, strategy);
                    string[] invdate = new string[3];
                    if (invdateresult.Contains(':'))
                    {
                        invdate = invdateresult.Split(':');
                        int invind = invdate.GetLength(0);
                        Global.InvoiceDate = invdate[invind - 1].ToString().Trim();
                    }
                    else if (invdateresult.Contains('\t'))
                    {
                        invdate = invdateresult.Split('\t');
                        int invind = invdate.GetLength(0);
                        Global.InvoiceDate = invdate[invind - 1].ToString().Trim();
                    }
                    //else if (invdateresult.Contains('-'))
                    //{
                    //    invdate = invdateresult.Split('-');
                    //    int invind = invdate.GetLength(0);
                    //    Global.InvoiceDate = invdate[invind - 1].ToString().Trim();
                    //}
                    else
                    {
                        Global.InvoiceDate = invdateresult.Trim();
                    }
                    //Global.InvoiceDate = "11/11/2019";
                    //int invind = invdate.GetLength(0);
                    //Global.InvoiceDate = invdate[invind-1].ToString().Trim();
                }

                if (trucknocoord.Length > 0 && trucknocoord.ToString().Trim() != "")
                {
                    RenderFilter[] filters = new RenderFilter[1];
                    LocationTextExtractionStrategy regionFilter = new LocationTextExtractionStrategy();
                    string[] arrtrucknocoord = trucknocoord.Split(',');
                    filters[0] = new RegionTextRenderFilter(new iTextSharp.text.Rectangle(int.Parse(arrtrucknocoord[0]), int.Parse(arrtrucknocoord[1]), int.Parse(arrtrucknocoord[2]), int.Parse(arrtrucknocoord[3])));
                    FilteredTextRenderListener strategy = new FilteredTextRenderListener(regionFilter, filters);

                    String truckresult = PdfTextExtractor.GetTextFromPage(reader, 1, strategy);
                    string[] truck = new string[3];
                    if (truckresult.Contains(':'))
                    {
                        truck = truckresult.Split(':');
                        int invind = truck.GetLength(0);
                        Global.TruckNumber = truck[invind - 1].ToString().Trim();
                    }
                    else if (truckresult.Contains('\t'))
                    {
                        truck = truckresult.Split('\t');
                        int invind = truck.GetLength(0);
                        Global.TruckNumber = truck[invind - 1].ToString().Trim();
                    }
                    else if (truckresult.Contains('-'))
                    {
                        truck = truckresult.Split('-');
                        int invind = truck.GetLength(0);
                        Global.TruckNumber = truck[invind - 1].ToString().Trim();
                    }
                    else
                    {
                        Global.TruckNumber = truckresult.Trim();
                    }

                }

                if (ewaycodecoord.Length > 0 && ewaycodecoord.ToString().Trim() != "")
                {
                    RenderFilter[] filters = new RenderFilter[1];
                    LocationTextExtractionStrategy regionFilter = new LocationTextExtractionStrategy();
                    string[] arrvendorcodecoord = ewaycodecoord.Split(',');
                    filters[0] = new RegionTextRenderFilter(new iTextSharp.text.Rectangle(int.Parse(arrvendorcodecoord[0]), int.Parse(arrvendorcodecoord[1]), int.Parse(arrvendorcodecoord[2]), int.Parse(arrvendorcodecoord[3])));
                    FilteredTextRenderListener strategy = new FilteredTextRenderListener(regionFilter, filters);

                    String vendorresult = PdfTextExtractor.GetTextFromPage(reader, 1, strategy);
                    string[] vendor = new string[3];
                    if (vendorresult.Contains(':'))
                    {
                        vendor = vendorresult.Split(':');
                        int invind = vendor.GetLength(0);
                        Global.Ewaybill = vendor[invind - 1].ToString().Trim();
                    }
                    else if (vendorresult.Contains('\t'))
                    {
                        vendor = vendorresult.Split('\t');
                        int invind = vendor.GetLength(0);
                        Global.Ewaybill = vendor[invind - 1].ToString().Trim();
                    }
                    else if (vendorresult.Contains('-'))
                    {
                        vendor = vendorresult.Split('-');
                        int invind = vendor.GetLength(0);
                        Global.Ewaybill = vendor[invind - 1].ToString().Trim();
                    }
                    else
                    {
                        Global.Ewaybill = vendorresult.Trim();
                    }

                }
                if (ponocoord.Length > 0 && ponocoord.ToString().Trim() != "")
                {
                    RenderFilter[] filters = new RenderFilter[1];
                    LocationTextExtractionStrategy regionFilter = new LocationTextExtractionStrategy();
                    string[] arrvendorcodecoord = ponocoord.Split(',');
                    filters[0] = new RegionTextRenderFilter(new iTextSharp.text.Rectangle(int.Parse(arrvendorcodecoord[0]), int.Parse(arrvendorcodecoord[1]), int.Parse(arrvendorcodecoord[2]), int.Parse(arrvendorcodecoord[3])));
                    FilteredTextRenderListener strategy = new FilteredTextRenderListener(regionFilter, filters);

                    String invresult = PdfTextExtractor.GetTextFromPage(reader, 1, strategy);
                    string[] invno = new string[3];
                    if (invresult.Contains(':'))
                    {
                        invno = invresult.Split(':');
                        int invind = invno.GetLength(0);
                        Global.Po = invno[invind - 1].ToString().Trim();
                    }
                    else if (invresult.Contains('\t'))
                    {
                        invno = invresult.Split('\t');
                        int invind = invno.GetLength(0);
                        Global.Po = invno[invind - 1].ToString().Trim();
                    }
                    else if (invresult.Contains('-'))
                    {
                        invno = invresult.Split('-');
                        int invind = invno.GetLength(0);
                        Global.Po = invno[invind - 1].ToString().Trim();
                    }
                    //int invind = invno.GetLength(0);
                    //Global.InvoiceNumber = invno[invind-1].ToString().Trim();
                    else
                    {
                        Global.Po = invresult.Trim();
                    }
                }
                if (partnocoord.Length > 0 && partnocoord.ToString().Trim() != "")
                {
                    RenderFilter[] filters = new RenderFilter[1];
                    LocationTextExtractionStrategy regionFilter = new LocationTextExtractionStrategy();
                    string[] arrvendorcodecoord = partnocoord.Split(',');
                    filters[0] = new RegionTextRenderFilter(new iTextSharp.text.Rectangle(int.Parse(arrvendorcodecoord[0]), int.Parse(arrvendorcodecoord[1]), int.Parse(arrvendorcodecoord[2]), int.Parse(arrvendorcodecoord[3])));
                    FilteredTextRenderListener strategy = new FilteredTextRenderListener(regionFilter, filters);
                                                                                         
                    String invresult = PdfTextExtractor.GetTextFromPage(reader, 1, strategy).Replace("-", "");
                    string[] invno = new string[3];
                    if (invresult.Contains(':'))
                    {
                        invno = invresult.Split(':');
                        int invind = invno.GetLength(0);
                        Global.Part = invno[invind - 1].ToString().Trim();
                    }

                    else
                    {
                        Global.Part = invresult.Trim();
                    }
                }
                if (qtycoord.Length > 0 && qtycoord.ToString().Trim() != "")
                {
                    RenderFilter[] filters = new RenderFilter[1];
                    LocationTextExtractionStrategy regionFilter = new LocationTextExtractionStrategy();
                    string[] arrvendorcodecoord = qtycoord.Split(',');
                    filters[0] = new RegionTextRenderFilter(new iTextSharp.text.Rectangle(int.Parse(arrvendorcodecoord[0]), int.Parse(arrvendorcodecoord[1]), int.Parse(arrvendorcodecoord[2]), int.Parse(arrvendorcodecoord[3])));
                    FilteredTextRenderListener strategy = new FilteredTextRenderListener(regionFilter, filters);

                    String invresult = PdfTextExtractor.GetTextFromPage(reader, 1, strategy);
                    string[] invno = new string[3];
                    if (invresult.Contains(':'))
                    {
                        invno = invresult.Split(':');
                        int invind = invno.GetLength(0);
                        Global.Qty = invno[invind - 1].ToString().Trim();
                    }
                    else if (invresult.Contains('\t'))
                    {
                        invno = invresult.Split('\t');
                        int invind = invno.GetLength(0);
                        Global.Qty = invno[invind - 1].ToString().Trim();
                    }
                    else if (invresult.Contains('-'))
                    {
                        invno = invresult.Split('-');
                        int invind = invno.GetLength(0);
                        Global.Qty = invno[invind - 1].ToString().Trim();
                    }
                    //int invind = invno.GetLength(0);
                    //Global.InvoiceNumber = invno[invind-1].ToString().Trim();
                    else
                    {
                        Global.Qty = invresult.Trim();
                    }
                }
                if (ratecoord.Length > 0 && ratecoord.ToString().Trim() != "")
                {
                    RenderFilter[] filters = new RenderFilter[1];
                    LocationTextExtractionStrategy regionFilter = new LocationTextExtractionStrategy();
                    string[] arrvendorcodecoord = ratecoord.Split(',');
                    filters[0] = new RegionTextRenderFilter(new iTextSharp.text.Rectangle(int.Parse(arrvendorcodecoord[0]), int.Parse(arrvendorcodecoord[1]), int.Parse(arrvendorcodecoord[2]), int.Parse(arrvendorcodecoord[3])));
                    FilteredTextRenderListener strategy = new FilteredTextRenderListener(regionFilter, filters);

                    String invresult = PdfTextExtractor.GetTextFromPage(reader, 1, strategy);
                    string[] invno = new string[3];
                    if (invresult.Contains(':'))
                    {
                        invno = invresult.Split(':');
                        int invind = invno.GetLength(0);
                        Global.Rate = invno[invind - 1].ToString().Trim();
                    }
                    else if (invresult.Contains('\t'))
                    {
                        invno = invresult.Split('\t');
                        int invind = invno.GetLength(0);
                        Global.Rate = invno[invind - 1].ToString().Trim();
                    }
                    else if (invresult.Contains('-'))
                    {
                        invno = invresult.Split('-');
                        int invind = invno.GetLength(0);
                        Global.Rate = invno[invind - 1].ToString().Trim();
                    }
                    //int invind = invno.GetLength(0);
                    //Global.InvoiceNumber = invno[invind-1].ToString().Trim();
                    else
                    {
                        Global.Rate = invresult.Trim();
                    }
                }
                if (asscoord.Length > 0 && asscoord.ToString().Trim() != "")
                {
                    RenderFilter[] filters = new RenderFilter[1];
                    LocationTextExtractionStrategy regionFilter = new LocationTextExtractionStrategy();
                    string[] arrvendorcodecoord = asscoord.Split(',');
                    filters[0] = new RegionTextRenderFilter(new iTextSharp.text.Rectangle(int.Parse(arrvendorcodecoord[0]), int.Parse(arrvendorcodecoord[1]), int.Parse(arrvendorcodecoord[2]), int.Parse(arrvendorcodecoord[3])));
                    FilteredTextRenderListener strategy = new FilteredTextRenderListener(regionFilter, filters);

                    String invresult = PdfTextExtractor.GetTextFromPage(reader, 1, strategy);
                    string[] invno = new string[3];
                    if (invresult.Contains(':'))
                    {
                        invno = invresult.Split(':');
                        int invind = invno.GetLength(0);
                        Global.Ass = invno[invind - 1].ToString().Trim();
                    }
                    else if (invresult.Contains('\t'))
                    {
                        invno = invresult.Split('\t');
                        int invind = invno.GetLength(0);
                        Global.Ass = invno[invind - 1].ToString().Trim();
                    }
                    else if (invresult.Contains('-'))
                    {
                        invno = invresult.Split('-');
                        int invind = invno.GetLength(0);
                        Global.Ass = invno[invind - 1].ToString().Trim();
                    }
                    //int invind = invno.GetLength(0);
                    //Global.InvoiceNumber = invno[invind-1].ToString().Trim();
                    else
                    {
                        Global.Ass = invresult.Trim();
                    }
                }
                if (irncoord.Length > 0 && irncoord.ToString().Trim() != "")
                {
                    RenderFilter[] filters = new RenderFilter[1];
                    LocationTextExtractionStrategy regionFilter = new LocationTextExtractionStrategy();
                    string[] arrvendorcodecoord = irncoord.Split(',');
                    filters[0] = new RegionTextRenderFilter(new iTextSharp.text.Rectangle(int.Parse(arrvendorcodecoord[0]), int.Parse(arrvendorcodecoord[1]), int.Parse(arrvendorcodecoord[2]), int.Parse(arrvendorcodecoord[3])));
                    FilteredTextRenderListener strategy = new FilteredTextRenderListener(regionFilter, filters);

                    String invresult = PdfTextExtractor.GetTextFromPage(reader, 1, strategy).Replace("\n", "");
                    string[] invno = new string[3];
                    if (invresult.Contains(':'))
                    {
                        invno = invresult.Split(':');
                        int invind = invno.GetLength(0);
                        Global.Irn = invno[invind - 1].ToString().Trim();
                    }

                    //int invind = invno.GetLength(0);
                    //Global.InvoiceNumber = invno[invind-1].ToString().Trim();
                    else
                    {
                        Global.Irn = invresult.Trim();
                    }
                }
                if (cgstcoord.Length > 0 && cgstcoord.ToString().Trim() != "")
                {
                    RenderFilter[] filters = new RenderFilter[1];
                    LocationTextExtractionStrategy regionFilter = new LocationTextExtractionStrategy();
                    string[] arrvendorcodecoord = cgstcoord.Split(',');
                    filters[0] = new RegionTextRenderFilter(new iTextSharp.text.Rectangle(int.Parse(arrvendorcodecoord[0]), int.Parse(arrvendorcodecoord[1]), int.Parse(arrvendorcodecoord[2]), int.Parse(arrvendorcodecoord[3])));
                    FilteredTextRenderListener strategy = new FilteredTextRenderListener(regionFilter, filters);

                    String invresult = PdfTextExtractor.GetTextFromPage(reader, 1, strategy);
                    string[] invno = new string[3];
                    if (invresult.Contains(':'))
                    {
                        invno = invresult.Split(':');
                        int invind = invno.GetLength(0);
                        Global.Cgst = invno[invind - 1].ToString().Trim();
                    }
                    else if (invresult.Contains('\t'))
                    {
                        invno = invresult.Split('\t');
                        int invind = invno.GetLength(0);
                        Global.Cgst = invno[invind - 1].ToString().Trim();
                    }
                    else if (invresult.Contains('-'))
                    {
                        invno = invresult.Split('-');
                        int invind = invno.GetLength(0);
                        Global.Cgst = invno[invind - 1].ToString().Trim();
                    }
                    //int invind = invno.GetLength(0);
                    //Global.InvoiceNumber = invno[invind-1].ToString().Trim();
                    else
                    {
                        Global.Cgst = invresult.Trim();
                    }
                }
                if (sgstcoord.Length > 0 && sgstcoord.ToString().Trim() != "")
                {
                    RenderFilter[] filters = new RenderFilter[1];
                    LocationTextExtractionStrategy regionFilter = new LocationTextExtractionStrategy();
                    string[] arrvendorcodecoord = sgstcoord.Split(',');
                    filters[0] = new RegionTextRenderFilter(new iTextSharp.text.Rectangle(int.Parse(arrvendorcodecoord[0]), int.Parse(arrvendorcodecoord[1]), int.Parse(arrvendorcodecoord[2]), int.Parse(arrvendorcodecoord[3])));
                    FilteredTextRenderListener strategy = new FilteredTextRenderListener(regionFilter, filters);

                    String invresult = PdfTextExtractor.GetTextFromPage(reader, 1, strategy);
                    string[] invno = new string[3];
                    if (invresult.Contains(':'))
                    {
                        invno = invresult.Split(':');
                        int invind = invno.GetLength(0);
                        Global.Sgst = invno[invind - 1].ToString().Trim();
                    }
                    else if (invresult.Contains('\t'))
                    {
                        invno = invresult.Split('\t');
                        int invind = invno.GetLength(0);
                        Global.Sgst = invno[invind - 1].ToString().Trim();
                    }
                    else if (invresult.Contains('-'))
                    {
                        invno = invresult.Split('-');
                        int invind = invno.GetLength(0);
                        Global.Sgst = invno[invind - 1].ToString().Trim();
                    }
                    //int invind = invno.GetLength(0);
                    //Global.InvoiceNumber = invno[invind-1].ToString().Trim();
                    else
                    {
                        Global.Sgst = invresult.Trim();
                    }
                }
                if (totvalcoord.Length > 0 && totvalcoord.ToString().Trim() != "")
                {
                    RenderFilter[] filters = new RenderFilter[1];
                    LocationTextExtractionStrategy regionFilter = new LocationTextExtractionStrategy();
                    string[] arrvendorcodecoord = totvalcoord.Split(',');
                    filters[0] = new RegionTextRenderFilter(new iTextSharp.text.Rectangle(int.Parse(arrvendorcodecoord[0]), int.Parse(arrvendorcodecoord[1]), int.Parse(arrvendorcodecoord[2]), int.Parse(arrvendorcodecoord[3])));
                    FilteredTextRenderListener strategy = new FilteredTextRenderListener(regionFilter, filters);

                    String invresult = PdfTextExtractor.GetTextFromPage(reader, 1, strategy);
                    string[] invno = new string[3];
                    if (invresult.Contains(':'))
                    {
                        invno = invresult.Split(':');
                        int invind = invno.GetLength(0);
                        Global.Totval = invno[invind - 1].ToString().Trim();
                    }
                    else if (invresult.Contains('\t'))
                    {
                        invno = invresult.Split('\t');
                        int invind = invno.GetLength(0);
                        Global.Totval = invno[invind - 1].ToString().Trim();
                    }
                    else if (invresult.Contains('-'))
                    {
                        invno = invresult.Split('-');
                        int invind = invno.GetLength(0);
                        Global.Totval = invno[invind - 1].ToString().Trim();
                    }
                    //int invind = invno.GetLength(0);
                    //Global.InvoiceNumber = invno[invind-1].ToString().Trim();
                    else
                    {
                        Global.Totval = invresult.Trim();
                    }
                }
                if (tcscoord.Length > 0 && tcscoord.ToString().Trim() != "")
                {
                    RenderFilter[] filters = new RenderFilter[1];
                    LocationTextExtractionStrategy regionFilter = new LocationTextExtractionStrategy();
                    string[] arrvendorcodecoord = tcscoord.Split(',');
                    filters[0] = new RegionTextRenderFilter(new iTextSharp.text.Rectangle(int.Parse(arrvendorcodecoord[0]), int.Parse(arrvendorcodecoord[1]), int.Parse(arrvendorcodecoord[2]), int.Parse(arrvendorcodecoord[3])));
                    FilteredTextRenderListener strategy = new FilteredTextRenderListener(regionFilter, filters);

                    String invresult = PdfTextExtractor.GetTextFromPage(reader, 1, strategy);
                    string[] invno = new string[3];
                    if (invresult.Contains(':'))
                    {
                        invno = invresult.Split(':');
                        int invind = invno.GetLength(0);
                        Global.Tcs = invno[invind - 1].ToString().Trim();
                    }
                    else if (invresult.Contains('\t'))
                    {
                        invno = invresult.Split('\t');
                        int invind = invno.GetLength(0);
                        Global.Tcs = invno[invind - 1].ToString().Trim();
                    }
                    else if (invresult.Contains('-'))
                    {
                        invno = invresult.Split('-');
                        int invind = invno.GetLength(0);
                        Global.Tcs = invno[invind - 1].ToString().Trim();
                    }
                    //int invind = invno.GetLength(0);
                    //Global.InvoiceNumber = invno[invind-1].ToString().Trim();
                    else
                    {
                        Global.Tcs = invresult.Trim();
                    }
                }
                if (shopcdcoord.Length > 0 && shopcdcoord.ToString().Trim() != "")
                {
                    RenderFilter[] filters = new RenderFilter[1];
                    LocationTextExtractionStrategy regionFilter = new LocationTextExtractionStrategy();
                    string[] arrvendorcodecoord = shopcdcoord.Split(',');
                    filters[0] = new RegionTextRenderFilter(new iTextSharp.text.Rectangle(int.Parse(arrvendorcodecoord[0]), int.Parse(arrvendorcodecoord[1]), int.Parse(arrvendorcodecoord[2]), int.Parse(arrvendorcodecoord[3])));
                    FilteredTextRenderListener strategy = new FilteredTextRenderListener(regionFilter, filters);

                    String invresult = PdfTextExtractor.GetTextFromPage(reader, 1, strategy);
                    string[] invno = new string[3];
                    if (invresult.Contains(':'))
                    {
                        invno = invresult.Split(':');
                        int invind = invno.GetLength(0);
                        Global.Shopcode = invno[invind - 1].ToString().Trim();
                    }
                    else if (invresult.Contains('\t'))
                    {
                        invno = invresult.Split('\t');
                        int invind = invno.GetLength(0);
                        Global.Shopcode = invno[invind - 1].ToString().Trim();
                    }
                    else if (invresult.Contains('-'))
                    {
                        invno = invresult.Split('-');
                        int invind = invno.GetLength(0);
                        Global.Shopcode = invno[invind - 1].ToString().Trim();
                    }
                    //int invind = invno.GetLength(0);
                    //Global.InvoiceNumber = invno[invind-1].ToString().Trim();
                    else
                    {
                        Global.Shopcode = invresult.Trim();
                    }
                }
                if (Hsncoord.Length > 0 && Hsncoord.ToString().Trim() != "")
                {
                    RenderFilter[] filters = new RenderFilter[1];
                    LocationTextExtractionStrategy regionFilter = new LocationTextExtractionStrategy();
                    string[] arrvendorcodecoord = Hsncoord.Split(',');
                    filters[0] = new RegionTextRenderFilter(new iTextSharp.text.Rectangle(int.Parse(arrvendorcodecoord[0]), int.Parse(arrvendorcodecoord[1]), int.Parse(arrvendorcodecoord[2]), int.Parse(arrvendorcodecoord[3])));
                    FilteredTextRenderListener strategy = new FilteredTextRenderListener(regionFilter, filters);

                    String invresult = PdfTextExtractor.GetTextFromPage(reader, 1, strategy);
                    string[] invno = new string[3];
                    if (invresult.Contains(':'))
                    {
                        invno = invresult.Split(':');
                        int invind = invno.GetLength(0);
                        Global.HSN = invno[invind - 1].ToString().Trim();
                    }

                    else if (invresult.Contains('-'))
                    {
                        invno = invresult.Split('-');
                        int invind = invno.GetLength(0);
                        Global.HSN = invno[invind - 1].ToString().Trim();
                    }
                    //int invind = invno.GetLength(0);
                    //Global.InvoiceNumber = invno[invind-1].ToString().Trim();
                    else
                    {
                        Global.HSN = invresult.Replace("HSN/SAC", "").Trim();
                    }
                }
                if (Igstcoord.Length > 0 && Igstcoord.ToString().Trim() != "")
                {
                    RenderFilter[] filters = new RenderFilter[1];
                    LocationTextExtractionStrategy regionFilter = new LocationTextExtractionStrategy();
                    string[] arrvendorcodecoord = Igstcoord.Split(',');
                    filters[0] = new RegionTextRenderFilter(new iTextSharp.text.Rectangle(int.Parse(arrvendorcodecoord[0]), int.Parse(arrvendorcodecoord[1]), int.Parse(arrvendorcodecoord[2]), int.Parse(arrvendorcodecoord[3])));
                    FilteredTextRenderListener strategy = new FilteredTextRenderListener(regionFilter, filters);

                    String invresult = PdfTextExtractor.GetTextFromPage(reader, 1, strategy);
                    string[] invno = new string[3];
                    if (invresult.Contains(':'))
                    {
                        invno = invresult.Split(':');
                        int invind = invno.GetLength(0);
                        Global.Igst = invno[invind - 1].ToString().Trim();
                    }
                    else if (invresult.Contains('\t'))
                    {
                        invno = invresult.Split('\t');
                        int invind = invno.GetLength(0);
                        Global.Igst = invno[invind - 1].ToString().Trim();
                    }
                    else if (invresult.Contains('-'))
                    {
                        invno = invresult.Split('-');
                        int invind = invno.GetLength(0);
                        Global.Igst = invno[invind - 1].ToString().Trim();
                    }
                    //int invind = invno.GetLength(0);
                    //Global.InvoiceNumber = invno[invind-1].ToString().Trim();
                    else
                    {
                        Global.Igst = invresult.Trim();
                    }
                }

            }
            reader.Close();
        }


        //public string GetFinancialYear(DateTime curDate)
        public string GetFinancialYear(string curDate)
        {
          
            int CurrentYear = int.Parse(curDate.Substring(8, 2));
            int PreviousYear = (int.Parse(curDate.Substring(8, 2)) - 1);
            int NextYear = (int.Parse(curDate.Substring(8, 2)) + 1);
            string PreYear = PreviousYear.ToString();
            string NexYear = NextYear.ToString();
            string CurYear = CurrentYear.ToString();
            string FinYear = null;
            string[] month = new string[4];
            int CurrentMonth = 0;
            if (curDate.Contains("/"))
            {
                month = curDate.Split('/');
               
                if (month[1].Length == 2)
                {
                    CurrentMonth = int.Parse(curDate.Substring(3, 2));
                }
                else if (month[1].Length == 1)
                {
                    CurrentMonth = int.Parse(curDate.Substring(3, 1));
                }
            }
            else if(curDate.Contains("-"))
            {
                month = curDate.Split('-');

                if (month[1].Length == 2)
                {
                    CurrentMonth = int.Parse(curDate.Substring(3, 2));
                }
                else if (month[1].Length == 1)
                {
                    CurrentMonth = int.Parse(curDate.Substring(3, 1));
                }
            }
            else
            {
                month = curDate.Split('.');

                if (month[1].Length == 2)
                {
                    CurrentMonth = int.Parse(curDate.Substring(3, 2));
                }
                else if (month[1].Length == 1)
                {
                    CurrentMonth = int.Parse(curDate.Substring(3, 1));
                }
            }
                //int CurrentMonth= int.Parse(curDate.Split('/').ToString());
                //if (curDate.Month > 3)
                if (CurrentMonth > 3)
            {
                FinYear = CurYear + NexYear;
            }
            else
            {
                FinYear = PreYear + CurYear;
            }
            return FinYear;
        }

        //public void AssignInvoiceNo_TruckNo(string src, string type)
        //{
        //    string truckNumber = string.Empty;
        //    string pathToPdf = src;
        //    string pathToXml = Path.ChangeExtension(pathToPdf, ".xml");

        //    SautinSoft.PdfFocus f = new SautinSoft.PdfFocus();
        //    f.XmlOptions.ConvertNonTabularDataToSpreadsheet = true;
        //    f.GetPageSize(1);
        //    f.OpenPdf(pathToPdf);

        //    if (f.PageCount > 0)
        //    {
        //        int result = f.ToXml(pathToXml);

        //        //Show HTML document in browser
        //        if (result == 0)
        //        {
        //            //string truckNo = "Mode of Transport / Vehicle No. :";
        //            //string invNo = "No.:";
        //            //string invNo = "INV.NO:";
        //            XmlDocument doc = new XmlDocument();
        //            doc.Load(pathToXml);
        //            foreach (XmlNode xmlPage in doc.DocumentElement.SelectNodes("page"))
        //            {
        //                foreach (XmlNode xmlTable in xmlPage.SelectNodes("table"))
        //                {
        //                    foreach (XmlNode xmlRow in xmlTable.SelectNodes("row"))
        //                    {
        //                        if (type == "INV")
        //                        {
        //                            int countCell = 0;
        //                            bool invnoExist = false;
        //                            foreach (XmlNode xmlCell in xmlRow.SelectNodes("cell"))
        //                            {
        //                                countCell++;
        //                                if (xmlCell.InnerText == "No.:")
        //                                {
        //                                    invnoExist = true;
        //                                }

        //                                if (invnoExist == true)
        //                                {
        //                                    if (xmlRow.SelectNodes("cell").Count == countCell)
        //                                    {
        //                                        Global.InvoiceNumber = xmlCell.InnerText.Trim();
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        if (type == "TRUCK")
        //                        {
        //                            int counttruckCell = 0;
        //                            int counttruckCells = 0;
        //                            bool trucknoExist = false;
        //                            bool trucknoExist_new = false;

        //                            foreach (XmlNode xmlCell in xmlRow.SelectNodes("cell"))
        //                            {
        //                                counttruckCell++;
        //                                if (xmlCell.InnerText == "Vehicle No.")
        //                                {
        //                                    trucknoExist = true;
        //                                }
        //                                if (trucknoExist == true)
        //                                {
        //                                    if (counttruckCell == xmlRow.SelectNodes("cell").Count - 1)
        //                                    {
        //                                        Global.TruckNumber = xmlCell.InnerText;
        //                                    }
        //                                    if (Global.TruckNumber == "")
        //                                    {
        //                                        if (counttruckCell == xmlRow.SelectNodes("cell").Count)
        //                                        {
        //                                            Global.TruckNumber = xmlCell.InnerText;
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                            if (Global.TruckNumber == null)
        //                            {
        //                                foreach (XmlNode xmlCell in xmlRow.SelectNodes("cell"))
        //                                {
        //                                    counttruckCells++;
        //                                    if (xmlCell.InnerText == "Mode of Transport / Vehicle No.")
        //                                    {
        //                                        trucknoExist_new = true;
        //                                    }
        //                                    if (trucknoExist_new == true)
        //                                    {
        //                                        if (counttruckCells == xmlRow.SelectNodes("cell").Count)
        //                                        {
        //                                            Global.TruckNumber = xmlCell.InnerText;
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    f.ClosePdf();
        //    if (System.IO.File.Exists(pathToXml))
        //    {
        //        File.Delete(pathToXml);
        //    }
        //}

        public int SplitAndSave(string invoicePath, string outputPath, string tdocno_from, string invtype)
        {
            string _from = tdocno_from.ToString().Trim();

            FileInfo file = new FileInfo(invoicePath);
            string name = file.Name.Substring(0, file.Name.LastIndexOf("."));

            using (iTextSharp.text.pdf.PdfReader pdfReader = new iTextSharp.text.pdf.PdfReader(invoicePath))
            {
                for (int pageNumber = 1; pageNumber <= pdfReader.NumberOfPages; pageNumber++)
                //for (int pageNumber = 1; pageNumber <= 1; pageNumber++)
                {
                    //string vinvono = Convert.ToDecimal(_from).ToString("000000");
                    string vinvono = "";

                    if (pageNumber == 1)
                        vinvono = _from.ToString().Trim() + "ORIGINAL" + invtype.ToString().Trim();
                    else if (pageNumber == 2)
                        vinvono = _from.ToString().Trim() + "DUPLICATE" + invtype.ToString().Trim();
                    else if (pageNumber == 3)
                        vinvono = _from.ToString().Trim() + "TRIPLICATE" + invtype.ToString().Trim();
                    else if (pageNumber == 4)
                        vinvono = _from.ToString().Trim() + "EXTRA" + invtype.ToString().Trim();
                    else
                        vinvono = _from.ToString().Trim() + "ACKNOWLEDGEMENT" + invtype.ToString().Trim();
                    Spire.Pdf.PdfDocument doc = new Spire.Pdf.PdfDocument(invoicePath);
                    doc.FileInfo.IncrementalUpdate = false;
                    doc.CompressionLevel = PdfCompressionLevel.Best;

                    foreach (PdfPageBase page in doc.Pages)
                    {
                        if (page != null)
                        {
                            if (page.ImagesInfo != null)
                            {
                                foreach (PdfImageInfo info in page.ImagesInfo)
                                {
                                    page.TryCompressImage(info.Index);
                                }
                            }
                        }
                        ////Extracts images from page
                        //System.Drawing.Image[] images = page.ExtractImages();
                        //if (images != null && images.Length > 0)
                        //{
                        //    //Traverses all images
                        //    for (int j = 0; j < images.Length; j++)
                        //    {
                        //        System.Drawing.Image image = images[j];
                        //        PdfBitmap bp = new PdfBitmap(image);
                        //        //Reduces the quality of the image
                        //        bp.Quality = 50;
                        //        //Replaces the old image in the document with the compressed image
                        //        page.ReplaceImage(j, bp);
                        //    }
                        //}
                    }
                    doc.SaveToFile(outputPath + vinvono + ".pdf");
                    //// Get CGHS No.
                    //iTextSharp.text.pdf.parser.ITextExtractionStrategy strategy = new iTextSharp.text.pdf.parser.SimpleTextExtractionStrategy();
                    //string currentText = iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(pdfReader, pageNumber, strategy);
                    //// Get CGHS No from the pdf as per your pdf containt.
                    //// string CGHSNo = currentText.Split(new string[] { "DEBIT NOTE" }, StringSplitOptions.None)[1].Split('\n')[0];


                    //// Generate New pdf with CGHS No.
                    ////string filename = "A"+pageNumber+".pdf";
                    //iTextSharp.text.Document document = new iTextSharp.text.Document();
                    //iTextSharp.text.pdf.PdfCopy pdfCopy = new iTextSharp.text.pdf.PdfCopy(document, new FileStream(outputPath + vinvono + ".pdf", FileMode.Create));

                    //pdfCopy.SetPdfVersion(iTextSharp.text.pdf.PdfWriter.PDF_VERSION_1_5);

                    //pdfCopy.CompressionLevel = iTextSharp.text.pdf.PdfStream.BEST_COMPRESSION;

                    ////pdfCopy.CompressionLevel = PdfStream.DEFAULT_COMPRESSION;
                    //pdfCopy.SetFullCompression();

                    //document.Open();


                    //pdfCopy.AddPage(pdfCopy.GetImportedPage(pdfReader, pageNumber));

                    //document.Close();

                    //   //_from++;
                }
                pdfReader.Close();
                MoveFile(invoicePath, "PROCESSED");
                return pdfReader.NumberOfPages;
            }
        }

        public void MoveFile(string source, string location)
        {

            System.IO.StreamReader fileMove = new System.IO.StreamReader(source);
            string _SourceFile = source;

            //check folder
            if (!System.IO.Directory.Exists(Global.InvoicePath + "\\" + location + "\\"))
            {
                System.IO.Directory.CreateDirectory(Global.InvoicePath + "\\" + location + "\\");
            }
            string _DestFile = Global.InvoicePath + "\\" + location + "\\" + System.IO.Path.GetFileName(source);
            fileMove.Close();
            fileMove.Dispose();

            //Check File
            if (!System.IO.File.Exists(_DestFile))
            {
                File.Move(_SourceFile, _DestFile);
                System.IO.Directory.CreateDirectory(Global.InvoicePath + "\\" + location + "\\");
            }
            else
            {
                System.IO.File.Delete(_DestFile);
                File.Move(_SourceFile, _DestFile);
            }
        }

        public void LogErrors(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            string path = AppDomain.CurrentDomain.BaseDirectory + "LogError.txt";
            // flush every 20 seconds as you do it

            sb.AppendLine();
            sb.Append("Log Time: " + DateTime.Now);
            sb.AppendLine();
            sb.Append("--------------------------------------------------");
            sb.AppendLine();
            if (ex != null)
            {
                sb.Append("Exception: " + ex.ToString());
                if (ex.InnerException != null)
                {
                    sb.Append("Inner Exception: " + ex.InnerException.ToString());
                }
            }
            sb.AppendLine();
            sb.Append("--------------------------------------------------");
            sb.AppendLine();

            File.AppendAllText(path, sb.ToString());
            sb.Clear();
        }
        public void othercustomer(String Data)
        {
            StringBuilder sb = new StringBuilder();
            string path = AppDomain.CurrentDomain.BaseDirectory + "OtherInvoiceDetails.txt";
            // flush every 20 seconds as you do it

            sb.AppendLine();
            sb.Append("Log Time: " + DateTime.Now);
            sb.AppendLine();
            sb.Append("--------------------------------------------------");
            sb.AppendLine();
            //if (ex != null)
            //{
            //    sb.Append("Exception: " + ex.ToString());
            //    if (ex.InnerException != null)
            //    {
            //        sb.Append("Inner Exception: " + ex.InnerException.ToString());
            //    }
            //}
            if (Data.ToString().Trim() != "" && Data.ToString().Trim() != null)
            {
                sb.Append("Other Customer PDF was found. Please add your customer in Customer Master");
            }
            sb.AppendLine();
            sb.Append("--------------------------------------------------");
            sb.AppendLine();

            File.AppendAllText(path, sb.ToString());
            sb.Clear();
        }
        public void LogSignedDetails(String Data)
        {
            StringBuilder sb = new StringBuilder();
            string path = AppDomain.CurrentDomain.BaseDirectory + "OtherInvoiceDetails.txt";
            // flush every 20 seconds as you do it

            sb.AppendLine();
            sb.Append("Log Time: " + DateTime.Now);
            sb.AppendLine();
            sb.Append("--------------------------------------------------");
            sb.AppendLine();
            //if (ex != null)
            //{
            //    sb.Append("Exception: " + ex.ToString());
            //    if (ex.InnerException != null)
            //    {
            //        sb.Append("Inner Exception: " + ex.InnerException.ToString());
            //    }
            //}
            if (Data.ToString().Trim() != "" && Data.ToString().Trim() != null)
            {
                sb.Append(Global.CustomerName + " Customer Invoice No : " + Data.ToString().Trim() + " was Signed Successfully.");
            }
            sb.AppendLine();
            sb.Append("--------------------------------------------------");
            sb.AppendLine();

            File.AppendAllText(path, sb.ToString());
            sb.Clear();
        }

    }
}
