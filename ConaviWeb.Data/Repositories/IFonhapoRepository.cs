using ConaviWeb.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConaviWeb.Data.Repositories
{
    public interface IFonhapoRepository
    {
        Task<IEnumerable<string>> GetFonhapo();
        Task<bool> UpdateFonhapo(Fonhapo fonhapo);
    }
}
