using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Source.Scripts.InputSystem
{
    public interface IPlayerInput
    {
        public event Action<int> NotePressed;
    }
}
