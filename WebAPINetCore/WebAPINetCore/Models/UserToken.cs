using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPINetCore.Models
{
    public class UserToken
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }

        public string Rut { get; set; }

        public string IdUser { get; set; }

        public string Nombre { get; set; }

        public string TipoUser { get; set; }

        public string IdSession { get; set; }
    }
}
