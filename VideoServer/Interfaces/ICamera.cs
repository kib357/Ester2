using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace VideoServer.Interfaces
{
    public interface ICamera
    {
        void Connect();
        void Disconnect();
        void Play();
        void Play(string date, string time, int speed);
        void Pause();
        void PrevFrame();
        void NextFrame();
        void Stop();
        void GetFrame();
        Dictionary<string, List<string>> GetHistory(bool all);
        List<string> GetHistory(string date);
        Dictionary<string, List<string>> GetEvents();
    }
}
