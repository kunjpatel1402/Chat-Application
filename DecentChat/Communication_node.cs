using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections;
using System.Security.Cryptography;
using System.Numerics;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;
using System.ComponentModel;
using Microsoft.Maui.Storage;
using System.Collections.ObjectModel;


namespace DecentChat
{
    public class Communication_node : INotifyPropertyChanged, IDisposable
    {
        private static Communication_node instance = null;
        private static readonly object padlock = new object();
        private SynchronizationContext uiContext;
        public static Communication_node Instance
        {
            get
            {
                lock (padlock)
                { 
                    if (instance == null)
                    {
                        instance = new Communication_node();
                    }
                    return instance;
                }
            }
        }

        public string Details
        {
            get { return this.ToString(); }
        }
        private int bits = 5;
        private string _node_name;
        public string node_name
        {
            get { return _node_name; }
            set
            {
                if (_node_name != value)
                {
                    _node_name = value;
                    OnPropertyChanged(nameof(node_name));
                    OnPropertyChanged(nameof(Details));
                }
            }
        }
        private int _hash_val;
        public int hash_val
        {
            get { return _hash_val; }
            set
            {
                if (_hash_val != value)
                {
                    _hash_val = value;
                    OnPropertyChanged(nameof(hash_val));
                    OnPropertyChanged(nameof(Details));
                }
            }
        }
        private string _ip_address;
        public string ip_address
        {
            get { return _ip_address; }
            set
            {
                if (_ip_address != value)
                {
                    _ip_address = value;
                    OnPropertyChanged(nameof(ip_address));
                    OnPropertyChanged(nameof(Details));
                }
            }
        }
        private int _port;
        public int port
        {
            get { return _port; }
            set
            {
                if (_port != value)
                {
                    _port = value;
                    OnPropertyChanged(nameof(port));
                    OnPropertyChanged(nameof(Details));
                }
            }
        }
        private Node? _next_node;
        public Node? next_node
        {
            get { return _next_node; }
            set
            {
                if (_next_node != value)
                {
                    _next_node = value;
                    this.fingers[0] = _next_node;
                    this.next_nodes[1] = _next_node;
                    OnPropertyChanged(nameof(next_node));
                    OnPropertyChanged(nameof(Details));
                }
            }
        }
        private Node? _prev_node;
        public Node? prev_node
        {
            get { return _prev_node; }
            set
            {
                if (_prev_node != value)
                {
                    _prev_node = value;
                    OnPropertyChanged(nameof(prev_node));
                    OnPropertyChanged(nameof(Details));
                }
            }
        }
        private Node _data;
        public Node data
        {
            get { return _data; }
            set
            {
                if (_data != value)
                {
                    _data = value;
                    OnPropertyChanged(nameof(data));
                    OnPropertyChanged(nameof(Details));
                }
            }
        }
        private ObservableCollection<Node?> _fingers;
        public ObservableCollection<Node?> fingers
        {
            get { return _fingers; }
            set
            {
                if (_fingers != value)
                {
                    _fingers = value;
                    OnPropertyChanged(nameof(fingers));
                    OnPropertyChanged(nameof(Details));
                }
            }
        }
        private ObservableCollection<Node?> _next_nodes;
        public ObservableCollection<Node?> next_nodes
        {
            get { return _next_nodes; }
            set
            {
                if (_next_nodes != value)
                {

                    _next_nodes = value;
                    OnPropertyChanged(nameof(next_nodes));
                    OnPropertyChanged(nameof(Details));
                }
            }
        }
        private ObservableCollection<Contact> _contacts;
        public ObservableCollection<Contact> contacts
        {
            get { return _contacts; }
            set
            {
                if (_contacts != value)
                {
                    _contacts = value;
                    OnPropertyChanged(nameof(contacts));
                }
            }
        }
        private Contact _selected_contact;
        public Contact selected_contact
        {
            get { return _selected_contact; }
            set 
            {
                if (_selected_contact != value)
                {
                    _selected_contact = value;
                    OnPropertyChanged(nameof(selected_contact));
                    if (_selected_contact != null)
                    {
                        this.IsContactSelected = true;
                        this.message_thread_logger.LogInformation("selected contact" + _selected_contact.ToString());
                        Update_messages();
                    }
                    else
                    {
                        this.IsContactSelected = false;
                    }
                }
            } 
        }
        private bool _isContactSelected;

        public bool IsContactSelected
        {
            get => _isContactSelected;
            set
            {
                _isContactSelected = value;
                OnPropertyChanged(nameof(IsContactSelected));
            }
        }
        private ObservableCollection<Message> _selected_contact_chat;
        public ObservableCollection<Message> selected_contact_chat 
        {
            get { return _selected_contact_chat; }
            set
            {
                if (_selected_contact_chat != value)
                {
                    _selected_contact_chat = value;
                    OnPropertyChanged(nameof(selected_contact_chat));
                }
            }
        }
        public void Update_messages()
        {
            IEnumerable<DataRow> rows_filtered = this.chat.AsEnumerable().Where(row =>
            (row.Field<int>("From") == this.hash_val && row.Field<int>("To") == this.selected_contact.hash_val) ||
            (row.Field<int>("From") == this.selected_contact.hash_val && row.Field<int>("To") == this.hash_val)
            );
            List<Message> messages = rows_filtered.Select(row => new Message
            {
                From = (int)row["From"],
                To = (int)row["To"],
                Text = (string)row["Text"],
                Date = DateTime.Parse(row["Date"].ToString()),
                Alignment = (int)row["From"] != this.hash_val ? LayoutOptions.Start : LayoutOptions.EndAndExpand,
                BubbleColor = (int)row["From"] != this.hash_val ? Color.FromHex("#ADD8E6") : Color.FromHex("#90EE90"),
            }).OrderBy(m => m.Date).ToList();
            foreach (var message in messages)
            {
                this.message_thread_logger.LogInformation($"Date: {message.Date} From: {message.From}, Text: {message.Text}, Alignment: {message.Alignment}");
            }
            this.selected_contact_chat = new ObservableCollection<Message>(messages);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    eventHandler(this, new PropertyChangedEventArgs(propertyName));
                });
            }
        }
        private DataTable chat;
        private DataTable others_chat;
        private DataTable contact_list;
        private Queue task_queue;
        private ILogger reciever_thread_logger;
        private ILogger sender_thread_logger;
        private ILogger message_thread_logger;
        private Communication comms;
        private bool status;
        private Thread? reciever_thread;
        private Thread? sender_thread;

        private ILogger setup_logger(string logger_name, string filename)
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var fullPath = Path.Combine(basePath, filename);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder => {
                builder.AddFile(fullPath);
            });
            return loggerFactory.CreateLogger(logger_name);
        }
        private void print_dict(Dictionary<string, object> payload, ILogger logger)
        {
            logger.LogInformation("Printing dictionary---------------------------");
            foreach (var item in payload)
            {
                if (item.Value != null)
                {
                    if (item.Value is Dictionary<string, object>)
                    {
                        logger.LogInformation(item.Key + " " + "Dictionary");
                        this.print_dict((Dictionary<string, object>)item.Value, logger);
                    }
                    else if (item.Value is (Dictionary<string, object>[]))
                    {
                        logger.LogInformation(item.Key + " " + "Array of dictionaries");
                        foreach (Dictionary<string, object> dict in (Dictionary<string, object>[])item.Value)
                        {
                            this.print_dict(dict, logger);
                        }
                    }
                    logger.LogInformation(item.Key + " " + item.Value.ToString());
                }
                else logger.LogInformation(item.Key + " null");
            }
            logger.LogInformation("End of dictionary----------------------------");
        }
        private bool check_e(int a, int b, int c)
        {
            if ((a < b) && ((a < c) && (c <= b))) return true;
            else if ((a > b) && !((a >= c) && (c > b))) return true;
            else return false;
        }
        private bool check_l(int a, int b, int c)
        {
            if ((a < b) && ((a <= c) && (c < b))) return true;
            else if ((a > b) && !((a > c) && (c >= b))) return true;
            else return false;
        }
        private int hash_name()
        {
            byte[] data = Encoding.UTF8.GetBytes(this.node_name);
            SHA1 sha1 = SHA1.Create();
            byte[] hashData = sha1.ComputeHash(data);
            BigInteger int_value = new BigInteger(hashData.Reverse().ToArray());
            int ret_value = (int)(BigInteger.Abs(int_value) % (BigInteger.Pow(2, bits)));
            return ret_value;
        }
        private Dictionary<string, Object> get_dict(string query, Object? data)
        {
            Dictionary<string, Object> payload = new Dictionary<string, object>();
            payload["ip_address"] = this.ip_address;
            payload["port"] = this.port;
            payload["node_name"] = this.node_name;
            payload["hash_val"] = this.hash_val;
            payload["query"] = query;
            if ((data is Dictionary<string, object>) && (query == "message"))
            {
                payload["To"] = ((Dictionary<string, object>)data)["To"];
                payload["data"] = ((Dictionary<string, object>)data)["message"];
                payload["hash_code"] = ((Dictionary<string, object>)data)["hash_code"];
            }
            else payload["data"] = data;
            // sender_thread_logger.LogInformation("Get dict called with query " + query);
            // print_dict(payload, this.sender_thread_logger);
            return payload;
        }
        public string Add_contact(string contact_name, int hash_val)
        {
            if (contact_name == null) return ("Invalid Name!!!");
            this.message_thread_logger.LogInformation(contact_name + " " + hash_val.ToString());
            var rowExists = this.contact_list.AsEnumerable().Any(row => row.Field<string>("Name") == contact_name);
            if (rowExists) return ("A contact already exists with the same name");
            rowExists = this.contact_list.AsEnumerable().Any(row => row.Field<int>("Hash_val") == hash_val);
            if (rowExists) return ("A contact already exists with the same hash_val");
            DataRow newRow = this.contact_list.NewRow();
            newRow["Name"] = contact_name;
            newRow["Hash_val"] = hash_val;
            this.contact_list.Rows.Add(newRow);
            Contact nc = new Contact(contact_name, hash_val);
            this.contacts.Add(nc);
            this.message_thread_logger.LogInformation("Added " + nc);
            return ("Contact Added!!");
        }
        public Communication_node(string node_name = "1", string ip_address = "127.0.0.1", int port = 5000, Node? next_node = null, Node? prev_node = null)
        {
            this.node_name = node_name;
            this.hash_val = this.hash_name();
            this.port = port;
            this.ip_address = ip_address;
            this.next_node = next_node;
            this.prev_node = prev_node;
            this.chat = new DataTable();
            this.uiContext = SynchronizationContext.Current;
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var fullPath = Path.Combine(basePath, this.node_name + ".xml");
            var fullPath_other = Path.Combine(basePath, this.node_name + "_others.xml");
            var fullPath_contacts = Path.Combine(basePath, this.node_name + "_contacts.xml");
            this._selected_contact = new Contact("null", -1);
            this.IsContactSelected = false;
            if (File.Exists(fullPath))
            {
                chat.ReadXml(fullPath);
            }
            else
            {
                this.chat.TableName = "Chat";
                this.chat.Columns.Add("Date", typeof(DateTime));
                this.chat.Columns.Add("sender_node_name", typeof(string));
                this.chat.Columns.Add("From", typeof(int));
                this.chat.Columns.Add("To", typeof(int));
                this.chat.Columns.Add("Text", typeof(string));
                this.chat.Columns.Add("hash_code", typeof(int));
            }
            this.others_chat = new DataTable();
            if (File.Exists(fullPath_other))
            {
                others_chat.ReadXml(fullPath_other);
            }
            else
            {
                this.others_chat.TableName = "OthersChat";
                this.others_chat.Columns.Add("Date", typeof(DateTime));
                this.others_chat.Columns.Add("sender_node_name", typeof(string));
                this.others_chat.Columns.Add("From", typeof(int));
                this.others_chat.Columns.Add("To", typeof(int));
                this.others_chat.Columns.Add("Text", typeof(string));
                this.others_chat.Columns.Add("hash_code", typeof(int));
            }
            this.contact_list = new DataTable();
            if (File.Exists(fullPath_contacts))
            {
                contact_list.ReadXml(fullPath_contacts);
            }
            else
            {
                this.contact_list.TableName = "ContactList";
                this.contact_list.Columns.Add("Name", typeof(string));
                this.contact_list.Columns.Add("Hash_val", typeof(int));
            }
            this.data = new Node(this.node_name, this.hash_val, this.ip_address, this.port);
            this.fingers = new ObservableCollection<Node?>(new Node[bits]);
            this.next_nodes = new ObservableCollection<Node?>(new Node[bits+1]);
            this.contacts = new ObservableCollection<Contact>();
            foreach (DataRow row in this.contact_list.Rows)
            {
                this.contacts.Add(new Contact((string)row["Name"], (int)row["Hash_val"]));
            }
            for (int i = 0; i < bits; i++)
            {
                if (this.fingers != null) this.fingers[i] = null;

                if (this.next_nodes != null) this.next_nodes[i] = null;
            }
            if (this.next_nodes != null) this.next_nodes[bits] = null;
            if (this.next_nodes != null) this.next_nodes[0] = this.data;
            this.task_queue = new Queue();
            this.reciever_thread_logger = setup_logger(node_name + "_RecieverThreadLogger", node_name + "_reciever_thread.log");
            this.sender_thread_logger = setup_logger(node_name + "_SenderThreadLogger", node_name + "_sender_thread.log");
            this.message_thread_logger = setup_logger(node_name + "_MessageThreadLogger", node_name + "_message_thread.log");
            this.comms = new Communication(this.ip_address, this.port, this.sender_thread_logger, this.reciever_thread_logger);
            this.sender_thread_logger.LogInformation("Node created");
            this.reciever_thread_logger.LogInformation("Node created");
            this.message_thread_logger.LogInformation("Node created");
            this.reciever_thread_logger.LogInformation("Starting node");
            this.comms.On();
            this.reciever_thread = new Thread(new ParameterizedThreadStart(this.comms.reciver_func));
            this.reciever_thread.Start(this.recieve_function);
        }
        ~Communication_node()
        {
            this.Dispose();
        }
        public void Dispose()
        {

            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var fullPath = Path.Combine(basePath, this.node_name + ".xml");
            var fullPath_other = Path.Combine(basePath, this.node_name + "_others.xml");
            var fullPath_contacts = Path.Combine(basePath, this.node_name + "_contacts.xml");
            if (this.status)
            {
                this.chat.WriteXml(fullPath, XmlWriteMode.WriteSchema);
                this.others_chat.WriteXml(fullPath_other, XmlWriteMode.WriteSchema);
                this.contact_list.WriteXml(fullPath_contacts, XmlWriteMode.WriteSchema);
                this.status = false;
                this.reciever_thread_logger.LogInformation("Closing node");
                this.sender_thread_logger.LogInformation("Closing node");
                this.send_payload(this.ip_address, this.port, this.get_dict("close_node", null));
                this.reciever_thread_logger.LogInformation("Node closed, joining reciever thread");
                this.sender_thread_logger.LogInformation("Node closed, joining sender thread");
                this.reciever_thread.Join();
                this.reciever_thread_logger.LogInformation("Reciever thread joined");
                this.sender_thread_logger.LogInformation("Reciever thread joined");
                this.reciever_thread_logger.LogInformation("Joining sender thread");
                this.sender_thread_logger.LogInformation("Joining sender thread");
                this.sender_thread.Join();
                this.reciever_thread_logger.LogInformation("Sender thread joined");
                this.sender_thread_logger.LogInformation("Sender thread joined");
            }
            else
            {
                this.chat.WriteXml(fullPath, XmlWriteMode.WriteSchema);
                this.others_chat.WriteXml(fullPath_other, XmlWriteMode.WriteSchema);
                this.contact_list.WriteXml(fullPath_contacts, XmlWriteMode.WriteSchema);
                this.status = false;
                this.reciever_thread_logger.LogInformation("Closing node");
                this.sender_thread_logger.LogInformation("Closing node");
                this.send_payload(this.ip_address, this.port, this.get_dict("close_node", null));
                this.reciever_thread_logger.LogInformation("Node closed, joining reciever thread");
                this.sender_thread_logger.LogInformation("Node closed, joining sender thread");
                this.reciever_thread.Join();
                this.reciever_thread_logger.LogInformation("Reciever thread joined");
                this.sender_thread_logger.LogInformation("Reciever thread joined");

            }
            instance = null;
        }
        public void stop()
        {
            this.Dispose();
        }
        public void start()
        {
            if (!this.status)
            {
                this.status = true;
                // start thread;
                this.sender_thread_logger.LogInformation("Starting node");
                this.sender_thread = new Thread(new ThreadStart(this.preiodic_updates));
                this.sender_thread.Start();
            }
        }
        public override string ToString()
        {
            StringBuilder details = new StringBuilder();
            details.Append("This node:" + this.data.ToString());
            if (this.next_node != null) details.Append("Next node:" + this.next_node.ToString());
            else details.Append("Next node:null\n");
            if (this.prev_node != null) details.Append("Prev node:" + this.prev_node.ToString());
            else details.Append("Prev node:null\n");
            details.Append("Fingers:\n");
            for (int i = 0; i < bits; i++)
            {
                if ((this.fingers != null) && (this.fingers[i] != null)) details.Append(this.fingers[i].ToString());
                else details.Append("null\n");
            }
            details.Append("Next nodes:\n");
            for (int i = 0; i < bits + 1; i++)
            {
                if ((this.next_nodes != null) && (this.next_nodes[i] != null)) details.Append(this.next_nodes[i].ToString());
                else details.Append("null\n");
            }
            return details.ToString();
        }
        public Dictionary<string, object> send_payload(string ip_address, int port, Dictionary<string, object> payload)
        {
            sender_thread_logger.LogInformation("Sending payload to " + ip_address + " " + port.ToString());
            print_dict(payload, this.sender_thread_logger);
            return this.comms.sender_func(ip_address, port, payload);
        }
        
        public Node closest_preceding_node(int hash_val)
        {
            reciever_thread_logger.LogInformation("Finding closest preceding node" + hash_val.ToString());
            for (int i = bits - 1; i >= 0; i--)
            {
                if (this.fingers[i] != null)
                {
                    reciever_thread_logger.LogInformation("Checking finger " + i);
                    reciever_thread_logger.LogInformation(fingers[i].ToString());
                    // sender_thread_logger.LogInformation(check_e(this.hash_val, hash_val, this.fingers[i].hash_val).ToString());
                    // sender_thread_logger.LogInformation(check_if_alive(this.fingers[i]).ToString());
                    if (this.check_e(this.hash_val, hash_val, this.fingers[i].hash_val) && this.check_if_alive(this.fingers[i]))
                    {
                        return this.fingers[i];
                    }
                }
            }
            reciever_thread_logger.LogInformation("Returning this node");
            return this.data;
        }
        public Dictionary<string, Object> find_successor(Node node)
        {
            if (node.hash_val == this.hash_val)
            {
                return this.get_dict("find_successor", this.data);
            }
            else if (check_e(this.hash_val, this.next_node.hash_val, node.hash_val))
            {
                return this.get_dict("find_successor", this.next_node);
            }
            else
            {
                var closest_node = this.closest_preceding_node(node.hash_val);
                return this.send_payload(closest_node.ip_address, closest_node.port, this.get_dict("find_successor", node));
            }
        }
        public void fix_fingers()
        {
            sender_thread_logger.LogInformation("checking predecessor");
            this.check_predecessor();
            sender_thread_logger.LogInformation("Fixing next nodes");
            this.fix_next_nodes();
            sender_thread_logger.LogInformation("Updating next nodes");
            this.update_next_nodes();
            for (int i = 0; i < bits; i++)
            {
                Node target_node = new Node("", (int)((this.hash_val + (int)Math.Pow(2, i)) % (Math.Pow(2, bits))), "", -1);
                var x = this.find_successor(target_node);
                if (x.ContainsKey("data") && (x["data"] is Node))
                {
                    this.fingers[i] = (Node)x["data"];
                }
                else
                {
                    this.fingers[i] = null;
                    break;
                }
            }
        }
        public void update_next_nodes()
        {
            for (int i = 2; i < bits + 1; i++)
            {
                try
                {
                    var x = this.send_payload(this.next_nodes[i - 1].ip_address, this.next_nodes[i - 1].port, this.get_dict("get_next_node", null));
                    if (x.ContainsKey("data") && (x["data"] is Node))
                    {
                        this.next_nodes[i] = (Node)x["data"];
                    }
                    else
                    {
                        this.next_nodes[i] = null;
                        this.next_nodes[i - 1] = null;
                        break;
                    }
                }
                catch (Exception e)
                {
                    this.next_nodes[i] = null;
                    this.next_nodes[i - 1] = null;
                    this.sender_thread_logger.LogError("Error in update_next_nodes " + i);
                    this.sender_thread_logger.LogError(e.Message);
                    break;
                }
            }
        }
        public void fix_next_nodes()
        {
            bool x = false;
            int j = 1;
            if (!this.check_if_alive(this.next_node))
            {
                while (j <= bits)
                {
                    x = this.check_if_alive(this.next_nodes[j]);
                    if (x)
                    {
                        this.next_nodes[1] = this.next_nodes[j];
                        break;
                    }
                    else
                    {
                        j++;
                    }
                }
                if (j > this.bits)
                {
                    this.message_thread_logger.LogCritical("No next node found");
                }
                else
                {
                    sender_thread_logger.LogInformation("New next node found");
                    sender_thread_logger.LogInformation(this.next_nodes[1].ToString());
                    this.next_node = this.next_nodes[1];
                    this.fingers[0] = this.next_nodes[1];
                }
            }
            else
            {
                sender_thread_logger.LogInformation("Next node is alive");
                sender_thread_logger.LogInformation(this.next_node.ToString());
                this.next_nodes[1] = this.next_node;
            }
        }
        public Dictionary<string, Object> notify(Node prev_node)
        {
            if ((this.prev_node == null) || (check_e(this.prev_node.hash_val, this.hash_val, prev_node.hash_val)))
            {
                this.prev_node = prev_node;
            }
            return this.get_dict("notify", this.prev_node);
        }
        public Dictionary<string, Object> stabilize()
        {
            try
            {
                Node x = (Node)this.send_payload(this.next_node.ip_address, this.next_node.port, this.get_dict("get_prev_node", null))["data"];
                if ((x != null) && (this.check_if_alive(x)) && (check_e(this.hash_val, this.next_node.hash_val, x.hash_val)))
                {
                    sender_thread_logger.LogInformation("Setting next node");
                    sender_thread_logger.LogInformation(this.next_node.ToString());
                    sender_thread_logger.LogInformation(x.ToString());
                    sender_thread_logger.LogInformation(x.ToString());
                    this.next_node = x;
                    this.next_nodes[1] = x;
                    this.fingers[0] = x;
                    
                }
            }
            catch (Exception e)
            {
                this.sender_thread_logger.LogError("Error in stabilize");
                this.sender_thread_logger.LogError(e.Message);
            }
            return this.send_payload(this.next_node.ip_address, this.next_node.port, this.get_dict("notify", this.data));
        }
        public bool join(Node node)
        {
            Dictionary<string, object> ret = this.send_payload(node.ip_address, node.port, this.get_dict("find_successor", this.data));
            if (ret.ContainsKey("data"))
            {
                this.next_node = (Node)ret["data"];
                this.next_nodes[1] = this.next_node;
                this.fingers[0] = this.next_node;
                
                return true;
            }
            else
            {
                return false;
            }
        }
        public void print_table(DataTable dt, ILogger logger)
        {
            foreach (DataRow row in dt.Rows)
            {
                foreach (DataColumn col in dt.Columns)
                {
                    logger.LogInformation($"{col.ColumnName}: {row[col]}");
                }
            }
        }

        public Dictionary<string, Object> recieve_function(Dictionary<string, Object> payload)
        {
            print_dict(payload, this.reciever_thread_logger);
            Dictionary<string, Object> ret;
            try
            {
                string query = (string)payload["query"];
                if (query == "find_successor")
                {
                    Node node = (Node)payload["data"];
                    ret = this.find_successor(node);
                }
                else if (query == "notify")
                {
                    Node node = (Node)payload["data"];
                    ret = this.notify(node);
                }
                else if (query == "get_prev_node")
                {
                    ret = this.get_dict("get_prev_node", this.prev_node);
                }
                else if (query == "get_next_node")
                {
                    ret = this.get_dict("get_next_node", this.next_node);
                }
                else if ((query == "close_node") && ((string)payload["ip_address"] == this.ip_address) && ((int)payload["port"] == this.port) && ((string)payload["node_name"] == this.node_name) && ((int)payload["hash_val"] == this.hash_val))
                {
                    ret = this.get_dict("close_node", "close_node");
                }
                else if (query == "message")
                {
                    if ((int)payload["To"] == this.hash_val)
                    {
                        this.reciever_thread_logger.LogInformation("Message is addresed to me");
                        this.print_table(this.contact_list, this.reciever_thread_logger);
                        bool rowExists = this.contact_list.AsEnumerable().Any(row => (row.Field<int>("Hash_val") == (int)payload["hash_val"]));
                        this.reciever_thread_logger.LogInformation("Checked for existing contact " + rowExists.ToString());    
                        if (!rowExists)
                        {
                        this.reciever_thread_logger.LogInformation("Adding new contact");
                        Contact temp = new Contact((string)payload["node_name"], (int)payload["hash_val"]);
                        this.reciever_thread_logger.LogInformation("Created new object " + temp.ToString());
                        this.uiContext.Post(_ =>
                        {
                            this.contacts.Add(temp);
                        }, null);
                        this.reciever_thread_logger.LogInformation("1");
                        DataRow dr_ = this.contact_list.NewRow();
                        this.reciever_thread_logger.LogInformation("2");
                        dr_["Name"] = (string)payload["node_name"];
                        this.reciever_thread_logger.LogInformation("3");
                        dr_["Hash_val"] = (int)payload["hash_val"];
                        this.reciever_thread_logger.LogInformation("4");
                        this.contact_list.Rows.Add(dr_);
                        this.reciever_thread_logger.LogInformation("Added new Contact");
                        }
                        else
                        {
                            this.reciever_thread_logger.LogInformation("Contact exists");
                        }
                        this.reciever_thread_logger.LogInformation("Adding new message entry");
                        DataRow dr = this.chat.NewRow();
                        dr["Date"] = new DateTime(1970, 1, 1).AddMilliseconds(long.Parse(((string)payload["date_time"]).Substring(6, 13)));
                        dr["sender_node_name"] = payload["node_name"];
                        dr["From"] = payload["hash_val"];
                        dr["To"] = payload["To"];
                        dr["Text"] = payload["data"];
                        dr["hash_code"] = payload["hash_code"];
                        this.chat.Rows.Add(dr);
                        this.reciever_thread_logger.LogInformation("Message added to table");
                        if ((this.selected_contact != null)&&((int)payload["hash_val"] == this.selected_contact.hash_val))   this.Update_messages();
                    }
                    else
                    {
                        DataRow dr = this.others_chat.NewRow();
                        dr["Date"] = new DateTime(1970, 1, 1).AddMilliseconds(long.Parse(((string)payload["date_time"]).Substring(6, 13)));
                        dr["sender_node_name"] = payload["node_name"];
                        dr["From"] = payload["hash_val"];
                        dr["To"] = payload["To"];
                        dr["Text"] = payload["data"];
                        dr["hash_code"] = payload["hash_code"];
                        this.others_chat.Rows.Add(dr);
                    }
                    this.message_thread_logger.LogInformation("Message recived from " + payload["ip_address"] + " " + payload["port"].ToString() + ":" + payload["data"]);
                    ret = this.get_dict("message", "Ack");
                }
                else if (query == "check_if_alive")
                {
                    ret = this.get_dict("check_if_alive", this.data);
                }
                else if (query == "get_pending_messages")
                {
                    IEnumerable<DataRow> rows_filtered = this.others_chat.AsEnumerable().Where(row =>
                    (row.Field<int>("To") == (int)payload["hash_val"])
                    );
                    Message[] messages = new Message[rows_filtered.Count()];
                    int i = 0;
                    List<DataRow> rowsToDelete = new List<DataRow>();
                    foreach (DataRow row in rows_filtered)
                    {
                        Message message = new Message();
                        message.Date = (DateTime)row["Date"];
                        message.sender_node_name = (string)row["sender_node_name"];
                        message.From = (int)row["From"];
                        message.To = (int)row["To"];
                        message.Text = (string)row["Text"];
                        message.hash_code = (int)row["hash_code"];
                        messages[i] = message;
                        rowsToDelete.Add(row);
                        i++;
                    }

                    //foreach (DataRow row in rowsToDelete)
                    //{
                    //    row.Delete();
                    //}
                    //others_chat.AcceptChanges();

                    ret = this.get_dict("get_pending_messages", messages);
                }
                else
                {
                    ret = this.get_dict("invalid_query", null);
                }
            }
            catch (Exception e)
            {
                this.reciever_thread_logger.LogError("Error in recieve_function");
                this.reciever_thread_logger.LogError(e.Message);
                ret = this.get_dict("invalid_query", null);
            }
            print_dict(ret, this.reciever_thread_logger);
            return ret;
        }
        private bool check_if_alive(Node node)
        {
            if (node == this.data) return true;
            else if (node == null) return false;
            try
            {
                Dictionary<string, Object> payload = this.get_dict("check_if_alive", null);
                var x = send_payload(node.ip_address, node.port, payload);

                if (x.ContainsKey("data") && (x["data"] is Node) && ((string)x["query"] == "check_if_alive"))
                {
                    // sender_thread_logger.LogInformation(node.ToString());
                    // sender_thread_logger.LogInformation(x["data"].ToString());
                    if ((Node)x["data"] == node) return true;
                    else return false;
                }
                else return false;
            }
            catch (Exception e)
            {
                this.sender_thread_logger.LogError("Error in check_if_alive " + node.ToString());
                this.sender_thread_logger.LogError(e.Message);
                return false;
            }
        }
        public void check_predecessor()
        {
            if ((this.prev_node != null) && (!this.check_if_alive(this.prev_node)))
            {
                this.prev_node = null;
            }
        }
        public bool send_message(int hash_val, string message)
        {

            this.message_thread_logger.LogInformation("Sending Message to  " + hash_val.ToString());
            this.message_thread_logger.LogInformation(message);
            int st = hash_val;
            int tar = (hash_val + 10) % (int)Math.Pow(2, bits);
            int cnt = 0;
            Dictionary<string, Object> message_data = new Dictionary<string, Object>();
            message_data["sender_node_name"] = this.node_name; 
            message_data["From"] = this.hash_val;
            message_data["To"] = hash_val;
            message_data["message"] = message;
            message_data["date"] = DateTime.UtcNow;
            int hash_code = message_data.GetHashCode();
            message_data["hash_code"] = hash_code;
            HashSet<int> visited = new HashSet<int>();
            while (check_l(st, tar, hash_val) && (!visited.Contains(hash_val)))
            {
                Node new_node = new Node("", hash_val, "", -1);
                Node x = (Node)this.find_successor(new_node)["data"];
                if (check_l(st, tar, x.hash_val) && (!visited.Contains(x.hash_val)))
                {
                    if (x.hash_val != this.hash_val)
                    {
                        message_thread_logger.LogInformation(x.ToString());
                        var ret = this.send_payload(x.ip_address, x.port, this.get_dict("message", message_data));
                        visited.Add(x.hash_val);
                        if ((ret is Dictionary<string, Object>) && ((string)ret["query"] == "message") && ((string)ret["data"] == "Ack")) cnt++;
                    }
                    else
                    {
                        DataRow dr = this.others_chat.NewRow();
                        dr["Date"] = message_data["date"];
                        dr["sender_node_name"] = message_data["sender_node_name"];
                        dr["From"] = message_data["From"];
                        dr["To"] = message_data["To"];
                        dr["Text"] = message_data["message"];
                        dr["hash_code"] = message_data["hash_code"];
                        others_chat.Rows.Add(dr);
                        cnt++;
                    }
                    hash_val = (x.hash_val + 1) % (int)Math.Pow(2, bits); ;
                }
                else
                {
                    break;
                }
            }
            if (cnt > 0)
            {
                DataRow dr = this.chat.NewRow();
                dr["Date"] = message_data["date"];
                dr["sender_node_name"] = message_data["sender_node_name"];
                dr["From"] = message_data["From"];
                dr["To"] = message_data["To"];
                dr["Text"] = message_data["message"];
                dr["hash_code"] = message_data["hash_code"];
                chat.Rows.Add(dr);
                return true;
            }
            else return false;
        }
        public void get_pending_messages()
        {
            int st = (this.hash_val + 1) % (int)Math.Pow(2, bits);
            int hash_val = st;
            int tar = (this.hash_val + 10) % (int)Math.Pow(2, bits);
            HashSet<int> visited = new HashSet<int>();
            while (check_l(st, tar, hash_val) && (!visited.Contains(hash_val)))
            {
                Node new_node = new Node("", hash_val, "", -1);
                Node x = (Node)this.find_successor(new_node)["data"];
                if (check_l(st, tar, x.hash_val) && (!visited.Contains(x.hash_val)))
                {
                    if (x.hash_val != this.hash_val)
                    {
                        message_thread_logger.LogInformation(x.ToString());
                        var ret = this.send_payload(x.ip_address, x.port, this.get_dict("get_pending_messages", null));
                        visited.Add(x.hash_val);
                        if ((ret is Dictionary<string, Object>) && ((string)ret["query"] == "get_pending_messages"))
                        {
                            object[] messages = (object[])ret["data"];
                            foreach (Message message in messages)
                            {
                                this.message_thread_logger.LogInformation(message.hash_code.ToString() + " " + message.Text);
                                DataRow dr = this.chat.NewRow();
                                dr["Date"] = message.Date;
                                dr["sender_node_name"] = message.sender_node_name;
                                dr["From"] = message.From;
                                dr["To"] = message.To;
                                dr["Text"] = message.Text;
                                dr["hash_code"] = message.hash_code;
                                if (!this.chat.AsEnumerable().Any(row => row.Field<int>("hash_code") == dr.Field<int>("hash_code") ))
                                {
                                    this.chat.Rows.Add(dr);
                                    bool rowExists = this.contact_list.AsEnumerable().Any(row => row.Field<int>("Hash_val") == message.From);
                                    if (!rowExists)
                                    {
                                        this.uiContext.Post(_ =>
                                        {
                                            this.contacts.Add(new Contact((string)message.sender_node_name, (int)message.From));
                                        }, null);
                                        DataRow dr_ = this.contact_list.NewRow();
                                        dr_["Name"] = (string)message.From.ToString();
                                        dr_["Hash_val"] = (int)message.From;
                                        this.contact_list.Rows.Add(dr_); ;
                                    }
                                }


                            }
                        }
                    }
                    hash_val = (x.hash_val + 1) % (int)Math.Pow(2, bits); ;
                }
                else
                {
                    break;
                }
            }
        }
        public void preiodic_updates()
        {
            while (this.status)
            {
                sender_thread_logger.LogInformation("Updating FIngers");
                this.fix_fingers();
                OnPropertyChanged(nameof(Details));
                sender_thread_logger.LogInformation("Updating Next Nodes");
                this.stabilize();
                OnPropertyChanged(nameof(Details));
                message_thread_logger.LogInformation("Getting pending Messages");
                this.get_pending_messages();
                sender_thread_logger.LogInformation("Checking tasks");
                if (this.task_queue.Count > 0)
                {
                    Dictionary<string, Object> task = (Dictionary<string, Object>)this.task_queue.Dequeue();
                    if ((string)task["type"] == "send_message")
                    {
                        int hash_val = (int)task["hash_val"];
                        string message = (string)task["message"];
                        if (this.send_message(hash_val, message))
                        {
                            this.message_thread_logger.LogInformation("Message sent");
                            
                            this.Update_messages();
                        }
                        else
                        {
                            this.message_thread_logger.LogInformation("Message not sent");
                        }
                    }
                    else if ((string)task["type"] == "get_pending_messages")
                    {
                        this.get_pending_messages();
                    }
                }
                sender_thread_logger.LogInformation("Sleeping");
                Thread.Sleep(5000);
            }
        }
        public void add_task(Dictionary<string, Object> task)
        {
            this.task_queue.Enqueue(task);
        }
        public void print_chat(int hash_val)
        {
            IEnumerable<DataRow> rows_filtered = this.chat.AsEnumerable().Where(row =>
            (row.Field<int>("From") == this.hash_val && row.Field<int>("To") == hash_val) ||
            (row.Field<int>("From") == hash_val && row.Field<int>("To") == this.hash_val)
            );
            DataTable tblFiltered;
            if (rows_filtered.Count() > 0)
            {
                tblFiltered = rows_filtered.CopyToDataTable();
            }
            else
            {
                tblFiltered = new DataTable();
                tblFiltered.Columns.Add("Date");
                tblFiltered.Columns.Add("From");
                tblFiltered.Columns.Add("To");
                tblFiltered.Columns.Add("Text");
            }
            var sortedRows = from row in tblFiltered.AsEnumerable()
                             orderby row.Field<DateTime>("Date")
                             select row;
            DataTable sortedTable;
            if (sortedRows.Count() > 0)
            {
                sortedTable = sortedRows.CopyToDataTable();
            }
            else
            {
                sortedTable = new DataTable();
                sortedTable.Columns.Add("Date");
                sortedTable.Columns.Add("From");
                sortedTable.Columns.Add("To");
                sortedTable.Columns.Add("Text");
            }
            Console.WriteLine("-----------------------------------------------------------------------------------");
            //print table contents
            foreach (DataRow row in sortedTable.Rows)
            {
                Console.WriteLine(row["Date"] + " [" + row["From"] + "] :" + row["Text"]);
            }
            Console.WriteLine("-----------------------------------------------------------------------------------");
        }
        public void print_table()
        {
            Console.WriteLine(this.chat.TableName);
            foreach (DataColumn col in this.chat.Columns)
            {
                Console.Write(col.ColumnName + " ");
            }
            Console.WriteLine();
            Console.WriteLine("-----------------------------------------------------------------------------------");
            foreach (DataRow row in this.chat.Rows)
            {
                Console.WriteLine(row["Date"] + " [" + row["From"] + "] :" + row["Text"]);
            }
            Console.WriteLine("-----------------------------------------------------------------------------------");
            Console.WriteLine(this.others_chat.TableName);
            foreach (DataColumn col in this.others_chat.Columns)
            {
                Console.Write(col.ColumnName + " ");
            }
            Console.WriteLine();
            Console.WriteLine("-----------------------------------------------------------------------------------");
            foreach (DataRow row in this.others_chat.Rows)
            {
                Console.WriteLine(row["Date"] + " [" + row["From"] + "] :" + row["Text"]);
            }
            Console.WriteLine("-----------------------------------------------------------------------------------");
        }
    }
}
