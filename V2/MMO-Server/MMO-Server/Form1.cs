using System;
using System.Windows.Forms;
using MMO_Server.Networking;

namespace MMO_Server
{
    public partial class UnityGameServer : Form
    {
        public static UnityGameServer Instance;

        private NetworkTraffic m_NetworkTraffic;
        private BaseNetwork m_BaseNetwork;

        private bool m_IsRunning;

        public UnityGameServer()
        {
            InitializeComponent();

            Instance = this;
            m_BaseNetwork = new BaseNetwork();
        }

        private void UnityGameServer_Load(object sender, EventArgs e)
        {

        }

        private void StartServer_Click(object sender, EventArgs e)
        {
            if (!m_IsRunning)
            {
                m_BaseNetwork.InitializeServer();
                m_NetworkTraffic = new NetworkTraffic(m_BaseNetwork);
                m_IsRunning = true;

            }
        }
        public void DebugLog(string msg)
        {
            string prefix = DateTime.Now + ": ";
            string appendString = prefix + msg + "\n";

            if (TB_Consolelog.InvokeRequired)
                Invoke(new Action<string>(s => TB_Consolelog.AppendText(s)), appendString);
            else TB_Consolelog.AppendText(appendString);
        }
    }
}