using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Threading;
using UnityEditor.VersionControl;

public class Menu_Manager : MonoBehaviour
{

    public IPAddress Direccion_Ip = IPAddress.Parse("192.168.1.105");
    public int puerto = 8095;
    Socket server;
    [SerializeField] private InputField m_UserName = null;
    [SerializeField] private InputField m_Password = null;
    public TextMeshProUGUI m_Prefab = null;
    public TextMeshProUGUI m_conectados = null;
    public TextMeshProUGUI m_numero = null;
    public TextMeshProUGUI m_usuario = null;


    Thread atender;

    delegate void DelegadoParaPonerTexto(string texto);
    public void SaveDataBeforeSceneLoad()
    {
        // Guardar datos relevantes utilizando PlayerPrefs u otra forma de almacenamiento de datos
        PlayerPrefs.SetString("Conectados", m_conectados.text);
        PlayerPrefs.SetString("Usuario", m_usuario.text);
        PlayerPrefs.Save();
    }

    // Cargar datos después de que se haya cargado una nueva escena
    public void LoadDataAfterSceneLoad()
    {
        // Cargar datos guardados utilizando PlayerPrefs u otra forma de almacenamiento de datos
        m_conectados.text = PlayerPrefs.GetString("Conectados"); // 0 es el valor predeterminado si no se encuentra la clave
        m_usuario.text = PlayerPrefs.GetString("Usuario");
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadDataAfterSceneLoad();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Game_Scene(string Scene)
    {
        
        SaveDataBeforeSceneLoad();
        SceneManager.LoadScene(Scene);
    }
    public void Exit()
    {
        IPEndPoint ipep = new IPEndPoint(this.Direccion_Ip, puerto);

        //Creamos el socket para podernos conectar.
        server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        string mensaje = "0/"+ m_usuario.text;
        server.Connect(ipep);
        //Mensaje de desconexión string mensaje = "0/";
        byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje); 
        server.Send(msg);
        AtenderServidor();
        //server.Shutdown(SocketShutdown.Both);
        //server.Close();
    }

    private void AtenderServidor()
    {
        while (true)
        {
            //Recibimos mensaje del servidor
            byte[] msg2 = new byte[2000];
            server.Receive(msg2);
            string[] trozos = Encoding.ASCII.GetString(msg2).Split('/');
            string mensaje;
            mensaje = trozos[1];

            int codigo = Convert.ToInt32(trozos[0]);
            
            int x = 2;
            int y = trozos.Length;

            switch (codigo)
            {
                case 1:  // respuesta a longitud
                    
                    m_Prefab.text = mensaje;
                    
                    string lista = null;
                    //m_conectados.text = trozos[2];
                    while (x<y)
                    {
                        lista = lista + "\n" + trozos[x];
                        x++;
                    }
                    m_conectados.text = lista;
                    m_usuario.text = m_UserName.text;
                    if (trozos[1].Contains("Bienvenido"))
                    {Game_Scene("Main_Menu"); }
                    

                    //m_conectados.text = trozos[2];

                    break;
                case 2:      //respuesta a si mi nombre es bonito

                    m_Prefab.text = mensaje;

                    break;

                case 4:     //Recibimos notificacion

                    //Haz tu lo que no me dejas hacer a mi
                    m_numero.text = mensaje;

                    break;
                case 5:


                    m_conectados.text = trozos[2];
                    

                    break;
                case 0:
                    string listaa = null;
                    //m_conectados.text = trozos[2];
                    while (x-1 < y)
                    {
                        listaa = listaa + "\n" + trozos[x-1];
                        x++;
                    }
                    m_conectados.text = listaa;
                    m_usuario.text = null;

                    break;
            }
        }
    }
    public void Log_In()
    {
        //Creamos un IPEndPoint con el ip del servidor y el puerto del servidor al q nos vamos a conectar.
        // IPAddress Direccion_Ip = IPAddress.Parse("192.168.1.105");
        IPEndPoint ipep = new IPEndPoint(this.Direccion_Ip, puerto);

        //Creamos el socket para podernos conectar.
        server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {

            if (m_UserName.text != "" && m_Password.text != "")

            {
                server.Connect(ipep); //Intentamos conectarnos al socket
                                      //this.BackColor = Color.Green;

                
                
                //MessageBox.Show("Conectado!!");
                string NOMBRE = m_UserName.text; //Variable para almacenar el nombre q vamos a enviar
                string CONTRASEÑA = m_Password.text;

                //Enviamos al servidor la informacion
                byte[] Mensaje_Cliente = System.Text.Encoding.ASCII.GetBytes("1/" + NOMBRE + "/" + CONTRASEÑA);
                server.Send(Mensaje_Cliente);

                //Recibimos la respuesta del servidor
                //byte[] Respuesta = new byte[100];
                //string Mensaje;
                //server.Receive(Respuesta);
                //Mensaje = Encoding.ASCII.GetString(Respuesta);
                //m_Prefab.text = Mensaje;
                
                AtenderServidor();
                
                //MessageBox.Show(Mensaje);
            }
            else
            {
                //MessageBox.Show("Porfavor, rellene los datos para ingresar");
            }


           
            //server.Close();

        }
        catch (SocketException)
        {
            m_Prefab.text = "Nosepudo";
            //MessageBox.Show("No se ha podido establecer la conexion");
            return;
        }
    }
    public void Sign_Up()
    {
        //Creamos un IPEndPoint con el ip del servidor y el puerto del servidor al q nos vamos a conectar.

        IPEndPoint ipep = new IPEndPoint(Direccion_Ip, puerto);

        //Creamos el socket para podernos conectar.
        server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {

            if (m_UserName.text != "" && m_Password.text != "")
            {
                server.Connect(ipep); //Intentamos conectarnos al socket
                                      //this.BackColor = Color.Green;
                //MessageBox.Show("Conectado!!");
                string NOMBRE = m_UserName.text; //Variable para almacenar el nombre q vamos a enviar
                string CONTRASEÑA = m_Password.text;

                //Enviamos al servidor la informacion
                byte[] Mensaje_Cliente = System.Text.Encoding.ASCII.GetBytes("2/" + NOMBRE + "/" + CONTRASEÑA);
                server.Send(Mensaje_Cliente);
                AtenderServidor();
                //Recibimos la respuesta del servidor
                //byte[] Respuesta = new byte[100];
                //string Mensaje;
                //server.Receive(Respuesta);
                //Mensaje = Encoding.ASCII.GetString(Respuesta);
                //m_Prefab.text = Mensaje;
                //MessageBox.Show(Mensaje);
                
            }
            else
            {
                //MessageBox.Show("Porfavor, rellene los datos para ingresar");
                m_Prefab.text = "Porfavor, rellene los datos para ingresar";
            }

            //this.BackColor = Color.Gray;
            server.Shutdown(SocketShutdown.Both);
            server.Close();

        }
        catch (SocketException ex)
        {
            m_Prefab.text = "No se ha podido establecer la conexion";
            return;
        }
    }

}
