using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAS.UI.Elements {
    public interface ITouchable {

        public void OnTouch(int x, int y);
        public bool ShouldDoOriginCheck();
    }
}
