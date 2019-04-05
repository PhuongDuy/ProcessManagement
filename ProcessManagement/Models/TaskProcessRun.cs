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
    
    public partial class TaskProcessRun
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TaskProcessRun()
        {
            this.Comments = new HashSet<Comment>();
        }
    
        public int Id { get; set; }
        public int IdStep { get; set; }
        public Nullable<int> IdRole { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<int> Status { get; set; }
        public string ValueInputText { get; set; }
        public string ValueInputFile { get; set; }
        public string ValueFormJson { get; set; }
        public string Color { get; set; }
        public int Position { get; set; }
        public string ApproveBy { get; set; }
        public string DoneBy { get; set; }
        public Nullable<System.DateTime> Done_At { get; set; }
        public Nullable<System.DateTime> Approve_At { get; set; }
        public System.DateTime Created_At { get; set; }
        public System.DateTime Updated_At { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual AspNetUser AspNetUser1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual Role Role { get; set; }
        public virtual Status Status1 { get; set; }
        public virtual StepRun StepRun { get; set; }
    }
}
