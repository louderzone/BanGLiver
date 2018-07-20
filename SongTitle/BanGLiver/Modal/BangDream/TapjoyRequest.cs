using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanGLiver.Modal.BangDream
{
    public class TapjoyRequest
    {
        public List<TapjoyEvent> Events { get; set; } = new List<TapjoyEvent>();
    }

    public class TapjoyEvent
    {
        public string Name { get; set; }
        public string P1 { get; set; }
        public string P2 { get; set; }
    }
}
