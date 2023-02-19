namespace ModpackUpdateManager.Managers
{
    public class UserMessaging
    {
        public static Form1 MainForm { get; private set; }

        public UserMessaging(Form1 _MainForm)
        {
            MainForm = _MainForm;
        }

        public void ShowMessage(string message, bool logOutput)
        {
            MainForm.SetOutputText(message);
            if (logOutput)
            {
                LogFile.LogMessage(message);
            }
        }
    }
}
