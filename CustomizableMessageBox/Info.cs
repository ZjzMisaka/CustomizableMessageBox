using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizableMessageBox
{
    public enum MessageBoxType
    {
        CustomizableMessageBox,
        SystemMessageBox
    }

    public static class Info
    {
        private static Stack<Exception> stackException = new Stack<Exception>();
        public static Stack<Exception> StackException { get => stackException; set => stackException = value; }

        private static bool isLastShowSucceed;
        public static bool IsLastShowSucceed { get => isLastShowSucceed; set => isLastShowSucceed = value; }

        public static bool PrintLog(MessageBoxType messageBoxType)
        {
            if(StackException.Count == 0)
            {
                return false;
            }

            string msg = "Message: \n" + StackException.Peek().Message + "\nStackTrace: \n" + StackException.Peek().StackTrace;
            switch (messageBoxType)
            {
                case MessageBoxType.CustomizableMessageBox:
                    MessageBox.Show(msg, "Log", System.Windows.MessageBoxButton.OK);
                    break;
                case MessageBoxType.SystemMessageBox:
                    System.Windows.MessageBox.Show(msg, "Log", System.Windows.MessageBoxButton.OK);
                    break;
            }
            return true;
        }

        public static bool PrintLog(string path, bool needHoldStack = false, bool needAppend = true)
        {
            Stack<Exception> temp = new Stack<Exception>(StackException);
            while (StackException.Count > 0)
            {
                Exception ex = StackException.Pop();
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(path, needAppend))
                {
                    file.WriteLine(DateTime.Now.ToLocalTime().ToString() + "\nMessage: \n   " + ex.Message + "\nStackTrace: \n" + ex.StackTrace + "\n");
                }
            }
            if (needHoldStack)
            {
                StackException = temp;
            }
            return true;
        }
    }
}
