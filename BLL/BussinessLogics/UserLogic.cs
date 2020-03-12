using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using BLL.Helpers;
using BLL.Models;
using DAL.Entities;
using DAL.UnitOfWorks;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.BussinessLogics
{
    public class UserLogic : IUserLogic
    {
        #region objects and constructors
        private readonly IUnitOfWork _uow;
        private AppSetting appSetting;
        public UserLogic(IUnitOfWork uow, IOptions<AppSetting> options)
        {
            _uow = uow;
            appSetting = options.Value;
        }
        #endregion


        public ChallengeContent ViewChallengeContent(Guid ChallengeId)
        {
            var challenge = _uow.GetRepository<Challenge>().GetAll().FirstOrDefault(c => c.ChallengeId == ChallengeId);
            var posName = _uow.GetRepository<Position>().GetAll().SingleOrDefault(p => p.PositionId == challenge.PositionId).Name;
            if (challenge == null)
            {
                throw new ArgumentNullException();
            }
            if (posName == null)
            {
                throw new ArgumentNullException();
            }
            ChallengeContent challengeContent = new ChallengeContent
            {
                ChallengeName = challenge.Name,
                ChallengeDescription = challenge.Content,
                PositionName = posName
            };
            return challengeContent;
        }

        public List<ChallengeProfile> ViewChallengesList(UserProfile userProfile)
        {
            string PosName = userProfile.PositionName;
            Guid posID = _uow.GetRepository<Position>().GetAll().SingleOrDefault(p => p.Name == PosName).PositionId;
            if (posID == null)
            {
                throw new ArgumentNullException();
            }
            List<Challenge> ChallengesList = _uow
                .GetRepository<Challenge>()
                .GetAll()
                .Where(c => c.PositionId == posID).ToList();
            List<ChallengeProfile> ChallengeProfilesList = ChallengesList.Select(c => new ChallengeProfile
            {
                ChallengeId = c.ChallengeId,
                ChallengeName = c.Name
            }).ToList();

            return ChallengeProfilesList;
        }


        public async Task<bool> WritingAnObjectAsync(Stream fileStream, string fileName, string directory = null)
        {
            IAmazonS3 client = new AmazonS3Client(appSetting.AWSAccessKey, appSetting.AWSSecretKey, RegionEndpoint.APSoutheast1);
            try
            {
                var fileTransferUtility = new TransferUtility(client);
                var bucketPath = !string.IsNullOrWhiteSpace(directory)
                   ? appSetting.BucketName + @"/" + directory
                   : appSetting.BucketName;

                var fileUploadRequest = new TransferUtilityUploadRequest()
                {
                    BucketName = appSetting.BucketName,
                    Key = fileName,
                    InputStream = fileStream
                };
                await fileTransferUtility.UploadAsync(fileUploadRequest);
                return true;
            }
            catch (AmazonS3Exception)
            {
                return false;
            }
        }

    }
}
