using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GITIGamingWebsite.Models
{
    public class amaticgame
    {
        public int          id              { get; set; }
        public string       symbol          { get; set; }
        public string       name            { get; set; }
        public int          hasdeveloped    { get; set; }
        public DateTime?    updatetime      { get; set; }
    }
}