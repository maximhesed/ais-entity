//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ais.model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Groups
    {
        public byte id { get; set; }
        public byte pid { get; set; }
        public byte adid { get; set; }
        public byte gsid { get; set; }
        public byte cid { get; set; }
        public System.DateTime comp_date { get; set; }
        public Nullable<int> lid { get; set; }
    
        public virtual Leads Leads { get; set; }
    }
}
