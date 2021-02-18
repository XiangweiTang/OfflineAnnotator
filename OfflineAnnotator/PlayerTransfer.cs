using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MciPlayer;

namespace OfflineAnnotator
{
    class PlayerTransfer
    {
        private MciPlayer.MciPlayer MP;
        public PlayerTransfer(string audioPath)
        {
            MP = new MciPlayer.MciPlayer(audioPath);
        }
    }
}
