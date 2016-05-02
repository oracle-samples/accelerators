/* * *******************************************************************************************
*  $ACCELERATOR_HEADER_PLACE_HOLDER$
*  SHA1: $Id: dceabb75f269c84b912f07312652e505999cbf27 $
* *********************************************************************************************
*  File: $ACCELERATOR_HEADER_FILE_NAME_PLACE_HOLDER$
* ****************************************************************************************** */

using System.AddIn;
using RightNow.AddIns.AddInViews;

namespace Accelerator.OutOfOffice.Client
{
    [AddIn("OutOfOfficeClient AddIn", Version = "1.0.0.0")]    
    public class OutOfOfficeClientAddIn : IAutomationClient
    {
        public static IGlobalContext GlobalContext { get; private set; }
        public static IAutomationContext AutoContext { get; private set; }

        #region IAutomationClient Members

        public void SetAutomationContext(IAutomationContext context)
        {
            AutoContext = context;
        }

        #endregion

        #region IAddInBase Members

        public bool Initialize(IGlobalContext context)
        {
            GlobalContext = context;
            return true;
        }

        #endregion
    }
}
