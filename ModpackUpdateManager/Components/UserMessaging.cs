namespace ModpackUpdateManager.Components
{
    public class UserMessaging
    {
        public static Form1 MainForm { get; private set; }

        public UserMessaging(Form1 _MainForm)
        {
            MainForm = _MainForm;
        }

        public void ShowMessage(string message)
        {
            MainForm.SetOutputText(message);
        }

        public void ShowMessageBox(string message, string title)
        {
            MainForm.ShowMessageBox(message, title);
        }
    }
}
