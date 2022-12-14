//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DSMData
{
    using System;
    
    public partial class SP_FILTER_INVOICE_Result
    {
        public int InvoiceId { get; set; }
        public string CustomerName { get; set; }
        public string VehicleNumber { get; set; }
        public string VendorCode { get; set; }
        public string ShopCode { get; set; }
        public string PONumber { get; set; }
        public string PartNumber { get; set; }
        public string InvNumber { get; set; }
        public Nullable<System.DateTime> InvDate { get; set; }
        public string InvQuantity { get; set; }
        public string InvValue { get; set; }
        public string TarrifNumber { get; set; }
        public string BedAmount { get; set; }
        public string SGST { get; set; }
        public string IGST { get; set; }
        public string VatAmount { get; set; }
        public string UnitPrice { get; set; }
        public string MaterialCost { get; set; }
        public string CGST { get; set; }
        public string ConsigneePartCost { get; set; }
        public string ExciseDutyCost { get; set; }
        public string AssessableValue { get; set; }
        public string CSTAmount { get; set; }
        public string ToolCost { get; set; }
        public string ConsigneeMatlCost { get; set; }
        public string GSTN { get; set; }
        public string BatchNumber { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string DSStatus { get; set; }
        public byte[] OriginalPDF { get; set; }
        public byte[] DuplicatePDF { get; set; }
        public byte[] QuadraplicatePDF { get; set; }
        public byte[] ExtraPDF { get; set; }
        public string PrintStatus { get; set; }
        public string APIStatus { get; set; }
        public string APIMessage { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
    }
}
