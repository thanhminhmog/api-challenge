using BLL.Models;
using BLL.Helpers;
using DAL.Entities;
using DAL.UnitOfWorks;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL.BussinessLogics
{
    public class GuestLogic : IGuestLogic
    {
        #region OnCall
        protected readonly IUnitOfWork _uow;
        protected readonly IOptions<AppSetting> _options;

        public GuestLogic(IUnitOfWork uow, IOptions<AppSetting> options)
        {
            _uow = uow;
            _options = options;
        }
        #endregion


        public string Login(UserLogin user)
        {
            User loggedUser = _uow.GetRepository<User>()
                                  .GetAll()
                                  .SingleOrDefault(u => u.Email == user.Email && u.ConfirmationCode == user.ConfirmationCode);
            string positionName = _uow.GetRepository<Position>().GetAll().SingleOrDefault(p => p.PositionId == loggedUser.PositionId).Name;
            UserProfile userProfile = new UserProfile
            {
                Email = loggedUser.Email,
                PositionName = positionName
            };
            TokenManager tokenManager = new TokenManager(_options);
            string tokenString = tokenManager.CreateAccessToken(userProfile);
            return tokenString;
        }

        public UserLogin Register(UserRegister user)
        {
            Guid positionId = _uow.GetRepository<Position>().GetAll().SingleOrDefault(p => p.Name == user.PositionName).PositionId;
            string newCofimationCode = new ConfirmationCodeManager().GenerateConfimationCode();

            //  Use domain class for now, will implement mapper later
            User newUser = new User
            {
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
                PositionId = positionId,
                UserId = Guid.NewGuid(),
                ConfirmationCode = newCofimationCode
            };

            _uow.GetRepository<User>().Insert(newUser);
            _uow.Commit();
            return new UserLogin { Email = newUser.Email, ConfirmationCode = newUser.ConfirmationCode };
        }
    }
}
