﻿using DSMData;
using DSMData.Model;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DSM.ViewModels
{
    public class ReportViewModel : BindableBase
    {
        DSMModelData objDSMModelData;
        public ICommand SearchInvoiceCommand { get; set; }
        public ICommand ExportInvoiceCommand { get; set; }

        private string _InvNumber;
        public string InvNumber
        {
            get { return _InvNumber; }
            set { SetProperty(ref _InvNumber, value); }
        }

        private DateTime _FromDate;
        public DateTime FromDate
        {
            get { return _FromDate; }
            set { SetProperty(ref _FromDate, value); }
        }

        private DateTime _ToDate;
        public DateTime ToDate
        {
            get { return _ToDate; }
            set { SetProperty(ref _ToDate, value); }
        }

        private ObservableCollection<InvoiceReportModel> _invoiceList = new ObservableCollection<InvoiceReportModel>();
        public ObservableCollection<InvoiceReportModel> InvoiceList
        {
            get { return _invoiceList; }
            set { SetProperty(ref _invoiceList, value); }
        }

        public ReportViewModel()
        {
            objDSMModelData = new DSMModelData();
            FromDate = DateTime.Now;
            ToDate = DateTime.Now;
            InvoiceList = new ObservableCollection<InvoiceReportModel>(objDSMModelData.FilterInvoiceData("", FromDate.ToString(), ToDate.ToString()));
            SearchInvoiceCommand = new DelegateCommand(SearchInvoice);
            ExportInvoiceCommand = new DelegateCommand(ExportExcel);
        }

        public void SearchInvoice()
        {
            InvoiceList = new ObservableCollection<InvoiceReportModel>(objDSMModelData.FilterInvoiceData(InvNumber == null ? "" : InvNumber, FromDate.ToString(), ToDate.ToString()));
        }

        public void ExportExcel()
        {
            if (InvoiceList.Count() > 0)
            {
                var invList = new List<InvoiceReportModel>(InvoiceList);
                ListToExcel(invList);
            }
            else
            {
                MessageBox.Show("No records!");
            }
        }

        public void ListToExcel<T>(List<T> list)
        {
            //start excel
            Microsoft.Office.Interop.Excel.Application excapp = new Microsoft.Office.Interop.Excel.Application();

            //if you want to make excel visible
            excapp.Visible = true;

            //create a blank workbook
            var workbook = excapp.Workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);

            //or open one - this is no pleasant, but yue're probably interested in the first parameter
            string workbookPath = AppDomain.CurrentDomain.BaseDirectory + "ExportInvoiceReport.xlsx";
            var worksbook = excapp.Workbooks.Open(workbookPath,
                0, false, 5, "", "", false, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "",
                true, false, 0, true, false, false);

            //Not done yet. You have to work on a specific sheet - note the cast
            //You may not have any sheets at all. Then you have to add one with NsExcel.Worksheet.Add()
            var sheet = (Microsoft.Office.Interop.Excel.Worksheet)worksbook.Sheets[1]; //indexing starts from 1
            sheet.Rows.Clear();

            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));

            for (int i = 0; i < properties.Count; i++)
                sheet.Cells[1, i + 1] = properties[i].Name;

            for (int i = 0; i < list.Count; i++)
                for (int j = 0; j < properties.Count; j++)
                    sheet.Cells[i + 2, j + 1] = properties[j].GetValue(list[i]);

        }
    }
}
