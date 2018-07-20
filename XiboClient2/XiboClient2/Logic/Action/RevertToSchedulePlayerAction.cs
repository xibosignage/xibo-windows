using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiboClient2.Logic.Action
{
    class RevertToSchedulePlayerAction : PlayerActionInterface
    {
        public const string Name = "revertToSchedule";

        public string GetActionName()
        {
            return Name;
        }
    }
}
