using System;
using System.Collections.Generic;
using System.Text;
using Main.Logic.Phisics;

namespace Main.Logic.Registers {
    public class PhisicalRegister :ObjectRegister<IPhisicalObject>{
        new public static PhisicalRegister getInstance() {
            return new PhisicalRegister();
        }
        protected PhisicalRegister() { }
        public void update(int interval) {
            foreach (IPhisicalObject obj in this.container) {
                obj.prepareNextStatus(interval);
                obj.validate();
            }
        }
    }
}
