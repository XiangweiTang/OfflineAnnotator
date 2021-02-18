using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MciPlayer
{
    public class MciPlayer
    {
        public string AudioPath { get; }
        private const string ALIAS = "audio";
        private bool OpenedFlag = false;
        public MciPlayer(string audioPath)
        {
            AudioPath = audioPath;
        }

        public void Open()
        {
            string openString = $"open \"{AudioPath}\" alias {ALIAS}";
            Mci.Run(openString);
            OpenedFlag = true;
        }

        public void PlayFrom(int miliseconds)
        {
            if (!OpenedFlag)
                Open();
            string playFromString = $"play {ALIAS} from {miliseconds}";
            Mci.Run(playFromString);
        }

        public void Pause()
        {
            if (OpenedFlag)
            {
                string pauseString = $"pause {ALIAS}";
                Mci.Run(pauseString);
            }
        }

        public string Position()
        {
            if (!OpenedFlag)
                return "0";
            string statusString = $"status {ALIAS} position notify";
            StringBuilder sb = new StringBuilder();
            Mci.Run(statusString, sb, 128);
            return sb.ToString();
        }
    }
}
