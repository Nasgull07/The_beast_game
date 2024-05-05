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
using UnityEditor;
using System.Collections.Concurrent;

public class Menu_Manager : MonoBehaviour
{

    public IPAddress Direccion_Ip = IPAddress.Parse("10.4.119.5"); //192.168.1.105   
    private int puerto = 50033;
    Socket server;

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

    string[] menssaje;

    Thread atender;

    IPEndPoint ipep;
    Socket Server;

    int id_loby;

    delegate void DelegadoParaPonerTexto(string texto);
    public void SaveDataBeforeSceneLoad()
    {
        // Guardar datos relevantes utilizando PlayerPrefs u otra forma de almacenamiento de datos
        PlayerPrefs.SetString("Conectados", m_conectados.text);
        PlayerPrefs.SetString("Usuario", m_Usuario_Conectado.text);
        PlayerPrefs.Save();
    }

    // Cargar datos despu�s de que se haya cargado una nueva escena
    public void LoadDataAfterSceneLoad()
    {
        // Cargar datos guardados utilizando PlayerPrefs u otra forma de almacenamiento de datos
        m_conectados.text = PlayerPrefs.GetString("Conectados"); // 0 es el valor predeterminado si no se encuentra la clave
        m_Usuario_Conectado.text = PlayerPrefs.GetString("Usuario");
    }

    // Start is called before the first frame update
    void Start()
    {
        Main_Menu.enabled = true;
        Loby.enabled = false;
        Invitacion.enabled = false;
        LoadDataAfterSceneLoad();
        m_Usuario_Conectado.text = null;
        ipep = new IPEndPoint(this.Direccion_Ip, puerto);

        //Creamos el socket para podernos conectar.
        server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {


            server.Connect(ipep);
            byte[] Mensaje_Cliente = System.Text.Encoding.ASCII.GetBytes("3/");
            server.Send(Mensaje_Cliente);

            atender = new Thread(AtenderServidor);
            atender.Start();



        }
        catch (SocketException)
        {
            m_Prefab.text = "No se pudo establecer conexion con el servidor";

            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Procesar mensajes en la cola de mensajes
        string[] mensaje;
        while (mensajeQueue.TryDequeue(out mensaje))
        {
            // Procesar el mensaje
            int codigo = Convert.ToInt32(mensaje[0]);
            string mensajeTexto = mensaje[1];
            int x = 2;
            int y = mensaje.Length;

            switch (codigo)
            {
                case 1:  // respuesta a longitud
                    m_Prefab.text = mensajeTexto;
                    string lista = null;
                    while (x < y)
                    {
                        lista = lista + "\n" + mensaje[x];
                        x++;
                    }

                    m_Usuario_Conectado.text = m_UserName.text;
                    m_Usuario_Conectado_Loby.text = m_Usuario_Conectado.text;
                    Main_Menu.enabled = false;
                    Loby.enabled = true;
                    m_lista_invitar.text = m_conectados.text;


                    break;
                case 2:      // respuesta a si mi nombre es bonito
                    m_Prefab.text = mensajeTexto;
                    break;

                case 3:

                    if (Main_Menu.enabled == true)
                    {
                        m_conectados.text = null;
                        int h = 1;
                        while (h < mensaje.Length)
                        {
                            m_conectados.text = m_conectados.text + mensaje[h] + "\n";
                            h++;
                        }
                    }
                    else if (Loby.enabled == true)
                    {
                        m_lista_invitar.text = null;
                        int h = 1;
                        while (h < mensaje.Length)
                        {
                            m_lista_invitar.text = m_lista_invitar.text + mensaje[h] + "\n";
                            h++;
                        }

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
                    if (Invitacion.enabled == false)
                    {
                        Invitacion.enabled = true;
                        string host = mensaje[1];
                        id_loby = Convert.ToInt32(mensaje[2]);
                        m_Mensaje_Invitacion.text = host + " te ha invitado a una sala con id:" + Convert.ToString(id_loby);
                    }
                    

                    break;
                case 7:
                    id_loby = Convert.ToInt32(mensaje[1]);
                    m_mensaje_loby.text= mensaje[2];
                    

                    break;

                case 8:

                    m_mensaje_loby.text = mensaje[1];
                    break;
                case 9:

                    m_Loby_Info.text = "ID Loby: "+mensaje[1] + '\n' + "Jugadores en la Loby:" + mensaje[2]+ mensaje[3]+ mensaje[4]+ mensaje[5];

                    break;
            }
        }

    }
    public void Game_Scene(string Scene)
    {

        SaveDataBeforeSceneLoad();
        SceneManager.LoadScene(Scene);
    }
    public void Exit()
    {

        string mensaje = "0/" + m_Usuario_Conectado.text;
        if (m_Usuario_Conectado.text != null)
        {

            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

        }
    }

    private void AtenderServidor()
    {
        while (true)
        {
            //Recibimos mensaje del servidor
            byte[] msg2 = new byte[2000];
            server.Receive(msg2);
            string[] mensaje = Encoding.ASCII.GetString(msg2).Split('/');

            mensajeQueue.Enqueue(mensaje);


        }
    }
    public void Log_In()
    {

        if (m_Usuario_Conectado.text == null)
        {

            try
            {

                if (m_UserName.text != "" && m_Password.text != "")

                {
                    string NOMBRE = m_UserName.text; //Variable para almacenar el nombre q vamos a enviar
                    string CONTRASE�A = m_Password.text;


                    byte[] Mensaje_Cliente = System.Text.Encoding.ASCII.GetBytes("1/" + NOMBRE + "/" + CONTRASE�A);
                    server.Send(Mensaje_Cliente);



                }




            }
            catch (SocketException)
            {
                m_Prefab.text = m_Usuario_Conectado.text;

                return;
            }
        }
        else { m_Prefab.text = "Ya hay una sesion iniciada, si quiere cambiar de sesion, desconectese de la actual."; }
    }
    public void Aceptar_Invitacion()
    {
        byte[] Mensaje_Cliente = System.Text.Encoding.ASCII.GetBytes("6/"+ m_Usuario_Conectado_Loby.text +"/1/" + Convert.ToString(id_loby));
        server.Send(Mensaje_Cliente);
        Invitacion.enabled = false;

    }
    public void Rechazar_Invitacion()
    {
        byte[] Mensaje_Cliente = System.Text.Encoding.ASCII.GetBytes("6/"+ m_Usuario_Conectado_Loby.text +"/0/" + Convert.ToString( id_loby));
        server.Send(Mensaje_Cliente);
        Invitacion.enabled = false;
    }
    public void Sign_Up()
    {

        try
        {

            if (m_UserName.text != "" && m_Password.text != "")
            {


                string NOMBRE = m_UserName.text; //Variable para almacenar el nombre q vamos a enviar
                string CONTRASE�A = m_Password.text;


                byte[] Mensaje_Cliente = System.Text.Encoding.ASCII.GetBytes("2/" + NOMBRE + "/" + CONTRASE�A);
                server.Send(Mensaje_Cliente);


            }
            else
            {

                m_Prefab.text = "Porfavor, rellene los datos para ingresar";
            }


        }
        catch (SocketException ex)
        {
            m_Prefab.text = "No se ha podido establecer la conexion";
            return;
        }
    }

    public void Invite()
    {

        try
        {

            if (m_Invite.text != "")
            {


                string NOMBRE_HOST = m_Usuario_Conectado_Loby.text; 
                string NOMBRE_CLIENT = m_Invite.text; 



                byte[] Mensaje_Cliente = System.Text.Encoding.ASCII.GetBytes("5/" + NOMBRE_HOST + "/" + NOMBRE_CLIENT);
                server.Send(Mensaje_Cliente);


            }
            else
            {

                //m_Prefab.text = "Porfavor, rellene los datos para ingresar";
            }


        }
        catch (SocketException ex)
        {
            m_Prefab.text = "No se ha podido establecer la conexion";
            return;
        }
    }

    public void Exit1()
    {
        Main_Menu.enabled = true;
        Loby.enabled = false;

    }
}