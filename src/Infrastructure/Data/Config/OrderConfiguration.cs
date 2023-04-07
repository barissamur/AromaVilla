﻿using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Config
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            // address sınıfı veritabanına eklenmedi. adresin property'leri order içine eklendi
            builder.OwnsOne(x => x.ShippingAddress, ba =>   // içinde shippingadres yer alır ve onun özellikleri
            {
                ba.WithOwner(); //her shipping adresin mutlaka sahibi vardır

                ba.Property(a => a.Street)
                    .HasMaxLength(180);

                ba.Property(a => a.City)
                    .HasMaxLength(100);

                ba.Property(a => a.State)
                    .HasMaxLength(60);

                ba.Property(a => a.Country)
                    .HasMaxLength(90);

                ba.Property(a => a.ZipCode)
                    .HasMaxLength(18);
            });
        }
    }
}