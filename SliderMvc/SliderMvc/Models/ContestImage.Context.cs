﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SliderMvc.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class PhotoGalleryEntities2 : DbContext
    {
        public PhotoGalleryEntities2()
            : base("name=PhotoGalleryEntities2")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ContestImage> ContestImages { get; set; }

        public System.Data.Entity.DbSet<SliderMvc.Models.UserDetail> UserDetails { get; set; }
    }
}