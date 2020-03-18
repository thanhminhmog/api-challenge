using BLL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Interfaces
{
    public interface IAdminLogic
    {
        List<PageModel> GetPageModelList();
        List<PageModel> GetSearchPageModelList(string name);
    }
}
