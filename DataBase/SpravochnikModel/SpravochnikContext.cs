using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;


namespace DataBase.SpravochnikModel
{
    public class SpravochnikContext : DbContext
    {
        public SpravochnikContext() : base("HomeConnection") { }

        public DbSet<Spravochnik> Spravochniks { get; set; }
    }
}
