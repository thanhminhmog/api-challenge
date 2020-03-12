using BLL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.BussinessLogics
{
    public interface IUserLogic
    {
        public List<ChallengeProfile> ViewChallengesList(UserProfile userProfile);
        public ChallengeContent ViewChallengeContent(Guid ChallengeId);
    }
}
