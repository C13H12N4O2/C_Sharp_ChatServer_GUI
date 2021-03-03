using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.IO;

namespace ChatServer
{
    public partial class Form1 : Form
    {
        private Dictionary<string, string> _dictionary = new Dictionary<string, string>();
        private UdpClient _server;
        private IPEndPoint _ipe;
        private DataTable _dt;

        public Form1()
        {
            InitializeComponent();
            _initDataTable();
            _initServer();
            _receiveData();
        }

        private void _initDataTable()
        {
            _dt = new DataTable();
            _dt.TableName = "DataTable";
            _dt.Columns.Add("UID", typeof(string));
            _dt.Columns.Add("Chat", typeof(string));
            _gridControlChatHistory.DataSource = _dt;
            _gridViewChatHistory.CustomDrawCell += _gridViewChatHistory_CustomDrawCell;
        }

        private void _initServer()
        {
            try
            {
                _ipe = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);
                _server = new UdpClient(_ipe);
                Debug.Print("UDP 서버 실행 중...\n");
            }
            catch (Exception e)
            {
                Debug.Print(e.ToString());
            }
        }

        private async Task _receiveData()
        {
            while (true)
            {
                DataRow insertData = _dt.NewRow();
                var receiveResult = await _server.ReceiveAsync();
                insertData["UID"] = Encoding.Default.GetString(receiveResult.Buffer);
                receiveResult = await _server.ReceiveAsync();
                insertData["Chat"] = Encoding.Default.GetString(receiveResult.Buffer);
                _dt.Rows.Add(insertData);
            }
        }

        private void _btnSave_Click(object sender, EventArgs e)
        {
            string filename = "grid_data.xml";

            System.IO.FileStream stream = new System.IO.FileStream(filename, System.IO.FileMode.Create);

            System.Xml.XmlTextWriter xmlWriter = new System.Xml.XmlTextWriter(stream, System.Text.Encoding.Unicode);
            _dt.WriteXml(xmlWriter);
            xmlWriter.Close();
        }

        private void _gridViewChatHistory_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            DataRow row = _gridViewChatHistory.GetDataRow(e.RowHandle);
        }
    }
}
