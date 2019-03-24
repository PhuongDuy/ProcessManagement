//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProcessManagement.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TaskProcess
    {
        public int Id { get; set; }
        public int IdStep { get; set; }
        public Nullable<int> IdRole { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ValueInputText { get; set; }
        public string ValueInputFile { get; set; }
        public string ValueFormJson { get; set; }
        public string Color { get; set; }
        public int Position { get; set; }
        public Nullable<bool> IsRun { get; set; }
        public System.DateTime Created_At { get; set; }
        public System.DateTime Updated_At { get; set; }
    
        public virtual Role Role { get; set; }
        public virtual Step Step { get; set; }
    }
}
