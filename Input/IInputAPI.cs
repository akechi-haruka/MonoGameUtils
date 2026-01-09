using Microsoft.Xna.Framework;
using OAS.Error;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAS.Input {
    
    public interface IInputAPI {

        public void EarlyUpdate(GameTime time);

        public void LateUpdate(GameTime time);
        
        public void Initialize(bool isErrorDisp);

        public Exception GetError();
        public DateTime? GetErrorTime();
        public void ResetError();
    }
}
