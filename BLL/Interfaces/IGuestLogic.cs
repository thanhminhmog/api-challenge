using BLL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Interfaces
{
    public interface IGuestLogic
    {

        string Login(UserLogin user);
        UserLogin Register(UserRegister user);
    }
}
