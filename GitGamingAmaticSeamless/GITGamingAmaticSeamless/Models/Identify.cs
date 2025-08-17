using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace GITIGamingWebsite.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
            : base("WebGameConnectionString")
        {

        }
    }
}