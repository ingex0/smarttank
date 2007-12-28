using System;
using System.Collections.Generic;
using System.Text;

namespace SmartTank.Update
{
    public class UpdateManager
    {
        List<IEnumerable<IUpdater>> updateGroups = new List<IEnumerable<IUpdater>>();

        public void AddGroup(IEnumerable<IUpdater> group)
        {
            updateGroups.Add( group );
        }

        public void ClearGroups ()
        {
            updateGroups.Clear();
        }

        public void Update ( float seconds )
        {
            foreach (IEnumerable<IUpdater> group in updateGroups)
            {
                foreach (IUpdater updater in group)
                {
                    updater.Update( seconds );
                }
            }
        }
    }
}
