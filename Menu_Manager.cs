using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Threading;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using UnityEditor.VersionControl;
using UnityEditor;


public class Menu_Manager : MonoBehaviour
{
    public IPAddress Direccion_Ip = IPAddress.Parse("10.4.119.5");//   192.168.1.105
    private int puerto = 50030;
    private Socket server;

    [SerializeField] private InputField m_UserName = null;
    [SerializeField] private InputField m_Password = null;
    [SerializeField] private InputField m_Invite = null;
    [SerializeField] private InputField m_Mensaje_Chat = null;

    public TextMeshProUGUI m_Prefab = null;
    public TextMeshProUGUI m_conectados = null;
    public TextMeshProUGUI m_numero = null;
    public TextMeshProUGUI m_lista_invitar = null;
    public TextMeshProUGUI m_Mensaje_Invitacion = null;
    public TextMeshProUGUI m_mensaje_loby = null;
    public TextMeshProUGUI m_Chat = null;
    public TextMeshProUGUI m_Usuario_Conectado = null;
    public TextMeshProUGUI m_Usuario_Conectado_Loby = null;
    public TextMeshProUGUI m_Loby_Info = null;

    public Canvas Main_Menu = null;
    public Canvas Invitacion = null;
    public Canvas Loby = null;

    public Button Aceptar = null;
    public Button Rechazar = null;
    public Button Enviar = null;

    private ConcurrentQueue<string[]> mensajeQueue = new ConcurrentQueue<string[]>();
    private Thread atender;

    int id_loby;


    

    void Start()
    {
        Main_Menu.enabled = true;
        Loby.enabled = false;
        Invitacion.enabled = false;
        //LoadDataAfterSceneLoad();
        m_Usuario_Conectado.text = null;
        IPEndPoint ipep = new IPEndPoint(Direccion_Ip, puerto);

        // Create the socket to connect.
        server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            server.Connect(ipep);
            byte[] Mensaje_Cliente = Encoding.ASCII.GetBytes("3/");
            server.Send(Mensaje_Cliente);
            atender = new Thread(AtenderServidor);
            atender.Start();
        }
        catch (SocketException)
        {
            m_Prefab.text = "No se pudo establecer conexión con el servidor";
            return;
        }
    }

    void Update()
    {
        string[] mensaje;
        while (mensajeQueue.TryDequeue(out mensaje))
        {
            int codigo = Convert.ToInt32(mensaje[0]);
            string mensajeTexto = mensaje[1];
            int x = 2;
            int y = mensaje.Length;

            switch (codigo)
            {
                case 1:
                    m_Prefab.text = mensajeTexto;
                    m_Usuario_Conectado.text = m_UserName.text;
                    m_Usuario_Conectado_Loby.text = m_Usuario_Conectado.text;
                    Main_Menu.enabled = false;
                    Loby.enabled = true;

                    string lista = string.Join("\n", mensaje.Skip(2));
                    m_lista_invitar.text = lista;
                    break;

                case 2:
                    m_Prefab.text = mensajeTexto;
                    break;

                case 3:
                    string listaConectados = string.Join("\n", mensaje.Skip(1));
                    if (Main_Menu.enabled == true)
                    {
                        m_conectados.text = listaConectados;
                    }
                    else if (Loby.enabled == true)
                    {
                        m_lista_invitar.text = listaConectados;
                    }
                    break;

                case 4:
                    m_numero.text = mensajeTexto;
                    break;

                case 5:
                    m_conectados.text = mensaje[2];
                    break;

                case 0:
                    m_Usuario_Conectado.text = null;
                    break;

                case 6:
                    if (!Invitacion.enabled)
                    {
                        Invitacion.enabled = true;
                        string host = mensaje[1];
                        id_loby = Convert.ToInt32(mensaje[2]);
                        m_Mensaje_Invitacion.text = host + " te ha invitado a una sala con id: " + id_loby.ToString();
                    }
                    break;

                case 7:
                    id_loby = Convert.ToInt32(mensaje[1]);
                    m_mensaje_loby.text = mensaje[2];
                    break;

                case 8:
                    m_mensaje_loby.text = mensaje[1];
                    break;

                case 9:
                    m_Loby_Info.text = "ID Loby: " + mensaje[1] + '\n' + "Jugadores en la Loby:" + string.Join(" ", mensaje.Skip(2));
                    break;

                case 10:
                    if (mensaje.Length > 1)
                    { // Asegurar que hay al menos un mensaje para procesar
                      // Utiliza string.Join para concatenar todos los mensajes con saltos de línea
                        m_Chat.text = string.Join("\n", mensaje.Skip(1));
                    }
                    
                    break;
            }
        }
    }

    public void Game_Scene(string Scene)
    {
        //SaveDataBeforeSceneLoad();
        SceneManager.LoadScene(Scene);
    }

    public void Exit()
    {
        string mensaje = "0/" + m_Usuario_Conectado.text;
        if (!string.IsNullOrEmpty(m_Usuario_Conectado.text))
        {
            byte[] msg = Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }
    }

    private void AtenderServidor()
    {
        while (true)
        {
            byte[] msg2 = new byte[2000];
            server.Receive(msg2);
            string[] mensaje = Encoding.ASCII.GetString(msg2).Split('/');
            mensajeQueue.Enqueue(mensaje);
        }
    }

    public void Log_In()
    {
        if (string.IsNullOrEmpty(m_Usuario_Conectado.text))
        {
            if (!string.IsNullOrEmpty(m_UserName.text) && !string.IsNullOrEmpty(m_Password.text))
            {
                string NOMBRE = m_UserName.text;
                string CONTRASEÑA = m_Password.text;
                byte[] Mensaje_Cliente = Encoding.ASCII.GetBytes("1/" + NOMBRE + "/" + CONTRASEÑA);
                server.Send(Mensaje_Cliente);
            }
            else
            {
                m_Prefab.text = "Ya hay una sesión iniciada, si quiere cambiar de sesión, desconéctese de la actual.";
            }
        }
    }

    public void Aceptar_Invitacion()
    {
        byte[] Mensaje_Cliente = Encoding.ASCII.GetBytes("6/" + m_Usuario_Conectado_Loby.text + "/1/" + Convert.ToString(id_loby));
        server.Send(Mensaje_Cliente);
        Invitacion.enabled = false;
    }

    public void Rechazar_Invitacion()
    {
        byte[] Mensaje_Cliente = Encoding.ASCII.GetBytes("6/" + m_Usuario_Conectado_Loby.text + "/0/" + Convert.ToString(id_loby));
        server.Send(Mensaje_Cliente);
        Invitacion.enabled = false;
    }

    public void Sign_Up()
    {
        if (!string.IsNullOrEmpty(m_UserName.text) && !string.IsNullOrEmpty(m_Password.text))
        {
            string NOMBRE = m_UserName.text;
            string CONTRASEÑA = m_Password.text;
            byte[] Mensaje_Cliente = Encoding.ASCII.GetBytes("2/" + NOMBRE + "/" + CONTRASEÑA);
            server.Send(Mensaje_Cliente);
        }
        else
        {
            m_Prefab.text = "Por favor, rellene los datos para ingresar";
        }
    }

    public void Invite()
    {
        if (!string.IsNullOrEmpty(m_Invite.text))
        {
            string NOMBRE_HOST = m_Usuario_Conectado_Loby.text;
            string NOMBRE_CLIENT = m_Invite.text;
            byte[] Mensaje_Cliente = Encoding.ASCII.GetBytes("5/" + NOMBRE_HOST + "/" + NOMBRE_CLIENT);
            server.Send(Mensaje_Cliente);
        }
        else
        {
            //m_Prefab.text = "Por favor, rellene los datos para ingresar";
        }
    }
    public void EnviarChat()
    {

        if (!string.IsNullOrEmpty(m_Mensaje_Chat.text))
        {
           byte[] Mensaje_Cliente = Encoding.ASCII.GetBytes("7/" + m_Usuario_Conectado_Loby.text + "/" + id_loby + "/" + m_Mensaje_Chat.text);
            server.Send(Mensaje_Cliente); 
            m_Mensaje_Chat.text = null;
        }
        
    }
    public void Exit1()
    {
        Main_Menu.enabled = true;
        Loby.enabled = false;
    }
}