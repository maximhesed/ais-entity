//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ais.src.model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Campaigns
    {
        public byte gid { get; set; }
        public System.DateTime comp_date { get; set; }
        public decimal budget_starting { get; set; }
        public decimal budget_contractors { get; set; }
        public bool status { get; set; }
    
        public virtual Groups Groups { get; set; }
    }
}