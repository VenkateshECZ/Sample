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
using System.Windows.Navigation;
using System.Windows.Shapes;
using iTextSharp.text;
using iTextSharp.text.log;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using iTextSharp.text.pdf.parser;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Unity;
using Microsoft.Practices.Unity;
using DSMData;
using System.Collections.ObjectModel;
using DSMData.Model;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace DSM.Views
{
    /// <summary>
    /// Interaction logic for Input_Layout.xaml
    /// </summary>
    public partial class Input_Layout : UserControl
    {
        public Input_Layout()
        {
           
            InitializeComponent();
        }

        public DSMModelData objDSMModelData;
        public string outputFile;
        static string Filename;
        //public static int count = 0;
        private ObservableCollection<CustomerDisplayModel> _Templates;
        public ObservableCollection<CustomerDisplayModel> Templates
        {
            get { return _Templates; }
            set { _Templates = value; }
        }

        private CustomerDisplayModel _Template;
        public new CustomerDisplayModel Template
        {
            get { return _Template; }
            set
            {
                _Template = value;
                //NotifyPropertyChanged("Customer");
                NotifyPropertyChanged("Template");
                
            }
        }

        public event PropertyChangedEventHandler PropertyChangeds;
        private void NotifyPropertyChanged([CallerMemberName()] string propertyName = null)
        {
            PropertyChangeds?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void Save_btn_Click(object sender, RoutedEventArgs e)
        {

            //pdfviewer.Dispose();
            pdfviewer.Refresh();
            //pdfviewer.Navigate("about:blank");
            pdfviewer.Navigate(FilePath.Text);
            //Process proc = new Process();
            //proc.StartInfo.FileName = System.IO.Path.GetFullPath(FilePath.Text);
            //proc.Start();
            //proc.WaitForExit(12000);

            await Task.Delay(3000);
            try
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(System.IO.Path.GetDirectoryName(outputFile));
                var pdffile = System.IO.Path.GetDirectoryName(FilePath.Text) + "Output" + "\\" + Filename + ".pdf";

                //if (File.Exists(System.IO.Path.GetDirectoryName(FilePath.Text) + "Output" + "\\" + Filename + ".pdf"))
                    
                foreach (FileInfo file in di.GetFiles())
                {
                    
                    file.Delete();

                }
                // save co ord to database
                //count = 5;
                //count++;
                Temp_name.IsEnabled = false;
                if (Inv_no_chk.IsChecked==true)
                {
                    objDSMModelData = new DSMModelData();
                    string coordinate = Left_txt.Text + "," + Bott_txt.Text + "," + Right_txt.Text + "," + Top_txt.Text;
                    string TemplateName = ComboBoxZone.SelectedItem.ToString();
                    objDSMModelData.UpdateClientCoordinates(coordinate, TemplateName, 1);
                    MessageBox.Show("Invoice Number Co-Ordinates Saved successfully");
                    reset();
                    //txtHintPassword.Text = "Enter Invoice Date";
                }
                else if (Inv_date_chk.IsChecked == true)
                {
                    objDSMModelData = new DSMModelData();
                    string coordinate = Left_txt.Text + "," + Bott_txt.Text + "," + Right_txt.Text + "," + Top_txt.Text;
                    string TemplateName = ComboBoxZone.SelectedItem.ToString();
                    objDSMModelData.UpdateClientCoordinates(coordinate, TemplateName, 2);
                    MessageBox.Show("Invoice Date Co-Ordinates Saved successfully");
                    reset();
                    //txtHintPassword.Text = "Enter Vendor Code";

                }
                else if (Vendor_chk.IsChecked == true)
                {
                    objDSMModelData = new DSMModelData();
                    string coordinate = Left_txt.Text + "," + Bott_txt.Text + "," + Right_txt.Text + "," + Top_txt.Text;
                    string TemplateName = ComboBoxZone.SelectedItem.ToString();
                    objDSMModelData.UpdateClientCoordinates(coordinate, TemplateName, 3);
                    MessageBox.Show("Ewaybill Number Co-Ordinates Saved successfully");
                    reset();
                    //txtHintPassword.Text = "Enter Truck Number";
                }
                else if (Truck_chk.IsChecked == true)
                {
                    objDSMModelData = new DSMModelData();
                    string coordinate = Left_txt.Text + "," + Bott_txt.Text + "," + Right_txt.Text + "," + Top_txt.Text;
                    string TemplateName = ComboBoxZone.SelectedItem.ToString();
                    objDSMModelData.UpdateClientCoordinates(coordinate, TemplateName, 4);
                    MessageBox.Show("Truck Number Co-Ordinates Saved successfully");
                    reset();
                    //txtHintPassword.Text = "Enter Client Name";
                }
                else if (Client_chk.IsChecked == true)
                {
                    objDSMModelData = new DSMModelData();
                    string coordinate = Left_txt.Text + "," + Bott_txt.Text + "," + Right_txt.Text + "," + Top_txt.Text;
                    string TemplateName = ComboBoxZone.SelectedItem.ToString();
                    objDSMModelData.UpdateClientCoordinates(coordinate, TemplateName, 5);
                    MessageBox.Show("Client Name Co-Ordinates Saved successfully");
                    reset();
                    //txtHintPassword.Text = "Enter DS Authorized signatory";
                }
                else if ( DS_chk.IsChecked == true)
                {
                    objDSMModelData = new DSMModelData();
                    string coordinate = Left_txt.Text + "," + Bott_txt.Text + "," + Right_txt.Text + "," + Top_txt.Text;
                    string TemplateName = ComboBoxZone.SelectedItem.ToString();
                    objDSMModelData.UpdateClientCoordinates(coordinate, TemplateName, 6);
                    MessageBox.Show("DS Co-Ordinates Saved successfully");
                    reset();
                    //count = 0;
                    //MessageBox.Show("Completed Successfuly");
                    txtHintPassword.Text = "";

                }
                else if (po_no_chk.IsChecked == true)
                {
                    objDSMModelData = new DSMModelData();
                    string coordinate = Left_txt.Text + "," + Bott_txt.Text + "," + Right_txt.Text + "," + Top_txt.Text;
                    string TemplateName = ComboBoxZone.SelectedItem.ToString();
                    objDSMModelData.UpdateClientCoordinates(coordinate, TemplateName, 7);
                    MessageBox.Show("PO Co-Ordinates Saved successfully");
                    reset();
                    //count = 0;
                    //MessageBox.Show("Completed Successfuly");
                    txtHintPassword.Text = "";

                }
                else if (part_no_chk.IsChecked == true)
                {
                    objDSMModelData = new DSMModelData();
                    string coordinate = Left_txt.Text + "," + Bott_txt.Text + "," + Right_txt.Text + "," + Top_txt.Text;
                    string TemplateName = ComboBoxZone.SelectedItem.ToString();
                    objDSMModelData.UpdateClientCoordinates(coordinate, TemplateName, 8);
                    MessageBox.Show("Part No Co-Ordinates Saved successfully");
                    reset();
                    //count = 0;
                    //MessageBox.Show("Completed Successfuly");
                    txtHintPassword.Text = "";

                }
                else if (Qty_chk.IsChecked == true)
                {
                    objDSMModelData = new DSMModelData();
                    string coordinate = Left_txt.Text + "," + Bott_txt.Text + "," + Right_txt.Text + "," + Top_txt.Text;
                    string TemplateName = ComboBoxZone.SelectedItem.ToString();
                    objDSMModelData.UpdateClientCoordinates(coordinate, TemplateName, 9);
                    MessageBox.Show("Qty Co-Ordinates Saved successfully");
                    reset();
                    //count = 0;
                    //MessageBox.Show("Completed Successfuly");
                    txtHintPassword.Text = "";

                }
                else if (rate_chk.IsChecked == true)
                {
                    objDSMModelData = new DSMModelData();
                    string coordinate = Left_txt.Text + "," + Bott_txt.Text + "," + Right_txt.Text + "," + Top_txt.Text;
                    string TemplateName = ComboBoxZone.SelectedItem.ToString();
                    objDSMModelData.UpdateClientCoordinates(coordinate, TemplateName, 10);
                    MessageBox.Show("Basic Rate Co-Ordinates Saved successfully");
                    reset();
                    //count = 0;
                    //MessageBox.Show("Completed Successfuly");
                    txtHintPassword.Text = "";

                }
                else if (ass_val_chk.IsChecked == true)
                {
                    objDSMModelData = new DSMModelData();
                    string coordinate = Left_txt.Text + "," + Bott_txt.Text + "," + Right_txt.Text + "," + Top_txt.Text;
                    string TemplateName = ComboBoxZone.SelectedItem.ToString();
                    objDSMModelData.UpdateClientCoordinates(coordinate, TemplateName, 11);
                    MessageBox.Show("Assessable Co-Ordinates Saved successfully");
                    reset();
                    //count = 0;
                    //MessageBox.Show("Completed Successfuly");
                    txtHintPassword.Text = "";

                }
                else if (cgst_chk.IsChecked == true)
                {
                    objDSMModelData = new DSMModelData();
                    string coordinate = Left_txt.Text + "," + Bott_txt.Text + "," + Right_txt.Text + "," + Top_txt.Text;
                    string TemplateName = ComboBoxZone.SelectedItem.ToString();
                    objDSMModelData.UpdateClientCoordinates(coordinate, TemplateName, 12);
                    MessageBox.Show("CGST Co-Ordinates Saved successfully");
                    reset();
                    //count = 0;
                    //MessageBox.Show("Completed Successfuly");
                    txtHintPassword.Text = "";

                }
                else if (sgst_chk.IsChecked == true)
                {
                    objDSMModelData = new DSMModelData();
                    string coordinate = Left_txt.Text + "," + Bott_txt.Text + "," + Right_txt.Text + "," + Top_txt.Text;
                    string TemplateName = ComboBoxZone.SelectedItem.ToString();
                    objDSMModelData.UpdateClientCoordinates(coordinate, TemplateName, 13);
                    MessageBox.Show("SGST Co-Ordinates Saved successfully");
                    reset();
                    //count = 0;
                    //MessageBox.Show("Completed Successfuly");
                    txtHintPassword.Text = "";

                }
                
                else if (tot_val_chk.IsChecked == true)
                {
                    objDSMModelData = new DSMModelData();
                    string coordinate = Left_txt.Text + "," + Bott_txt.Text + "," + Right_txt.Text + "," + Top_txt.Text;
                    string TemplateName = ComboBoxZone.SelectedItem.ToString();
                    objDSMModelData.UpdateClientCoordinates(coordinate, TemplateName, 14);
                    MessageBox.Show("Total Invoice Co-Ordinates Saved successfully");
                    reset();
                    //count = 0;
                    //MessageBox.Show("Completed Successfuly");
                    txtHintPassword.Text = "";

                }
                else if (Irn_chk.IsChecked == true)
                {
                    objDSMModelData = new DSMModelData();
                    string coordinate = Left_txt.Text + "," + Bott_txt.Text + "," + Right_txt.Text + "," + Top_txt.Text;
                    string TemplateName = ComboBoxZone.SelectedItem.ToString();
                    objDSMModelData.UpdateClientCoordinates(coordinate, TemplateName, 15);
                    MessageBox.Show("Irn Co-Ordinates Saved successfully");
                    reset();
                    //count = 0;
                    //MessageBox.Show("Completed Successfuly");
                    txtHintPassword.Text = "";

                }
                else if (tcs_chk.IsChecked == true)
                {
                    objDSMModelData = new DSMModelData();
                    string coordinate = Left_txt.Text + "," + Bott_txt.Text + "," + Right_txt.Text + "," + Top_txt.Text;
                    string TemplateName = ComboBoxZone.SelectedItem.ToString();
                    objDSMModelData.UpdateClientCoordinates(coordinate, TemplateName, 16);
                    MessageBox.Show("TCS Co-Ordinates Saved successfully");
                    reset();
                    //count = 0;
                    //MessageBox.Show("Completed Successfuly");
                    txtHintPassword.Text = "";

                }
                else if (shop_cd_chk.IsChecked == true)
                {
                    objDSMModelData = new DSMModelData();
                    string coordinate = Left_txt.Text + "," + Bott_txt.Text + "," + Right_txt.Text + "," + Top_txt.Text;
                    string TemplateName = ComboBoxZone.SelectedItem.ToString();
                    objDSMModelData.UpdateClientCoordinates(coordinate, TemplateName, 17);
                    MessageBox.Show("Shop Code Co-Ordinates Saved successfully");
                    reset();
                    //count = 0;
                    //MessageBox.Show("Completed Successfuly");
                    txtHintPassword.Text = "";

                }
                else if (HSN_chk.IsChecked == true)
                {
                    objDSMModelData = new DSMModelData();
                    string coordinate = Left_txt.Text + "," + Bott_txt.Text + "," + Right_txt.Text + "," + Top_txt.Text;
                    string TemplateName = ComboBoxZone.SelectedItem.ToString();
                    objDSMModelData.UpdateClientCoordinates(coordinate, TemplateName, 18);
                    MessageBox.Show("HSN Code Co-Ordinates Saved successfully");
                    reset();
                    //count = 0;
                    //MessageBox.Show("Completed Successfuly");
                    txtHintPassword.Text = "";

                }
                else if (IGST_chk.IsChecked == true)
                {
                    objDSMModelData = new DSMModelData();
                    string coordinate = Left_txt.Text + "," + Bott_txt.Text + "," + Right_txt.Text + "," + Top_txt.Text;
                    string TemplateName = ComboBoxZone.SelectedItem.ToString();
                    objDSMModelData.UpdateClientCoordinates(coordinate, TemplateName, 19);
                    MessageBox.Show("IGST Co-Ordinates Saved successfully");
                    reset();
                    //count = 0;
                    //MessageBox.Show("Completed Successfuly");
                    txtHintPassword.Text = "";

                }
                else if (CGSTRT_chk.IsChecked == true)
                {
                    objDSMModelData = new DSMModelData();
                    string coordinate = Left_txt.Text + "," + Bott_txt.Text + "," + Right_txt.Text + "," + Top_txt.Text;
                    string TemplateName = ComboBoxZone.SelectedItem.ToString();
                    objDSMModelData.UpdateClientCoordinates(coordinate, TemplateName, 20);
                    MessageBox.Show("CGSTRT Co-Ordinates Saved successfully");
                    reset();
                    //count = 0;
                    //MessageBox.Show("Completed Successfuly");
                    txtHintPassword.Text = "";

                }
                else if (SGSTRT_chk.IsChecked == true)
                {
                    objDSMModelData = new DSMModelData();
                    string coordinate = Left_txt.Text + "," + Bott_txt.Text + "," + Right_txt.Text + "," + Top_txt.Text;
                    string TemplateName = ComboBoxZone.SelectedItem.ToString();
                    objDSMModelData.UpdateClientCoordinates(coordinate, TemplateName, 21);
                    MessageBox.Show("SGSTRT Co-Ordinates Saved successfully");
                    reset();
                    //count = 0;
                    //MessageBox.Show("Completed Successfuly");
                    txtHintPassword.Text = "";

                }
                else if (IGSTRT_chk.IsChecked == true)
                {
                    objDSMModelData = new DSMModelData();
                    string coordinate = Left_txt.Text + "," + Bott_txt.Text + "," + Right_txt.Text + "," + Top_txt.Text;
                    string TemplateName = ComboBoxZone.SelectedItem.ToString();
                    objDSMModelData.UpdateClientCoordinates(coordinate, TemplateName, 22);
                    MessageBox.Show("IGSTRT Co-Ordinates Saved successfully");
                    reset();
                    //count = 0;
                    //MessageBox.Show("Completed Successfuly");
                    txtHintPassword.Text = "";

                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Retry");
            }

        }

        private void Coord_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (FilePath.Text != "")
                {
                    if (ComboBoxZone.Text != "")
                    {
                        if (Inv_no_chk.IsChecked == true || Inv_date_chk.IsChecked == true || Vendor_chk.IsChecked == true || Truck_chk.IsChecked == true || Client_chk.IsChecked == true || DS_chk.IsChecked == true || po_no_chk.IsChecked == true || part_no_chk.IsChecked == true || Qty_chk.IsChecked == true || rate_chk.IsChecked == true || ass_val_chk.IsChecked == true || cgst_chk.IsChecked == true || sgst_chk.IsChecked == true || tot_val_chk.IsChecked == true || Irn_chk.IsChecked == true || tcs_chk.IsChecked == true || shop_cd_chk.IsChecked == true || HSN_chk.IsChecked == true || IGST_chk.IsChecked == true || CGSTRT_chk.IsChecked == true || SGSTRT_chk.IsChecked == true || IGSTRT_chk.IsChecked == true)
                        {
                            if (Search_txt.Text != "")
                            {
                                using (PdfReader reader = new PdfReader(FilePath.Text))
                                {

                                    var parser = new PdfReaderContentParser(reader);

                                    var strategy = parser.ProcessContent(1, new LocationTextExtractionStrategyWithPosition());

                                    var res = strategy.GetLocations(Search_txt.Text);

                                    reader.Close();

                                    var searchResult = res.Where(p => p.Text.Contains(Search_txt.Text)).OrderBy(p => p.Y).Reverse().ToList();

                                    double x = Math.Round((double)searchResult[0].X);
                                    double X_cord = Math.Round((double)searchResult[0].X1);
                                    double y = Math.Round((double)searchResult[0].Y);
                                    double Y_cord = Math.Round((double)searchResult[0].Y);
                                    Left_txt.Text = x.ToString(); //x1
                                    Bott_txt.Text = (y - 3).ToString(); //y1
                                    Right_txt.Text = X_cord.ToString(); //x2
                                    Top_txt.Text = ((Y_cord) + 12).ToString(); //y2


                                    //old frmt
                                    //double x = (double)searchResult[0].X;
                                    //string X_cord = Math.Round(x).ToString();
                                    //double y = (double)searchResult[0].Y;
                                    //string Y_cord = Math.Round(y).ToString();
                                    //Left_txt.Text = X_cord; //x1
                                    //Bott_txt.Text = (int.Parse(Y_cord) - 3).ToString(); //y1
                                    //Right_txt.Text = (int.Parse(X_cord) + 90).ToString(); //x2
                                    //Top_txt.Text = (int.Parse(Y_cord) + 40).ToString(); //y2

                                }
                            }
                            else
                            {
                                MessageBox.Show("Please enter search text..!");
                            }
                        }
                        else
                        {
                            MessageBox.Show("select the field which you want to set the Coordinate..");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please select Client ID");
                    }
                }
                else
                {
                    MessageBox.Show("Please Browse Filepath");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("No Data found");
            }
        }

        private void Browse_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    FilePath.Text = openFileDialog.FileName;
                    pdfviewer.Navigate("about:blank");
                    pdfviewer.Navigate(FilePath.Text);
                   
                }

            }
            catch (Exception)
            {
                // handle exception here;
            }
        }
        public void reset()
        {
            Left_txt.Text = "";
            Right_txt.Text = "";
            Bott_txt.Text = "";
            Top_txt.Text = "";
            Search_txt.Text = "";
            Inv_no_chk.IsChecked = false;
            Inv_date_chk.IsChecked = false;
            Vendor_chk.IsChecked = false;
            Truck_chk.IsChecked = false;
            Client_chk.IsChecked = false;
            DS_chk.IsChecked = false;
            po_no_chk.IsChecked = false;
            part_no_chk.IsChecked = false;
            Qty_chk.IsChecked = false;
            rate_chk.IsChecked = false;
            ass_val_chk.IsChecked = false;
            sgst_chk.IsChecked = false;
            cgst_chk.IsChecked = false;
            tot_val_chk.IsChecked = false;
            Irn_chk.IsChecked = false;
            tcs_chk.IsChecked = false;
            shop_cd_chk.IsChecked = false;
            HSN_chk.IsChecked = false;
            IGST_chk.IsChecked = false;
            CGSTRT_chk.IsChecked = false;
            SGSTRT_chk.IsChecked = false;
            IGSTRT_chk.IsChecked = false;
            //FilePath.Text = ""; 
        }
        
        private void Locate_btn_Click(object sender, RoutedEventArgs e)
        {
            Random rnd = new Random();
            Filename = rnd.Next(1000,9999).ToString();
            string inputFile = FilePath.Text;
            if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(FilePath.Text) +  "Output" + "\\"))
            {

                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(FilePath.Text)  + "Output" + "\\");
            }

            outputFile = System.IO.Path.GetDirectoryName(FilePath.Text)  + "Output" + "\\" + Filename + ".pdf";
            try
            {
                PdfReader pdfReader = new PdfReader(inputFile);
                using (FileStream fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    using (PdfStamper stamper = new PdfStamper(pdfReader, fs))
                    {
                        int PageCount = pdfReader.NumberOfPages;
                        for (int x = 1; x <= PageCount; x++)
                        {
                            PdfContentByte cb = stamper.GetOverContent(x);
                            iTextSharp.text.Rectangle rectangle = new iTextSharp.text.Rectangle(int.Parse(Left_txt.Text), int.Parse(Bott_txt.Text), int.Parse(Right_txt.Text), int.Parse(Top_txt.Text));
                            //rectangle.BackgroundColor = BaseColor.BLACK;
                            rectangle.Border = iTextSharp.text.Rectangle.BOX;
                            rectangle.BorderWidth = 1;
                            rectangle.BorderColor = new BaseColor(255,0,0);
                            cb.Rectangle(rectangle);
                        }
                    }
                    fs.Close();
                }

                pdfviewer.Navigate(outputFile);
            }
            catch(Exception)
            {
                MessageBox.Show("Loading...");
            }
        }

        private void Edit_btn_Click(object sender, RoutedEventArgs e)
        {
            Left_txt.IsEnabled = true;
            Right_txt.IsEnabled = true;
            Bott_txt.IsEnabled = true;
            Top_txt.IsEnabled = true;
        }
        
        private void Search_txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtHintPassword.Visibility = Visibility.Visible;
            if (Search_txt.Text.Length > 0)
            {
                txtHintPassword.Visibility = Visibility.Hidden;
            }
        }
         
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            //objDSMModelData = new DSMModelData();
            //Customer = new CustomerDisplayModel();
            //Customers = new ObservableCollection<CustomerDisplayModel>(objDSMModelData.GetAllCustomers());
            //choose_lbl.Visibility = Visibility.Hidden;
            //available_lbl.Visibility = Visibility.Hidden;
            //find_lbl.Visibility = Visibility.Hidden;
            //Search_txt.Visibility = Visibility.Hidden;
            //FilePath.Visibility = Visibility.Hidden;
            //ComboBoxZone.Visibility = Visibility.Hidden;
            //Browse_btn.Visibility = Visibility.Hidden;
            //view_btn.Visibility = Visibility.Hidden;
            //Coord_btn.Visibility = Visibility.Hidden;
            BindComboBox(ComboBoxZone);
            
        }

        public void BindComboBox(ComboBox comboBoxName)
        {
            //SqlConnection conn = new SqlConnection("your connection string");
            //SqlDataAdapter da = new SqlDataAdapter("Select ZoneId,ZoneName FROM tblZones", conn);
            //DataSet ds = new DataSet();
            //da.Fill(ds, "tblZones");

            objDSMModelData = new DSMModelData();
            Template = new CustomerDisplayModel();
            Templates = new ObservableCollection<CustomerDisplayModel>(objDSMModelData.GetAllTemplates());
            
            foreach (var template in Templates)
            {
                ComboBoxZone.Items.Add(template.Tempname);
            }
           
        }

        private void view_btn_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBoxZone.Text != "")
            {
                if (FilePath.Text != "")
                {


                    if (Inv_no_chk.IsChecked == true || Inv_date_chk.IsChecked == true || Vendor_chk.IsChecked == true || Truck_chk.IsChecked == true || Client_chk.IsChecked == true || DS_chk.IsChecked == true || po_no_chk.IsChecked==true || part_no_chk.IsChecked == true || Qty_chk.IsChecked == true || rate_chk.IsChecked == true || ass_val_chk.IsChecked == true || cgst_chk.IsChecked == true || sgst_chk.IsChecked == true || tot_val_chk.IsChecked == true || Irn_chk.IsChecked == true || tcs_chk.IsChecked == true || shop_cd_chk.IsChecked == true || HSN_chk.IsChecked == true || IGST_chk.IsChecked == true || CGSTRT_chk.IsChecked == true || SGSTRT_chk.IsChecked == true || IGSTRT_chk.IsChecked == true)
                    {
                        Random rnd = new Random();
                        Filename = rnd.Next(1000, 9999).ToString();
                        string inputFile = FilePath.Text;
                        if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(FilePath.Text) + "Output" + "\\"))
                        {

                            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(FilePath.Text) + "Output" + "\\");
                        }

                        outputFile = System.IO.Path.GetDirectoryName(FilePath.Text) + "Output" + "\\" + Filename + ".pdf";
                        //getting data from db
                        var dscoordinate = objDSMModelData.GetAllTemplatesCord(ComboBoxZone.SelectedItem.ToString());

                        string coord = "";

                        for (int i = 0; i < dscoordinate.Count; i++)
                        {
                            if (dscoordinate[i].Invnocoord != null && dscoordinate[i].Invdatecoord != null && dscoordinate[i].Vendorcoord != null && dscoordinate[i].Trucknocoord != null && dscoordinate[i].Clientnamecoord != null && dscoordinate[i].Dscoord != null)
                            {
                                if (Inv_no_chk.IsChecked == true)
                                {
                                    coord = dscoordinate[i].Invnocoord.ToString().Trim();
                                }
                                else if (Inv_date_chk.IsChecked == true)
                                {
                                    coord = dscoordinate[i].Invdatecoord.ToString().Trim();
                                }
                                else if (Vendor_chk.IsChecked == true)
                                {
                                    coord = dscoordinate[i].Vendorcoord.ToString().Trim();
                                }
                                else if (Truck_chk.IsChecked == true)
                                {
                                    coord = dscoordinate[i].Trucknocoord.ToString().Trim();
                                }
                                else if (Client_chk.IsChecked == true)
                                {
                                    coord = dscoordinate[i].Clientnamecoord.ToString().Trim();
                                }
                                else if (DS_chk.IsChecked == true)
                                {
                                    coord = dscoordinate[i].Dscoord.ToString().Trim();
                                }
                                else if (po_no_chk.IsChecked == true)
                                {
                                    coord = dscoordinate[i].Pocoord.ToString().Trim();
                                }
                                else if (part_no_chk.IsChecked == true)
                                {
                                    coord = dscoordinate[i].Partcoord.ToString().Trim();
                                }
                                else if (Qty_chk.IsChecked == true)
                                {
                                    coord = dscoordinate[i].Qtycoord.ToString().Trim();
                                }
                                else if (rate_chk.IsChecked == true)
                                {
                                    coord = dscoordinate[i].Ratecoord.ToString().Trim();
                                }
                                else if (ass_val_chk.IsChecked == true)
                                {
                                    coord = dscoordinate[i].Asscoord.ToString().Trim();
                                }
                                else if (CGSTRT_chk.IsChecked == true)
                                {
                                    coord = dscoordinate[i].Cgstrtcoord.ToString().Trim();
                                }
                                else if (SGSTRT_chk.IsChecked == true)
                                {
                                    coord = dscoordinate[i].Sgstrtcoord.ToString().Trim();
                                }
                                else if (IGSTRT_chk.IsChecked == true)
                                {
                                    coord = dscoordinate[i].Igstrtcoord.ToString().Trim();
                                }
                                else if (cgst_chk.IsChecked == true)
                                {
                                    coord = dscoordinate[i].Cgstcoord.ToString().Trim();
                                }
                                else if (sgst_chk.IsChecked == true)
                                {
                                    coord = dscoordinate[i].Sgstcoord.ToString().Trim();
                                }
                                else if (tot_val_chk.IsChecked == true)
                                {
                                    coord = dscoordinate[i].Totvalcoord.ToString().Trim();
                                }
                                else if (Irn_chk.IsChecked == true)
                                {
                                    coord = dscoordinate[i].Irncoord.ToString().Trim();
                                }
                                else if (tcs_chk.IsChecked == true)
                                {
                                    coord = dscoordinate[i].Tcscoord.ToString().Trim();
                                }
                                else if (shop_cd_chk.IsChecked == true)
                                {
                                    coord = dscoordinate[i].Shopcdcoord.ToString().Trim();
                                }
                                else if (HSN_chk.IsChecked == true)
                                {
                                    coord = dscoordinate[i].Hsncoord.ToString().Trim();
                                }
                                else if (IGST_chk.IsChecked == true)
                                {
                                    coord = dscoordinate[i].Igstcoord.ToString().Trim();
                                }
                            }
                            else
                            {
                                MessageBox.Show("Co-Ordinates not found..!");
                            }
                        }


                        if (coord.Length > 0 && coord.ToString().Trim() != "")
                        {
                            string[] arrcoord1 = coord.Split(',');

                            try
                            {
                                PdfReader pdfReader = new PdfReader(inputFile);
                                using (FileStream fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write, FileShare.None))
                                {
                                    using (PdfStamper stamper = new PdfStamper(pdfReader, fs))
                                    {
                                        int PageCount = pdfReader.NumberOfPages;
                                        for (int x = 1; x <= PageCount; x++)
                                        {
                                            PdfContentByte cb = stamper.GetOverContent(x);
                                            iTextSharp.text.Rectangle rectangle1 = new iTextSharp.text.Rectangle(int.Parse(arrcoord1[0]), int.Parse(arrcoord1[1]), int.Parse(arrcoord1[2]), int.Parse(arrcoord1[3]));

                                            //rectangle.BackgroundColor = BaseColor.BLACK;
                                            rectangle1.Border = iTextSharp.text.Rectangle.BOX;
                                            rectangle1.BorderWidth = 1;
                                            rectangle1.BorderColor = new BaseColor(255, 0, 0);
                                            cb.Rectangle(rectangle1);

                                        }
                                    }
                                    fs.Close();
                                }
                                Left_txt.Text = arrcoord1[0].ToString();
                                Bott_txt.Text = arrcoord1[1].ToString();
                                Right_txt.Text = arrcoord1[2].ToString();
                                Top_txt.Text = arrcoord1[3].ToString();
                                pdfviewer.Navigate(outputFile);
                            }

                            catch (Exception)
                            {
                                MessageBox.Show("Loading...");
                            }
                        }
                    }

                    else
                    {
                        MessageBox.Show("select the field which you want to update..");
                    }
                }
                else
                {
                    MessageBox.Show("Browse your file..");
                }
            }
            else
            {
                MessageBox.Show("Please select Client ID");
            }
        }

        private void Add_btn_Click(object sender, RoutedEventArgs e)
        {
            //choose_lbl.Visibility = Visibility.Visible;
            //available_lbl.Visibility = Visibility.Visible;
            //find_lbl.Visibility = Visibility.Visible;
            //Search_txt.Visibility = Visibility.Visible;
            //FilePath.Visibility = Visibility.Visible;
            //ComboBoxZone.Visibility = Visibility.Visible;
            //Browse_btn.Visibility = Visibility.Visible;
            //view_btn.Visibility = Visibility.Visible;
            //Coord_btn.Visibility = Visibility.Visible;
            //Add_btn.Visibility = Visibility.Hidden;
            //Temp_name.IsEnabled = false;
          
            objDSMModelData = new DSMModelData();
            objDSMModelData.SaveTemplateName(Temp_name.Text);
            ComboBoxZone.Items.Clear();
            BindComboBox(ComboBoxZone);
            MessageBox.Show("Template Added Successfully");
            Temp_name.Text = "";
        }

    
    }
}
