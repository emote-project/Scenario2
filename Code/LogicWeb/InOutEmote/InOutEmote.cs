using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InOutEmote.behaviours;
using InOutEmote.inputs;
using InOutEmote.Thalamus;
using LogicWebLib;

namespace InOutEmote
{
    public class InOutEmote : LogicFrame
    {
        private InOutThalamusClient _client;

        public InOutEmote()
        {
            _client = InOutThalamusClient.GetInstance();
            GameState.GetInstance().Initialize();


            AddInput(new EcoTurnIN());
            AddInput(new EnvTurnIN());
            AddInput(new MayorTurnIN());
            AddInput(new NewLevelReachedIN());
            AddInput(new GameEndedNoOilIN());
            AddInput(new GameEndedTimeUpIN());
            AddInput(new GameEndedWinIN());
            AddInput(new IsFirstSessionIN());
            AddInput(new IsFirstTurnIN());
            AddInput(new IsGameStartedIN());
            AddInput(new FirstPolicyDoneByRobotIN());
            AddInput(new FirstSkipDoneByRobotIN());
            AddInput(new FirstUpgradeDoneByRobotIN());
            AddInput(new StructureBuiltIN());
            AddInput(new ActionPlannedByAiForMayorIN());
            AddInput(new StructureUpgradedIN());
            AddInput(new PolicyImplementedIN());
            AddInput(new LearnerModelMemoryEventIN());
            AddInput(new UsersIdlingIN());
        }

        public override void Dispose()
        {
            base.Dispose();
            _client.Dispose();
        }
    }
}
