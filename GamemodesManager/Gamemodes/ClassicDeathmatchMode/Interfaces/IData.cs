using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamemodesManager.Gamemodes.ClassicDeathmatchMode.Interfaces
{
    public interface IData
    {
        bool IsFilled { get; set; }
        void Init();
        void Clear();
    }
}
