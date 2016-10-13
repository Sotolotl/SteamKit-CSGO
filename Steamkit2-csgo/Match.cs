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
    }
}