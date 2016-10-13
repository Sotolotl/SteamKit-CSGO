using System;
using SteamKit2.GC;
using SteamKit2.GC.CSGO.Internal;

namespace CSGO
{
    public partial class CsgoClient
    {
        public void PlayerProfileRequest(uint accountId, Action<CMsgGCCStrike15_v2_PlayersProfile> callback)
        {
            // For gods sake don't ask what the 32 is, i just copied it
            PlayerProfileRequest(accountId, 32, callback);
        }

        public void PlayerProfileRequest(uint accountId, uint reqLevel,
            Action<CMsgGCCStrike15_v2_PlayersProfile> callback)
        {
            _gcMap.Add((uint) ECsgoGCMsg.k_EMsgGCCStrike15_v2_PlayersProfile,
                msg => callback(new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_PlayersProfile>(msg).Body));

            var clientMsgProtobuf =
                new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_ClientRequestPlayersProfile>(
                    (uint) ECsgoGCMsg.k_EMsgGCCStrike15_v2_ClientRequestPlayersProfile)
                {
                    Body =
                    {
                        account_id = accountId,
                        request_level = reqLevel
                    }
                };

            _gameCoordinator.Send(clientMsgProtobuf, CsgoAppid);
        }
    }
}