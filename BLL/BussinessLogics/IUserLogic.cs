using BLL.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BLL.BussinessLogics
{
    public interface IUserLogic
    {
        public List<ChallengeProfile> ViewChallengesList(UserProfile userProfile);
        public ChallengeContent ViewChallengeContent(Guid ChallengeId);
        Task<bool> WritingAnObjectAsync(Stream fileStream, string fileName);
        Task<(Stream FileStream, string ContentType)> ReadFileAsync(string fileName);
    }
}
