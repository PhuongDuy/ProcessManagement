﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class PMSEntities : DbContext
    {
        public PMSEntities()
            : base("name=PMSEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<Input> Inputs { get; set; }
        public virtual DbSet<Participate> Participates { get; set; }
        public virtual DbSet<Process> Processes { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Step> Steps { get; set; }
        public virtual DbSet<TaskProcess> TaskProcesses { get; set; }
        public virtual DbSet<Type> Types { get; set; }
    }
}
