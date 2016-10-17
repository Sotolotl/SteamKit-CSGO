using System;
using SteamKit2.GC;
using SteamKit2.GC.CSGO.Internal;

namespace SteamKit.CSGO
{
    public partial class CsgoClient
    {
        /// <summary>
        ///     Request MatchmakingStats from the game coordinator.
        /// </summary>
        /// <param name="callback">The callback to be executed when the operation finishes.</param>
        public void MatchmakingStatsRequest(Action<CMsgGCCStrike15_v2_MatchmakingGC2ClientHello> callback)
        {
            _gcMap.Add((uint) ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchmakingGC2ClientHello,
                msg => callback(new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchmakingGC2ClientHello>(msg).Body));

            if (_debug)
                Console.WriteLine("Requesting Matchmaking stats");

            var clientGcMsgProtobuf =
                new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchmakingClient2GCHello>(
                    (uint) ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchmakingClient2GCHello);

            _gameCoordinator.Send(clientGcMsgProtobuf, CsgoAppid);
        }

        /// <summary>
        ///     Request the list of currently live games
        /// </summary>
        /// <param name="callback">The callback to be executed when the operation finishes.</param>
        public void RequestCurrentLiveGames(Action<CMsgGCCStrike15_v2_MatchList> callback)
        {
            _gcMap.Add((uint) ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchList,
                msg => callback(new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchList>(msg).Body));

            var clientGcMsgProtobuf = new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchListRequestCurrentLiveGames>(
                (uint) ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchListRequestCurrentLiveGames);

            _gameCoordinator.Send(clientGcMsgProtobuf, CsgoAppid);
        }

        /// <summary>
        ///     Requests current live game info for given user.
        /// </summary>
        /// <param name="accountId">Account to request</param>
        /// <param name="callback">The callback to be executed when the operation finishes.</param>
        public void RequestLiveGameForUser(uint accountId, Action<CMsgGCCStrike15_v2_MatchList> callback)
        {
            _gcMap.Add((uint) ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchList,
                msg => callback(new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchList>(msg).Body));

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
        /// <summary>
        ///     Requests info about game given a matchId, outcomeId, and token for a game.
        /// </summary>
        /// <param name="matchid">The ID of the match</param>
        /// <param name="outcome">the ID of the outcome of the match</param>
        /// <param name="token">No idea</param>
        /// <param name="callback">The callback to be executed when the operation finishes.</param>
        [Obsolete("This hasn't been tested yet, as i don't know what the parameters do :(")]
        public void RequestGame(ulong matchid, ulong outcome, uint token, Action<CMsgGCCStrike15_v2_MatchList> callback)
        {
            _gcMap.Add((uint) ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchList,
                msg => callback(new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchList>(msg).Body));

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

        /// <summary>
        ///     Requests a list of recent games for the given account id
        /// </summary>
        /// <param name="accountId">Account IDd for the request</param>
        /// <param name="callback">The callback to be executed when the operation finishes.</param>
        [Obsolete("The accountId parameter for requestRecentGames has been deprecated.")]
        public void RequestRecentGames(uint accountId, Action<CMsgGCCStrike15_v2_MatchList> callback)
        {
            _gcMap.Add((uint) ECsgoGCMsg.k_EMsgGCCStrike15_v2_MatchList,
                msg => callback(new ClientGCMsgProtobuf<CMsgGCCStrike15_v2_MatchList>(msg).Body));

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

        /// <summary>
        ///     Requests a list of recent games for the given account id
        /// </summary>
        /// <param name="callback">The callback to be executed when the operation finishes.</param>
        public void RequestRecentGames(Action<CMsgGCCStrike15_v2_MatchList> callback)
        {
#pragma warning disable 618 //b-b-but we're doing it the right way!
            RequestRecentGames(_steamUser.SteamID.AccountID, callback);
#pragma warning restore 618
        }
    }
}