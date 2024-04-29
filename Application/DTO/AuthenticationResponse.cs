using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class AuthenticationResponse
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }  
        public string? Token { get; set; }
    }
}
