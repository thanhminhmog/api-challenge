using BLL.Models;
using DAL.Entities;
using DAL.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL.BussinessLogics
{
    public class UserLogic : IUserLogic
    {
        private readonly IUnitOfWork _uow;
        public UserLogic(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public ChallengeContent ViewChallengeContent(Guid ChallengeId)
        {
            Challenge challenge = _uow.GetRepository<Challenge>().GetAll().FirstOrDefault(c => c.ChallengeId == ChallengeId);
            ChallengeContent challengeContent = new ChallengeContent
            {
                ChallengeName = challenge.Name,
                ChallengeDescription = challenge.Content,
                PositionName = _uow.GetRepository<Position>().GetAll().SingleOrDefault(p => p.PositionId == challenge.PositionId).Name
            };
            return challengeContent;
        }

        public List<ChallengeProfile> ViewChallengesList(UserProfile userProfile)
        {
            string PosName = userProfile.PositionName;
            Guid posID = _uow.GetRepository<Position>().GetAll().SingleOrDefault(p => p.Name == PosName).PositionId;
            List<Challenge> ChallengesList = _uow
                .GetRepository<Challenge>()
                .GetAll()
                .Where(c => c.PositionId == posID).ToList();
            List<ChallengeProfile> ChallengeProfilesList = new List<ChallengeProfile>();
            foreach (Challenge c in ChallengesList)
            {
                ChallengeProfilesList.Add(new ChallengeProfile
                {
                    ChallengeId = c.ChallengeId,
                    ChallengeName = c.Name
                });
            }

            return ChallengeProfilesList;
        }
    }
}
