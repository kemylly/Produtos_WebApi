using System;
using System.Collections.Generic;
using System.Text;
using estudo_api.Models;
using Microsoft.EntityFrameworkCore;

namespace estudo_api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Produto> Produtos {get; set;}
        public DbSet<Usuario> Usuarios {get;set;}
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base (options)
        {

        }
    }
}