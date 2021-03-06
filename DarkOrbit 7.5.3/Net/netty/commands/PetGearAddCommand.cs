﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ow.Utils;

namespace Ow.Net.netty.commands
{
    class PetGearAddCommand
    {
        public const short ID = 29895;

        public static byte[] write(PetGearTypeModule gearType, int level, int amount, bool enabled)
        {
            var param1 = new ByteArray(ID);
            param1.write(gearType.write());
            param1.writeInt(level);
            param1.writeInt(amount);
            param1.writeBoolean(enabled);
            return param1.ToByteArray();
        }
    }
}
