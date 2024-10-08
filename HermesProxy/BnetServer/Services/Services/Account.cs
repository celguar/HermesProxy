﻿// Copyright (c) CypherCore <http://github.com/CypherCore> All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE. See LICENSE file in the project root for full license information.

using Bgs.Protocol.Account.V1;
using Framework.Constants;
using System.Collections.Generic;

namespace BNetServer.Services
{
    public partial class BnetServices
    {
        [Service(ServiceRequirement.LoggedIn, OriginalHash.AccountService, 30)]
        BattlenetRpcErrorCode HandleGetAccountState(GetAccountStateRequest request, GetAccountStateResponse response)
        {
            if (request.Options.FieldPrivacyInfo)
            {
                response.State = new AccountState();
                response.State.PrivacyInfo = new PrivacyInfo();
                response.State.PrivacyInfo.IsUsingRid = false;
                response.State.PrivacyInfo.IsVisibleForViewFriends = false;
                response.State.PrivacyInfo.IsHiddenFromFriendFinder = true;

                response.Tags = new AccountFieldTags();
                response.Tags.PrivacyInfoTag = 0xD7CA834D;
            }

            return BattlenetRpcErrorCode.Ok;
        }

        [Service(ServiceRequirement.LoggedIn, OriginalHash.AccountService, 31)]
        BattlenetRpcErrorCode HandleGetGameAccountState(GetGameAccountStateRequest request, GetGameAccountStateResponse response)
        {
            if (request.Options.FieldGameLevelInfo)
            {
                var gameAccountInfo = GetSession().AccountInfo.GameAccounts.LookupByKey(request.GameAccountId.Low);
                if (gameAccountInfo != null)
                {
                    response.State = new GameAccountState();
                    response.State.GameLevelInfo = new GameLevelInfo();
                    response.State.GameLevelInfo.Name = gameAccountInfo.DisplayName;
                    response.State.GameLevelInfo.Program = 0x576f57; // "WoW" in Hex
                }

                response.Tags = new GameAccountFieldTags();
                response.Tags.GameLevelInfoTag = 0x5C46D483;
            }

            if (request.Options.FieldGameStatus)
            {
                if (response.State == null)
                    response.State = new GameAccountState();

                response.State.GameStatus = new GameStatus();

                var gameAccountInfo = GetSession().AccountInfo.GameAccounts.LookupByKey(request.GameAccountId.Low);
                if (gameAccountInfo != null)
                {
                    response.State.GameStatus.IsSuspended = gameAccountInfo.IsBanned;
                    response.State.GameStatus.IsBanned = gameAccountInfo.IsPermanenetlyBanned;
                    response.State.GameStatus.SuspensionExpires = (gameAccountInfo.UnbanDate * 1000000);
                }

                response.State.GameStatus.Program = 0x576f57; // "WoW" in Hex
                response.Tags.GameStatusTag = 0x98B75F99;
            }

            return BattlenetRpcErrorCode.Ok;
        }
    }
}
