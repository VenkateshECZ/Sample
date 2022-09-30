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
    using System.Collections.Generic;
    
    public partial class ClientMaster
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ClientMaster()
        {
            this.DigitalSignDetails = new HashSet<DigitalSignDetail>();
            this.EDIConfigurations = new HashSet<EDIConfiguration>();
        }
    
        public int ClientLineId { get; set; }
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<bool> IsDigitalSign { get; set; }
        public Nullable<bool> IsAPIpost { get; set; }
        public string APIUrl { get; set; }
        public Nullable<bool> APItype { get; set; }
        public Nullable<bool> PDFtype { get; set; }
        public Nullable<bool> PrintType { get; set; }
        public string PrintCopies { get; set; }
        public string VendorCode { get; set; }
        public string TemplateName { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DigitalSignDetail> DigitalSignDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EDIConfiguration> EDIConfigurations { get; set; }
    }
}
