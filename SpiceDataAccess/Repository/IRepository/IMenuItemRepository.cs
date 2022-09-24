using SpicesModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiceDataAccess.Repository.IRepository
{
    public interface IMenuItemRepository: IRepository<MenuItem>
    {
        Task Update(MenuItem obj);
    }
}
