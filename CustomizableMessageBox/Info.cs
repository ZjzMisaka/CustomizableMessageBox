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

            switch(messageBoxType)
            {
                case MessageBoxType.CustomizableMessageBox:
                    MessageBox.Show("Message: \n" + StackException.Peek().Message + "\nStackTrace: \n" + StackException.Peek().StackTrace, "Log", System.Windows.MessageBoxButton.OK);
                    break;
                case MessageBoxType.SystemMessageBox:
                    System.Windows.MessageBox.Show("Message: \n" + StackException.Peek().Message + "\nStackTrace: \n" + StackException.Peek().StackTrace, "Log", System.Windows.MessageBoxButton.OK);
                    break;
            }
            return true;
        }

        public static bool PrintLog(Uri uri, bool needHoldStack = false, bool needAppend = true)
        {
            Stack<Exception> temp = StackException;
            while (StackException.Count > 0)
            {
                Exception ex = StackException.Pop();
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(uri.AbsoluteUri, needAppend))
                {
                    file.WriteLine("Message: \n" + ex.Message + "\nStackTrace: \n\n" + ex.StackTrace);
                }
            }
            if(needHoldStack)
            {
                StackException = temp;
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
                    file.WriteLine("Message: \n" + ex.Message + "\nStackTrace: \n" + ex.StackTrace + "\n");
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
