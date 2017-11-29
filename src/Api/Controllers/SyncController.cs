﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bit.Core.Models.Api;
using Bit.Core.Services;
using Bit.Core.Repositories;
using Bit.Core;
using Bit.Core.Enums;
using Bit.Core.Exceptions;
using System.Linq;
using Bit.Core.Models.Table;
using System.Collections.Generic;

namespace Bit.Api.Controllers
{
    [Route("sync")]
    [Authorize("Application")]
    public class SyncController : Controller
    {
        private readonly IUserService _userService;
        private readonly IFolderRepository _folderRepository;
        private readonly ICipherRepository _cipherRepository;
        private readonly ICollectionRepository _collectionRepository;
        private readonly ICollectionCipherRepository _collectionCipherRepository;
        private readonly IOrganizationUserRepository _organizationUserRepository;
        private readonly GlobalSettings _globalSettings;

        public SyncController(
            IUserService userService,
            IFolderRepository folderRepository,
            ICipherRepository cipherRepository,
            ICollectionRepository collectionRepository,
            ICollectionCipherRepository collectionCipherRepository,
            IOrganizationUserRepository organizationUserRepository,
            GlobalSettings globalSettings)
        {
            _userService = userService;
            _folderRepository = folderRepository;
            _cipherRepository = cipherRepository;
            _collectionRepository = collectionRepository;
            _collectionCipherRepository = collectionCipherRepository;
            _organizationUserRepository = organizationUserRepository;
            _globalSettings = globalSettings;
        }

        [HttpGet("")]
        public async Task<SyncResponseModel> Get()
        {
            var user = await _userService.GetUserByPrincipalAsync(User);
            if(user == null)
            {
                throw new BadRequestException("User not found.");
            }

            var organizationUserDetails = await _organizationUserRepository.GetManyDetailsByUserAsync(user.Id,
                OrganizationUserStatusType.Confirmed);
            var folders = await _folderRepository.GetManyByUserIdAsync(user.Id);
            var ciphers = await _cipherRepository.GetManyByUserIdAsync(user.Id);

            IEnumerable<Collection> collections = null;
            IDictionary<Guid, IGrouping<Guid, CollectionCipher>> collectionCiphersGroupDict = null;
            if(organizationUserDetails.Any(o => o.Enabled))
            {
                collections = await _collectionRepository.GetManyByUserIdAsync(user.Id, false);
                var collectionCiphers = await _collectionCipherRepository.GetManyByUserIdAsync(user.Id);
                collectionCiphersGroupDict = collectionCiphers.GroupBy(c => c.CipherId).ToDictionary(s => s.Key);
            }

            var response = new SyncResponseModel(_globalSettings, user, organizationUserDetails, folders,
                collections, ciphers, collectionCiphersGroupDict);
            return response;
        }
    }
}
