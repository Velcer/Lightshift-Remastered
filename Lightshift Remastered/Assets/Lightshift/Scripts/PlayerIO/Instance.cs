using PlayerIOClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public abstract class Instance
{
    public Connection con;

    public Client client;

    public void Send(string type, params object[] args)
    {
        if (con != null && con.Connected)
            con.Send(type, args);
    }

    public abstract void Start();
}
