using System;
using SteamKit2.GC;
using SteamKit2.GC.CSGO.Internal;

namespace CSGO
{
    public partial class CsgoClient
    {
        /// <summary>
        /// Request MatchmakingStats from the game coordinator.
        /// </summary>
        /// <param name="callback">The callback to be executed when the operation finishes.</param>
        public void MatchmakingStatsRequest(Action<CMsgGCCStrike15_v2_MatchmakingGC2ClientHello> callback)
        {
            _gcMap.Add((uint) ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchmakingGC2ClientHello,
                msg => callback(new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchmakingGC2ClientHello>(msg).Body));

            if(_debug)
                Console.WriteLine("Requesting Matchmaking stats");

            var clientGcMsgProtobuf =
                new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchmakingClient2GCHello>(
                    (uint) ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchmakingClient2GCHello);

            _gameCoordinator.Send(clientGcMsgProtobuf, CsgoAppid);
        }
        public void RequestCurrentLiveGames(Action<CMsgGCCStrike15_v2_MatchList> callback)
        {
            _gcMap.Add((uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchList, msg => callback(new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchList>(msg).Body));

            var clientGcMsgProtobuf = new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchListRequestCurrentLiveGames>(
                (uint) ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchListRequestCurrentLiveGames);

            _gameCoordinator.Send(clientGcMsgProtobuf, CsgoAppid);
        }

        public void RequestLiveGameForUser(uint accountId, Action<CMsgGCCStrike15_v2_MatchList> callback)
        {
            _gcMap.Add((uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchList, msg => callback(new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchList>(msg).Body));

            var clientGcMsgProtobuf = new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchListRequestLiveGameForUser>(
                (uint) ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchListRequestLiveGameForUser)
            {
                Body =
                {
                    accountid = accountId
                }
            };

            _gameCoordinator.Send(clientGcMsgProtobuf, CsgoAppid);
        }

        //TODO: Find out what this does and what is required
        //TODO: Add correct packettype to Action
        //TODO: Resolve parameter types
        /*public void RequestWatchInfoFriends(dynamic request, Action callback)
        {
            new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_ClientRequestWatchInfoFriends>((uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_ClientRequestWatchInfoFriends2)
            {
                
            }
        }*/

        //TODO: Add correct packettype to Action
        //TODO: Resolve parameter types
        [Obsolete("This hasn't been tested yet, as i don't know what the parameters do :(")]
        public void RequestGame(ulong matchid, ulong outcome, uint token, Action callback)
        {
            var clientGcMsgProtobuf = new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchListRequestFullGameInfo>(
                (uint) ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchListRequestFullGameInfo)
            {
                Body =
                {
                    matchid = matchid,
                    outcomeid = outcome,
                    token = token
                }
            };

            _gameCoordinator.Send(clientGcMsgProtobuf, CsgoAppid);
        }

        //TODO: Add correct packettype to Action
        //TODO: Resolve parameter types
        public void RequestRecentGames(uint accountId, Action<CMsgGCCStrike15_v2_MatchList> callback)
        {
            _gcMap.Add((uint)ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchList, msg => callback(new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchList>(msg).Body));

            var clientGcMsgProtobuf = new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchListRequestRecentUserGames>(
                (uint) ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchListRequestRecentUserGames)
            {
                Body =
                {
                    accountid = accountId
                }
            };

            _gameCoordinator.Send(clientGcMsgProtobuf, CsgoAppid);
        }
    }
}