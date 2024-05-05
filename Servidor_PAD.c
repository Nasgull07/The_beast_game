#include <sys/types.h> 
#include <sys/socket.h> 
#include <netinet/in.h> 
#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <stdio.h>
#include <mysql.h>
#include <pthread.h>
#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <stdbool.h>
#include <netinet/in.h>
#include <pthread.h>

/*ESTRUCTURAS*/

typedef struct
{
	int *sock;
	char Nombre[20];
	int id;
} Cliente;

typedef struct
{
	Cliente cliente[100];
	int numero_clientes;
	
} Cliente_Lista;

typedef struct {
	char dato[20];
	
}Dato;

typedef struct {
	int rows;
	int cols;
	Dato data[10][10]; 
} Matriz;

typedef struct {
	char Nombre[20];
	int ID;
	int  Socket;
} Jugador;

typedef struct {
	Jugador jugadores[4];
	int numero_Jug;
	int ID_Loby;
} Loby;

typedef struct {
	Loby Lobys[20];
	int num_Lobys;
}Lobys;

/* VARIABLES GLOBALES*/

int contador;
pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;
char clientes[300];
bool login = false;
MYSQL *conn;
Cliente cliente;
Cliente_Lista Clis;
Lobys Lobyreal;
int i;
int puerto = 50033;
int Socket[100];
char peticion[512];
char respuesta[512];

/* FUNCIONES*/

void conectarBD() {
	
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexion: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	conn = mysql_real_connect (conn, "shiva2.upc.es","root", "mysql", "M7_BBDD_TheBeast", 0, NULL, 0);
	//conn = mysql_real_connect (conn, "localhost","root", "mysql", NULL, 0, NULL, 0);
	if (conn==NULL)
	{
		printf ("Error al inicializar la conexion: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	Lobyreal.num_Lobys = 0;
}

void ejecutarConsultaSQL(char *sql) {
	if (mysql_query(conn, sql) != 0) {
		fprintf(stderr, "Error en la consulta: %s\n", mysql_error(conn));
		mysql_close(conn);
		exit(EXIT_FAILURE);
	}
}


Matriz ejecutarConsulta(char *sql, Matriz matriz) {
	MYSQL_RES *res;
	MYSQL_ROW row;
	int err = mysql_query(conn, sql);
	if (err != 0) {
		printf("%s", sql);
		printf("Error al consultar datos de la base %u %s\n", mysql_errno(conn), mysql_error(conn));
		exit(1);
	}
	res = mysql_store_result(conn);
	
	if (res == NULL) {
		printf("Error al obtener el resultado de la consulta\n");
		
		return matriz;
		exit(1);
	}
	
	int num_rows = mysql_num_rows(res);
	int num_fields = mysql_num_fields(res);
	
	int i = 0;
	while ((row = mysql_fetch_row(res)) != NULL && i < 100) {
		for (int j = 0; j < num_fields; j++) {
			
			if (row[j] != NULL) {
				strcpy(matriz.data[i][j].dato, row[j]);
				
			}
		}
		i++;
	}
	
	mysql_free_result(res);
	
	return matriz;
}


void *AtenderCliente (void *socket)
{
	int sock_conn;
	int *s;
	s= (int *) socket;
	sock_conn= *s;	
	int ret;
	
	int terminar =0;
	// Entramos en un bucle para atender todas las peticiones de este cliente
	//hasta que se desconecte
	while (terminar ==0)
	{
		// Ahora recibimos la petici\ufff3n
		ret=read(sock_conn,peticion, sizeof(peticion));
		printf ("Recibido\n");
		
		// Tenemos que a\ufff1adirle la marca de fin de string 
		// para que no escriba lo que hay despues en el buffer
		peticion[ret]='\0';
		
		printf ("Peticion: %s\n",peticion);
		
		// vamos a ver que quieren
		char *p = strtok( peticion, "/");
		int codigo =  atoi (p);
		
		char Nombre[20];
		char Contrasenya[20];
		char Jugador_Invitado1[20];
		char Jugador_Invitado2[20];
		char Jugador_Invitado3[20];
		if (codigo != 0  && codigo != 3 && codigo != 5 && codigo != 6)
		{
			
			p = strtok( NULL, "/");
			strcpy (Nombre, p);
			p = strtok( NULL, "/");
			strcpy (Contrasenya, p);
			// Ya tenemos el nombre
			printf ("Codigo: %d, Nombre: %s, Contrasenya; %s\n", codigo, Nombre, Contrasenya);
		}
		else{
			if(codigo != 3)
			{
				p = strtok( NULL, "/");
				strcpy (Nombre, p);
				printf("%s", Nombre);
				if(codigo == 5)
				{
					p = strtok( NULL, "/");
					strcpy (Jugador_Invitado1, p);
					
					p = strtok( NULL, "/");
					strcpy (Jugador_Invitado2, p);
					
					p = strtok( NULL, "/");
					strcpy (Jugador_Invitado3, p);
				}
				
			}
			
		}
		if (codigo ==0) //peticion de desconexión
		{
			int k;
			for (k = 0; k < 10; k++) {
				
				
				if(strcmp(Clis.cliente[k].Nombre, Nombre) == 0)
				{
					
					strcpy(Clis.cliente[k].Nombre, "-");
				}				
			}
			
			contador --;
			
			char restantes[300];
			if(Clis.numero_clientes == 0)
			{sprintf (restantes, "0/no hay nadie conectado");
			write(sock_conn, restantes, strlen(restantes));
			
			
			}
			else{
				
				sprintf(restantes, "0/desconectado");
				write(sock_conn, restantes, strlen(restantes));
				Notificar(3);
			}
			
		}
		else if (codigo == 1) //piden la longitd del nombre
		{
			char *comando[100];
			sprintf(comando, "SELECT NOMBRE FROM jugadores WHERE NOMBRE = '%s' AND CONTRASENYA = '%s'", Nombre, Contrasenya);
			printf("SELECT NOMBRE, ID FROM jugadores WHERE NOMBRE = '%s' AND CONTRASENYA = '%s'", Nombre, Contrasenya);
			
			Matriz matriz;
			matriz = ejecutarConsulta(comando, matriz);
			
			if(strcmp(matriz.data[0][0].dato, Nombre)==0)
			{
				int g;
				bool existente;
				for(g = 0; g < Clis.numero_clientes; g++)
				{
					if(strcmp(Clis.cliente[g].Nombre, Nombre) == 0)
					{existente = true;}
				}
				if(existente == true)
				{
					
					sprintf (respuesta, "1/Bienvenido de nuevo %s, su session ya ha sido iniciada correctamente.", Nombre);
					
				}
				
				else
				{
					g=0;
					
					for(g = 0; g < Clis.numero_clientes; g++)
					{
						if(sock_conn == Clis.cliente[g].sock)
						{
							sprintf(Clis.cliente[g].Nombre, Nombre);
							Clis.cliente[g].id = atoi( matriz.data[0][1].dato);
							printf("%d", Clis.cliente[g].id);
						}
					}
					sprintf (respuesta, "1/Bienvenido de nuevo %s, su session ha sido iniciada correctamente.", Nombre);
					
					
					
					
					pthread_mutex_lock( &mutex ); //No me interrumpas ahora
					
					
					contador = contador +1;
					pthread_mutex_unlock( &mutex); 
					Notificar(3);
					
				}
			}
			else
			{
				sprintf (respuesta, "1/Ha escrito mal su usuario o Contrasenya");
				
			}
		}			
		
		
		else if (codigo ==2)
		{
			char *comando[100];
			sprintf(comando, "SELECT NOMBRE FROM jugadores WHERE NOMBRE = '%s'", Nombre);
			Matriz matriz;
			matriz = ejecutarConsulta(comando, matriz);
			if(strcmp(matriz.data[0][0].dato, Nombre) == 0)
			{sprintf(respuesta, "2/este usuario ya existe");
			}
			else
			{
				char comando[100];
				sprintf(comando, "INSERT INTO jugadores (NOMBRE, CONTRASENYA) VALUES ('%s', '%s')", Nombre, Contrasenya);
				ejecutarConsultaSQL(comando);
				sprintf (respuesta, "2/Hola, Gracias por registrarte en nuestro juego");
				
			}
		}
		if (codigo == 3)
		{
			sprintf(respuesta, "3/.");
			Give_Me_Onlines(Clis, clientes);
		}
		
		
		if (codigo == 5)
		{
			Lobyreal.Lobys[Lobyreal.num_Lobys].ID_Loby = Lobyreal.num_Lobys;
			sprintf(Lobyreal.Lobys[Lobyreal.num_Lobys].jugadores[0].Nombre,	Nombre);
			sprintf(Lobyreal.Lobys[Lobyreal.num_Lobys].jugadores[1].Nombre,	Jugador_Invitado1);
			sprintf(Lobyreal.Lobys[Lobyreal.num_Lobys].jugadores[2].Nombre,	Jugador_Invitado2);
			sprintf(Lobyreal.Lobys[Lobyreal.num_Lobys].jugadores[3].Nombre,	Jugador_Invitado3);
			
			int hhh = Encontrar_Posicion(Nombre);
			Lobyreal.Lobys[Lobyreal.num_Lobys].jugadores[0].ID = Clis.cliente[hhh].id;
			Lobyreal.Lobys[Lobyreal.num_Lobys].jugadores[0].Socket = Clis.cliente[hhh].sock;
			Lobyreal.Lobys[Lobyreal.num_Lobys].numero_Jug++;
			Lobyreal.num_Lobys++;
			char crearsala[100];
			sprintf(crearsala, "7/%d/su sala se ha creado correctamente",Lobyreal.Lobys[Lobyreal.num_Lobys-1].ID_Loby );
			write (sock_conn, crearsala, strlen(crearsala));
			sprintf(crearsala, "9/%d/%s/././.",Lobyreal.Lobys[Lobyreal.num_Lobys-1].ID_Loby, Nombre );
			write (sock_conn, crearsala, strlen(crearsala));
			int zz = 0;
			int y=0;
			while(zz<3)
			{	
				y = Encontrar_Posicion(Lobyreal.Lobys[Lobyreal.num_Lobys-1].jugadores[zz+1].Nombre);
				char invitacion[100];
				sprintf(invitacion, "6/%s/%d", Nombre, Lobyreal.Lobys[Lobyreal.num_Lobys-1].ID_Loby);
				if(y!=-1)
				{
					write (Clis.cliente[y].sock, invitacion, strlen(invitacion));
				}zz++;
			}
			
		}
		
		if (codigo == 6)
		{
			int filtro;
			int idloby;
			int posicion;
			p = strtok( NULL, "/");
			filtro = atoi(p);
			p = strtok( NULL, "/");
			idloby = atoi(p);
			printf("%d", filtro);
			printf("%d", idloby);
			if (filtro == 1)
			{
				int dd=0;
				while(dd<4)
				{
					if(strcmp(Nombre, Lobyreal.Lobys[idloby].jugadores[dd].Nombre) == 0)
					{posicion = dd;}
					dd++;
				}
				
				int hhh = Encontrar_Posicion(Nombre);
				Lobyreal.Lobys[idloby].jugadores[posicion].ID = Clis.cliente[hhh].id;
				Lobyreal.Lobys[idloby].jugadores[posicion].Socket = Clis.cliente[hhh].sock;
				Lobyreal.Lobys[idloby].numero_Jug++;
				char confirmacion[200];
				sprintf(confirmacion, "8/Se ha unido a la sala %d" , idloby);
				printf(confirmacion);
				write (Clis.cliente[hhh].sock, confirmacion, strlen(confirmacion));
				sprintf(confirmacion, "8/%s se ha unido a la sala", Nombre);
				printf(confirmacion);
				Notificar_Sala(confirmacion, idloby);
				dd=0;
				char listaconectados[100];
				sprintf(listaconectados, "9/%d", idloby );
				while(dd<4)
				{
					if(Lobyreal.Lobys[idloby].jugadores[dd].Socket != NULL)
					{
						sprintf(listaconectados + strlen(listaconectados), "/%s, ", Lobyreal.Lobys[idloby].jugadores[dd].Nombre);
						
					}
					else
					   {sprintf(listaconectados + strlen(listaconectados), "/.");}
					dd++;
				}
				
				Notificar_Sala(listaconectados, idloby);
				
				
			}
			else
			{
				char confirmacion[200];
				sprintf(confirmacion, "8/%s ha rechazado la invitacion" , Nombre);
				printf(confirmacion);
				Notificar_Sala(confirmacion, idloby);
			}
		}
		
		if (codigo != 0 && codigo != 5 && codigo != 6)
		{
			printf ("Respuesta: %s\n", respuesta);
			// Enviamos respuesta
			write (sock_conn,respuesta, strlen(respuesta));
			
			
		}
		
		
	}close(sock_conn); 
	
	
	// Se acabo el servicio para este cliente
	
}

void Notificar_Sala(char mensaje[300], int idloby)
{
	int bb = 0;
	while(bb<4)
	{
		if(Lobyreal.Lobys[idloby].jugadores[bb].Socket != NULL)
		{
			write (Lobyreal.Lobys[idloby].jugadores[bb].Socket, mensaje, strlen(mensaje));
		}
		bb++;
	}
}

int Encontrar_Posicion (char Nombre[20])
{
	int gg = 0;
	while(gg<Clis.numero_clientes)
	{
		if(strcmp(Nombre, Clis.cliente[gg].Nombre) == 0)
		{return gg;}
		else{gg++;}
	}
	printf("Error");
	return -1;
}


void Notificar(int codigo)
{
	char notificacion[200];
	
	Give_Me_Onlines(Clis, notificacion);
	char notificacion_Real[200];
	
	sprintf(notificacion_Real, "3");
	sprintf(notificacion_Real + strlen(notificacion_Real), notificacion);
	
	int jj;
	for(jj=0;jj<Clis.numero_clientes;jj++)
	{	
		printf(notificacion_Real);
		write(Clis.cliente[jj].sock, notificacion_Real, strlen(notificacion_Real));	
	}
}


void Give_Me_Onlines(Cliente_Lista Clis, char conectados[300]) {
	
	
	int h;
	for (h = 0; h < 300; h++) {
		conectados[h] = '\0';
	}
	int l;
	for (l = 0; l < Clis.numero_clientes; l++) {
		sprintf(conectados +strlen(conectados), "/%s", Clis.cliente[l].Nombre);
		
	}printf(conectados);
	sprintf(respuesta + strlen(respuesta), conectados);
}


int main(int argc, char *argv[])
{
	
	conectarBD();
	// Ejecutar la consulta para crear la base de datos y las tablas
	/*	ejecutarConsultaSQL("DROP DATABASE IF EXISTS JUEGO");*/
	/*	ejecutarConsultaSQL("CREATE DATABASE JUEGO");*/
	/*	ejecutarConsultaSQL("USE JUEGO");*/
	/*	ejecutarConsultaSQL("CREATE TABLE jugadores(ID INTEGER PRIMARY KEY AUTO_INCREMENT, NOMBRE varchar(20) not null, CONTRASENYA varchar(20)not null, PUNTOS integer, VICTORIAS integer)");*/
	/*	ejecutarConsultaSQL("CREATE TABLE partidas(ID integer primary key not null, FECHA varchar(20), TIEMPO time, JUGADORES integer)");*/
	/*	ejecutarConsultaSQL("CREATE TABLE relacionjp (ID integer primary key not null, ID_J integer not null, foreign key (ID_J) references jugadores(ID), ID_P integer not null, foreign key (ID_P) references partidas(ID))");*/
	
	// Insertar datos en las tablas
	/*	ejecutarConsultaSQL("INSERT INTO jugadores (ID, NOMBRE, CONTRASENYA, PUNTOS, VICTORIAS) VALUES (1, 'pol', 'contra', 100, 10), (2, 'Jugador2', 'contrasenya', 200, 15), (3, 'Jugador3', 'contrasenya3', 150, 12)");*/
	/*	ejecutarConsultaSQL("INSERT INTO partidas (ID, FECHA, TIEMPO, JUGADORES) VALUES (1, '03-01', '12:00:00', 3), (2, '03-02', '15:30:00', 2), (3, '03-03', '18:45:00', 1)");*/
	/*	ejecutarConsultaSQL("INSERT INTO relacionjp (ID, ID_J, ID_P) VALUES (1, 1, 1), (2, 2, 1)");*/
	
	
	int sock_conn, sock_listen;
	struct sockaddr_in serv_adr;
	
	if ((sock_listen = socket (AF_INET, SOCK_STREAM, 0)) < 0)
	{printf("Error creant socket");}
	
	memset(&serv_adr, 0, sizeof(serv_adr)); // inicialitza a zero serv_addr
	serv_adr.sin_family = AF_INET;
	
	serv_adr.sin_addr.s_addr = htonl (INADDR_ANY);
	serv_adr.sin_port = htons (puerto);
	
	if (bind (sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0)
		printf ("Error al bind");
	
	if (listen (sock_listen, 3) < 0) 
		printf("Error en el Listen");
	
	pthread_t  thread;
	i=0;
	for (;;)
	{
		printf ("Escuchando\n");
		sock_conn = accept (sock_listen, NULL, NULL);
		printf ("He recibido conexion\n");
		//sock_conn es el socket que usaremos para este cliente
		Clis.cliente[i].sock = sock_conn;
		Clis.numero_clientes++;
		Socket[i] = sock_conn;	
		pthread_create(&thread, NULL, AtenderCliente, &Socket[i]);
		
		i++;	
	}	
	mysql_close(conn);
}