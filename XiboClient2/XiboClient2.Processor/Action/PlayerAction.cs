using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XiboClient2.Processor.Action
{
    class PlayerAction : PlayerActionInterface
    {
        public string action;

        public DateTime createdDt;
        public int ttl;

        public String GetActionName()
        {
            return action;
        }
    }
}
