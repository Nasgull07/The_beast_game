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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Registro
{
    public partial class Form1 : Form
    {
        public IPAddress Direccion_Ip = IPAddress.Parse("192.168.1.105");
        public int puerto = 8024;
        Socket server;
        public Form1()
        {
            InitializeComponent();
        }

        private void Log_In_Button_Click(object sender, EventArgs e)
        {
            //Creamos un IPEndPoint con el ip del servidor y el puerto del servidor al q nos vamos a conectar.
           // IPAddress Direccion_Ip = IPAddress.Parse("192.168.1.105");
            IPEndPoint ipep = new IPEndPoint(this.Direccion_Ip, puerto);

            //Creamos el socket para podernos conectar.
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {

                if (User_Name.Text != "" && Password.Text != "")
                
                {   server.Connect(ipep); //Intentamos conectarnos al socket
                    //this.BackColor = Color.Green;
                    MessageBox.Show("Conectado!!");
                    string NOMBRE = User_Name.Text; //Variable para almacenar el nombre q vamos a enviar
                    string CONTRASEÑA = Password.Text;

                    //Enviamos al servidor la informacion
                    byte[] Mensaje_Cliente = System.Text.Encoding.ASCII.GetBytes("l/" + NOMBRE + "/" + CONTRASEÑA);
                    server.Send(Mensaje_Cliente);

                    //Recibimos la respuesta del servidor
                    byte[] Respuesta = new byte[100];
                    string Mensaje;
                    server.Receive(Respuesta);
                    Mensaje = Encoding.ASCII.GetString(Respuesta);
                    MessageBox.Show(Mensaje);
                }
                else
                {
                    MessageBox.Show("Porfavor, rellene los datos para ingresar");
                }

                this.BackColor = Color.Gray;
                server.Shutdown(SocketShutdown.Both);
                server.Close();

            }
            catch (SocketException)
            {
                MessageBox.Show("No se ha podido establecer la conexion");
                return;
            }
        }

        private void Sign_Up_Button_Click(object sender, EventArgs e)
        {
            //Creamos un IPEndPoint con el ip del servidor y el puerto del servidor al q nos vamos a conectar.
            
            IPEndPoint ipep = new IPEndPoint(Direccion_Ip, puerto);

            //Creamos el socket para podernos conectar.
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                
                if(User_Name.Text != "" && Password.Text != "")
                {
                    server.Connect(ipep); //Intentamos conectarnos al socket
                    //this.BackColor = Color.Green;
                    MessageBox.Show("Conectado!!");
                    string NOMBRE = User_Name.Text; //Variable para almacenar el nombre q vamos a enviar
                    string CONTRASEÑA = Password.Text;

                    //Enviamos al servidor la informacion
                    byte[] Mensaje_Cliente = System.Text.Encoding.ASCII.GetBytes("s/" + NOMBRE + "/" + CONTRASEÑA);
                    server.Send(Mensaje_Cliente);

                    //Recibimos la respuesta del servidor
                    byte[] Respuesta = new byte[100];
                    string Mensaje;
                    server.Receive(Respuesta);
                    Mensaje = Encoding.ASCII.GetString(Respuesta);
                    MessageBox.Show(Mensaje);
                }
                else
                {
                    MessageBox.Show("Porfavor, rellene los datos para ingresar");
                }

                this.BackColor = Color.Gray;
                server.Shutdown(SocketShutdown.Both);
                server.Close();

            }
            catch (SocketException ex)
            {
                MessageBox.Show("No se ha podido establecer la conexion");
                return;
            }

        }
    }
}
