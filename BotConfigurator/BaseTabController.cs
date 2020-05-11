using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotConfigurator
{
    /// <summary>
    /// Basic controller for tab controller.
    /// Such controllers provide easy ui integration with window and edited entities
    /// Also helps keep edited things in apropriated values
    /// </summary>
    abstract class BaseTabController
    {
        /// <summary>
        /// Occurs when edition entity changed
        /// </summary>
        public abstract void OnEntityChange();

        /// <summary>
        /// Hide/show edition fields for selected entity
        /// </summary>
        public abstract void ShowUi(bool show);

        /// <summary>
        /// Occurs when tab open (enter)
        /// </summary>
        public abstract void OnTabOpen();

        /// <summary>
        /// Occurs when tab close (leave)
        /// </summary>
        public abstract void OnTabClose();
    }
}
